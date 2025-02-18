using System;

namespace GICApplication
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }

    }
}
