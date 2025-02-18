using System;

namespace GICApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;

            // Create an instance of the repository
            var repository = new GICBankRepository();

            // Create the TransactionProcessor
            var transactionService = new GICBankService(repository);

            do
            {
                // options
                Console.WriteLine("\n=======================================================");
                Console.WriteLine("\nWelcome to Awesome GIC Bank! What would you like to do?");
                Console.WriteLine("\n======================================================="); 
                Console.WriteLine("[T] Input transactions");
                Console.WriteLine("[I] Define interest rules");
                Console.WriteLine("[P] Print statement");
                Console.WriteLine("[Q] Quit");

                // get user's input value
                Console.Write("Enter your choice: ");
                string input = Console.ReadLine()?.Trim().ToUpper();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Invalid input. Please enter a valid option.");
                    continue;
                }

                // exeucte the method based on user's input
                switch (input)
                {
                    case "T":
                        Console.Write("Please enter transaction details in <Date> <AccountNo> <Type> <Amount> format");
                        Console.Write("\r\n(or enter blank to go back to main menu) :");
                        transactionService.AddTransactions(Console.ReadLine().ToUpper());
                        Console.WriteLine("Transaction has been added successfully.");
                        break;
                    case "I":
                        Console.Write("\nPlease enter interest rules details in <Date> <RuleId> <Rate in %> format");
                        Console.Write("\r\n(or enter blank to go back to main menu) :");
                        transactionService.AddInterestRules(Console.ReadLine().ToUpper());
                        Console.WriteLine("Interest Rule has been added successfully.");
                        break;
                    case "P":
                        Console.Write("\nPlease enter account and month to generate the statement <Account> <Year><Month>");
                        Console.Write("\r\n(or enter blank to go back to main menu) :");
                        transactionService.PrintStatement(Console.ReadLine().ToUpper());
                        break;
                    case "Q":
                        Console.WriteLine("Thank you for using AwesomeGIC Bank. Have a nice day!");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            } while (!exit);

        }
    }
}
