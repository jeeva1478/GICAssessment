using System.Collections.Generic;

namespace GICApplication
{
    public class Account
    {
        public string AccountNumber { get; set; }
        public double AvailableBalance { get; set; }
        public List<Transaction> Transactions { get; set; }

    }
}
