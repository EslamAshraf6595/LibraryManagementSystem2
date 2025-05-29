using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class BorrowBookForm : Form
    {
        private ComboBox cmbMembers, cmbBooks;
        private DateTimePicker dtpDueDate;
        private Button btnBorrow, btnCancel;

        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public BorrowBookForm(int bookID)
        {
            InitializeComponent();
            LoadMembers();
            LoadAvailableBooks();
        }

        private void InitializeComponent()
        {
            this.Text = "Borrow Book";
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblMember = new Label() { Text = "Member:", Location = new System.Drawing.Point(20, 30) };
            cmbMembers = new ComboBox() { Location = new System.Drawing.Point(120, 25), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblBook = new Label() { Text = "Book:", Location = new System.Drawing.Point(20, 70) };
            cmbBooks = new ComboBox() { Location = new System.Drawing.Point(120, 65), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblDueDate = new Label() { Text = "Due Date:", Location = new System.Drawing.Point(20, 110) };
            dtpDueDate = new DateTimePicker() { Location = new System.Drawing.Point(120, 105), Width = 240 };

            btnBorrow = new Button() { Text = "Borrow", Location = new System.Drawing.Point(120, 150) };
            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(220, 150) };

            btnBorrow.Click += BtnBorrow_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblMember);
            this.Controls.Add(cmbMembers);
            this.Controls.Add(lblBook);
            this.Controls.Add(cmbBooks);
            this.Controls.Add(lblDueDate);
            this.Controls.Add(dtpDueDate);
            this.Controls.Add(btnBorrow);
            this.Controls.Add(btnCancel);
        }

        private void LoadMembers()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT MemberID, Name FROM Members";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbMembers.Items.Add(new ComboboxItem(reader.GetString("Name"), reader.GetInt32("MemberID")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message);
            }
        }

        private void LoadAvailableBooks()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT BookID, Title FROM Books WHERE AvailabilityStatus = TRUE AND NumberOfCopies > 0";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbBooks.Items.Add(new ComboboxItem(reader.GetString("Title"), reader.GetInt32("BookID")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading books: " + ex.Message);
            }
        }

        private void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (cmbMembers.SelectedItem == null || cmbBooks.SelectedItem == null)
            {
                MessageBox.Show("Please select both member and book.");
                return;
            }

            var memberItem = (ComboboxItem)cmbMembers.SelectedItem;
            var bookItem = (ComboboxItem)cmbBooks.SelectedItem;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Insert transaction
                    string query = @"INSERT INTO Transactions (TransactionDate, TransactionType, MemberID, BookID, DueDate)
                                     VALUES (@date, 'Borrow', @memberID, @bookID, @dueDate)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@memberID", memberItem.Value);
                    cmd.Parameters.AddWithValue("@bookID", bookItem.Value);
                    cmd.Parameters.AddWithValue("@dueDate", dtpDueDate.Value.Date);

                    cmd.ExecuteNonQuery();

                    // Update book copies
                    string updateBook = @"UPDATE Books SET NumberOfCopies = NumberOfCopies - 1 WHERE BookID = @bookID";
                    MySqlCommand cmdUpdate = new MySqlCommand(updateBook, conn);
                    cmdUpdate.Parameters.AddWithValue("@bookID", bookItem.Value);
                    cmdUpdate.ExecuteNonQuery();

                    MessageBox.Show("Book borrowed successfully.");
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during borrowing: " + ex.Message);
            }
        }
    }

    // Helper class for combobox items with text and value
    public class ComboboxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public ComboboxItem(string text, int value)
        {
            Text = text;
            Value = value;
        }
        public override string ToString() => Text;
    }
}
