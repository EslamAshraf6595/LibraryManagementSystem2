using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class MemberHistoryForm : Form
    {
        private int memberID;
        private DataGridView dgvTransactions, dgvFines;
        private Button btnPayFine;
        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public MemberHistoryForm(int memberID)
        {
            this.memberID = memberID;
            InitializeComponent();
            LoadTransactions();
            LoadFines();
        }

        private void InitializeComponent()
        {
            this.Text = "My Borrowing History and Fines";
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTransactions = new Label() { Text = "My Transactions:", Location = new System.Drawing.Point(20, 20) };
            dgvTransactions = new DataGridView()
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(850, 250),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Label lblFines = new Label() { Text = "My Fines:", Location = new System.Drawing.Point(20, 320) };
            dgvFines = new DataGridView()
            {
                Location = new System.Drawing.Point(20, 350),
                Size = new System.Drawing.Size(850, 180),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnPayFine = new Button() { Text = "Pay Selected Fine", Location = new System.Drawing.Point(20, 540) };
            btnPayFine.Click += BtnPayFine_Click;

            this.Controls.Add(lblTransactions);
            this.Controls.Add(dgvTransactions);
            this.Controls.Add(lblFines);
            this.Controls.Add(dgvFines);
            this.Controls.Add(btnPayFine);
        }

        private void LoadTransactions()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT t.TransactionID, t.TransactionDate, t.TransactionType, b.Title, t.DueDate, t.ReturnDate
                        FROM Transactions t
                        JOIN Books b ON t.BookID = b.BookID
                        WHERE t.MemberID = @memberID
                        ORDER BY t.TransactionDate DESC";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@memberID", memberID);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvTransactions.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transactions: " + ex.Message);
            }
        }

        private void LoadFines()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT f.FineID, f.Amount, f.IssueDate, f.PaymentStatus, t.TransactionID, b.Title
                        FROM Fines f
                        JOIN Transactions t ON f.TransactionID = t.TransactionID
                        JOIN Books b ON t.BookID = b.BookID
                        WHERE t.MemberID = @memberID
                        ORDER BY f.IssueDate DESC";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@memberID", memberID);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvFines.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading fines: " + ex.Message);
            }
        }

        private void BtnPayFine_Click(object sender, EventArgs e)
        {
            if (dgvFines.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a fine to pay.");
                return;
            }

            int fineID = Convert.ToInt32(dgvFines.SelectedRows[0].Cells["FineID"].Value);
            bool paymentStatus = Convert.ToBoolean(dgvFines.SelectedRows[0].Cells["PaymentStatus"].Value);

            if (paymentStatus)
            {
                MessageBox.Show("This fine has already been paid.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string updateFine = "UPDATE Fines SET PaymentStatus = TRUE WHERE FineID = @fineID";
                    MySqlCommand cmd = new MySqlCommand(updateFine, conn);
                    cmd.Parameters.AddWithValue("@fineID", fineID);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Fine payment successful!");
                    LoadFines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating fine payment: " + ex.Message);
            }
        }
    }
}
