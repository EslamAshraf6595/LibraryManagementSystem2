using System;
using System.Windows.Forms;

namespace LibraryManagementSystem2
{
    public class MainForm : Form
    {
        private string userRole;

        public MainForm(string role)
        {
            userRole = role;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Library Management System - Main";
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblWelcome = new Label()
            {
                Text = $"Welcome! Your role is: {userRole}",
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20)
            };

            this.Controls.Add(lblWelcome);

            // بناءً على الدور، ممكن تضيف أزرار أو صلاحيات مختلفة هنا
        }
    }
}
