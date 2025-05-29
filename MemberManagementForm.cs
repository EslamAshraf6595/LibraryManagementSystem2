using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class MemberManagementForm : Form
    {
        private DataGridView dgvMembers;
        private Button btnAddMember, btnEditMember, btnDeleteMember;
        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public MemberManagementForm()
        {
            InitializeComponent();
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.Text = "Member Management";
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvMembers = new DataGridView()
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(760, 380),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvMembers);

            btnAddMember = new Button() { Text = "Add Member", Location = new System.Drawing.Point(20, 420) };
            btnEditMember = new Button() { Text = "Edit Member", Location = new System.Drawing.Point(120, 420) };
            btnDeleteMember = new Button() { Text = "Delete Member", Location = new System.Drawing.Point(220, 420) };

            btnAddMember.Click += BtnAddMember_Click;
            btnEditMember.Click += BtnEditMember_Click;
            btnDeleteMember.Click += BtnDeleteMember_Click;

            this.Controls.Add(btnAddMember);
            this.Controls.Add(btnEditMember);
            this.Controls.Add(btnDeleteMember);
        }

        private void LoadMembers()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT MemberID, Name, Address, Phone, Email, RegistrationDate FROM Members";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
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

        private void BtnAddMember_Click(object sender, EventArgs e)
        {
            MemberEditForm editForm = new MemberEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadMembers();
            }
        }

        private void BtnEditMember_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member to edit.");
                return;
            }

            int memberID = Convert.ToInt32(dgvMembers.SelectedRows[0].Cells["MemberID"].Value);
            MemberEditForm editForm = new MemberEditForm(memberID);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadMembers();
            }
        }

        private void BtnDeleteMember_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member to delete.");
                return;
            }

            int memberID = Convert.ToInt32(dgvMembers.SelectedRows[0].Cells["MemberID"].Value);
            var result = MessageBox.Show("Are you sure you want to delete this member?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Members WHERE MemberID = @memberID";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@memberID", memberID);
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
