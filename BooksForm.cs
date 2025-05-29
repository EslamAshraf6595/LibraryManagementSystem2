using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class BooksForm : Form
    {
        private DataGridView dgvBooks;
        private Button btnAdd, btnEdit, btnDelete;
        private Database db = new Database();

        public BooksForm()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Books";
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvBooks = new DataGridView()
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(680, 300),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnAdd = new Button() { Text = "Add Book", Location = new System.Drawing.Point(10, 320) };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button() { Text = "Edit Book", Location = new System.Drawing.Point(110, 320) };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button() { Text = "Delete Book", Location = new System.Drawing.Point(210, 320) };
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(dgvBooks);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }

        private void LoadBooks()
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT BookID, Title, Author, Genre, ISBN, AvailabilityStatus, NumberOfCopies FROM Books";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            BookDetailsForm form = new BookDetailsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadBooks();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to edit.");
                return;
            }

            int bookId = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);
            BookDetailsForm form = new BookDetailsForm(bookId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadBooks();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to delete.");
                return;
            }

            int bookId = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);

            var confirm = MessageBox.Show("Are you sure you want to delete this book?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = db.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM Books WHERE BookID = @bookId";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@bookId", bookId);
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
