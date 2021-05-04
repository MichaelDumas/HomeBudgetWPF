using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Budget;
using System.IO;
using System.Linq;
namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IDataView
    {
        private List<object> dataSource;
        private List<BudgetItem> items;
        private DataPresenter presenter;
        public HomeBudget budget { get; set; }


        public DataPresenter Presenter 
        {
            get { return presenter; }
            set { presenter = value; }
        }
        public List<object> DataSource 
        {
            get { return dataSource; }
            set { dataSource = value; }

        }

        DataPresenter IDataView.presenter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

            presenter = new DataPresenter(this, budget);
            InitializeComponent();
            InitializeStandardDisplay();
            PopulateComboBox();

        }

        #region Initialization

        private void PopulateComboBox()
        {
            cmbCategory.DisplayMemberPath = "Description";
            cmbCategory.ItemsSource = budget.categories.List();
            cmbCategory.SelectedIndex = 1;
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
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(!presenter.Search(budgetItemsDataGrid.SelectedIndex, txtSearch.Text))
            {
                //no items found so pop up message box
                MessageBox.Show("No items found", "NO RESULTS", MessageBoxButton.OK);
            }

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
                txtSearch.IsReadOnly = true;
                //UpdateToBoth(start, end, isFilterChecked, catIdForFilter);
                InitializeByCategoryAndMonthDisplay(presenter.GetCategoryDescriptions());
                return;
            }

            // if by month checkbox is checked
            if (ByMonth.IsChecked.Value)
            {
                txtSearch.IsReadOnly = true;
                InitializeByMonthDisplay();
                return;
            }

            // if by category checkbox is checked
            if (ByCategory.IsChecked.Value)
            {
                txtSearch.IsReadOnly = true;
                InitializeByCategoryDisplay();
                return;
            }

            InitializeStandardDisplay();
            txtSearch.IsReadOnly = false;

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

        private void SearchBox_Click(object sender, RoutedEventArgs e)
        {
            // Only apply function if user has entered text in the search box
            if(budgetItemsDataGrid.ItemsSource == null)
            {
                return;
            }

            List<BudgetItem> filtered = new List<BudgetItem>();// = items.Where(item => item.ShortDescription.ToLower().Contains(txtSearchBox.Text.ToLower())); // to lowers ensure that the search is not case-sensitive

            // Filtering a new list of budget items to match search input
            foreach(BudgetItem item in items)
            {
                if (item.ShortDescription.ToLower().Contains(txtSearch.Text.ToLower()) || item.Amount.ToString().Equals(txtSearch.Text.ToLower()))
                {
                    filtered.Add(item);
                }
            }

            if(filtered.Count <= 0)
            {
                MessageBox.Show("No Results Found.");
                return;
            }

            budgetItemsDataGrid.ItemsSource = filtered;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            budget.CloseDB();
        }

        #endregion

        #region Interface
        public void ResetFocusAfterUpdate(int itemIndex)
        {
            List<object> list = (List<object>)budgetItemsDataGrid.ItemsSource;

            budgetItemsDataGrid.SelectedIndex = itemIndex;
            budgetItemsDataGrid.Focus();
            budgetItemsDataGrid.ScrollIntoView(list[itemIndex]);
        }

        public void DataClear()
        {
            budgetItemsDataGrid.Columns.Clear();
        }

        public void InitializeStandardDisplay()
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

            btnSearch.IsEnabled = true;
            txtSearch.IsEnabled = true;

            budgetItemsDataGrid.Columns.Clear();

            presenter.GetStandardDisplayValues(start, end, isFilterChecked, catIdForFilter);

            budgetItemsDataGrid.ItemsSource = DataSource;
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

        public void InitializeByMonthDisplay()
        {
            DateTime? start = dpStart.SelectedDate;
            DateTime? end = dpEnd.SelectedDate;
            bool isFilterChecked = filter.IsChecked.Value;
            int catIdForFilter = -1;
            btnSearch.IsEnabled = false;
            txtSearch.IsEnabled = false;

            if (isFilterChecked)
            {
                Category category = (Category)cmbCategory.SelectedItem;
                catIdForFilter = category.Id;
            }
            presenter.GetByMonthDisplayValues(start, end, isFilterChecked, catIdForFilter);

            // Right align text
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));


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
            budgetItemsDataGrid.ItemsSource = DataSource;

        }

        public void InitializeByCategoryDisplay()
        {
            DateTime? start = dpStart.SelectedDate;
            DateTime? end = dpEnd.SelectedDate;
            bool isFilterChecked = filter.IsChecked.Value;
            int catIdForFilter = -1;
            btnSearch.IsEnabled = false;
            txtSearch.IsEnabled = false;

            if (isFilterChecked)
            {
                Category category = (Category)cmbCategory.SelectedItem;
                catIdForFilter = category.Id;
            }
            presenter.GetByCategoryDisplayValues(start, end, isFilterChecked, catIdForFilter);

            // Right align text
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));


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
            budgetItemsDataGrid.ItemsSource = DataSource;
        }

        public void InitializeByCategoryAndMonthDisplay(List<string> usedCategoryList)
        {
            DateTime? start = dpStart.SelectedDate;
            DateTime? end = dpEnd.SelectedDate;
            bool isFilterChecked = filter.IsChecked.Value;
            int catIdForFilter = -1;
            btnSearch.IsEnabled = false;
            txtSearch.IsEnabled = false;

            if (isFilterChecked)
            {
                Category category = (Category)cmbCategory.SelectedItem;
                catIdForFilter = category.Id;
            }
            presenter.GetByMonthAndCategoryDisplayValues(start, end, isFilterChecked, catIdForFilter);

            // Right align text
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));


            DataGridTextColumn monthColumn = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(monthColumn);
            monthColumn.Binding = new Binding("[Month]");
            monthColumn.Header = "Month";

            // freeze the first column
            budgetItemsDataGrid.FrozenColumnCount = 1;

            // loop through the categories and add a column for each one
            foreach (String desc in presenter.GetCategoryDescriptions())
            {
                DataGridTextColumn newColumn = new DataGridTextColumn();
                budgetItemsDataGrid.Columns.Add(newColumn);
                newColumn.Binding = new Binding("[" + desc + "]");
                newColumn.Binding.StringFormat = "$00.00";
                newColumn.Header = desc;
            }

            DataGridTextColumn totalColumn = new DataGridTextColumn();
            budgetItemsDataGrid.Columns.Add(totalColumn);
            totalColumn.Binding = new Binding("[Total]");
            totalColumn.Binding.StringFormat = "$00.00";
            totalColumn.Header = "Total";
            totalColumn.CellStyle = s;
            budgetItemsDataGrid.ItemsSource = DataSource;
        }
        #endregion
    }
}
