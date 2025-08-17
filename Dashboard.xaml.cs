using System;
using System.Configuration;
using System.Windows;
using MySql.Data.MySqlClient;

namespace library_management_system
{
    public partial class Dashboard : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public Dashboard()
        {
            InitializeComponent();

            // Get connection string from App.config or hardcode here

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                TotalBooksTextBlock.Text = $"Total Books: {ExecuteScalar(conn, "SELECT COUNT(*) FROM books;")}";
                ActiveMembersTextBlock.Text = $"Active Members: {ExecuteScalar(conn, "SELECT COUNT(*) FROM users WHERE role='librarian' OR 'student';")}";
                OverdueBooksTextBlock.Text = $"Overdue Books: {ExecuteScalar(conn, "SELECT COUNT(*) FROM transactions WHERE due_date < CURDATE() AND return_date IS NULL;")}";

                // Assuming you have created_at column in books for new books added today
                NewBooksAddedTextBlock.Text = $"New Books Added: {ExecuteScalar(conn, "SELECT COUNT(*) FROM books ")}";

                // Assuming borrow_date in borrow_records for books borrowed today
                BooksBorrowedTodayTextBlock.Text = $"Books Borrowed Today: {ExecuteScalar(conn, "SELECT COUNT(*) FROM transactions WHERE DATE(borrow_date) = CURDATE();")}";

                // Assuming returned_date in borrow_records for books returned today
                BooksReturnedTodayTextBlock.Text = $"Books Returned Today: {ExecuteScalar(conn, "SELECT COUNT(*) FROM transactions WHERE DATE(return_date) = CURDATE();")}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private int ExecuteScalar(MySqlConnection conn, string query)
        {
            using var cmd = new MySqlCommand(query, conn);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        // Navigation button handlers (your existing methods)
        private void GoToDashboard(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You are already on the Dashboard page.");
        }

        private void GoToManageUsers(object sender, RoutedEventArgs e)
        {
            var manageUsers = new ManageUsers();
            manageUsers.Show();
            this.Close();
        }

        private void GoToBorrowingPolicies(object sender, RoutedEventArgs e)
        {
            var borrowingPolicies = new BorrowingPolicies();
            borrowingPolicies.Show();
            this.Close();
        }

        private void GoToBooks(object sender, RoutedEventArgs e)
        {
            var booksManagement = new BooksManagement();
            booksManagement.Show();
            this.Close();
        }

        private void GoToReports(object sender, RoutedEventArgs e)
        {
            var reports = new Reports();
            reports.Show();
            this.Close();
        }

        private void GoToAccount(object sender, RoutedEventArgs e)
        {
            var account = new AdminAccount();
            account.Show();
            this.Close();
        }
    }
}
