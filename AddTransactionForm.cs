using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class AddTransactionForm : Form
    {
        private ComboBox cmbMembers, cmbBooks, cmbTransactionType;
        private DateTimePicker dtpDueDate, dtpReserveUntil;
        private Label lblMember, lblBook, lblTransactionType, lblDueDate, lblReserveUntil;
        private Button btnAdd, btnCancel;

        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public AddTransactionForm()
        {
            InitializeComponent();
            LoadMembers();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.Text = "Add Transaction";
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            lblMember = new Label() { Text = "Member:", Location = new System.Drawing.Point(20, 20) };
            cmbMembers = new ComboBox() { Location = new System.Drawing.Point(150, 20), Width = 200 };

            lblBook = new Label() { Text = "Book:", Location = new System.Drawing.Point(20, 60) };
            cmbBooks = new ComboBox() { Location = new System.Drawing.Point(150, 60), Width = 200 };

            lblTransactionType = new Label() { Text = "Transaction Type:", Location = new System.Drawing.Point(20, 100) };
            cmbTransactionType = new ComboBox() { Location = new System.Drawing.Point(150, 100), Width = 200 };
            cmbTransactionType.Items.AddRange(new string[] { "Borrow", "Renew", "Reserve" });
            cmbTransactionType.SelectedIndexChanged += CmbTransactionType_SelectedIndexChanged;

            lblDueDate = new Label() { Text = "Due Date:", Location = new System.Drawing.Point(20, 140) };
            dtpDueDate = new DateTimePicker() { Location = new System.Drawing.Point(150, 140), Width = 200 };

            lblReserveUntil = new Label() { Text = "Reserve Until:", Location = new System.Drawing.Point(20, 180), Visible = false };
            dtpReserveUntil = new DateTimePicker() { Location = new System.Drawing.Point(150, 180), Width = 200, Visible = false };

            btnAdd = new Button() { Text = "Add", Location = new System.Drawing.Point(150, 230) };
            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(260, 230) };

            btnAdd.Click += BtnAdd_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblMember);
            this.Controls.Add(cmbMembers);
            this.Controls.Add(lblBook);
            this.Controls.Add(cmbBooks);
            this.Controls.Add(lblTransactionType);
            this.Controls.Add(cmbTransactionType);
            this.Controls.Add(lblDueDate);
            this.Controls.Add(dtpDueDate);
            this.Controls.Add(lblReserveUntil);
            this.Controls.Add(dtpReserveUntil);
            this.Controls.Add(btnAdd);
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
                    MySqlDataReader reader = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    cmbMembers.DataSource = dt;
                    cmbMembers.DisplayMember = "Name";
                    cmbMembers.ValueMember = "MemberID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message);
            }
        }

        private void LoadBooks()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT BookID, Title FROM Books WHERE AvailabilityStatus = TRUE AND NumberOfCopies > 0";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    cmbBooks.DataSource = dt;
                    cmbBooks.DisplayMember = "Title";
                    cmbBooks.ValueMember = "BookID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading books: " + ex.Message);
            }
        }

        private void CmbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTransactionType.SelectedItem.ToString() == "Reserve")
            {
                lblReserveUntil.Visible = true;
                dtpReserveUntil.Visible = true;
            }
            else
            {
                lblReserveUntil.Visible = false;
                dtpReserveUntil.Visible = false;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbMembers.SelectedIndex == -1 || cmbBooks.SelectedIndex == -1 || cmbTransactionType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int memberID = (int)cmbMembers.SelectedValue;
            int bookID = (int)cmbBooks.SelectedValue;
            string transactionType = cmbTransactionType.SelectedItem.ToString();
            DateTime transactionDate = DateTime.Now.Date;
            DateTime? dueDate = null;
            DateTime? reserveUntil = null;

            if (transactionType == "Borrow" || transactionType == "Renew")
            {
                dueDate = dtpDueDate.Value.Date;
            }
            else if (transactionType == "Reserve")
            {
                reserveUntil = dtpReserveUntil.Value.Date;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO Transactions 
                        (TransactionDate, TransactionType, MemberID, BookID, DueDate, ReserveUntil)
                        VALUES (@transactionDate, @transactionType, @memberID, @bookID, @dueDate, @reserveUntil)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@transactionDate", transactionDate);
                    cmd.Parameters.AddWithValue("@transactionType", transactionType);
                    cmd.Parameters.AddWithValue("@memberID", memberID);
                    cmd.Parameters.AddWithValue("@bookID", bookID);
                    cmd.Parameters.AddWithValue("@dueDate", (object)dueDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@reserveUntil", (object)reserveUntil ?? DBNull.Value);

                    cmd.ExecuteNonQuery();

                    // تحديث عدد النسخ المتوفرة في حالة الاستعارة
                    if (transactionType == "Borrow")
                    {
                        string updateBook = "UPDATE Books SET NumberOfCopies = NumberOfCopies - 1 WHERE BookID = @bookID";
                        MySqlCommand cmdUpdateBook = new MySqlCommand(updateBook, conn);
                        cmdUpdateBook.Parameters.AddWithValue("@bookID", bookID);
                        cmdUpdateBook.ExecuteNonQuery();
                    }

                    MessageBox.Show("Transaction added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding transaction: " + ex.Message);
            }
        }
    }
}
