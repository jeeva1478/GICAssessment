using System;
using System.Collections.Generic;
using System.Linq;

namespace GICApplication
{
    public class GICBankRepository : IGICBankRepository
    {
        /* Declaration */
        private static List<Account> accounts = new List<Account>();
        private static List<InterestRule> interestRule = new List<InterestRule>();

        public void AddTransactions(InputTransaction inputData)
        {
            // Fetch or create an account
            var account = accounts.FirstOrDefault(x => x.AccountNumber == inputData.AccountNumber);

            // Validate transaction
            if (!IsValidTransaction(inputData, account)) return;

            // Determine new balance
            double balance = account?.AvailableBalance ?? 0;
            double newBalance = inputData.TransactionType.ToUpper() == "W" ? balance - inputData.Amount : balance + inputData.Amount;

            var transaction = new Transaction
            {
                TransactionId = GetTransactionId(inputData.TransactionDate, inputData.TransactionType, account),
                AccountNumber = inputData.AccountNumber,
                Amount = inputData.Amount,
                TransactionDate = inputData.TransactionDate,
                TransactionType = inputData.TransactionType,
                Balance = newBalance
            };

            // Add or update account
            if (account == null)
            {
                accounts.Add(new Account
                {
                    AccountNumber = inputData.AccountNumber,
                    AvailableBalance = newBalance,
                    Transactions = new List<Transaction> { transaction }
                });
            }
            else
            {
                account.AvailableBalance = newBalance;
                account.Transactions.Add(transaction);
            }
        }

        public void AddInterestRules(InputRule inputData)
        {
            if (inputData.Rate < 0)
            {
                Console.WriteLine("Invalid interest rate.");
                return;
            }

            interestRule.Add(new InterestRule
            {
                Date = inputData.RuleDate,
                RuleId = inputData.RuleId,
                Rate = inputData.Rate
            });
        }

        public void PrintStatement(InputPrint inputData)
        {
            var account = accounts.FirstOrDefault(x => x.AccountNumber == inputData.AccountNumber);
            if (account == null || account.Transactions == null || !account.Transactions.Any())
            {
                Console.WriteLine("No transactions found for this account.");
                return;
            }

            var transactions = account.Transactions
                .Where(t => t.TransactionDate.ToString("yyyyMM") == inputData.MonthYear.ToString("yyyyMM"))
                .ToList();

            if (!transactions.Any())
            {
                Console.WriteLine("No transactions found for the selected month.");
                return;
            }

            // Calculate and add interest transaction
            CalculateInterest(transactions, inputData.MonthYear, account);

            Console.WriteLine($"Account: {inputData.AccountNumber}");
            Console.WriteLine("| {0,-10} | {1,-11} | {2,-6} | {3,-10} | {4,-10} |", "Date", "Txn Id", "Type", "Amount", "Balance");
            Console.WriteLine(new string('-', 60));

            foreach (var txn in transactions)
            {
                Console.WriteLine("| {0,-10} | {1,-11} | {2,-6} | {3,10:N2} | {4,10:N2} |",
                    txn.TransactionDate.ToString("yyyyMMdd"),
                    txn.TransactionId,
                    txn.TransactionType,
                    txn.Amount,
                    txn.Balance);
            }
        }

        private string GetTransactionId(DateTime transactionDate, string transactionType, Account? account)
        {
            int count = account?.Transactions.Count(t => t.TransactionType.Equals(transactionType, StringComparison.OrdinalIgnoreCase)) ?? 0;
            return $"{transactionDate:yyyyMMdd}-{count + 1:00}";
        }

        private bool IsValidTransaction(InputTransaction transaction, Account? account)
        {
            if (transaction == null)
            {
                Console.WriteLine("Invalid transaction format.");
                return false;
            }

            if (account == null && transaction.TransactionType.Equals("W", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Withdrawal is not allowed from a zero balance account.");
                return false;
            }

            if (account != null && transaction.TransactionType.Equals("W", StringComparison.OrdinalIgnoreCase) && account.AvailableBalance < transaction.Amount)
            {
                Console.WriteLine("Insufficient funds for withdrawal.");
                return false;
            }

            if (transaction.Amount <= 0)
            {
                Console.WriteLine("Invalid transaction amount.");
                return false;
            }

            return true;
        }

        private void CalculateInterest(List<Transaction> transactions, DateTime statementMonthYear, Account account)
        {
            if (!transactions.Any() || !interestRule.Any()) return;

            double totalInterest = 0.0;

            // get number of days for given month & year
            DateTime statementDate = statementMonthYear;
            int totalDays = DateTime.DaysInMonth(statementDate.Year, statementDate.Month);

            for (int iCnt = 0; iCnt < transactions.Count; iCnt++)
            {
                // calculate start and end date for each transaction
                DateTime startDate = transactions[iCnt].TransactionDate;
                DateTime endDate = iCnt == transactions.Count - 1 ? statementDate.AddDays(totalDays) : transactions[iCnt + 1].TransactionDate;

                // get interest rate & amount for each day
                for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
                {
                    double rate = interestRule?.Where(r => r.Date <= date).OrderByDescending(r => r.Date).FirstOrDefault()?.Rate ?? 0.0;

                    // calculate interest amount
                    double dailyInterest = transactions[iCnt]?.Balance * (rate / 100) ?? 0.0;
                    totalInterest += Math.Round(dailyInterest, 2);
                }
            }

            // total interest amount
            var totalInterestAmount = Math.Round(totalInterest / 365, 2);

            // add interest entry in the transaction list
            transactions.Add(new Transaction
            {
                AccountNumber = account.AccountNumber,
                TransactionDate = statementDate.AddDays(totalDays - 1),
                Amount = totalInterestAmount,
                Balance = account.AvailableBalance + totalInterestAmount,
                TransactionType = Enum.GetName(typeof(TransactionType), TransactionType.I)
            });
        }
    }
}
