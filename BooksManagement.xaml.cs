using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace library_management_system
{
    public partial class BooksManagement : Window
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        private ObservableCollection<BookModel> books = new ObservableCollection<BookModel>();

        public BooksManagement()
        {
            InitializeComponent();
            LoadBooksFromDatabase();
        }

        private void LoadBooksFromDatabase()
        {
            books.Clear();

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT * FROM books";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new BookModel                                                                     
                            {
                                BookId = Convert.ToInt32(reader["book_id"]),
                                Title = reader["title"].ToString(),
                                Author = reader["author"].ToString(),
                                Genre = reader["genre"].ToString(),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                AvailableQuantity = Convert.ToInt32(reader["available_quantity"]),
                                Status = reader["status"].ToString()
                            });
                        }
                    }
                }

                BookList.ItemsSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshBookList_Click(object sender, RoutedEventArgs e)
        {
            LoadBooksFromDatabase();
        }

        private void ClearInputFields_Click(object sender, RoutedEventArgs e)
        {
            TitleBox.Clear();
            AuthorBox.Clear();
            FictionCheckBox.IsChecked = false;
            NonFictionCheckBox.IsChecked = false;
            ScienceCheckBox.IsChecked = false;
            HistoryCheckBox.IsChecked = false;
            BiographyCheckBox.IsChecked = false;
            QuantityBox.Clear();
            TitleBox.Focus();
        }

        private void AddBook(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleBox.Text) ||
                    string.IsNullOrWhiteSpace(AuthorBox.Text) ||
                    string.IsNullOrWhiteSpace(QuantityBox.Text))
                {
                    MessageBox.Show("Please fill in all required fields", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string genre = "";
                if (FictionCheckBox.IsChecked == true) genre += "Fiction, ";
                if (NonFictionCheckBox.IsChecked == true) genre += "Non-Fiction, ";
                if (ScienceCheckBox.IsChecked == true) genre += "Science, ";
                if (HistoryCheckBox.IsChecked == true) genre += "History, ";
                if (BiographyCheckBox.IsChecked == true) genre += "Biography, ";

                if (string.IsNullOrWhiteSpace(genre))
                {
                    MessageBox.Show("Please select at least one genre", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                genre = genre.TrimEnd(',', ' ');

                int quantity = int.Parse(QuantityBox.Text.Trim());

                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string insertQuery = @"INSERT INTO books (title, author, genre, quantity, available_quantity, status)
                                           VALUES (@title, @author,
                                           @genre, @quantity, @available_quantity, @status)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", TitleBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@author", AuthorBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@genre", genre);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@available_quantity", quantity);
                        cmd.Parameters.AddWithValue("@status", "Available");

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearInputFields_Click(sender, e);
                LoadBooksFromDatabase();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid number for quantity", "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void QuantityBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }

            if (sender is TextBox textBox && e.Text == "0" && textBox.Text.Length == 0)
            {
                e.Handled = true;
            }
        }

        private void QuantityBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete ||
                e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Up || e.Key == Key.Down ||
                e.Key == Key.Home || e.Key == Key.End)
            {
                return;
            }

            if ((e.Key < Key.D0 || e.Key > Key.D9) &&
                (e.Key < Key.NumPad0 || e.Key > Key.NumPad9))
            {
                e.Handled = true;
            }
        }

        private void GoToDashboard(object sender, RoutedEventArgs e) => NavigateToWindow(new Dashboard());
        private void GoToManageUsers(object sender, RoutedEventArgs e) => NavigateToWindow(new ManageUsers());
        private void GoToBorrowingPolicies(object sender, RoutedEventArgs e) => NavigateToWindow(new BorrowingPolicies());
        private void GoToBooks(object sender, RoutedEventArgs e) => MessageBox.Show("You are already on the Book Inventory page");
        private void GoToReports(object sender, RoutedEventArgs e) => NavigateToWindow(new Reports());
        private void GoToAccount(object sender, RoutedEventArgs e) => NavigateToWindow(new AdminAccount());

        private void NavigateToWindow(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
            this.Close();
        }


        // ✅ BookModel defined inside this class to avoid ambiguity
        private class BookModel
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
}
