﻿using System;
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
            PopulateComboBox();

        }
        private void PopulateComboBox()
        {
            cmbCategory.DisplayMemberPath = "Description";
            cmbCategory.ItemsSource = budget.categories.List();
            cmbCategory.SelectedIndex = 1;
        }

        private void InitializeDataGrid(DateTime? start = null, DateTime? end = null)
        {
            budgetItemsDataGrid.Columns.Clear();

            List<BudgetItem> budgetItems = budget.GetBudgetItems(start, end, false, 0);
            budgetItemsDataGrid.ItemsSource = budgetItems;
            budgetItemsDataGrid.Items.Refresh();


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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            budget.CloseDB();
        }

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

                // if only filter is checked
                if (!ByMonth.IsChecked.Value && !ByCategory.IsChecked.Value)
                {
                    budgetItemsDataGrid.ItemsSource = budget.GetBudgetItems(null, null, isFilterChecked, catIdForFilter);
                    return;
                }
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

            InitializeDataGrid(start, end);
        }

        private void UpdateToBoth(DateTime? start, DateTime? end, bool isFilterChecked, int catIdForFilter)
        {
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

            budgetItemsDataGrid.ItemsSource = budget.GetBudgetDictionaryByCategoryAndMonth(start, end, isFilterChecked, catIdForFilter);
        }

        private void UpdateToByMonth(DateTime? start, DateTime? end, bool filter, int catId)
        {
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

            budgetItemsDataGrid.ItemsSource = budget.GetBudgetItemsByMonth(start, end, filter, catId);

        }

        private void UpdateToByCategory(DateTime? start, DateTime? end, bool filter, int catId)
        {
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
    }
}
