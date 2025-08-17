using System;
using System.Windows;
using Library_management_system;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace library_management_system
{
    public partial class AdminAccount : Window
    {
        // STEP 01: Connection string from App.config
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        // STEP 02: Current logged-in user ID (replace with actual logic to fetch this dynamically)
        private int currentUserId = 1;

        public AdminAccount()
        {
            InitializeComponent();
        }

        // Navigation event handlers
        private void GoToDashboard(object sender, RoutedEventArgs e)
        {
            var dashboard = new Dashboard();
            dashboard.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dashboard.Show();
            this.Close();
        }

        private void GoToManageUsers(object sender, RoutedEventArgs e)
        {
            var manageUsers = new ManageUsers();
            manageUsers.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            manageUsers.Show();
            this.Close();
        }

        private void GoToBorrowingPolicies(object sender, RoutedEventArgs e)
        {
            var borrowingPolicies = new BorrowingPolicies();
            borrowingPolicies.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            borrowingPolicies.Show();
            this.Close();
        }

        private void GoToBookInventory(object sender, RoutedEventArgs e)
        {
            var bookInventory = new BooksManagement();
            bookInventory.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            bookInventory.Show();
            this.Close();
        }

        private void GoToReports(object sender, RoutedEventArgs e)
        {
            var reports = new Reports();
            reports.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            reports.Show();
            this.Close();
        }

        private void GoToAccount(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You are already on the Account page.");
        }

        // Event handler for updating personal information
        private void UpdateInformation(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                PasswordBox.SecurePassword.Length == 0)
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"
                        UPDATE update_information
                        SET 
                            first_name = @FirstName,
                            last_name = @LastName,
                            email = @Email,
                            password_hash = SHA2(@Password, 256)
                        WHERE user_id = @UserId;
                    ";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", FirstNameBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", LastNameBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", EmailBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", PasswordBox.Password);
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Personal information updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Update failed. No matching user found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmailBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Email validation logic (optional)
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.Show();
            this.Close();
        }
    }
}
