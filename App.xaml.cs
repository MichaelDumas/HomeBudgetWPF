using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            MessageBox.Show("Made it to app exit ", "yay", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow window = new MainWindow();
            window.budget.CloseDB();
        }
    }
}
