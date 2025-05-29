using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using BCrypt.Net;

namespace LibraryManagementSystem2
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername, txtEmail, txtPassword, txtConfirmPassword;
        private Button btnRegister, btnCancel;
        private Label lblUsername, lblEmail, lblPassword, lblConfirmPassword;
        private Database db = new Database();

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Register";
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblUsername = new Label() { Text = "Username:", Location = new System.Drawing.Point(20, 30), AutoSize = true };
            txtUsername = new TextBox() { Location = new System.Drawing.Point(150, 25), Width = 200 };

            lblEmail = new Label() { Text = "Email:", Location = new System.Drawing.Point(20, 70), AutoSize = true };
            txtEmail = new TextBox() { Location = new System.Drawing.Point(150, 65), Width = 200 };

            lblPassword = new Label() { Text = "Password:", Location = new System.Drawing.Point(20, 110), AutoSize = true };
            txtPassword = new TextBox() { Location = new System.Drawing.Point(150, 105), Width = 200, PasswordChar = '*' };

            lblConfirmPassword = new Label() { Text = "Confirm Password:", Location = new System.Drawing.Point(20, 150), AutoSize = true };
            txtConfirmPassword = new TextBox() { Location = new System.Drawing.Point(150, 145), Width = 200, PasswordChar = '*' };

            btnRegister = new Button() { Text = "Register", Location = new System.Drawing.Point(150, 190) };
            btnRegister.Click += BtnRegister_Click;

            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(260, 190) };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblUsername, txtUsername, lblEmail, txtEmail, lblPassword, txtPassword,
                lblConfirmPassword, txtConfirmPassword, btnRegister, btnCancel
            });
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill all required fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    // تحقق من وجود اسم المستخدم أو البريد مسبقاً
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username OR Email = @Email";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@username", username);
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Username or email already exists.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // هاش كلمة المرور
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                    string insertQuery = "INSERT INTO Users (Username, Email, PasswordHash, Role) VALUES (@username, @Email, @passwordHash, 'Member')";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@username", username);
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                    insertCmd.ExecuteNonQuery();

                    MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
