using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class FinesForm : Form
    {
        private DataGridView dgvFines;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        private Database db = new Database();

        public FinesForm()
        {
            InitializeComponent();
            LoadFines();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Fines";
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvFines = new DataGridView()
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(680, 300),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnAdd = new Button() { Text = "Add Fine", Location = new System.Drawing.Point(10, 320) };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button() { Text = "Edit Fine", Location = new System.Drawing.Point(110, 320) };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button() { Text = "Delete Fine", Location = new System.Drawing.Point(210, 320) };
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(dgvFines);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }

        private void LoadFines()
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT f.FineID, t.TransactionID, m.Name AS MemberName, b.Title AS BookTitle,
                               f.Amount, f.IssueDate, f.PaymentStatus
                        FROM Fines f
                        LEFT JOIN Transactions t ON f.TransactionID = t.TransactionID
                        LEFT JOIN Members m ON t.MemberID = m.MemberID
                        LEFT JOIN Books b ON t.BookID = b.BookID";
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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            FineDetailsForm form = new FineDetailsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadFines();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvFines.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a fine to edit.");
                return;
            }

            int fineId = Convert.ToInt32(dgvFines.SelectedRows[0].Cells["FineID"].Value);
            FineDetailsForm form = new FineDetailsForm(fineId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadFines();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvFines.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a fine to delete.");
                return;
            }

            int fineId = Convert.ToInt32(dgvFines.SelectedRows[0].Cells["FineID"].Value);

            var confirm = MessageBox.Show("Are you sure you want to delete this fine?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = db.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM Fines WHERE FineID = @fineId";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@fineId", fineId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Fine deleted successfully.");
                        LoadFines();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting fine: " + ex.Message);
                }
            }
        }
    }
}
