using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using library_management_system;
using Library_Management_System;
using System.Windows.Navigation;

// FOR DB - STEP 01
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Library_management_system
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            UsernameTextBox.Focus(); // Ensure it focuses on load
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString; //Mandatory - STEP 02

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Please enter both username and password.", "Login Error", MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open(); //Open Connection wiith DB
                    string query = "SELECT role FROM users WHERE username = @username AND password_hash = @password"; //SQL Query
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); // 

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string role = result.ToString().ToLower();

                        switch (role)
                        {
                            case "admin":
                                OpenAdminDashboard();
                                break;
                            case "librarian":
                                OpenLibrarianDashboard();
                                break;
                            case "student":
                               // OpenStudentDashboard();
                                break;
                            default:
                                ShowMessage("Unknown user role.", "Login Error", MessageBoxImage.Error);
                                break;
                        }
                    }
                    else
                    {
                        ShowMessage("Invalid username or password.", "Login Failed", MessageBoxImage.Error);
                        PasswordBox.Clear();
                        PasswordBox.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Database connection failed: " + ex.Message, "Error", MessageBoxImage.Error);
            }
        }


        private void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        private void OpenAdminDashboard()
        {
            var adminDashboard = new Dashboard();
            Application.Current.MainWindow = adminDashboard;
            adminDashboard.Show();
            this.Close();
        }
        /*
        private void OpenStudentDashboard()
        {
            var studentDashboard = new StudentDashboard();
            Application.Current.MainWindow = studentDashboard;
            studentDashboard.Show();
            this.Close();
        }*/
        private void OpenLibrarianDashboard()
        {
            var librarianDashboard = new LibrarianDashboard();
            Application.Current.MainWindow = librarianDashboard;
            librarianDashboard.Show();
            this.Close();
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == UsernameTextBox)
                {
                    PasswordBox.Focus();
                }
                else if (sender == PasswordBox)
                {
                    AttemptLogin();
                }
            }
        }

        private void ForgotPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowMessage("Please contact the system administrator to reset your password.",
                      "Forgot Password",
                      MessageBoxImage.Information);
        }

        private void CreateAccount_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowMessage("Account creation is currently administrator-only. Please contact your admin.",
                      "Create Account",
                      MessageBoxImage.Information);
        }
    }
}