using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace library_management_system
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Force dark theme initialization
            FrameworkElement.StyleProperty.OverrideMetadata(
                typeof(Window),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = Application.Current.FindResource(typeof(Window))
                });

            base.OnStartup(e);
        }
    }
}