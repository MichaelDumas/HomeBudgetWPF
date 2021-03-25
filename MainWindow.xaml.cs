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
using Budget;
using System.IO;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HomeBudget budget;
        public MainWindow()
        {
            if(!File.Exists("./config.txt"))
            {
                FirstSetupWindow setup = new FirstSetupWindow();
                setup.ShowDialog();
                if(!File.Exists("./config.txt"))
                    this.Close();
            }
            else
            {
                string filepath;
                StreamReader streamReader = new StreamReader("./config.txt");
                filepath = streamReader.ReadLine();
                filepath += "\\" + streamReader.ReadLine();
                streamReader.Close();
                budget = new HomeBudget(filepath, false);
            }
            InitializeComponent();

        }
        public void CloseDatabase()
        {
            budget.CloseDB();
        }

        private void btnAddExp_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow expenseWindow = new AddExpenseWindow(budget);
            expenseWindow.ShowDialog();
        }

        private void btnAddCat_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow categoryWindow = new AddCategoryWindow(budget);
            categoryWindow.ShowDialog();
        }
    }
}
