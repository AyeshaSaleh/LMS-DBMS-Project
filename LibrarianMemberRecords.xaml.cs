using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using library_management_system.L;
using LibrarySystem;

namespace Library_Management_System
{
    public partial class LibrarianMemberRecords : Window
    {
        private string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        private int currentUserId = 0;

        public class Member
        {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string FullName => $"{FirstName} {LastName}";
        }

        public class Transaction
        {
            public string BookTitle { get; set; }
            public DateTime BorrowDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Status { get; set; }
        }

        public LibrarianMemberRecords()
        {
            InitializeComponent();
            LoadAllMembers();
        }

        private void LoadAllMembers()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT user_id, first_name, last_name, email, phone_number FROM Users";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    List<Member> members = new List<Member>();
                    foreach (DataRow row in dt.Rows)
                    {
                        members.Add(new Member
                        {
                            UserId = Convert.ToInt32(row["user_id"]),
                            FirstName = row["first_name"].ToString(),
                            LastName = row["last_name"].ToString(),
                            Email = row["email"].ToString(),
                            PhoneNumber = row["phone_number"].ToString()
                        });
                    }

                    MembersListView.ItemsSource = members;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadAllMembers();
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"SELECT user_id, first_name, last_name, email, phone_number 
                                   FROM Users 
                                   WHERE first_name LIKE @search OR last_name LIKE @search 
                                   OR email LIKE @search OR phone_number LIKE @search";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", $"%{searchTerm}%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    List<Member> members = new List<Member>();
                    foreach (DataRow row in dt.Rows)
                    {
                        members.Add(new Member
                        {
                            UserId = Convert.ToInt32(row["user_id"]),
                            FirstName = row["first_name"].ToString(),
                            LastName = row["last_name"].ToString(),
                            Email = row["email"].ToString(),
                            PhoneNumber = row["phone_number"].ToString()
                        });
                    }

                    MembersListView.ItemsSource = members;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MembersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MembersListView.SelectedItem is Member selectedMember)
            {
                currentUserId = selectedMember.UserId;
                DisplayMemberDetails(selectedMember);
                LoadMemberTransactions(selectedMember.UserId);
            }
        }

        private void DisplayMemberDetails(Member member)
        {
            MemberNameText.Text = member.FullName;
            UserIdText.Text = member.UserId.ToString();
            EmailText.Text = member.Email;
            PhoneText.Text = member.PhoneNumber;
        }

        private void LoadMemberTransactions(int userId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"SELECT b.title AS book_title, t.borrow_date, t.due_date, 
                                     CASE 
                                         WHEN t.return_date IS NOT NULL THEN 'Returned'
                                         WHEN t.due_date < CURDATE() THEN 'Overdue'
                                         ELSE 'Borrowed'
                                     END AS status
                                     FROM Transactions t
                                     JOIN Books b ON t.book_id = b.book_id
                                     WHERE t.user_id = @userId
                                     ORDER BY t.due_date DESC";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    List<Transaction> transactions = new List<Transaction>();
                    foreach (DataRow row in dt.Rows)
                    {
                        transactions.Add(new Transaction
                        {
                            BookTitle = row["book_title"].ToString(),
                            BorrowDate = Convert.ToDateTime(row["borrow_date"]),
                            DueDate = Convert.ToDateTime(row["due_date"]),
                            Status = row["status"].ToString()
                        });
                    }

                    TransactionsDataGrid.ItemsSource = transactions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRecord_Click(object sender, RoutedEventArgs e)
        {
            if (currentUserId == 0)
            {
                MessageBox.Show("Please select a member first.", "Update", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Here you would implement your update logic
            MessageBox.Show($"Record for user ID {currentUserId} would be updated here.", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Navigation methods
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            LibrarianDashboard dashboard = new LibrarianDashboard();
            dashboard.Show();
            this.Close();
        }

      

        private void BookStatus_Click(object sender, RoutedEventArgs e)
        {
            LibrarianBookStatus bookStatus = new LibrarianBookStatus();
            bookStatus.Show();
            this.Close();
        }

        private void IssueReserve_Click(object sender, RoutedEventArgs e)
        {
            LibrarianIssue issueWindow = new LibrarianIssue();
            issueWindow.Show();
            this.Close();
        }

        private void MemberRecords_Click(object sender, RoutedEventArgs e)
        {
            // Already on this page
        }

        private void FineCollection_Click(object sender, RoutedEventArgs e)
        {
            FineCollectionPage records = new FineCollectionPage();
            records.Show();
            this.Close();
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianAccount(UserSession.CurrentUserId).Show();
            this.Close();
        }
    }
}