using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class BookEditForm : Form
    {
        private TextBox txtTitle, txtAuthor, txtGenre, txtISBN, txtCopies;
        private Button btnSave, btnCancel;
        private int bookID = 0; // صفر يعني إضافة جديد

        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public BookEditForm(int bookID = 0)
        {
            this.bookID = bookID;
            InitializeComponent();
            if (bookID != 0)
                LoadBookData();
        }

        private void InitializeComponent()
        {
            this.Text = bookID == 0 ? "Add Book" : "Edit Book";
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblTitle = new Label() { Text = "Title:", Location = new System.Drawing.Point(20, 20) };
            txtTitle = new TextBox() { Location = new System.Drawing.Point(120, 20), Width = 240 };

            Label lblAuthor = new Label() { Text = "Author:", Location = new System.Drawing.Point(20, 60) };
            txtAuthor = new TextBox() { Location = new System.Drawing.Point(120, 60), Width = 240 };

            Label lblGenre = new Label() { Text = "Genre:", Location = new System.Drawing.Point(20, 100) };
            txtGenre = new TextBox() { Location = new System.Drawing.Point(120, 100), Width = 240 };

            Label lblISBN = new Label() { Text = "ISBN:", Location = new System.Drawing.Point(20, 140) };
            txtISBN = new TextBox() { Location = new System.Drawing.Point(120, 140), Width = 240 };

            Label lblCopies = new Label() { Text = "Number of Copies:", Location = new System.Drawing.Point(20, 180) };
            txtCopies = new TextBox() { Location = new System.Drawing.Point(120, 180), Width = 240 };

            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(120, 220) };
            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(220, 220) };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblAuthor);
            this.Controls.Add(txtAuthor);
            this.Controls.Add(lblGenre);
            this.Controls.Add(txtGenre);
            this.Controls.Add(lblISBN);
            this.Controls.Add(txtISBN);
            this.Controls.Add(lblCopies);
            this.Controls.Add(txtCopies);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadBookData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Title, Author, Genre, ISBN, NumberOfCopies FROM Books WHERE BookID = @bookID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@bookID", bookID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitle.Text = reader.GetString("Title");
                            txtAuthor.Text = reader.GetString("Author");
                            txtGenre.Text = reader.GetString("Genre");
                            txtISBN.Text = reader.GetString("ISBN");
                            txtCopies.Text = reader.GetInt32("NumberOfCopies").ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading book data: " + ex.Message);
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
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query;
                    if (bookID == 0) // Insert
                    {
                        query = "INSERT INTO Books (Title, Author, Genre, ISBN, AvailabilityStatus, NumberOfCopies, ManagedByAdminID) " +
                                "VALUES (@title, @author, @genre, @isbn, @available, @copies, @admin)";
                    }
                    else // Update
                    {
                        query = "UPDATE Books SET Title=@title, Author=@author, Genre=@genre, ISBN=@isbn, NumberOfCopies=@copies WHERE BookID=@bookID";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@author", txtAuthor.Text.Trim());
                    cmd.Parameters.AddWithValue("@genre", txtGenre.Text.Trim());
                    cmd.Parameters.AddWithValue("@isbn", txtISBN.Text.Trim());
                    cmd.Parameters.AddWithValue("@copies", copies);

                    if (bookID == 0)
                    {
                        cmd.Parameters.AddWithValue("@available", true);
                        cmd.Parameters.AddWithValue("@admin", 1); // Adjust admin ID as needed
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@bookID", bookID);
                    }

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Book saved successfully.");
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving book: " + ex.Message);
            }
        }
    }
}
