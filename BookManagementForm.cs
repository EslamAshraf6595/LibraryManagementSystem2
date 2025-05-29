using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class BookManagementForm : Form
    {
        private DataGridView dgvBooks;
        private Button btnAddBook, btnEditBook, btnDeleteBook;
        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public BookManagementForm()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.Text = "Book Management";
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvBooks = new DataGridView()
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(760, 380),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvBooks);

            btnAddBook = new Button() { Text = "Add Book", Location = new System.Drawing.Point(20, 420) };
            btnEditBook = new Button() { Text = "Edit Book", Location = new System.Drawing.Point(120, 420) };
            btnDeleteBook = new Button() { Text = "Delete Book", Location = new System.Drawing.Point(220, 420) };

            btnAddBook.Click += BtnAddBook_Click;
            btnEditBook.Click += BtnEditBook_Click;
            btnDeleteBook.Click += BtnDeleteBook_Click;

            this.Controls.Add(btnAddBook);
            this.Controls.Add(btnEditBook);
            this.Controls.Add(btnDeleteBook);
        }

        private void LoadBooks()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT BookID, Title, Author, Genre, ISBN, NumberOfCopies FROM Books";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvBooks.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading books: " + ex.Message);
            }
        }

        private void BtnAddBook_Click(object sender, EventArgs e)
        {
            BookEditForm editForm = new BookEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadBooks();
            }
        }

        private void BtnEditBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to edit.");
                return;
            }

            int bookID = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);
            BookEditForm editForm = new BookEditForm(bookID);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadBooks();
            }
        }

        private void BtnDeleteBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to delete.");
                return;
            }

            int bookID = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);
            var result = MessageBox.Show("Are you sure you want to delete this book?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Books WHERE BookID = @bookID";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@bookID", bookID);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Book deleted successfully.");
                        LoadBooks();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting book: " + ex.Message);
                }
            }
        }
    }
}
