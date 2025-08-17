using System.Windows;

namespace Library_management_system
{
    public partial class ProjectOverview : Window
    {
        public ProjectOverview()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}