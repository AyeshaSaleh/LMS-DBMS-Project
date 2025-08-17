using library_management_system.Models;
using library_management_system.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace library_management_system
{
    public partial class ManageUsers : Window
    {
        private readonly UserService _userService;
        private User? _selectedUser;

        public ManageUsers()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadUsers();
            ClearFields();
        }

        private void LoadUsers()
        {
            var users = _userService.GetAllUsers();
            dgUsers.ItemsSource = users.ToList();
        }

        private void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please enter both Name and Email.");
                    return;
                }

                string role = AdminCheckBox.IsChecked == true ? "Admin" :
                              LibrarianCheckBox.IsChecked == true ? "Librarian" : "Member";

                string status = ActiveCheckBox.IsChecked == true ? "Active" : "Inactive";

                if (_selectedUser == null)
                {
                    // Add new user
                    var newUser = new User
                    {
                        Name = txtName.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Role = role,
                        Status = status
                    };

                    if (_userService.AddUser(newUser))
                    {
                        MessageBox.Show("User added successfully.");
                        LoadUsers();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("A user with this email already exists.");
                    }
                }
                else
                {
                    // Update existing user
                    _selectedUser.Name = txtName.Text.Trim();
                    _selectedUser.Email = txtEmail.Text.Trim();
                    _selectedUser.Role = role;
                    _selectedUser.Status = status;

                    if (_userService.UpdateUser(_selectedUser))
                    {
                        MessageBox.Show("User updated successfully.");
                        LoadUsers();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update user.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving user: " + ex.Message);
            }
        }

        private void ClearFields()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            AdminCheckBox.IsChecked = false;
            LibrarianCheckBox.IsChecked = false;
            MemberCheckBox.IsChecked = true;
            ActiveCheckBox.IsChecked = true;
            InactiveCheckBox.IsChecked = false;
            _selectedUser = null;
            btnSaveUser.Content = "Add User";
            dgUsers.SelectedItem = null;
        }

        private void ClearInputFields_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selected)
            {
                _selectedUser = selected;
                txtName.Text = selected.Name;
                txtEmail.Text = selected.Email;
                AdminCheckBox.IsChecked = selected.Role == "Admin";
                LibrarianCheckBox.IsChecked = selected.Role == "Librarian";
                MemberCheckBox.IsChecked = selected.Role == "Member";
                ActiveCheckBox.IsChecked = selected.Status == "Active";
                InactiveCheckBox.IsChecked = selected.Status == "Inactive";
                btnSaveUser.Content = "Update User";
            }
            else
            {
                ClearFields();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            var results = _userService.SearchUsers(searchTerm);
            dgUsers.ItemsSource = results.ToList();
        }

        // --- Navigation buttons (assuming your XAML buttons call these methods) ---

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = new Dashboard(); // Replace with your actual window
            dashboard.Show();
            this.Close();
        }

        private void BookInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            var books = new BooksManagement();
            books.Show();
            this.Close();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var account = new AdminAccount();
            account.Show();
            this.Close();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            var reports = new Reports();
            reports.Show();
            this.Close();
        }

        private void BorrowingPoliciesButton_Click(object sender, RoutedEventArgs e)
        {
            var policies = new BorrowingPolicies();
            policies.Show();
            this.Close();
        }

        private void ManageUsersButton_Click(object sender, RoutedEventArgs e)
        {
            // This can reload the same window or be omitted
            var manageUsers = new ManageUsers();
            manageUsers.Show();
            this.Close();
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            var booksWindow = new BooksManagement();
            booksWindow.Show();
            this.Close();
        }
    }
}
