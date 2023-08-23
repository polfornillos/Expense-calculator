using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FORNILLOS_FinalProject
{
    public partial class addExpense : Form
    {
        private ExpenseDataAccess expenseDataAccess;
        public Expense SelectedExpense { get; private set; }

        public addExpense(Expense expense = null)
        {
            InitializeComponent();
            SelectedExpense = expense;

            if (SelectedExpense != null)
            {
                this.Text = "Edit";
                textBox1.Text = SelectedExpense.Amount.ToString();
                dateTimePicker1.Value = SelectedExpense.Date;
                comboBox1.Text = SelectedExpense.Category;
                textBox2.Text = SelectedExpense.Notes;
                comboBox2.SelectedItem = SelectedExpense.Subscription;
            }
            else
            {
                comboBox2.SelectedItem = "None";
            }
        }

        public Expense GetExpense()
        {
            decimal amount = decimal.Parse(textBox1.Text);
            DateTime date = dateTimePicker1.Value;
            string category = comboBox1.Text;
            string notes = textBox2.Text;
            string selectedSubscription = comboBox2.SelectedItem.ToString();

            DateTime nextUpdateDate = DateTime.MaxValue;
            if (selectedSubscription == "Monthly")
                nextUpdateDate = date.AddMonths(1);
            else if (selectedSubscription == "Weekly")
                nextUpdateDate = date.AddDays(7);

            return new Expense
            {
                Amount = amount,
                Date = date,
                Category = category,
                Notes = notes,
                Subscription = selectedSubscription,
                NextUpdateDate = nextUpdateDate
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox1.Text, out decimal amount))
            {
                MessageBox.Show("Invalid amount. Please enter a numeric value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Notes cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Expense newExpense = GetExpense();
            ExpenseDataAccess expenseDataAccess = new ExpenseDataAccess("C:\\Users\\user\\Documents\\01 Codes\\C#\\FORNILLOS_FinalProject\\FORNILLOS_FinalProject\\expenseTracker.db");

            if (SelectedExpense != null)
            {
                newExpense.Id = SelectedExpense.Id;
                expenseDataAccess.UpdateExpense(newExpense);
            }
            else
            {
                expenseDataAccess.AddExpense(newExpense);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}