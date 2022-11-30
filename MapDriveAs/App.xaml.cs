using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MapDriveAs
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 2)
            {
                NetConnect.ConnectAs(e.Args[0], e.Args[1]);
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    }
}
