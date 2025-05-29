using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class MemberEditForm : Form
    {
        private TextBox txtName, txtAddress, txtPhone, txtEmail;
        private Button btnSave, btnCancel;
        private int memberID = 0;

        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public MemberEditForm(int memberID = 0)
        {
            this.memberID = memberID;
            InitializeComponent();
            if (memberID != 0)
                LoadMemberData();
        }

        private void InitializeComponent()
        {
            this.Text = memberID == 0 ? "Add Member" : "Edit Member";
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblName = new Label() { Text = "Name:", Location = new System.Drawing.Point(20, 20) };
            txtName = new TextBox() { Location = new System.Drawing.Point(120, 20), Width = 240 };

            Label lblAddress = new Label() { Text = "Address:", Location = new System.Drawing.Point(20, 60) };
            txtAddress = new TextBox() { Location = new System.Drawing.Point(120, 60), Width = 240 };

            Label lblPhone = new Label() { Text = "Phone:", Location = new System.Drawing.Point(20, 100) };
            txtPhone = new TextBox() { Location = new System.Drawing.Point(120, 100), Width = 240 };

            Label lblEmail = new Label() { Text = "Email:", Location = new System.Drawing.Point(20, 140) };
            txtEmail = new TextBox() { Location = new System.Drawing.Point(120, 140), Width = 240 };

            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(120, 200) };
            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(220, 200) };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblAddress);
            this.Controls.Add(txtAddress);
            this.Controls.Add(lblPhone);
            this.Controls.Add(txtPhone);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadMemberData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Name, Address, Phone, Email FROM Members WHERE MemberID = @memberID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@memberID", memberID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader.GetString("Name");
                            txtAddress.Text = reader.GetString("Address");
                            txtPhone.Text = reader.GetString("Phone");
                            txtEmail.Text = reader.GetString("Email");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading member data: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query;
                    if (memberID == 0) // Insert
                    {
                        query = "INSERT INTO Members (Name, Address, Phone, Email, RegistrationDate) " +
                                "VALUES (@name, @address, @phone, @email, CURDATE())";
                    }
                    else // Update
                    {
                        query = "UPDATE Members SET Name=@name, Address=@address, Phone=@phone, Email=@email WHERE MemberID=@memberID";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());

                    if (memberID != 0)
                        cmd.Parameters.AddWithValue("@memberID", memberID);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Member saved successfully.");
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving member: " + ex.Message);
            }
        }
    }
}
