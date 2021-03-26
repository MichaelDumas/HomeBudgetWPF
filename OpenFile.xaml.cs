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
using Budget;
using System.IO;
using System.Windows.Forms;

namespace WPF
{
    /// <summary>
    /// Interaction logic for OpenFile.xaml
    /// </summary>
    public partial class OpenFile : Window
    {
        public HomeBudget budget { get; set; }
        public OpenFile()
        {
            InitializeComponent();
        }

        private void btnOpenLastUsed_Click(object sender, RoutedEventArgs e)
        {
            string filepath;
            StreamReader streamReader = new StreamReader("./config.txt");
            filepath = streamReader.ReadLine();
            filepath += "\\" + streamReader.ReadLine();
            streamReader.Close();
            budget = new HomeBudget(filepath, false);
            Close();
        }

        private void btnOpenExisting_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            StreamReader streamReader = new StreamReader("./config.txt");
            openFileDialog.Filter = "Database files (*.db)|*.db";
            openFileDialog.InitialDirectory = streamReader.ReadLine();
            streamReader.Close();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                budget = new HomeBudget(openFileDialog.FileName, false);
                try
                {
                    StreamWriter file = new StreamWriter("./config.txt", false);
                    string[] path = openFileDialog.FileName.Split('\\');
                    string filepath = "";
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        if (i != path.Length - 2)
                            filepath += path[i] + "\\";
                        else
                            filepath += path[i];
                    }
                    file.WriteLine(filepath);
                    file.WriteLine(openFileDialog.SafeFileName);
                    file.Close();
                }
                catch (Exception error)
                {
                    System.Windows.MessageBox.Show(error.Message, "Could not write to config file", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            Close();
        }

        private void btnCreateNew_Click(object sender, RoutedEventArgs e)
        {
            FirstSetupWindow createNew = new FirstSetupWindow();
            createNew.ShowDialog();
            if(createNew.budget != null)
            {
                budget = createNew.budget;
                Close();
            }
        }
    }
}
