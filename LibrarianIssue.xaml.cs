using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using library_management_system.L;
using Library_Management_System;
using MySql.Data.MySqlClient;

namespace LibrarySystem
{
    public class Student
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Book
    {
        public string BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int AvailableQuantity { get; set; }
        public string Status { get; set; }
    }

    public class IssuedBook
    {
        public int IssueId { get; set; }
        public string BookTitle { get; set; }
        public string IssueDate { get; set; }
        public string DueDate { get; set; }
    }

    public partial class LibrarianIssue : Window
    {
        private string connectionString = ConfigurationManager
            .ConnectionStrings["MySqlConnection"].ConnectionString;

        public LibrarianIssue()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
            dpDueDate.SelectedDate = DateTime.Today.AddDays(14);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                if (!TableExists(conn, "users") || !TableExists(conn, "books"))
                {
                    MessageBox.Show("Required tables missing. Check database setup.",
                        "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!TableExists(conn, "book_issue"))
                    CreateBookIssueTable(conn);

                dgStudents.ItemsSource = LoadStudents(conn);
                dgBooks.ItemsSource = LoadBooks(conn);
                dgIssuedBooks.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool TableExists(MySqlConnection conn, string tableName)
        {
            string qry = @"SELECT 1 FROM information_schema.tables 
                           WHERE table_schema = DATABASE() AND table_name = @tbl";
            using var cmd = new MySqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@tbl", tableName);
            return cmd.ExecuteScalar() != null;
        }

        private void CreateBookIssueTable(MySqlConnection conn)
        {
            string qry = @"
                CREATE TABLE IF NOT EXISTS book_issue (
                    issue_id INT AUTO_INCREMENT PRIMARY KEY,
                    book_id VARCHAR(50),
                    user_id VARCHAR(50),
                    issue_date DATE,
                    due_date DATE,
                    return_date DATE NULL,
                    status VARCHAR(20) DEFAULT 'Issued',
                    fine_amount DECIMAL(10,2) DEFAULT 0,
                    FOREIGN KEY (book_id) REFERENCES books(book_id),
                    FOREIGN KEY (user_id) REFERENCES users(user_id)
                );";
            using var cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
        }

        private List<Student> LoadStudents(MySqlConnection conn)
        {
            var list = new List<Student>();
            string qry = @"SELECT user_id, first_name, last_name, email, phone_number
                           FROM users WHERE role = 'student'";
            using var cmd = new MySqlCommand(qry, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Student
                {
                    UserId = rdr["user_id"].ToString(),
                    FirstName = rdr["first_name"].ToString(),
                    LastName = rdr["last_name"].ToString(),
                    Email = rdr["email"].ToString(),
                    Phone = rdr["phone_number"].ToString()
                });
            }
            return list;
        }

        private List<Book> LoadBooks(MySqlConnection conn)
        {
            var list = new List<Book>();
            string qry = @"SELECT book_id, title, author, genre, available_quantity, status FROM books";
            using var cmd = new MySqlCommand(qry, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Book
                {
                    BookId = rdr["book_id"].ToString(),
                    Title = rdr["title"].ToString(),
                    Author = rdr["author"].ToString(),
                    Genre = rdr["genre"].ToString(),
                    AvailableQuantity = Convert.ToInt32(rdr["available_quantity"]),
                    Status = rdr["status"].ToString()
                });
            }
            return list;
        }

        private void dgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student s)
            {
                txtstudentuserId.Text = s.UserId;
                txtStudentfirstname.Text = s.FirstName;
                txtStudentlastname.Text = s.LastName;
                LoadIssuedBooks(s.UserId);
            }
            else
            {
                dgIssuedBooks.ItemsSource = null;
            }
        }

        private void dgBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBooks.SelectedItem is Book b)
            {
                txtSelectedBook.Text = b.Title;
                txtBookAuthor.Text = b.Author;
                txtBookStatus.Text = b.Status;
            }
        }

        private void LoadIssuedBooks(string userId)
        {
            var list = new List<IssuedBook>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string qry = @"
                SELECT bi.issue_id, b.title, bi.issue_date, bi.due_date 
                FROM book_issue bi
                JOIN books b ON bi.book_id = b.book_id
                WHERE bi.user_id = @uid AND bi.status = 'Issued'";
            using var cmd = new MySqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new IssuedBook
                {
                    IssueId = rdr.GetInt32("issue_id"),
                    BookTitle = rdr.GetString("title"),
                    IssueDate = rdr.GetDateTime("issue_date").ToShortDateString(),
                    DueDate = rdr.GetDateTime("due_date").ToShortDateString()
                });
            }
            dgIssuedBooks.ItemsSource = list;
        }

        private void IssueBook_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is not Student s || dgBooks.SelectedItem is not Book b)
            {
                MessageBox.Show("Please select both a student and a book", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (b.AvailableQuantity <= 0)
            {
                MessageBox.Show("Book not available", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string check = @"SELECT COUNT(*) FROM book_issue WHERE book_id=@bid AND user_id=@uid AND status='Issued'";
            using (var cmd = new MySqlCommand(check, conn))
            {
                cmd.Parameters.AddWithValue("@bid", b.BookId);
                cmd.Parameters.AddWithValue("@uid", s.UserId);
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    MessageBox.Show("Book already issued to this student", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            using (var tx = conn.BeginTransaction())
            {
                new MySqlCommand(
                    @"INSERT INTO book_issue (book_id, user_id, issue_date, due_date)
                      VALUES (@bid, @uid, @idate, @ddate)", conn, tx)
                {
                    Parameters =
                    {
                        new MySqlParameter("@bid", b.BookId),
                        new MySqlParameter("@uid", s.UserId),
                        new MySqlParameter("@idate", DateTime.Today),
                        new MySqlParameter("@ddate", dpDueDate.SelectedDate ?? DateTime.Today.AddDays(14))
                    }
                }.ExecuteNonQuery();

                new MySqlCommand(
                    @"UPDATE books SET available_quantity = available_quantity - 1 WHERE book_id = @bid",
                    conn, tx)
                {
                    Parameters = { new MySqlParameter("@bid", b.BookId) }
                }.ExecuteNonQuery();

                tx.Commit();
            }

            MessageBox.Show("Book issued successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDataFromDatabase();
        }

        // Navigation methods (unchanged)...

        private void Dashboard_Click(object sender, RoutedEventArgs e) => OpenWindow(new LibrarianDashboard());
        private void BookStatus_Click(object sender, RoutedEventArgs e) => OpenWindow(new LibrarianBookStatus());
        private void IssueReserve_Click(object sender, RoutedEventArgs e) { }
        private void MemberRecords_Click(object sender, RoutedEventArgs e) => OpenWindow(new LibrarianMemberRecords());
        private void FineCollectionClick(object sender, RoutedEventArgs e) => OpenWindow(new FineCollectionPage());
        private void Account_Click(object sender, RoutedEventArgs e) => OpenWindow(new LibrarianAccount(UserSession.CurrentUserId));
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void OpenWindow(Window w)
        {
            w.Show();
            Close();
        }
    }
}
