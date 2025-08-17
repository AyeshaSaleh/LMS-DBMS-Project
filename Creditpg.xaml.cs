using System.Windows;

// FOR DB - STEP 01
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Library_management_system
{
    public partial class Creditpg : Window
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString; //Mandatory - STEP 02
        public Creditpg()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of MainWindow
            MainWindow mainWindow = new MainWindow();

            // Show the MainWindow
            mainWindow.Show();

            // Close the current Creditpg window
            this.Close();
        }
    }
}