open System
open System.IO

type Account = { Name: string; Balance: decimal }

let cashAccount = { Name = "Cash"; Balance = 0M }
let bankAccount = { Name = "Bank"; Balance = 100M }

let prepareResult (amount, clear) =
    if clear then
        Console.Clear()
    for i in 1 .. amount do
        printfn "--------------------------------"

let withdraw amount (bankAccount: Account) (cashAccount: Account) =
    if bankAccount.Balance >= amount then
        let newBankAccount = { bankAccount with Balance = bankAccount.Balance - amount }
        let newCashAccount = { cashAccount with Balance = cashAccount.Balance + amount }
        prepareResult(1, true)
        printfn "Withdrawn %M from bank to cash" amount
        newBankAccount, newCashAccount
    else
        prepareResult(1, true)
        printfn "Insufficient funds in bank account"
        bankAccount, cashAccount

let deposit amount (bankAccount: Account) (cashAccount: Account) =
    if cashAccount.Balance >= amount then
        let newBankAccount = { bankAccount with Balance = bankAccount.Balance + amount }
        let newCashAccount = { cashAccount with Balance = cashAccount.Balance - amount }
        prepareResult(1, true)
        printfn "Deposited %M from cash to bank" amount
        newBankAccount, newCashAccount
    else
        prepareResult(1, true)
        printfn "Insufficient funds in cash account"
        bankAccount, cashAccount

let showBalances (bankAccount: Account) (cashAccount: Account) =
    prepareResult(1, true)
    printfn "Cash Account Balance: %M" cashAccount.Balance
    printfn "Bank Account Balance: %M" bankAccount.Balance

let saveAccounts (bankAccount: Account) (cashAccount: Account) =
    let data = sprintf "%M,%M" cashAccount.Balance bankAccount.Balance
    File.WriteAllText("accounts.txt", data)
    prepareResult(1, true)
    printfn "Accounts saved"
    prepareResult(2, false)

let loadAccounts () =
    if File.Exists("accounts.txt") then
        let data = File.ReadAllText("accounts.txt").Split(',')
        let cashAccount = { Name = "Cash"; Balance = decimal data.[0] }
        let bankAccount = { Name = "Bank"; Balance = decimal data.[1] }
        prepareResult(1, true)
        printfn "Accounts loaded"
        bankAccount, cashAccount
    else
        prepareResult(1, true)
        printfn "No saved accounts found"
        bankAccount, cashAccount

let main () =
    let bankAccount, cashAccount = loadAccounts()
    let rec loop (bankAccount: Account) (cashAccount: Account) =
        prepareResult(2, false)
        printf "1. Withdraw\n2. Deposit\n3. Show Balances\n4. Save and Exit\n> "
        let input = Console.ReadLine()
        let newBankAccount, newCashAccount =
            match input with
            | "1" -> printf "Amount to withdraw: "; withdraw (decimal (Console.ReadLine())) bankAccount cashAccount
            | "2" -> printf "Amount to deposit: "; deposit (decimal (Console.ReadLine())) bankAccount cashAccount
            | "3" -> showBalances bankAccount cashAccount; bankAccount, cashAccount
            | "4" -> saveAccounts bankAccount cashAccount; bankAccount, cashAccount
            | _ -> prepareResult(1, true); printfn "Invalid option"; bankAccount, cashAccount
        if input <> "4" then loop newBankAccount newCashAccount
    loop bankAccount cashAccount

main()