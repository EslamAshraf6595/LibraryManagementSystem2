using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class ReturnBookForm : Form
    {
        private int transactionID;
        private Label lblCondition;
        private TextBox txtCondition;
        private Button btnReturn, btnCancel;
        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public ReturnBookForm(int transactionID)
        {
            this.transactionID = transactionID;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Return Book";
            this.ClientSize = new System.Drawing.Size(400, 180);
            this.StartPosition = FormStartPosition.CenterParent;

            lblCondition = new Label() { Text = "Return Condition:", Location = new System.Drawing.Point(20, 30) };
            txtCondition = new TextBox() { Location = new System.Drawing.Point(140, 25), Width = 220 };

            btnReturn = new Button() { Text = "Return Book", Location = new System.Drawing.Point(140, 70) };
            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(260, 70) };

            btnReturn.Click += BtnReturn_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblCondition);
            this.Controls.Add(txtCondition);
            this.Controls.Add(btnReturn);
            this.Controls.Add(btnCancel);
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            string condition = txtCondition.Text.Trim();

            if (string.IsNullOrEmpty(condition))
            {
                MessageBox.Show("Please enter the return condition.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // تحديث حالة الإرجاع
                    string updateTransaction = @"
                        UPDATE Transactions 
                        SET TransactionType = 'Return', ReturnCondition = @condition, ReturnDate = @returnDate
                        WHERE TransactionID = @transactionID";

                    MySqlCommand cmdUpdateTransaction = new MySqlCommand(updateTransaction, conn);
                    cmdUpdateTransaction.Parameters.AddWithValue("@condition", condition);
                    cmdUpdateTransaction.Parameters.AddWithValue("@transactionID", transactionID);
                    cmdUpdateTransaction.Parameters.AddWithValue("@returnDate", DateTime.Now.Date);
                    cmdUpdateTransaction.ExecuteNonQuery();

                    // جلب تاريخ الاستحقاق لحساب الغرامة
                    string getDueDateQuery = "SELECT DueDate FROM Transactions WHERE TransactionID = @transactionID";
                    MySqlCommand cmdGetDueDate = new MySqlCommand(getDueDateQuery, conn);
                    cmdGetDueDate.Parameters.AddWithValue("@transactionID", transactionID);
                    DateTime dueDate = Convert.ToDateTime(cmdGetDueDate.ExecuteScalar());

                    // حساب عدد أيام التأخير
                    int daysLate = (DateTime.Now.Date - dueDate.Date).Days;

                    if (daysLate > 0)
                    {
                        decimal fineAmount = daysLate * 5; // 5 جنيه لكل يوم تأخير

                        // إدخال سجل الغرامة
                        string insertFine = @"
                            INSERT INTO Fines (TransactionID, Amount, IssueDate, PaymentStatus) 
                            VALUES (@transactionID, @amount, @issueDate, 0)";

                        MySqlCommand cmdInsertFine = new MySqlCommand(insertFine, conn);
                        cmdInsertFine.Parameters.AddWithValue("@transactionID", transactionID);
                        cmdInsertFine.Parameters.AddWithValue("@amount", fineAmount);
                        cmdInsertFine.Parameters.AddWithValue("@issueDate", DateTime.Now.Date);
                        cmdInsertFine.ExecuteNonQuery();

                        MessageBox.Show($"Book returned with a fine of {fineAmount} due to {daysLate} days late.");
                    }
                    else
                    {
                        MessageBox.Show("Book returned on time. No fine issued.");
                    }

                    // تحديث عدد النسخ المتاحة للكتاب
                    string getBookIDQuery = "SELECT BookID FROM Transactions WHERE TransactionID = @transactionID";
                    MySqlCommand cmdGetBookID = new MySqlCommand(getBookIDQuery, conn);
                    cmdGetBookID.Parameters.AddWithValue("@transactionID", transactionID);
                    int bookID = Convert.ToInt32(cmdGetBookID.ExecuteScalar());

                    string updateBook = "UPDATE Books SET NumberOfCopies = NumberOfCopies + 1 WHERE BookID = @bookID";
                    MySqlCommand cmdUpdateBook = new MySqlCommand(updateBook, conn);
                    cmdUpdateBook.Parameters.AddWithValue("@bookID", bookID);
                    cmdUpdateBook.ExecuteNonQuery();

                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error returning book: " + ex.Message);
            }
        }
    }
}
