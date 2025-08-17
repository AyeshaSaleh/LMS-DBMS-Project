using System;
using System.Windows;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using library_management_system.L;
using LibrarySystem;
using System.Diagnostics;

namespace Library_Management_System
{
    public partial class LibrarianDashboard : Window
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public LibrarianDashboard()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // 1. Book Summary
                    string bookQuery = @"
                        SELECT 
                            COUNT(*) AS TotalBooks,
                            SUM(available_quantity) AS AvailableBooks,
                            SUM(quantity - available_quantity) AS IssuedBooks
                        FROM books;
                    ";

                    MySqlCommand cmd = new MySqlCommand(bookQuery, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTotalBooks.Text = reader["TotalBooks"].ToString();
                            txtAvailableBooks.Text = reader["AvailableBooks"].ToString();
                            txtIssuedBooks.Text = reader["IssuedBooks"].ToString();
                        }
                    }

                    // 2. Late Returns
                    string lateReturnsQuery = "SELECT COUNT(*) FROM notifications WHERE notification_type = 'late_return';";
                    cmd = new MySqlCommand(lateReturnsQuery, conn);
                    txtLateReturns.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard data: " + ex.Message);
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Already on Dashboard.");
        }

        

        private void BookStatusButton_Click(object sender, RoutedEventArgs e)
        {
            Navigate(new LibrarianBookStatus());
        }

        private void IssueReserveButton_Click(object sender, RoutedEventArgs e)
        {
            Navigate(new LibrarianIssue());
        }

        
        private void MemberRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            Navigate(new LibrarianMemberRecords());
        }

        private void FineCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            Navigate(new FineCollectionPage());
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianAccount(UserSession.CurrentUserId).Show();
            this.Close();
        }
        private void Navigate(Window window)
        {
            window.Show();
            this.Close();
        }
    }
}
