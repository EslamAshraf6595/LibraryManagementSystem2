using System;

namespace LibraryManagementSystem2
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } // Borrow, Return, Renew, Reserve
        public int MemberID { get; set; }
        public int BookID { get; set; }
        public int LibrarianID { get; set; }
        public DateTime? DueDate { get; set; }
        public string ReturnCondition { get; set; }
        public DateTime? RenewedDueDate { get; set; }
        public DateTime? ReserveUntil { get; set; }

        public Transaction() { }

        public Transaction(int id, DateTime date, string type, int memberId, int bookId, int librarianId,
            DateTime? dueDate, string returnCondition, DateTime? renewedDate, DateTime? reserveUntil)
        {
            TransactionID = id;
            TransactionDate = date;
            TransactionType = type;
            MemberID = memberId;
            BookID = bookId;
            LibrarianID = librarianId;
            DueDate = dueDate;
            ReturnCondition = returnCondition;
            RenewedDueDate = renewedDate;
            ReserveUntil = reserveUntil;
        }
    }
}
