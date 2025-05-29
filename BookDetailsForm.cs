using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class BookDetailsForm : Form
    {
        private TextBox txtTitle, txtAuthor, txtGenre, txtISBN, txtCopies;
        private CheckBox chkAvailable;
        private Button btnSave, btnCancel;
        private int? bookId = null;
        private Database db = new Database();

        public BookDetailsForm(int? bookId = null)
        {
            this.bookId = bookId;
            InitializeComponent();

            if (bookId.HasValue)
                LoadBookDetails(bookId.Value);
        }

        private void InitializeComponent()
        {
            this.Text = bookId.HasValue ? "Edit Book" : "Add Book";
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblTitle = new Label() { Text = "Title:", Location = new System.Drawing.Point(20, 30) };
            txtTitle = new TextBox() { Location = new System.Drawing.Point(120, 25), Width = 240 };

            Label lblAuthor = new Label() { Text = "Author:", Location = new System.Drawing.Point(20, 70) };
            txtAuthor = new TextBox() { Location = new System.Drawing.Point(120, 65), Width = 240 };

            Label lblGenre = new Label() { Text = "Genre:", Location = new System.Drawing.Point(20, 110) };
            txtGenre = new TextBox() { Location = new System.Drawing.Point(120, 105), Width = 240 };

            Label lblISBN = new Label() { Text = "ISBN:", Location = new System.Drawing.Point(20, 150) };
            txtISBN = new TextBox() { Location = new System.Drawing.Point(120, 145), Width = 240 };

            Label lblCopies = new Label() { Text = "Copies:", Location = new System.Drawing.Point(20, 190) };
            txtCopies = new TextBox() { Location = new System.Drawing.Point(120, 185), Width = 240 };

            chkAvailable = new CheckBox() { Text = "Available", Location = new System.Drawing.Point(120, 220) };

            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(120, 250) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(220, 250) };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblTitle, txtTitle, lblAuthor, txtAuthor, lblGenre, txtGenre,
                lblISBN, txtISBN, lblCopies, txtCopies, chkAvailable,
                btnSave, btnCancel
            });
        }

        private void LoadBookDetails(int bookId)
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Books WHERE BookID = @bookId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@bookId", bookId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitle.Text = reader["Title"].ToString();
                            txtAuthor.Text = reader["Author"].ToString();
                            txtGenre.Text = reader["Genre"].ToString();
                            txtISBN.Text = reader["ISBN"].ToString();
                            txtCopies.Text = reader["NumberOfCopies"].ToString();
                            chkAvailable.Checked = Convert.ToBoolean(reader["AvailabilityStatus"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load book details: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text) ||
                string.IsNullOrWhiteSpace(txtGenre.Text) ||
                string.IsNullOrWhiteSpace(txtISBN.Text) ||
                !int.TryParse(txtCopies.Text, out int copies))
            {
                MessageBox.Show("Please fill all fields correctly.");
                return;
            }

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    string query;
                    if (bookId.HasValue)
                    {
                        query = @"UPDATE Books SET Title=@title, Author=@author, Genre=@genre, 
                                  ISBN=@isbn, NumberOfCopies=@copies, AvailabilityStatus=@available 
                                  WHERE BookID=@bookId";
                    }
                    else
                    {
                        query = @"INSERT INTO Books (Title, Author, Genre, ISBN, NumberOfCopies, AvailabilityStatus, ManagedByAdminID) 
                                  VALUES (@title, @author, @genre, @isbn, @copies, @available, 1)";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@author", txtAuthor.Text.Trim());
                    cmd.Parameters.AddWithValue("@genre", txtGenre.Text.Trim());
                    cmd.Parameters.AddWithValue("@isbn", txtISBN.Text.Trim());
                    cmd.Parameters.AddWithValue("@copies", copies);
                    cmd.Parameters.AddWithValue("@available", chkAvailable.Checked);

                    if (bookId.HasValue)
                        cmd.Parameters.AddWithValue("@bookId", bookId.Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Book saved successfully.");
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save book: " + ex.Message);
            }
        }
    }
}
