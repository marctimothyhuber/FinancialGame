open System
open System.IO
open System.Net.Http
open Newtonsoft.Json.Linq

type Account = { Type: string; Currency: string; Balance: decimal }

let cashAccountCHF = { Type = "Cash"; Currency = "CHF"; Balance = 0M }
let bankAccountCHF = { Type = "Bank"; Currency = "CHF"; Balance = 1000M }
let bankAccountUSD = { Type = "Bank"; Currency = "USD"; Balance = 0M }
let bankAccountEUR = { Type = "Bank"; Currency = "EUR"; Balance = 0M }

// Fetch exchange rates from API
let fetchExchangeRates apiKey =
    async {
        let url = sprintf "%s" apiKey
        use client = new HttpClient()
        let! response = client.GetStringAsync(url) |> Async.AwaitTask
        return JObject.Parse(response)
    }
// Calculate Exchange Rate
let calculateExchangeRate (fromCurrency: string) (toCurrency: string) (exchangeRates: JObject) =
    let fromRate = exchangeRates.["data"].[fromCurrency].Value<decimal>()
    let toRate = exchangeRates.["data"].[toCurrency].Value<decimal>()
    toRate / fromRate

let prepareResult (amount, clear) =
    if clear then
        Console.Clear()
    for i in 1 .. amount do
        printfn "--------------------------------"

let withdraw amount (bankAccountCHF: Account) (cashAccount: Account) (bankAccountUSD: Account) (bankAccountEUR: Account) =
    if bankAccountCHF.Balance >= amount then
        let newBankAccountCHF = { bankAccountCHF with Balance = bankAccountCHF.Balance - amount }
        let newCashAccountCHF = { cashAccount with Balance = cashAccount.Balance + amount }
        (newBankAccountCHF, newCashAccountCHF, bankAccountUSD, bankAccountEUR, Some(sprintf "Withdrawn %M CHF from bank to cash" amount))
    else
        (bankAccountCHF, cashAccount, bankAccountUSD, bankAccountEUR, Some("Insufficient funds in bank account"))

let deposit amount (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account) =
    if cashAccountCHF.Balance >= amount then
        let newBankAccountCHF = { bankAccountCHF with Balance = bankAccountCHF.Balance + amount }
        let newCashAccountCHF = { cashAccountCHF with Balance = cashAccountCHF.Balance - amount }
        (newBankAccountCHF, newCashAccountCHF, bankAccountUSD, bankAccountEUR, Some(sprintf "Deposited %M CHF from cash to bank" amount))
    else
        (bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR, Some("Insufficient funds in cash account"))

let showBalances (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account) =
    let balances = sprintf "Cash Account Balance:\nCHF:     %M\nBank Account Balance:\nCHF:     %M\nUSD:     %M\nEUR:     %M" 
                            cashAccountCHF.Balance bankAccountCHF.Balance bankAccountUSD.Balance bankAccountEUR.Balance
    balances

let saveAccounts (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account) =
    let data = sprintf "%M,%M,%M,%M" cashAccountCHF.Balance bankAccountCHF.Balance bankAccountUSD.Balance bankAccountEUR.Balance
    File.WriteAllText("accounts.txt", data)
    prepareResult(1, true)
    printfn "Accounts saved"
    prepareResult(2, false)

let loadAccounts () =
    if File.Exists("accounts.txt") then
        let data = File.ReadAllText("accounts.txt").Split(',')
        let cashAccountCHF = { Type = "Cash"; Currency = "CHF"; Balance = decimal data.[0] }
        let bankAccountCHF = { Type = "Bank"; Currency = "CHF"; Balance = decimal data.[1] }
        let bankAccountUSD = { Type = "Bank"; Currency = "USD"; Balance = decimal data.[2] }
        let bankAccountEUR = { Type = "Bank"; Currency = "EUR"; Balance = decimal data.[3] }
        prepareResult(1, true)
        printfn "Accounts loaded"
        bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
    else
        prepareResult(1, true)
        printfn "No saved accounts found"
        bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR

let tradeExchange (bankAccountFrom: Account) (bankAccountTo: Account) (amount: decimal) =
    // Import API KEY from file
    let apiKey = File.ReadAllText("APIKEY.txt").Trim()
    // Fetch exchange rates
    let exchangeRatesTask = fetchExchangeRates apiKey |> Async.RunSynchronously
    let rate = match bankAccountFrom.Currency, bankAccountTo.Currency with
                        | "CHF", "USD" -> calculateExchangeRate "CHF" "USD" exchangeRatesTask
                        | "USD", "CHF" -> calculateExchangeRate "USD" "CHF" exchangeRatesTask
                        | "CHF", "EUR" -> calculateExchangeRate "CHF" "EUR" exchangeRatesTask
                        | "EUR", "CHF" -> calculateExchangeRate "EUR" "CHF" exchangeRatesTask
                        | "USD", "EUR" -> calculateExchangeRate "USD" "EUR" exchangeRatesTask
                        | "EUR", "USD" -> calculateExchangeRate "EUR" "USD" exchangeRatesTask
                        | _, _ -> 0M
    if bankAccountFrom.Balance >= amount then
        let newBankAccountFrom = { bankAccountFrom with Balance = Math.Round(bankAccountFrom.Balance - amount,2) }
        let newBankAccountTo = { bankAccountTo with Balance = Math.Round(bankAccountTo.Balance + amount * rate,2) }
        newBankAccountFrom, newBankAccountTo
    else 
        prepareResult(1, true)
        printfn "Insufficient funds in bank account"
        bankAccountFrom, bankAccountTo

