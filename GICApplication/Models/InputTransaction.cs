using System;

namespace GICApplication
{
    public class InputTransaction
    {
        public DateTime TransactionDate { get; set; }
        public string AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public double Amount { get; set; }

    }
}
