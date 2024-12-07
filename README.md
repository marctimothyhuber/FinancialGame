# FinancialGame
## Motivation
This Project is part of my seminar *Programming Languages and Paradigms*. In this course I decided on learning more about F# since it enhances my understanding of functional programming and was a language that i wasn't already familiar with.

## Usage
To start the program enter "dotnet run" while you are in the main folder.

As soon as the program runs you can see what you are able to do in the console.
- Withdrawing is only possible in your "CHF" Accounts.
- Deposit is only possible in your "CHF" Accounts.
- Currency exchange is only possible between bank accounts. It uses a API to get the current exchange rates.
- Coin Toss is a "Casino Game". This is only available with your Cash account.
- Showing the Balances is used to see how your current account balances are.
- Saving and exiting the program stores your current accounts to a file so you can continue at the same point later on.

## API Used
For the submission I decided leaving my API key in the program so you can test my solution without having to create your own API key.

How you would add your own API key:

Create an account for the following API.

You get 5000 API calls per month for free.

*https://app.freecurrencyapi.com/dashboard*

Enter your API key with the full URL in to a "APIKEY.txt" file as follows:

*https://api.freecurrencyapi.com/v1/latest?apikey=YOURKEYHERE*


## Nice to haves implemented
# Gambling
For Gambling I implemented a street gambling game. A Coin Toss for double or nothing. This is only possible with your cash account since its like "casino/street" gambling.

## Unit Testing
I tried implementing unit tests with xUnit, nUnit and Expecto.
xUnit and nUnit are those trying to integrate into your IDE's testing suite while Expecto is running purely in the console.

Unfortunately F# has such bad Error handling that even after trying for several days, I wasn't able to fix the problems and had to submit the project without any tests. This was a hard decision to make since you told me you wished for unit tests but at some point I had to start learning for other exams since the first one is already one week after the deadline for this assignment.

# Things i've tried
First I decided to implement xUnit. This seemed partical since it is a well established testing solution for .NET languages. I first setup a new testing application and tryed to run the tests. Unfortunately I tried to import my F# Project to run the tests but this didn't work as I hoped. I searched on the F# forum, stackoverflow and asked about 10000 questions to chatgpt (not really that many but it felt like waaaay to many prompts...) The solutions I found couldn't help me and after that I decided to try the same thing with nUnit since this also was described as a good choice for F# and I thought maybe I can fix this issue with another testing solution. Well arround 12 hours (in several days) of fidgeting around with those two I thought I would give up. 

Then I had the idea to break the laws of good practice and make the unittest inside my project. Here I also tried both version and it seemed I am getting closer to it actually working. But the issue I had that my normal program didn't work anymore so you could no longer execute it and then some I had some other issues. Even though I finally could import the program I coudln't write executable testcases into the testfile but I couldn't find out why it wasn't executable... I tried asking chatgpt, looking in forums but I didn't find anything..

Last but not least i tryed implementing Expecto. I thought leaving the standard solutions and trying something else may help. But to be honest I was already so frustrated and had to start learning for other courses that as soon as I reached some similar issues that I faced before I already lost the motivation to continue working on it. Finally I decided to give up and hope that my solution was sufficient even without implementing unittests..

In total I spent about 2 weeks (not full days but around 1-2h a day) trying to find solutions that may be useful but none od them have actually worked.