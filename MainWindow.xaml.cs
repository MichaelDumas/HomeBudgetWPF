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

        public HomeBudget budget { get; set; }
        public MainWindow()
        {
            if(!File.Exists("./config.txt"))
            {
                FirstSetupWindow setup = new FirstSetupWindow();
                setup.ShowDialog();
                if(!File.Exists("./config.txt"))
                    this.Close();
                budget = setup.budget;
            }
            else
            {
                OpenFile open = new OpenFile();
                open.ShowDialog();
                if (open.budget is null)
                    this.Close();
                budget = open.budget;            
            }

            InitializeComponent();
            InitializeDataGrid();

        }
        private void PopulateComboBox()
        {
            cmbCategory.DisplayMemberPath = "Description";
            List<Category> categories = budget.categories.List();
            cmbCategory.ItemsSource = categories;
            cmbCategory.SelectedIndex = 1;
        }

        private void UpdateDataGrid()
        {
            DateTime start = (DateTime)dpStart.SelectedDate;
            DateTime end = (DateTime)dpEnd.SelectedDate;
            bool filterFlag = (bool)filter.IsChecked;
            int catId = cmbCategory.SelectedIndex;

            //if none are checked
            if(ByMonth.IsChecked == false && ByCategory.IsChecked == false)
            {

            }

            // if by month
            else if (ByMonth.IsChecked == true)
            {

            }
            // if by category
            else if (ByCategory.IsChecked == true)
            {
                List<BudgetItemsByCategory> budgetItemsByCategory = budget.GetBudgetItemsByCategory(start, end, filterFlag, catId);
                DataGridTextColumn catCol = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(catCol);
                catCol.Binding = new Binding("Category");

                DataGridTextColumn totalCol = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(totalCol);
                totalCol.Binding = new Binding("Total");
            }
            // if both
            else if (ByMonth.IsChecked == true && ByCategory.IsChecked == true)
            {
                List<Dictionary<string, object>> budgetItemsByCategoryAndMonth = budget.GetBudgetDictionaryByCategoryAndMonth(start, end, filterFlag, catId);
            }
        }

        private void InitializeDataGrid()
        {

            List<BudgetItem> budgetItems = budget.GetBudgetItems(null, null, false, 0);
            budgetItemsDataGrid.ItemsSource = budgetItems;


            DataGridTextColumn dateCol = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(dateCol);
            dateCol.Binding = new Binding("Date");
            dateCol.Binding.StringFormat = "dd-MM-yyyy";
            dateCol.Header = "Date";

            DataGridTextColumn catCol = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(catCol);
            catCol.Binding = new Binding("Category");
            catCol.Header = "Category";

            DataGridTextColumn descCol = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(descCol);
            descCol.Binding = new Binding("ShortDescription");
            descCol.Header = "Description";

            DataGridTextColumn amountCol = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(amountCol);
            amountCol.Binding = new Binding("Amount");
            amountCol.Binding.StringFormat = "$00.00";
            amountCol.Header = "Amount";

            DataGridTextColumn balanceCol = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(balanceCol);
            balanceCol.Binding = new Binding("Balance");
            balanceCol.Binding.StringFormat = "$00.00";
            balanceCol.Header = "Balance";

        }

        private void btnByMonth_Click(object sender, RoutedEventArgs e)
        {
            budgetItemsDataGrid.Columns.Clear();

           // DateTime start = (DateTime)dpStart.SelectedDate;
            //DateTime end = (DateTime)dpEnd.SelectedDate;
            bool filterFlag = (bool)filter.IsChecked;
            int catId = cmbCategory.SelectedIndex;

            if (ByMonth.IsChecked == true && ByCategory.IsChecked == false)
            {
                List<BudgetItemsByMonth> budgetItemsByMonth = budget.GetBudgetItemsByMonth(null, null, false, 0);
                budgetItemsDataGrid.ItemsSource = budgetItemsByMonth;

                DataGridTextColumn monthCol = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(monthCol);
                monthCol.Binding = new Binding("Month");
                monthCol.Header = "Month";

                DataGridTextColumn totalCol = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(totalCol);
                totalCol.Binding = new Binding("Total");
                totalCol.Binding.StringFormat = "$00.00";
                totalCol.Header = "Total";
            }
            else if (ByMonth.IsChecked == true && ByCategory.IsChecked == true)
            {

            }

        }
        private void btnByCategory_Click(object sender, RoutedEventArgs e)
        {
            budgetItemsDataGrid.Columns.Clear();

            if (ByCategory.IsChecked == true && ByCategory.IsChecked == false)
            {
                DateTime start = (DateTime)dpStart.SelectedDate;
                DateTime end = (DateTime)dpEnd.SelectedDate;
                bool filterFlag = (bool)filter.IsChecked;
                int catId = cmbCategory.SelectedIndex;

                List<BudgetItemsByCategory> budgetItemsByCategory = budget.GetBudgetItemsByCategory(start, end, filterFlag, catId);
                DataGridTextColumn catCol = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(catCol);
                catCol.Binding = new Binding("Category");

                DataGridTextColumn totalCol = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(totalCol);
                totalCol.Binding = new Binding("Total");
            }
            else if (ByMonth.IsChecked == true && ByCategory.IsChecked == true)
            {

            }
        }

        private void btnAddExp_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow expenseWindow = new AddExpenseWindow(budget);
            expenseWindow.ShowDialog();

            // updating the datagrid
            budgetItemsDataGrid.ItemsSource = budget.GetBudgetItems(null, null, false, 0);
        }

        private void btnAddCat_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow categoryWindow = new AddCategoryWindow(budget);
            categoryWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            budget.CloseDB();
        }

    }
}
