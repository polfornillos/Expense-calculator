namespace FORNILLOS_FinalProject
{
    public partial class Form1 : Form
    {
        private List<Expense> expensesList = new List<Expense>();
        public Form1()
        {
            InitializeComponent();
            expensesList = new List<Expense>();

            LoadCategories();
            LoadExpenses();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns["Notes"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            LoadExpenses();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            addExpense addExpenseForm = new addExpense(); 
            DialogResult result = addExpenseForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                Expense newExpense = addExpenseForm.GetExpense();
                ExpenseDataAccess expenseDataAccess = new ExpenseDataAccess("C:\\Users\\user\\Documents\\01 Codes\\C#\\FORNILLOS_FinalProject\\FORNILLOS_FinalProject\\expenseTracker.db");

                if (newExpense.Subscription == "Monthly" || newExpense.Subscription == "Weekly")
                {
                    UpdateRecurringExpenses();
                    expensesList.Add(newExpense);
                    ShowSubscriptionMessage(newExpense.Subscription);
                }
                else if (newExpense.Subscription == "None")
                {
                    expensesList.Add(newExpense);
                }
                else
                {
                    if (addExpenseForm.SelectedExpense != null)
                    {
                        newExpense.Id = addExpenseForm.SelectedExpense.Id;
                        expenseDataAccess.UpdateExpense(newExpense);
                        ShowSubscriptionMessage(newExpense.Subscription);
                    }
                    else
                    {
                        expenseDataAccess.AddExpense(newExpense);
                        ShowSubscriptionMessage(newExpense.Subscription);
                    }
                }

                LoadExpenses();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedExpenseId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                Expense selectedExpense = expensesList.Find(expense => expense.Id == selectedExpenseId);

                addExpense addExpense = new addExpense(selectedExpense);
                DialogResult result = addExpense.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Expense updatedExpense = addExpense.GetExpense();

                    int index = expensesList.FindIndex(expense => expense.Id == updatedExpense.Id);
                    if (index != -1)
                    {
                        expensesList[index] = updatedExpense;
                    }
                    ExpenseDataAccess expenseDataAccess = new ExpenseDataAccess("C:\\Users\\user\\Documents\\01 Codes\\C#\\FORNILLOS_FinalProject\\FORNILLOS_FinalProject\\expenseTracker.db");
                    expenseDataAccess.UpdateExpense(updatedExpense);

                    LoadExpenses();
                }
            }
            else
            {
                MessageBox.Show("Please select an expense to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedExpenseId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                Expense selectedExpense = expensesList.Find(expense => expense.Id == selectedExpenseId);

                DialogResult result = MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ExpenseDataAccess expenseDataAccess = new ExpenseDataAccess("C:\\Users\\user\\Documents\\01 Codes\\C#\\FORNILLOS_FINALPROJECT\\FORNILLOS_FinalProject\\expenseTracker.db");
                    expenseDataAccess.DeleteExpense(selectedExpenseId);

                    expensesList.Remove(selectedExpense);

                    LoadExpenses();
                }
            }
            else
            {
                MessageBox.Show("Please select an expense to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCategory = comboBox1.SelectedItem.ToString();

            if (selectedCategory == "All")
            {
                LoadExpenses();
            }
            else
            {
                List<Expense> filteredExpenses = expensesList.FindAll(expense => expense.Category == selectedCategory);

                dataGridView1.Rows.Clear();
                foreach (var expense in filteredExpenses)
                {
                    dataGridView1.Rows.Add(expense.Id, expense.Amount, expense.Date, expense.Category, expense.Notes, expense.Subscription);
                }
            }
        }

        private void LoadExpenses()
        {
            dataGridView1.Rows.Clear();

            ExpenseDataAccess expenseDataAccess = new ExpenseDataAccess("C:\\Users\\user\\Documents\\01 Codes\\C#\\FORNILLOS_FinalProject\\FORNILLOS_FinalProject\\expenseTracker.db");
            expensesList = expenseDataAccess.GetAllExpenses();

            foreach (var expense in expensesList)
            {
                dataGridView1.Rows.Add(expense.Id, expense.Amount, expense.Date, expense.Category, expense.Notes, expense.Subscription);
            }
            dataGridView1.Columns["Id"].Visible = false;
        }

        private void LoadCategories()
        {
            ExpenseDataAccess expenseDataAccess = new ExpenseDataAccess("C:\\Users\\user\\Documents\\01 Codes\\C#\\FORNILLOS_FinalProject\\FORNILLOS_FinalProject\\expenseTracker.db");
            List<string> categories = expenseDataAccess.GetAllCategories();
            comboBox1.Items.Add("All");
            comboBox1.Items.AddRange(categories.ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void AutoUpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateRecurringExpenses();
        }

        private void UpdateRecurringExpenses()
        {
            DateTime currentDate = DateTime.Today;
            foreach (var expense in expensesList)
            {
                if (expense.Subscription == "Monthly" && expense.NextUpdateDate <= currentDate)
                {
                    UpdateExpenseInDatabase(expense);
                    expense.NextUpdateDate = expense.NextUpdateDate.AddMonths(1);
                }
                else if (expense.Subscription == "Weekly" && expense.NextUpdateDate <= currentDate)
                {
                    UpdateExpenseInDatabase(expense);
                    expense.NextUpdateDate = expense.NextUpdateDate.AddDays(7);
                }
            }
        }
        private void UpdateExpenseInDatabase(Expense expense)
        {
            ExpenseDataAccess expenseDataAccess = new ExpenseDataAccess("C:\\Users\\user\\Documents\\01 Codes\\C#\\FORNILLOS_FinalProject\\FORNILLOS_FinalProject\\expenseTracker.db");
            expenseDataAccess.UpdateExpense(expense);
        }
        private void ShowSubscriptionMessage(string frequency)
        {
            MessageBox.Show("Subscription set to " + frequency + " successfully.", "Subscription Set", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}