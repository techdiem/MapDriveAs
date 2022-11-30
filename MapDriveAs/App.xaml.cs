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
            if (e.Args.Length >= 1)
            {
                //CLI Mode
                cli_parse(e.Args);
                Environment.Exit(0);
            }
            else
            {
                //Gui Mode
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        private void cli_parse(string[] args)
        {
            switch (args[0])
            {
                case "mount":
                    if (args.Length == 3)
                    {
                        cli_mount(args[1], args[2]);
                    }
                    else
                    {
                        cli_help();
                    }
                    break;
                case "unmount":
                    if (args.Length == 2)
                    {
                        cli_unmount(args[1]);
                    }
                    else
                    {
                        cli_help();
                    }
                    break;
                case "toggle":
                    if (args.Length == 3)
                    {
                        cli_toggle(args[1], args[2]);
                    }
                    else
                    {
                        cli_help();
                    }
                    break;
                default:
                    cli_help();
                    break;
            }
        }

        private void cli_mount(string letter, string share)
        {
            NetConnect.ConnectAs(letter, share);
        }

        private void cli_unmount(string letter)
        {
            NetConnect.UnmountNetworkDrive(letter);
        }

        private void cli_toggle(string letter, string share)
        {
            if (NetConnect.GetConnectionStatus(letter))
            {
                cli_unmount(letter);
            }
            else
            {
                cli_mount(letter, share);
            }
        }

        private void cli_help()
        {
            string help_text = @"
MapDriveAs Usage:

MapDriveAs.exe [command] [DriveLetter] [SharePath]
Commands: mount, unmount, toggle

Example:
MapDriveAs.exe mount ""Y:"" ""\\filesrv.local\share""
MapDriveAs.exe unmount ""Y:""
MapDriveAs.exe toggle ""Y:"" ""\\filesrv.local\share""

GUI Mode:
Without any arguments, a gui will start where you can map drive Y to some share.
                ";

            MessageBox.Show(help_text, "MapDriveAs", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
