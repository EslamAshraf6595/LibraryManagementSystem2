using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class FineManagementForm : Form
    {
        private DataGridView dgvFines;
        private Button btnMarkPaid;
        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public FineManagementForm()
        {
            InitializeComponent();
            LoadFines();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Fines";
            this.ClientSize = new System.Drawing.Size(900, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvFines = new DataGridView()
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(850, 350),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvFines);

            btnMarkPaid = new Button() { Text = "Mark Selected Fine as Paid", Location = new System.Drawing.Point(20, 380) };
            btnMarkPaid.Click += BtnMarkPaid_Click;
            this.Controls.Add(btnMarkPaid);
        }

        private void LoadFines()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT f.FineID, f.Amount, f.IssueDate, f.PaymentStatus,
                               t.TransactionID, m.Name AS MemberName, b.Title AS BookTitle
                        FROM Fines f
                        JOIN Transactions t ON f.TransactionID = t.TransactionID
                        JOIN Members m ON t.MemberID = m.MemberID
                        JOIN Books b ON t.BookID = b.BookID
                        ORDER BY f.IssueDate DESC";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
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

        private void BtnMarkPaid_Click(object sender, EventArgs e)
        {
            if (dgvFines.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a fine to mark as paid.");
                return;
            }

            int fineID = Convert.ToInt32(dgvFines.SelectedRows[0].Cells["FineID"].Value);
            bool paymentStatus = Convert.ToBoolean(dgvFines.SelectedRows[0].Cells["PaymentStatus"].Value);

            if (paymentStatus)
            {
                MessageBox.Show("This fine is already marked as paid.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string updateQuery = "UPDATE Fines SET PaymentStatus = TRUE WHERE FineID = @fineID";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@fineID", fineID);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Fine marked as paid successfully.");
                    LoadFines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating fine status: " + ex.Message);
            }
        }
    }
}
