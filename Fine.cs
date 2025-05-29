using System;

namespace LibraryManagementSystem2
{
    public class Fine
    {
        public int FineID { get; set; }
        public int TransactionID { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public bool PaymentStatus { get; set; }

        public Fine() { }

        public Fine(int fineID, int transactionID, decimal amount, DateTime issueDate, bool paid)
        {
            FineID = fineID;
            TransactionID = transactionID;
            Amount = amount;
            IssueDate = issueDate;
            PaymentStatus = paid;
        }

        public override string ToString()
        {
            return $"Fine: {Amount:C} (Paid: {PaymentStatus}) - Issued on {IssueDate:yyyy-MM-dd}";
        }
    }
}
