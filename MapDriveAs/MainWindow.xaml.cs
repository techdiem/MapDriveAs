using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace MapDriveAs
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            remoteHost.Focus();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (remoteHost.Text != "")
            {

                NetConnect.ConnectAs("Y:", remoteHost.Text);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            } 
            else if (e.Key == Key.Escape)
            {
                Environment.Exit(0);
            }
        }
    }
}
