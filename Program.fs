open System
open System.IO

type Account = { Name: string; mutable Balance: decimal }
type ExchangeRate = { FromCurrency: string; ToCurrency: string; Rate: decimal }

let cashAccount = { Name = "Cash"; Balance = 0M }
let bankAccount = { Name = "Bank"; Balance = 100M }

let withdraw amount =
    if bankAccount.Balance >= amount then
        bankAccount.Balance <- bankAccount.Balance - amount
        cashAccount.Balance <- cashAccount.Balance + amount
        printfn "----------------------"
        printfn "Withdrawn %M from bank to cash" amount
        printfn "----------------------"
        printfn "----------------------"
    else
        printfn "----------------------"
        printfn "Insufficient funds in bank account"
        printfn "----------------------"
        printfn "----------------------"

let deposit amount =
    if cashAccount.Balance >= amount then
        cashAccount.Balance <- cashAccount.Balance - amount
        bankAccount.Balance <- bankAccount.Balance + amount
        printfn "----------------------"
        printfn "Deposited %M from cash to bank" amount
        printfn "----------------------"
        printfn "----------------------"
    else
        printfn "----------------------"
        printfn "Insufficient funds in cash account"
        printfn "----------------------"
        printfn "----------------------"

let showBalances () =
    printfn "----------------------"
    printfn "Cash Account Balance: %M" cashAccount.Balance
    printfn "Bank Account Balance: %M" bankAccount.Balance
    printfn "----------------------"
    printfn "----------------------"

let saveAccounts () =
    let data = sprintf "%M,%M" cashAccount.Balance bankAccount.Balance
    File.WriteAllText("accounts.txt", data)
    printfn "Accounts saved"

let loadAccounts () =
    if File.Exists("accounts.txt") then
        let data = File.ReadAllText("accounts.txt").Split(',')
        cashAccount.Balance <- decimal data.[0]
        bankAccount.Balance <- decimal data.[1]
        printfn "Accounts loaded"
    else
        printfn "No saved accounts found"

let main () =
    loadAccounts()
    let rec loop () =
        printfn "1. Withdraw\n2. Deposit\n3. Show Balances\n4. Save and Exit"
        let input = Console.ReadLine()
        match input with
        | "1" -> printf "Amount to withdraw: "; withdraw (decimal (Console.ReadLine())); loop()
        | "2" -> printf "Amount to deposit: "; deposit (decimal (Console.ReadLine())); loop()
        | "3" -> showBalances(); loop()
        | "4" -> saveAccounts()
        | _ -> printfn "Invalid option\n----------------------"; loop()
    loop()

main()