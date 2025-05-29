using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class MemberDetailsForm : Form
    {
        private TextBox txtName, txtAddress, txtPhone, txtEmail;
        private DateTimePicker dtpRegistrationDate;
        private Button btnSave, btnCancel;

        private int? memberId = null;
        private Database db = new Database();

        public MemberDetailsForm(int? memberId = null)
        {
            this.memberId = memberId;
            InitializeComponent();
            if (memberId.HasValue)
                LoadMemberDetails(memberId.Value);
        }

        private void InitializeComponent()
        {
            this.Text = memberId.HasValue ? "Edit Member" : "Add Member";
            this.ClientSize = new System.Drawing.Size(400, 320);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblName = new Label() { Text = "Name:", Location = new System.Drawing.Point(20, 20) };
            txtName = new TextBox() { Location = new System.Drawing.Point(120, 20), Width = 240 };

            Label lblAddress = new Label() { Text = "Address:", Location = new System.Drawing.Point(20, 60) };
            txtAddress = new TextBox() { Location = new System.Drawing.Point(120, 60), Width = 240 };

            Label lblPhone = new Label() { Text = "Phone:", Location = new System.Drawing.Point(20, 100) };
            txtPhone = new TextBox() { Location = new System.Drawing.Point(120, 100), Width = 240 };

            Label lblEmail = new Label() { Text = "Email:", Location = new System.Drawing.Point(20, 140) };
            txtEmail = new TextBox() { Location = new System.Drawing.Point(120, 140), Width = 240 };

            Label lblRegDate = new Label() { Text = "Registration Date:", Location = new System.Drawing.Point(20, 180) };
            dtpRegistrationDate = new DateTimePicker() { Location = new System.Drawing.Point(120, 180), Width = 240, Format = DateTimePickerFormat.Short };

            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(120, 230) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(220, 230) };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblName, txtName,
                lblAddress, txtAddress,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblRegDate, dtpRegistrationDate,
                btnSave, btnCancel
            });
        }

        private void LoadMemberDetails(int id)
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Name, Address, Phone, Email, RegistrationDate FROM Members WHERE MemberID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader.GetString("Name");
                            txtAddress.Text = reader.GetString("Address");
                            txtPhone.Text = reader.GetString("Phone");
                            txtEmail.Text = reader.GetString("Email");
                            dtpRegistrationDate.Value = reader.GetDateTime("RegistrationDate");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading member details: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            DateTime regDate = dtpRegistrationDate.Value;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Name is required.");
                return;
            }

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd;
                    if (memberId.HasValue)
                    {
                        cmd = new MySqlCommand(@"UPDATE Members SET Name=@name, Address=@address, Phone=@phone,
                            Email=@email, RegistrationDate=@regDate WHERE MemberID=@id", conn);
                        cmd.Parameters.AddWithValue("@id", memberId.Value);
                    }
                    else
                    {
                        cmd = new MySqlCommand(@"INSERT INTO Members (Name, Address, Phone, Email, RegistrationDate)
                            VALUES (@name, @address, @phone, @email, @regDate)", conn);
                    }

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@regDate", regDate);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Member saved successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving member: " + ex.Message);
            }
        }
    }
}
