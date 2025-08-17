using System;
using System.Windows;
using library_management_system.L;
using Library_management_system;
using LibrarySystem;

namespace Library_Management_System
{
    public static class UserSession
    {
        public static int CurrentUserId { get; private set; }

        public static void Login(int userId)
        {
            CurrentUserId = userId;
        }

        public static void Logout()
        {
            CurrentUserId = 0;
        }
    }

    public partial class LibrarianAccount : Window
    {
        private int _userId;

        public LibrarianAccount(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadUserInfo();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e) => SwitchWindow(new LibrarianDashboard());
        private void BookStatus_Click(object sender, RoutedEventArgs e) => SwitchWindow(new LibrarianBookStatus());
        private void IssueReserve_Click(object sender, RoutedEventArgs e) => SwitchWindow(new LibrarianIssue());
        private void MemberRecords_Click(object sender, RoutedEventArgs e) => SwitchWindow(new LibrarianMemberRecords());
        private void FineCollection_Click(object sender, RoutedEventArgs e) => SwitchWindow(new FineCollectionPage());

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You are already on the Account page.");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            SwitchWindow(new MainWindow());
        }

        private void SwitchWindow(Window window)
        {
            window.Show();
            this.Close();
        }

        private void LoadUserInfo()
        {
            // Simulate data for development/demo purposes
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            EmailTextBox.Text = "";
        }

        private void UpdateInformation_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Account information updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
