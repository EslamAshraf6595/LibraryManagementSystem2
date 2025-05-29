using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class TransactionManagementForm : Form
    {
        private DataGridView dgvTransactions;
        private Button btnAddTransaction, btnReturnBook;
        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public TransactionManagementForm()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Transactions";
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvTransactions = new DataGridView()
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(850, 400),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvTransactions);

            btnAddTransaction = new Button() { Text = "Add Transaction", Location = new System.Drawing.Point(20, 440) };
            btnAddTransaction.Click += BtnAddTransaction_Click;
            this.Controls.Add(btnAddTransaction);

            btnReturnBook = new Button() { Text = "Return Selected Book", Location = new System.Drawing.Point(150, 440) };
            btnReturnBook.Click += BtnReturnBook_Click;
            this.Controls.Add(btnReturnBook);
        }

        private void LoadTransactions()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT t.TransactionID, t.TransactionDate, t.TransactionType, m.Name AS MemberName, b.Title AS BookTitle, t.DueDate, t.ReturnDate
                        FROM Transactions t
                        JOIN Members m ON t.MemberID = m.MemberID
                        JOIN Books b ON t.BookID = b.BookID
                        ORDER BY t.TransactionDate DESC";

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

        private void BtnAddTransaction_Click(object sender, EventArgs e)
        {
            AddTransactionForm addForm = new AddTransactionForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadTransactions();
            }
        }

        private void BtnReturnBook_Click(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a transaction to return.");
                return;
            }

            int transactionID = Convert.ToInt32(dgvTransactions.SelectedRows[0].Cells["TransactionID"].Value);

            ReturnBookForm returnForm = new ReturnBookForm(transactionID);
            if (returnForm.ShowDialog() == DialogResult.OK)
            {
                LoadTransactions();
            }
        }
    }
}
