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

namespace WPF
{
    /// <summary>
    /// Interaction logic for AddExpenseWindow.xaml
    /// </summary>
    
    public partial class AddExpenseWindow : Window
    {

        private HomeBudget _homeBudget;
        private DateTime defaultDateTime = DateTime.Now;

        public AddExpenseWindow()
        {
            InitializeComponent();

            dpDate.SelectedDate = defaultDateTime;

            PopulateComboBox();

        }

        public AddExpenseWindow(HomeBudget homeBudget)
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Now;
            _homeBudget = homeBudget;
            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            //populates combobox from the list of categories gotten from the home budget
            cmbCategory.DisplayMemberPath = "Description";
            cmbCategory.ItemsSource = _homeBudget.categories.List();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //on click validates expense with the method and adds it to the databse if it has a valid expense
            if(ValidExpense())
            {
                //gets the values necessary to create the new expense
                DateTime date = dpDate.SelectedDate.Value.Date;
                Category cat = (Category) cmbCategory.SelectedItem;
                double.TryParse(txtAmount.Text, out double amount);
                string description = txtDescription.Text;

                //in a try catch to catch any throws thrown by the homebudget or the database
                try
                {
                    //if paid with credit card adds the expense twice
                    if (rdbCredit.IsChecked.Value)
                        _homeBudget.expenses.Add(date, cat.Id, -amount, description);

                    //adds the expense shows a message and closes the window
                    _homeBudget.expenses.Add(date, cat.Id, amount, description);
                    MessageBox.Show("Expense was added successfully to the database", "EXPENSE ADDED", MessageBoxButton.OK);
                    Close();
                }
                catch(Exception exception)
                {
                    MessageBox.Show(exception.Message, "EXPENSE COULD NOT BE ADDED", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Close();
            }
        }

        private bool ValidExpense()
        {
            //builder to accumulate the things to add if it is an invalid expense
            StringBuilder builder = new StringBuilder();

            //validates the amount and adds appropriate message to the builder if not valid
            if (!double.TryParse(txtAmount.Text, out double amount))
                builder.AppendLine("Amount needs to be a number");

            //validates the description and adds appropriate message to the builder if not valid
            string description = txtDescription.Text;
            if (String.IsNullOrEmpty(description))
                builder.AppendLine("Description cannot be emtpy");

            //checks if category was selected and adds appropriate message to the builder if not valid
            if (cmbCategory.SelectedIndex == -1)
                builder.AppendLine("Please select a category");

            //if the builder is emtpy meaning that the expense if valid returns true
            if (String.IsNullOrEmpty(builder.ToString()))
                return true;

            //output the builder on a message box to show the user what is wrong with the expense and return false;
            MessageBox.Show(builder.ToString(), "INVALID EXPENSE", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}
