using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class TransactionDetailsForm : Form
    {
        private ComboBox cbTransactionType, cbMembers, cbBooks, cbLibrarians;
        private DateTimePicker dtpTransactionDate, dtpDueDate, dtpRenewedDueDate, dtpReserveUntil;
        private TextBox txtReturnCondition;
        private Button btnSave, btnCancel;

        private int? transactionId = null;
        private Database db = new Database();

        public TransactionDetailsForm(int? transactionId = null)
        {
            this.transactionId = transactionId;
            InitializeComponent();
            LoadDropdowns();
            if (transactionId.HasValue)
                LoadTransactionDetails(transactionId.Value);
        }

        private void InitializeComponent()
        {
            this.Text = transactionId.HasValue ? "Edit Transaction" : "Add Transaction";
            this.ClientSize = new System.Drawing.Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblType = new Label() { Text = "Transaction Type:", Location = new System.Drawing.Point(20, 20) };
            cbTransactionType = new ComboBox() { Location = new System.Drawing.Point(150, 20), Width = 250 };
            cbTransactionType.Items.AddRange(new string[] { "Borrow", "Return", "Renew", "Reserve" });
            cbTransactionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTransactionType.SelectedIndex = 0;

            Label lblMember = new Label() { Text = "Member:", Location = new System.Drawing.Point(20, 60) };
            cbMembers = new ComboBox() { Location = new System.Drawing.Point(150, 60), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblBook = new Label() { Text = "Book:", Location = new System.Drawing.Point(20, 100) };
            cbBooks = new ComboBox() { Location = new System.Drawing.Point(150, 100), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblLibrarian = new Label() { Text = "Librarian:", Location = new System.Drawing.Point(20, 140) };
            cbLibrarians = new ComboBox() { Location = new System.Drawing.Point(150, 140), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblTransactionDate = new Label() { Text = "Transaction Date:", Location = new System.Drawing.Point(20, 180) };
            dtpTransactionDate = new DateTimePicker() { Location = new System.Drawing.Point(150, 180), Width = 250, Format = DateTimePickerFormat.Short };

            Label lblDueDate = new Label() { Text = "Due Date:", Location = new System.Drawing.Point(20, 220) };
            dtpDueDate = new DateTimePicker() { Location = new System.Drawing.Point(150, 220), Width = 250, Format = DateTimePickerFormat.Short };

            Label lblReturnCondition = new Label() { Text = "Return Condition:", Location = new System.Drawing.Point(20, 260) };
            txtReturnCondition = new TextBox() { Location = new System.Drawing.Point(150, 260), Width = 250 };

            Label lblRenewedDueDate = new Label() { Text = "Renewed Due Date:", Location = new System.Drawing.Point(20, 300) };
            dtpRenewedDueDate = new DateTimePicker() { Location = new System.Drawing.Point(150, 300), Width = 250, Format = DateTimePickerFormat.Short };

            Label lblReserveUntil = new Label() { Text = "Reserve Until:", Location = new System.Drawing.Point(20, 340) };
            dtpReserveUntil = new DateTimePicker() { Location = new System.Drawing.Point(150, 340), Width = 250, Format = DateTimePickerFormat.Short };

            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(150, 380) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(250, 380) };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblType, cbTransactionType,
                lblMember, cbMembers,
                lblBook, cbBooks,
                lblLibrarian, cbLibrarians,
                lblTransactionDate, dtpTransactionDate,
                lblDueDate, dtpDueDate,
                lblReturnCondition, txtReturnCondition,
                lblRenewedDueDate, dtpRenewedDueDate,
                lblReserveUntil, dtpReserveUntil,
                btnSave, btnCancel
            });
        }

        private void LoadDropdowns()
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    // Load Members
                    MySqlCommand cmd = new MySqlCommand("SELECT MemberID, Name FROM Members", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbMembers.Items.Add(new ComboboxItem(reader.GetString("Name"), reader.GetInt32("MemberID")));
                        }
                    }

                    // Load Books
                    cmd = new MySqlCommand("SELECT BookID, Title FROM Books", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbBooks.Items.Add(new ComboboxItem(reader.GetString("Title"), reader.GetInt32("BookID")));
                        }
                    }

                    // Load Librarians (Admins)
                    cmd = new MySqlCommand("SELECT AdminID, AdminName FROM Admins", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbLibrarians.Items.Add(new ComboboxItem(reader.GetString("AdminName"), reader.GetInt32("AdminID")));
                        }
                    }
                }

                if (cbMembers.Items.Count > 0)
                    cbMembers.SelectedIndex = 0;
                if (cbBooks.Items.Count > 0)
                    cbBooks.SelectedIndex = 0;
                if (cbLibrarians.Items.Count > 0)
                    cbLibrarians.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message);
            }
        }

        private void LoadTransactionDetails(int id)
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Transactions WHERE TransactionID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cbTransactionType.SelectedItem = reader.GetString("TransactionType");
                            SelectComboBoxItemByValue(cbMembers, reader.GetInt32("MemberID"));
                            SelectComboBoxItemByValue(cbBooks, reader.GetInt32("BookID"));
                            SelectComboBoxItemByValue(cbLibrarians, reader.GetInt32("LibrarianID"));
                            dtpTransactionDate.Value = reader.GetDateTime("TransactionDate");

                            dtpDueDate.Value = reader["DueDate"] == DBNull.Value ? DateTime.Now : reader.GetDateTime("DueDate");
                            txtReturnCondition.Text = reader["ReturnCondition"] == DBNull.Value ? "" : reader.GetString("ReturnCondition");
                            dtpRenewedDueDate.Value = reader["RenewedDueDate"] == DBNull.Value ? DateTime.Now : reader.GetDateTime("RenewedDueDate");
                            dtpReserveUntil.Value = reader["ReserveUntil"] == DBNull.Value ? DateTime.Now : reader.GetDateTime("ReserveUntil");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transaction details: " + ex.Message);
            }
        }

        private void SelectComboBoxItemByValue(ComboBox comboBox, int value)
        {
            foreach (ComboboxItem item in comboBox.Items)
            {
                if (item.Value == value)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cbTransactionType.SelectedItem == null ||
                cbMembers.SelectedItem == null ||
                cbBooks.SelectedItem == null ||
                cbLibrarians.SelectedItem == null)
            {
                MessageBox.Show("Please select all required fields.");
                return;
            }

            string type = cbTransactionType.SelectedItem.ToString();
            int memberId = ((ComboboxItem)cbMembers.SelectedItem).Value;
            int bookId = ((ComboboxItem)cbBooks.SelectedItem).Value;
            int librarianId = ((ComboboxItem)cbLibrarians.SelectedItem).Value;
            DateTime transactionDate = dtpTransactionDate.Value;
            DateTime? dueDate = dtpDueDate.Value;
            string returnCondition = txtReturnCondition.Text.Trim();
            DateTime? renewedDueDate = dtpRenewedDueDate.Value;
            DateTime? reserveUntil = dtpReserveUntil.Value;

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd;
                    if (transactionId.HasValue)
                    {
                        cmd = new MySqlCommand(@"
                            UPDATE Transactions SET
                                TransactionType=@type,
                                MemberID=@memberId,
                                BookID=@bookId,
                                LibrarianID=@librarianId,
                                TransactionDate=@transactionDate,
                                DueDate=@dueDate,
                                ReturnCondition=@returnCondition,
                                RenewedDueDate=@renewedDueDate,
                                ReserveUntil=@reserveUntil
                            WHERE TransactionID=@id", conn);

                        cmd.Parameters.AddWithValue("@id", transactionId.Value);
                    }
                    else
                    {
                        cmd = new MySqlCommand(@"
                            INSERT INTO Transactions
                                (TransactionType, MemberID, BookID, LibrarianID, TransactionDate, DueDate, ReturnCondition, RenewedDueDate, ReserveUntil)
                            VALUES
                                (@type, @memberId, @bookId, @librarianId, @transactionDate, @dueDate, @returnCondition, @renewedDueDate, @reserveUntil)", conn);
                    }

                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@memberId", memberId);
                    cmd.Parameters.AddWithValue("@bookId", bookId);
                    cmd.Parameters.AddWithValue("@librarianId", librarianId);
                    cmd.Parameters.AddWithValue("@transactionDate", transactionDate);
                    cmd.Parameters.AddWithValue("@dueDate", dueDate);
                    cmd.Parameters.AddWithValue("@returnCondition", returnCondition);
                    cmd.Parameters.AddWithValue("@renewedDueDate", renewedDueDate);
                    cmd.Parameters.AddWithValue("@reserveUntil", reserveUntil);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Transaction saved successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving transaction: " + ex.Message);
            }
        }

        private class ComboboxItem
        {
            public string Text { get; }
            public int Value { get; }

            public ComboboxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
