using System.Configuration;
using System.Windows;
using library_management_system;


namespace Library_management_system
{
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EnterSystem_Click(object sender, RoutedEventArgs e)
        {
            // Open the login window
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void CreditPage_Click(object sender, RoutedEventArgs e)
        {
            // Open the credit page window
            Creditpg creditWindow = new Creditpg();
            creditWindow.Show();
            this.Hide();
        }
        private void ProjectOverview_Click(object sender, RoutedEventArgs e)
        {
            ProjectOverview overview = new ProjectOverview();
            overview.Show();
            this.Close();
        }

    }
}