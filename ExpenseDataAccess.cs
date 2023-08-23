using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FORNILLOS_FinalProject
{
    public class ExpenseDataAccess
    {
        private readonly string connectionString;

        public ExpenseDataAccess(string dbPath)
        {
            connectionString = $"Data Source={dbPath};Version=3;";
        }

        public void AddExpense(Expense expense)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "INSERT INTO Expenses (Amount, Date, Category, Notes, Subscription) " +
                                 "VALUES (@Amount, @Date, @Category, @Notes, @Subscription)";
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Date", expense.Date);
                    command.Parameters.AddWithValue("@Category", expense.Category);
                    command.Parameters.AddWithValue("@Notes", expense.Notes);
                    command.Parameters.AddWithValue("@Subscription", expense.Subscription);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateExpense(Expense expense)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE Expenses SET Amount=@Amount, Date=@Date, Category=@Category, " +
                                  "Notes=@Notes, Subscription=@Subscription WHERE Id=@Id";
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Date", expense.Date);
                    command.Parameters.AddWithValue("@Category", expense.Category);
                    command.Parameters.AddWithValue("@Notes", expense.Notes);
                    command.Parameters.AddWithValue("@Subscription", expense.Subscription);
                    command.Parameters.AddWithValue("@Id", expense.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteExpense(int expenseId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM Expenses WHERE Id=@Id";
                    command.Parameters.AddWithValue("@Id", expenseId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Expense GetExpenseByDetails(decimal amount, DateTime date, string category, string notes, string subscription)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT * FROM Expenses WHERE Amount=@Amount AND Date=@Date AND Category=@Category AND Notes=@Notes AND Subscription=@Subscription";
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Notes", notes);
                    command.Parameters.AddWithValue("@Subscription", subscription);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Expense
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Category = Convert.ToString(reader["Category"]),
                                Notes = Convert.ToString(reader["Notes"]),
                                Subscription = Convert.ToString(reader["Subscription"]),
                                NextUpdateDate = Convert.ToDateTime(reader["NextUpdateDate"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Expense> GetAllExpenses()
        {
            List<Expense> expenses = new List<Expense>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT * FROM Expenses";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Expense expense = new Expense
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Category = Convert.ToString(reader["Category"]),
                                Notes = Convert.ToString(reader["Notes"]),
                                Subscription = Convert.ToString(reader["subscription"])
                            };
                            expenses.Add(expense);
                        }
                    }
                }
            }
            return expenses;
        }

        public List<string> GetAllCategories()
        {
            List<string> categories = new List<string>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT DISTINCT Category FROM Expenses";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["Category"].ToString();
                            categories.Add(category);
                        }
                    }
                }
            }

            return categories;
        }
    }
}
