open System
open System.IO

type Account = { Type: string; Currency: string ;Balance: decimal }

let cashAccountCHF = { Type = "Cash"; Currency = "CHF"; Balance = 0M }
let bankAccountCHF = { Type = "Bank" ;Currency = "CHF"; Balance = 100M }
let bankAccountUSD = { Type = "Bank" ;Currency = "USD"; Balance = 0M }
let bankAccountEUR = { Type = "Bank" ;Currency = "EUR"; Balance = 0M }


let prepareResult (amount, clear) =
    if clear then
        Console.Clear()
    for i in 1 .. amount do
        printfn "--------------------------------"

let withdraw amount (bankAccountCHF: Account) (cashAccount: Account) (bankAccountUSD: Account) (bankAccountEUR: Account) =
    if bankAccountCHF.Balance >= amount then
        let newBankAccountCHF = { bankAccountCHF with Balance = bankAccountCHF.Balance - amount }
        let newCashAccountCHF = { cashAccountCHF with Balance = cashAccountCHF.Balance + amount }
        prepareResult(1, true)
        printfn "Withdrawn %M CHF from bank to cash" amount
        newBankAccountCHF, newCashAccountCHF, bankAccountUSD, bankAccountEUR
    else
        prepareResult(1, true)
        printfn "Insufficient funds in bank account"
        bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR

let deposit amount (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account) =
    if cashAccountCHF.Balance >= amount then
        let newBankAccountCHF = { bankAccountCHF with Balance = bankAccountCHF.Balance + amount }
        let newCashAccountCHF = { cashAccountCHF with Balance = cashAccountCHF.Balance - amount }
        prepareResult(1, true)
        printfn "Deposited %M CHF from cash to bank" amount
        newBankAccountCHF, newCashAccountCHF, bankAccountUSD, bankAccountEUR
    else
        prepareResult(1, true)
        printfn "Insufficient funds in cash account"
        bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR

let showBalances (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account) =
    prepareResult(1, true)
    printfn "Cash Account Balance:\nCHF:     %M" cashAccountCHF.Balance
    printfn "Bank Account Balance:"
    printfn "CHF:     %M\nUSD:     %M\nEUR:     %M" bankAccountCHF.Balance bankAccountUSD.Balance bankAccountEUR.Balance

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

let trade (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account)  =
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
    
    bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR

let main () =
    let bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR = loadAccounts()
    let rec loop (bankAccountCHF: Account) (cashAccountCHF: Account) (bankAccountUSD: Account) (bankAccountEUR: Account)=
        prepareResult(2, false)
        printf "1. Withdraw\n2. Deposit\n3. Currency Exchange\n4. Show Balances\n5. Save and Exit\n> "
        let input = Console.ReadLine()
        let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR =
            match input with
            | "1" -> 
                printf "Amount to withdraw: "
                let amount = decimal (Console.ReadLine())
                let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR = 
                    withdraw amount bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR
            | "2" -> 
                printf "Amount to deposit: "
                let amount = decimal (Console.ReadLine())
                let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR = 
                    deposit amount bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR
            | "3" -> 
                prepareResult(1, true)
                let newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR = 
                    trade bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                newBankAccountCHF, newCashAccountCHF, newBankAccountUSD, newBankAccountEUR
            | "4" -> 
                showBalances bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
            | "5" -> 
                saveAccounts bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR
                bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
            | _ -> 
                prepareResult(1, true)
                printfn "Invalid option"
                bankAccountCHF, cashAccountCHF, bankAccountUSD, bankAccountEUR
            
        if input <> "5" then 
            loop newBankAccountCHF newCashAccountCHF newBankAccountUSD newBankAccountEUR
    loop bankAccountCHF cashAccountCHF bankAccountUSD bankAccountEUR

main()