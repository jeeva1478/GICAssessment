using System;
using System.Globalization;

namespace GICApplication
{
    public class GICBankService
    {
        private readonly IGICBankRepository _transactionRepository;

        public GICBankService(IGICBankRepository transactionRepository)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        public void AddTransactions(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Transaction data is empty or null.");
                return;
            }

            string[] transactionDetails = input.Split(' ');

            if (transactionDetails.Length < 4)
            {
                Console.WriteLine("Incorrect transaction data, expected format: <Date> <Account> <Type> <Amount>.");
                return;
            }

            if (!DateTime.TryParseExact(transactionDetails[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime transactionDate))
            {
                Console.WriteLine("Invalid date format. Expected format: yyyyMMdd.");
                return;
            }

            if (!double.TryParse(transactionDetails[3], out double amount))
            {
                Console.WriteLine("Invalid amount format. Please enter a valid number.");
                return;
            }

            var transaction = new InputTransaction
            {
                TransactionDate = transactionDate,
                AccountNumber = transactionDetails[1],
                TransactionType = transactionDetails[2],
                Amount = amount
            };

            try
            {
                _transactionRepository.AddTransactions(transaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding transaction: {ex.Message}");
            }
        }

        public void AddInterestRules(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Interest rule data is empty or null.");
                return;
            }

            string[] interestRules = input.Split(' ');

            if (interestRules.Length < 3)
            {
                Console.WriteLine("Incorrect interest rule data, expected format: <Date> <RuleId> <Rate in %>.");
                return;
            }

            if (!DateTime.TryParseExact(interestRules[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ruleDate))
            {
                Console.WriteLine("Invalid date format. Expected format: yyyyMMdd.");
                return;
            }

            if (!double.TryParse(interestRules[2], out double rate))
            {
                Console.WriteLine("Invalid rate format. Please enter a valid percentage.");
                return;
            }

            var rule = new InputRule
            {
                RuleId = interestRules[1],
                RuleDate = ruleDate,
                Rate = rate
            };

            try
            {
                _transactionRepository.AddInterestRules(rule);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding interest rule: {ex.Message}");
            }
        }

        public void PrintStatement(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Print statement data is empty or null.");
                return;
            }

            string[] print = input.Split(' ');

            if (print.Length < 2)
            {
                Console.WriteLine("Incorrect print data, expected format: <Account> <Year><Month>.");
                return;
            }

            if (!DateTime.TryParseExact(print[1], "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime monthYear))
            {
                Console.WriteLine("Invalid date format. Expected format: yyyyMM.");
                return;
            }

            var printStatement = new InputPrint
            {
                AccountNumber = print[0],
                MonthYear = monthYear
            };

            try
            {
                _transactionRepository.PrintStatement(printStatement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error printing statement: {ex.Message}");
            }
        }
    }
}