let trade (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account)  =
    let apiKey = File.ReadAllText("APIKEY.txt").Trim()
    printfn "Trading is only available in your bank account"
    printfn "Your Bank Accounts:"
    printfn "CHF     %M" bankAccountCHF.Balance
    printfn "USD     %M" bankAccountUSD.Balance
    printfn "EUR     %M" bankAccountEUR.Balance
    prepareResult(1, false)
    let tradeableFrom = [ "CHF"; "USD"; "EUR" ]
    printf "Select currency to trade from (CHF, USD, EUR): "
    let tradeFrom = Console.ReadLine()
    let tradeableTo = tradeableFrom |> List.filter(fun x -> x <> tradeFrom)
    printf "Select currency to trade to (%s, %s): " tradeableTo.[0] tradeableTo.[1]
    let tradeTo = Console.ReadLine()
    printf "Amount to trade: "
    let amount = decimal (Console.ReadLine())
    prepareResult(1,true)
    let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR =
        match tradeFrom, tradeTo with
        | "CHF", "USD" ->
            let newBankAccountCHF, newBankAccountUSD = tradeExchange bankAccountCHF bankAccountUSD amount
            printfn "Trading %M CHF to USD" amount
            newBankAccountCHF, cashAccountCHF, newBankAccountUSD, bankAccountEUR
        | "USD", "CHF" ->
            let newBankAccountUSD, newBankAccountCHF = tradeExchange bankAccountUSD bankAccountCHF amount
            printfn "Trading %M USD to CHF" amount
            newBankAccountCHF, cashAccountCHF, newBankAccountUSD, bankAccountEUR
        | "CHF", "EUR" ->
            let newBankAccountCHF, newBankAccountEUR = tradeExchange bankAccountCHF bankAccountEUR amount
            printfn "Trading %M CHF to EUR" amount
            newBankAccountCHF, cashAccountCHF, bankAccountUSD, newBankAccountEUR
        | "EUR", "CHF" ->
            let newBankAccountEUR, newBankAccountCHF = tradeExchange bankAccountEUR bankAccountCHF amount
            printfn "Trading %M EUR to CHF" amount
            newBankAccountCHF, cashAccountCHF, bankAccountUSD, newBankAccountEUR
        | "USD", "EUR" ->
            let newBankAccountUSD, newBankAccountEUR = tradeExchange bankAccountUSD bankAccountEUR amount
            printfn "Trading %M USD to EUR" amount
            bankAccountCHF, cashAccountCHF, newBankAccountUSD, newBankAccountEUR
        | "EUR", "USD" ->
            let newBankAccountEUR, newBankAccountUSD = tradeExchange bankAccountEUR bankAccountUSD amount
            printfn "Trading %M EUR to USD" amount
            bankAccountCHF, cashAccountCHF, newBankAccountUSD, newBankAccountEUR
        | _ ->
            prepareResult(1, true)
            printfn "Invalid trade pair"
            bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
    newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR

let coinToss (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account)  =
    printfn "Coin Toss is only available in your cash account"
    printfn "Your Cash Account Balance:"
    printfn "CHF     %M" cashAccountCHF.Balance
    prepareResult(1, false)
    printfn "Enter amount to bet: "
    let amount = decimal (Console.ReadLine())
    if amount > cashAccountCHF.Balance then
        prepareResult(1, true)
        printfn "Insufficient funds in cash account"
        bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
    else
        printfn "Heads or Tails: "
        let decision = Console.ReadLine().Trim().ToLower()
        let coin = if Random().Next(0, 2) = 0 then "heads" else "tails"
        let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR =
            if decision = coin then
                let newCashAccountCHF = { cashAccountCHF with Balance = cashAccountCHF.Balance + amount }
                printfn "You won %M CHF" amount
                bankAccountCHF, newCashAccountCHF, bankAccountUSD, bankAccountEUR
            else
                let newCashAccountCHF = { cashAccountCHF with Balance = cashAccountCHF.Balance - amount }
                printfn "You lost %M CHF" amount
                bankAccountCHF, newCashAccountCHF, bankAccountUSD, bankAccountEUR
        bankAccountCHF, newCashAccountCHF, bankAccountUSD, bankAccountEUR

let main () =
    let bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR = loadAccounts()
    let rec loop (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account)=
        prepareResult(2, false)
        printf "1. Withdraw\n2. Deposit\n3. Currency Exchange\n4. Coin Toss\n5. Show Balances\n6. Save and Exit\n> "
        let input = Console.ReadLine()
        let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR =
            match input with
            | "1" -> 
                printf "Amount to withdraw: "
                let amount = decimal (Console.ReadLine())
                prepareResult(1,true)
                let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR, message = 
                    withdraw amount bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                printfn "%s" (message |> Option.defaultValue "")
                newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR
            | "2" -> 
                printf "Amount to deposit: "
                let amount = decimal (Console.ReadLine())
                prepareResult(1,true)
                let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR, message = 
                    deposit amount bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                printfn "%s" (message |> Option.defaultValue "")
                newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR
            | "3" -> 
                prepareResult(1, true)
                let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR = 
                    trade bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR
            | "4" -> 
                prepareResult(1, true)
                let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR = 
                    coinToss bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR
            | "5" -> 
                let balances = showBalances bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                prepareResult(1,true)
                printfn "%s" balances
                bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
            | "6" -> 
                prepareResult(1,true)
                saveAccounts bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
            | _ -> 
                prepareResult(1, true)
                printfn "Invalid option"
                bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
            
        if input <> "6" then 
            loop newBankAccountCHF newCashAccountCHF newBankAccountUSD newBankAccountEUR
    loop bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR

main()
