using System.Windows;

// FOR DB - STEP 01
using MySql.Data.MySqlClient;
using System.Configuration;

namespace library_management_system
{
    public partial class Reports : Window
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString; //Mandatory - STEP 02
        public ReportsViewModel ViewModel { get; }

        public Reports()
        {
            InitializeComponent();
            ViewModel = new ReportsViewModel();
            DataContext = ViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BooksBorrowedChart.InvalidatePlot(true);
            BookCategoriesChart.InvalidatePlot(true);
        }

        private void GoToDashboard(object sender, RoutedEventArgs e)
        {
            new Dashboard().Show();
            this.Close();
        }

        private void GoToManageUsers(object sender, RoutedEventArgs e)
        {
            new ManageUsers().Show();
            this.Close();
        }

        private void GoToBorrowingPolicies(object sender, RoutedEventArgs e)
        {
            new BorrowingPolicies().Show();
            this.Close();
        }

        private void GoToBooks(object sender, RoutedEventArgs e)
        {
            new BooksManagement().Show();
            this.Close();
        }

        private void GoToReports(object sender, RoutedEventArgs e)
        {
            // Already on reports page
        }

        private void GoToAccount(object sender, RoutedEventArgs e)
        {
            new AdminAccount().Show();
            this.Close();
        }
    }
}