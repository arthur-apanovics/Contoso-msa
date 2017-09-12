using System;

namespace ContosoData.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public int AccountId { get; set; }
        public string TransactionType { get; set; }
        public float Amount { get; set; }
        public string RecepientName { get; set; }
        public string RecepientAccountNumber { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }
}
