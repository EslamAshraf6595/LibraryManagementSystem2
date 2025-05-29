using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class MembersForm : Form
    {
        private DataGridView dgvMembers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        private Database db = new Database();

        public MembersForm()
        {
            InitializeComponent();
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Members";
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvMembers = new DataGridView()
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(680, 300),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnAdd = new Button() { Text = "Add Member", Location = new System.Drawing.Point(10, 320) };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button() { Text = "Edit Member", Location = new System.Drawing.Point(110, 320) };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button() { Text = "Delete Member", Location = new System.Drawing.Point(210, 320) };
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(dgvMembers);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }

        private void LoadMembers()
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT MemberID, Name, Address, Phone, Email, RegistrationDate FROM Members";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvMembers.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            MemberDetailsForm form = new MemberDetailsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMembers();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member to edit.");
                return;
            }

            int memberId = Convert.ToInt32(dgvMembers.SelectedRows[0].Cells["MemberID"].Value);
            MemberDetailsForm form = new MemberDetailsForm(memberId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMembers();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member to delete.");
                return;
            }

            int memberId = Convert.ToInt32(dgvMembers.SelectedRows[0].Cells["MemberID"].Value);

            var confirm = MessageBox.Show("Are you sure you want to delete this member?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = db.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM Members WHERE MemberID = @memberId";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@memberId", memberId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Member deleted successfully.");
                        LoadMembers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting member: " + ex.Message);
                }
            }
        }
    }
}
