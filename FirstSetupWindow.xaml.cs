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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using Budget;

namespace WPF
{
    /// <summary>
    /// Interaction logic for FirstSetupWindow.xaml
    /// </summary>
    public partial class FirstSetupWindow : Window
    {
        public HomeBudget budget { get; set; }
        public FirstSetupWindow()
        {
            InitializeComponent();
        }

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if(txtbDir.Text == "")
            {
                System.Windows.MessageBox.Show("Directory cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtbFile.Text == "")
            {
                System.Windows.MessageBox.Show("File name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string databaseFile = txtbDir.Text + "\\\\" + txtbFile.Text + ".db";
                budget = new HomeBudget(databaseFile, true);
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Could not create database", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                StreamWriter file = new StreamWriter("./config.txt", false);
                file.WriteLine(txtbDir.Text);
                file.WriteLine(txtbFile.Text + ".db");
                file.Close();
            }
            catch(Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Could not create config file", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.Close();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK)
                {
                    txtbDir.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
