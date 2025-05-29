using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class TransactionsForm : Form
    {
        private DataGridView dgvTransactions;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        private Database db = new Database();

        public TransactionsForm()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Transactions";
            this.ClientSize = new System.Drawing.Size(800, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvTransactions = new DataGridView()
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(780, 300),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnAdd = new Button() { Text = "Add Transaction", Location = new System.Drawing.Point(10, 320) };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button() { Text = "Edit Transaction", Location = new System.Drawing.Point(130, 320) };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button() { Text = "Delete Transaction", Location = new System.Drawing.Point(250, 320) };
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(dgvTransactions);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }

        private void LoadTransactions()
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            t.TransactionID, t.TransactionDate, t.TransactionType, 
                            m.Name AS MemberName, b.Title AS BookTitle, a.AdminName AS LibrarianName,
                            t.DueDate, t.ReturnCondition, t.RenewedDueDate, t.ReserveUntil
                        FROM Transactions t
                        LEFT JOIN Members m ON t.MemberID = m.MemberID
                        LEFT JOIN Books b ON t.BookID = b.BookID
                        LEFT JOIN Admins a ON t.LibrarianID = a.AdminID";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            TransactionDetailsForm form = new TransactionDetailsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadTransactions();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a transaction to edit.");
                return;
            }

            int transactionId = Convert.ToInt32(dgvTransactions.SelectedRows[0].Cells["TransactionID"].Value);
            TransactionDetailsForm form = new TransactionDetailsForm(transactionId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadTransactions();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a transaction to delete.");
                return;
            }

            int transactionId = Convert.ToInt32(dgvTransactions.SelectedRows[0].Cells["TransactionID"].Value);

            var confirm = MessageBox.Show("Are you sure you want to delete this transaction?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = db.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM Transactions WHERE TransactionID = @transactionId";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@transactionId", transactionId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Transaction deleted successfully.");
                        LoadTransactions();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting transaction: " + ex.Message);
                }
            }
        }
    }
}
