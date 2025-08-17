using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using library_management_system.L;
using LibrarySystem;
using MySql.Data.MySqlClient;

namespace Library_Management_System
{
    public partial class FineCollectionPage : Window
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        public ObservableCollection<Fine> Fines { get; set; } = new ObservableCollection<Fine>();

        public FineCollectionPage()
        {
            InitializeComponent();
            dgFines.ItemsSource = Fines;
            LoadAllFines();
        }

        private void LoadAllFines()
        {
            LoadFinesByStatus(""); // Load all
        }

        private void LoadFinesByStatus(string status)
        {
            try
            {
                Fines.Clear();
                using (var conn = new MySqlConnection(connStr))
                {
                    string query = @"
                        SELECT f.fine_id, u.user_id, u.first_name, u.last_name,
                               f.amount, f.date_issued, f.status
                        FROM fines f
                        JOIN users u ON f.user_id = u.user_id";

                    if (!string.IsNullOrEmpty(status))
                        query += " WHERE f.status = @status";

                    query += " ORDER BY f.date_issued DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(status))
                            cmd.Parameters.AddWithValue("@status", status);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Fines.Add(new Fine()
                                {
                                    FineId = Convert.ToInt32(reader["fine_id"]),
                                    UserId = Convert.ToInt32(reader["user_id"]),
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    Amount = Convert.ToDecimal(reader["amount"]),
                                    Date = Convert.ToDateTime(reader["date_issued"]),
                                    Status = reader["status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading fines: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddFine_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtUserId.Text, out int userId) ||
                !decimal.TryParse(txtAmount.Text, out decimal amount))
            {
                MessageBox.Show("Please enter valid User ID and Amount", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // Verify user exists and get names
                    using (var userCmd = new MySqlCommand("SELECT first_name, last_name FROM users WHERE user_id = @userId", conn))
                    {
                        userCmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = userCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("User not found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            txtFirstName.Text = reader["first_name"].ToString();
                            txtLastName.Text = reader["last_name"].ToString();
                        }
                    }

                    // Insert fine
                    using (var insertCmd = new MySqlCommand(@"
                        INSERT INTO fines (user_id, amount, date_issued, status)
                        VALUES (@userId, @amount, NOW(), 'Pending')", conn))
                    {
                        insertCmd.Parameters.AddWithValue("@userId", userId);
                        insertCmd.Parameters.AddWithValue("@amount", amount);

                        if (insertCmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Fine added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearInputFields();
                            LoadAllFines();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add fine.", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding fine: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (rbPaidOnly.IsChecked == true)
            {
                LoadFinesByStatus("Paid");
            }
            else
            {
                LoadFinesByStatus(""); // Load all
            }
        }

        private void txtUserId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtUserId.Text, out int userId))
            {
                try
                {
                    using (var conn = new MySqlConnection(connStr))
                    using (var cmd = new MySqlCommand("SELECT first_name, last_name FROM users WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFirstName.Text = reader["first_name"].ToString();
                                txtLastName.Text = reader["last_name"].ToString();
                            }
                            else
                            {
                                txtFirstName.Text = string.Empty;
                                txtLastName.Text = string.Empty;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching user details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            txtUserId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtFineId.Text = "";
            txtAmount.Text = "";
        }

        // Navigation Methods
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianDashboard().Show();
            Close();
        }

        

        private void BookStatusButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianBookStatus().Show();
            Close();
        }

        private void IssueReserveButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianIssue().Show();
            Close();
        }

        private void MemberRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianMemberRecords().Show();
            Close();
        }

        private void FineCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            // Already on this page
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianAccount(UserSession.CurrentUserId).Show();
            Close();
        }

        public class Fine
        {
            public int FineId { get; set; }
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public decimal Amount { get; set; }
            public DateTime Date { get; set; }
            public string Status { get; set; }
            public string FullName => $"{FirstName} {LastName}";
        }
    }
}
