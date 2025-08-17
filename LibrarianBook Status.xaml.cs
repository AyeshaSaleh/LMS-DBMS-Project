using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using System.Configuration;
using Library_Management_System;
using LibrarySystem;

namespace library_management_system.L
{
    public partial class LibrarianBookStatus : Window
    {
        private string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public LibrarianBookStatus()
        {
            InitializeComponent();
            LoadBooks(); // Load books when window initializes
        }

        private void LoadBooks(string filterQuery = "")
        {
            try
            {
                BooksPanel.Children.Clear();
                var books = GetBooksFromDatabase(filterQuery);

                if (books.Count == 0)
                {
                    var noResultsText = new TextBlock
                    {
                        Text = "No books found matching your criteria",
                        Foreground = Brushes.White,
                        FontSize = 18,
                        Margin = new Thickness(20)
                    };
                    BooksPanel.Children.Add(noResultsText);
                    return;
                }

                foreach (var book in books)
                {
                    var border = new Border
                    {
                        Style = (Style)FindResource("BookCard"),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1F3E"))
                    };

                    var stack = new StackPanel();

                    // Title
                    stack.Children.Add(new TextBlock
                    {
                        Text = book.Title,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        FontSize = 20,
                        Margin = new Thickness(5)
                    });

                    // Author
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Author: {book.Author}",
                        Foreground = Brushes.White,
                        Margin = new Thickness(5)
                    });

                    // Genre
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Genre: {book.Genre}",
                        Foreground = Brushes.White,
                        Margin = new Thickness(5)
                    });

                    // Status with color coding
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Status: {book.Status}",
                        Foreground = book.Status == "Available" ? Brushes.LightGreen : Brushes.Red,
                        Margin = new Thickness(5)
                    });

                    // Quantity information
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Available: {book.AvailableQuantity}/{book.Quantity}",
                        Foreground = Brushes.White,
                        Margin = new Thickness(5)
                    });

                    border.Child = stack;
                    BooksPanel.Children.Add(border);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<Book> GetBooksFromDatabase(string filterQuery = "")
        {
            var books = new List<Book>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string query = @"SELECT book_id, title, author, genre, 
                                   quantity, available_quantity, status 
                                   FROM Books";

                    if (!string.IsNullOrEmpty(filterQuery))
                    {
                        query += " WHERE " + filterQuery;
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book
                            {
                                BookId = reader.GetInt32("book_id"),
                                Title = reader["title"].ToString(),
                                Author = reader["author"].ToString(),
                                Genre = reader["genre"].ToString(),
                                Quantity = reader.GetInt32("quantity"),
                                AvailableQuantity = reader.GetInt32("available_quantity"),
                                Status = reader["status"].ToString()
                            });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return books;
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> filters = new List<string>();

                // Title filter
                if (!string.IsNullOrWhiteSpace(TitleFilterBox.Text))
                {
                    filters.Add($"title LIKE '%{MySqlHelper.EscapeString(TitleFilterBox.Text)}%'");
                }

                // Author filter
                if (!string.IsNullOrWhiteSpace(AuthorFilterBox.Text))
                {
                    filters.Add($"author LIKE '%{MySqlHelper.EscapeString(AuthorFilterBox.Text)}%'");
                }

                // Genre filter
                if (!string.IsNullOrWhiteSpace(GenreFilterBox.Text))
                {
                    filters.Add($"genre LIKE '%{MySqlHelper.EscapeString(GenreFilterBox.Text)}%'");
                }

                // Status filter with checkboxes
                bool showAvailable = AvailableCheckBox.IsChecked == true;
                bool showUnavailable = UnavailableCheckBox.IsChecked == true;

                if (showAvailable && !showUnavailable)
                {
                    filters.Add("available_quantity > 0");
                }
                else if (!showAvailable && showUnavailable)
                {
                    filters.Add("available_quantity = 0");
                }
                else if (!showAvailable && !showUnavailable)
                {
                    // If neither is checked, show nothing
                    filters.Add("1 = 0"); // Always false condition
                }

                string whereClause = filters.Count > 0 ? string.Join(" AND ", filters) : "";
                LoadBooks(whereClause);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Navigation methods
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianDashboard().Show();
            this.Close();
        }

       

        private void BookStatusButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You are already viewing Book Status.");
        }

        private void IssueReserveButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianIssue().Show();
            this.Close();
        }

       

        private void MemberRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianMemberRecords().Show();
            this.Close();
        }
        private void FineCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            new FineCollectionPage().Show();
            this.Close();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            new LibrarianAccount(UserSession.CurrentUserId).Show();
            this.Close();
        }
    }

    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Status { get; set; }
    }
}