open System
open System.IO

type Account = { Name: string; mutable Balance: decimal }

let cashAccount = { Name = "Cash"; Balance = 0M }
let bankAccount = { Name = "Bank"; Balance = 100M }

let prepareResult (amount, clear) =
    if clear then
        Console.Clear()
    for i in 1 .. amount do
        printfn "--------------------------------"

let withdraw amount =
    if bankAccount.Balance >= amount then
        bankAccount.Balance <- bankAccount.Balance - amount
        cashAccount.Balance <- cashAccount.Balance + amount
        prepareResult(1, true)
        printfn "Withdrawn %M from bank to cash" amount
    else
        prepareResult(1, true)
        printfn "Insufficient funds in bank account"

let deposit amount =
    if cashAccount.Balance >= amount then
        cashAccount.Balance <- cashAccount.Balance - amount
        bankAccount.Balance <- bankAccount.Balance + amount
        prepareResult(1, true)
        printfn "Deposited %M from cash to bank" amount
    else
        prepareResult(1, true)
        printfn "Insufficient funds in cash account"

let showBalances () =
    prepareResult(1, true)
    printfn "Cash Account Balance: %M" cashAccount.Balance
    printfn "Bank Account Balance: %M" bankAccount.Balance

let saveAccounts () =
    let data = sprintf "%M,%M" cashAccount.Balance bankAccount.Balance
    File.WriteAllText("accounts.txt", data)
    prepareResult(1, true)
    printfn "Accounts saved"
    prepareResult(2, false)

let loadAccounts () =
    if File.Exists("accounts.txt") then
        let data = File.ReadAllText("accounts.txt").Split(',')
        cashAccount.Balance <- decimal data.[0]
        bankAccount.Balance <- decimal data.[1]
        prepareResult(1, true)
        printfn "Accounts loaded"
    else
        prepareResult(1, true)
        printfn "No saved accounts found"

let main () =
    loadAccounts()
    let rec loop () =
        prepareResult(2, false)
        printf "1. Withdraw\n2. Deposit\n3. Show Balances\n4. Save and Exit\n> "
        let input = Console.ReadLine()
        match input with
        | "1" -> printf "Amount to withdraw: "; withdraw (decimal (Console.ReadLine())); loop()
        | "2" -> printf "Amount to deposit: "; deposit (decimal (Console.ReadLine())); loop()
        | "3" -> showBalances(); loop()
        | "4" -> saveAccounts()
        | _ -> prepareResult(1, true); printfn "Invalid option" ; loop()
    loop()

main()