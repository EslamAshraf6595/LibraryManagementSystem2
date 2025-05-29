using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

public class BooksForm : Form
{
    private DataGridView dgvBooks;
    private TextBox txtSearch;
    private DataTable booksTable;

    public BooksForm()
    {
        InitializeComponent();
        LoadBooks();
    }

    private void InitializeComponent()
    {
        this.Text = "Books";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        txtSearch = new TextBox();
        txtSearch.Location = new Point(20, 20);
        txtSearch.Width = 300;
        txtSearch.PlaceholderText = "Search by title, author or genre...";
        txtSearch.TextChanged += TxtSearch_TextChanged;

        dgvBooks = new DataGridView();
        dgvBooks.Location = new Point(20, 60);
        dgvBooks.Size = new Size(740, 480);
        dgvBooks.ReadOnly = true;
        dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        this.Controls.Add(txtSearch);
        this.Controls.Add(dgvBooks);
    }

    private void LoadBooks()
    {
        // هنا حط بياناتك من قاعدة البيانات أو مؤقتًا نضيف بيانات
        booksTable = new DataTable();
        booksTable.Columns.Add("Title");
        booksTable.Columns.Add("Author");
        booksTable.Columns.Add("Genre");

        booksTable.Rows.Add("The Alchemist", "Paulo Coelho", "Fiction");
        booksTable.Rows.Add("Clean Code", "Robert C. Martin", "Programming");
        booksTable.Rows.Add("Introduction to Algorithms", "Thomas H. Cormen", "Education");

        dgvBooks.DataSource = booksTable;
    }

    private void TxtSearch_TextChanged(object sender, EventArgs e)
    {
        string filter = txtSearch.Text.Trim().Replace("'", "''");

        if (string.IsNullOrEmpty(filter))
        {
            dgvBooks.DataSource = booksTable;
        }
        else
        {
            DataView dv = booksTable.DefaultView;
            dv.RowFilter = $"Title LIKE '%{filter}%' OR Author LIKE '%{filter}%' OR Genre LIKE '%{filter}%'";
            dgvBooks.DataSource = dv.ToTable();
        }
    }
}
