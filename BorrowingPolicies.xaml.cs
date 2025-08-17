using System.Windows;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace library_management_system
{
    public partial class BorrowingPolicies : Window
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public BorrowingPolicies()
        {
            InitializeComponent();
            LoadPoliciesFromDatabase();
        }

        private void LoadPoliciesFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT * FROM borrowing_policies WHERE policy_id = 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtStandardLoan.Text = reader["standard_loan_period_days"].ToString();
                            txtExtendedLoan.Text = reader["extended_loan_period_days"].ToString();
                            txtMaxRenewals.Text = reader["maximum_renewals"].ToString();
                            txtDailyFine.Text = reader["daily_fine_rate"].ToString();
                            txtMaxFine.Text = reader["maximum_fine"].ToString();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading policies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (!int.TryParse(txtStandardLoan.Text, out int standardLoan) || standardLoan <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Standard Loan Period.");
                return;
            }

            if (!int.TryParse(txtExtendedLoan.Text, out int extendedLoan) || extendedLoan <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Extended Loan Period.");
                return;
            }

            if (!int.TryParse(txtMaxRenewals.Text, out int maxRenewals) || maxRenewals < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Maximum Renewals.");
                return;
            }

            if (!decimal.TryParse(txtDailyFine.Text, out decimal dailyFine) || dailyFine < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Daily Fine Rate.");
                return;
            }

            if (!decimal.TryParse(txtMaxFine.Text, out decimal maxFine) || maxFine < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Maximum Fine.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string updateQuery = @"UPDATE borrowing_policies SET 
                        standard_loan_period_days = @standardLoan,
                        extended_loan_period_days = @extendedLoan,
                        maximum_renewals = @maxRenewals,
                        daily_fine_rate = @dailyFine,
                        maximum_fine = @maxFine
                        WHERE policy_id = 1";

                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);

                    cmd.Parameters.AddWithValue("@standardLoan", standardLoan);
                    cmd.Parameters.AddWithValue("@extendedLoan", extendedLoan);
                    cmd.Parameters.AddWithValue("@maxRenewals", maxRenewals);
                    cmd.Parameters.AddWithValue("@dailyFine", dailyFine);
                    cmd.Parameters.AddWithValue("@maxFine", maxFine);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show("Borrowing policies updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Failed to update borrowing policies.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error saving policies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Navigation methods (unchanged)
        private void GoToDashboard(object sender, RoutedEventArgs e)
        {
            var dashboard = new Dashboard();
            dashboard.Show();
            this.Close();
        }

        private void GoToManageUsers(object sender, RoutedEventArgs e)
        {
            var manageUsers = new ManageUsers();
            manageUsers.Show();
            this.Close();
        }

        private void GoToBorrowingPolicies(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You are already on the Borrowing Policies page.");
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

        // Optional empty event handlers (can be removed if unused)
        private void txtMinAge_TextChanged_1(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void txtMaxItems_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
    }
}
