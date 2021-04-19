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
                budget = setup.budget;
                if (!File.Exists("./config.txt") || budget is null)
                {
                    this.Close();
                    return;
                }
            }
            else
            {
                OpenFile open = new OpenFile();
                open.ShowDialog();
                if (open.budget is null)
                {
                    this.Close();
                    return;
                }
                budget = open.budget;            
            }

            InitializeComponent();
            InitializeDataGrid();
            PopulateComboBox();

        }

        #region Initialization

        private void PopulateComboBox()
        {
            cmbCategory.DisplayMemberPath = "Description";
            cmbCategory.ItemsSource = budget.categories.List();
            cmbCategory.SelectedIndex = 1;
        }

        private void InitializeDataGrid(DateTime? start = null, DateTime? end = null, bool isFiltered = false, int catId = -1)
        {
            budgetItemsDataGrid.Columns.Clear();

            List<BudgetItem> budgetItems = budget.GetBudgetItems(start, end, isFiltered, catId);
            budgetItemsDataGrid.ItemsSource = budgetItems;
            budgetItemsDataGrid.Items.Refresh();

            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

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
            amountCol.CellStyle = s;

            DataGridTextColumn balanceCol = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(balanceCol);
            balanceCol.Binding = new Binding("Balance");
            balanceCol.Binding.StringFormat = "$00.00";
            balanceCol.Header = "Balance";
            balanceCol.CellStyle = s;
        }
        #endregion

        #region Clicks

        private void btnAddExp_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow expenseWindow = new AddExpenseWindow(budget);
            expenseWindow.ShowDialog();

            // updating the data in datagrid after adding an expense
            UpdateDataGrid(sender, e);
        }

        private void btnAddCat_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow categoryWindow = new AddCategoryWindow(budget);
            categoryWindow.ShowDialog();

            // update the combo box after adding a category
            cmbCategory.ItemsSource = budget.categories.List();
        }

        private void DeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            // Only allow user to delete if the datagrid is showing all unfiltered budget items
            if(ByMonth.IsChecked == true || ByCategory.IsChecked == true)
            {
                MessageBox.Show("Please uncheck all summaries in order to modify or delete a budget item. ");
                return;
            }

            int expenseId = ((BudgetItem)budgetItemsDataGrid.SelectedItem).ExpenseID;
            budget.expenses.Delete(expenseId);
            List<BudgetItem> budgetItems = budget.GetBudgetItems(null, null, false, 0);
            budgetItemsDataGrid.ItemsSource = budgetItems;
        }

        private void ModifyExpense_Click(object sender, RoutedEventArgs e)
        {
            // Only allow user to modify if the datagrid is showing all unfiltered budget items
            if (ByMonth.IsChecked == true || ByCategory.IsChecked == true)
            {
                MessageBox.Show("Please uncheck all summaries in order to modify or delete a budget item. ");
                return;
            }

            int expenseId = ((BudgetItem)budgetItemsDataGrid.SelectedItem).ExpenseID;
            Expense expense = null;
            List<Expense> expenses = budget.expenses.List();
            foreach(Expense exp in expenses)
            {
                if (exp.Id == expenseId)
                {
                    expense = exp;
                }
            }


            ModifyExpenseWindow modifyWindow = new ModifyExpenseWindow(budget, expense);
            modifyWindow.ShowDialog();
            List<BudgetItem> budgetItems = budget.GetBudgetItems(null, null, false, 0);
            budgetItemsDataGrid.ItemsSource = budgetItems;

        }

        private void Modify_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Only allow user to modify if the datagrid is showing all unfiltered budget items
            if (ByMonth.IsChecked == true || ByCategory.IsChecked == true)
            {
                return;
            }

            int expenseId = ((BudgetItem)budgetItemsDataGrid.SelectedItem).ExpenseID;
            Expense expense = null;
            List<Expense> expenses = budget.expenses.List();
            foreach (Expense exp in expenses)
            {
                if (exp.Id == expenseId)
                {
                    expense = exp;
                }
            }


            ModifyExpenseWindow modifyWindow = new ModifyExpenseWindow(budget, expense);
            modifyWindow.ShowDialog();
            List<BudgetItem> budgetItems = budget.GetBudgetItems(null, null, false, 0);
            budgetItemsDataGrid.ItemsSource = budgetItems;

        }
        #endregion

        #region Updating

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // updates the data after combobox selection changed only if the filter is checked
            if(filter.IsChecked.Value)
            {
                UpdateDataGrid(sender, e);
            }
        }

        private void UpdateDataGrid(object sender, RoutedEventArgs e)
        {
            DateTime? start = dpStart.SelectedDate;
            DateTime? end = dpEnd.SelectedDate;
            bool isFilterChecked = filter.IsChecked.Value;
            int catIdForFilter = -1;

            if (isFilterChecked)
            {
                Category category = (Category)cmbCategory.SelectedItem;
                catIdForFilter = category.Id;
            }

            // if by month and by category checkboxes are checked
            if (ByMonth.IsChecked.Value && ByCategory.IsChecked.Value)
            {
                UpdateToBoth(start, end, isFilterChecked, catIdForFilter);
                return;
            }

            // if by month checkbox is checked
            if (ByMonth.IsChecked.Value)
            {
                UpdateToByMonth(start, end, isFilterChecked, catIdForFilter);
                return;
            }

            // if by category checkbox is checked
            if (ByCategory.IsChecked.Value)
            {
                UpdateToByCategory(start, end, isFilterChecked, catIdForFilter);
                return;
            }

            InitializeDataGrid(start, end, isFilterChecked, catIdForFilter);
        }

        private void UpdateToBoth(DateTime? start, DateTime? end, bool isFilterChecked, int catIdForFilter)
        {
            // Right align text
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            // reset columns, adds necessary columns and set the item source
            budgetItemsDataGrid.Columns.Clear();

            DataGridTextColumn monthColumn = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(monthColumn);
            monthColumn.Binding = new Binding("[Month]");
            monthColumn.Header = "Month";

            // freeze the first column
            budgetItemsDataGrid.FrozenColumnCount = 1;

            // loop through the categories and add a column for each one
            foreach (Category category in budget.categories.List())
            {
                DataGridTextColumn newColumn = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(newColumn);
                newColumn.Binding = new Binding("[" + category.Description + "]");
                newColumn.Binding.StringFormat = "$00.00";
                newColumn.Header = category.Description;
            }
             
            DataGridTextColumn totalColumn = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(totalColumn);
            totalColumn.Binding = new Binding("[Total]");
            totalColumn.Binding.StringFormat = "$00.00";
            totalColumn.Header = "Total";
            totalColumn.CellStyle = s;

            budgetItemsDataGrid.ItemsSource = budget.GetBudgetDictionaryByCategoryAndMonth(start, end, isFilterChecked, catIdForFilter);
        }

        private void UpdateToByMonth(DateTime? start, DateTime? end, bool filter, int catId)
        {
            // Right align text
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            // reset columns, adds necessary columns and set the item source
            budgetItemsDataGrid.Columns.Clear();

            DataGridTextColumn monthColumn = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(monthColumn);
            monthColumn.Binding = new Binding("Month");
            monthColumn.Header = "Month";

            DataGridTextColumn totalColumn = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(totalColumn);
            totalColumn.Binding = new Binding("Total");
            totalColumn.Binding.StringFormat = "$00.00";
            totalColumn.Header = "Total";
            totalColumn.CellStyle = s;
            budgetItemsDataGrid.ItemsSource = budget.GetBudgetItemsByMonth(start, end, filter, catId);

        }

        private void UpdateToByCategory(DateTime? start, DateTime? end, bool filter, int catId)
        {
            // Right align text
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            // reset columns, adds necessary columns and set the item source
            budgetItemsDataGrid.Columns.Clear();

            DataGridTextColumn categoryTotal = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(categoryTotal);
            categoryTotal.Binding = new Binding("Category");
            categoryTotal.Header = "Category";

            DataGridTextColumn totalColumn = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(totalColumn);
            totalColumn.Binding = new Binding("Total");
            totalColumn.Binding.StringFormat = "$00.00";
            totalColumn.Header = "Total";
            totalColumn.CellStyle = s;

            budgetItemsDataGrid.ItemsSource = budget.GetBudgetItemsByCategory(start, end, filter, catId);
        }

        private void dpSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // message pops up if start date is after end date and datagrid doesn't update
            if(dpStart.SelectedDate.HasValue && dpEnd.SelectedDate.HasValue && (dpStart.SelectedDate.Value > dpEnd.SelectedDate.Value))
            {
                MessageBox.Show("End date cannot be after start date \nTable not updated \nEnter a valid start and end date to update the table", "INVALID DATES", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                UpdateDataGrid(sender, e);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            budget.CloseDB();
        }

        #endregion

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int idx = budgetItemsDataGrid.SelectedIndex;
            List<BudgetItem> list = (List<BudgetItem>) budgetItemsDataGrid.ItemsSource;

            //set it to 0 if negative else set it to i + 1
            int c = idx + 1 > 0 ? idx + 1 : 0;

            for (int i = c; i < list.Count(); i++)
            {
                if(list[i].ShortDescription.Contains(txtSearch.Text))
                {
                    budgetItemsDataGrid.SelectedIndex = i;
                    budgetItemsDataGrid.Focus();
                    return;
                }
            }

            //second for loop to wrap
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].ShortDescription.Contains(txtSearch.Text))
                {
                    budgetItemsDataGrid.SelectedIndex = i;
                    budgetItemsDataGrid.Focus();
                    return;
                }
            }

            MessageBox.Show("No items found", "NO RESULTS", MessageBoxButton.OK);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            dpStart.SelectedDate = null;
            dpEnd.SelectedDate = null;
            filter.IsChecked = false;
            cmbCategory.SelectedIndex = 1;
            ByMonth.IsChecked = false;
            ByCategory.IsChecked = false;
        }
    }
}
