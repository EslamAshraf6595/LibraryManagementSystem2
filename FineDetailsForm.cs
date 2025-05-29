using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class FineDetailsForm : Form
    {
        private ComboBox cbTransactions;
        private TextBox txtAmount;
        private DateTimePicker dtpIssueDate;
        private CheckBox chkPaymentStatus;
        private Button btnSave, btnCancel;

        private int? fineId = null;
        private Database db = new Database();

        public FineDetailsForm(int? fineId = null)
        {
            this.fineId = fineId;
            InitializeComponent();
            LoadTransactions();
            if (fineId.HasValue)
                LoadFineDetails(fineId.Value);
        }

        private void InitializeComponent()
        {
            this.Text = fineId.HasValue ? "Edit Fine" : "Add Fine";
            this.ClientSize = new System.Drawing.Size(400, 280);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblTransaction = new Label() { Text = "Transaction:", Location = new System.Drawing.Point(20, 20) };
            cbTransactions = new ComboBox() { Location = new System.Drawing.Point(120, 20), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblAmount = new Label() { Text = "Amount:", Location = new System.Drawing.Point(20, 60) };
            txtAmount = new TextBox() { Location = new System.Drawing.Point(120, 60), Width = 240 };

            Label lblIssueDate = new Label() { Text = "Issue Date:", Location = new System.Drawing.Point(20, 100) };
            dtpIssueDate = new DateTimePicker() { Location = new System.Drawing.Point(120, 100), Width = 240, Format = DateTimePickerFormat.Short };

            chkPaymentStatus = new CheckBox() { Text = "Paid", Location = new System.Drawing.Point(120, 140) };

            btnSave = new Button() { Text = "Save", Location = new System.Drawing.Point(120, 180) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Cancel", Location = new System.Drawing.Point(220, 180) };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTransaction, cbTransactions,
                lblAmount, txtAmount,
                lblIssueDate, dtpIssueDate,
                chkPaymentStatus,
                btnSave, btnCancel
            });
        }

        private void LoadTransactions()
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT TransactionID FROM Transactions", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbTransactions.Items.Add(new ComboboxItem(reader.GetInt32("TransactionID").ToString(), reader.GetInt32("TransactionID")));
                        }
                    }
                    if (cbTransactions.Items.Count > 0)
                        cbTransactions.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transactions: " + ex.Message);
            }
        }

        private void LoadFineDetails(int id)
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT TransactionID, Amount, IssueDate, PaymentStatus FROM Fines WHERE FineID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SelectComboBoxItemByValue(cbTransactions, reader.GetInt32("TransactionID"));
                            txtAmount.Text = reader.GetDecimal("Amount").ToString();
                            dtpIssueDate.Value = reader.GetDateTime("IssueDate");
                            chkPaymentStatus.Checked = reader.GetBoolean("PaymentStatus");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading fine details: " + ex.Message);
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
            if (cbTransactions.SelectedItem == null)
            {
                MessageBox.Show("Please select a transaction.");
                return;
            }

            if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount))
            {
                MessageBox.Show("Please enter a valid amount.");
                return;
            }

            int transactionId = ((ComboboxItem)cbTransactions.SelectedItem).Value;
            DateTime issueDate = dtpIssueDate.Value;
            bool paymentStatus = chkPaymentStatus.Checked;

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd;
                    if (fineId.HasValue)
                    {
                        cmd = new MySqlCommand(@"UPDATE Fines SET TransactionID=@transactionId, Amount=@amount,
                            IssueDate=@issueDate, PaymentStatus=@paymentStatus WHERE FineID=@id", conn);
                        cmd.Parameters.AddWithValue("@id", fineId.Value);
                    }
                    else
                    {
                        cmd = new MySqlCommand(@"INSERT INTO Fines (TransactionID, Amount, IssueDate, PaymentStatus)
                            VALUES (@transactionId, @amount, @issueDate, @paymentStatus)", conn);
                    }

                    cmd.Parameters.AddWithValue("@transactionId", transactionId);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@issueDate", issueDate);
                    cmd.Parameters.AddWithValue("@paymentStatus", paymentStatus);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Fine saved successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving fine: " + ex.Message);
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

            public override string ToString() => Text;
        }
    }
}
