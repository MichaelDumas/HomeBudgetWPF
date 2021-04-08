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
    /// Interaction logic for ModifyExpenseWindow.xaml
    /// </summary>
    
    public partial class ModifyExpenseWindow : Window
    {

        private HomeBudget _homeBudget;
        private List<Category> categories;
        private Category creditCardCategory = null;
        private Expense _expense;

        public ModifyExpenseWindow(HomeBudget homeBudget, Expense expense)
        {
            _homeBudget = homeBudget;
            _expense = expense;
            InitializeComponent();
            dpDate.SelectedDate = _expense.Date;
            txtAmount.Text = _expense.Amount.ToString();
            PopulateComboBox();
        }

        //This method populates combobox from the list of categories gotten from the home budget
        //and sets the selected category to the expense's current category
        private void PopulateComboBox()
        {
            cmbCategory.DisplayMemberPath = "Description";
            categories = _homeBudget.categories.List();
            cmbCategory.ItemsSource = categories;
            cmbCategory.SelectedIndex = 1;
            cmbCategory.SelectedItem = _expense.Category;
        }

        //This method closes the window when the user clicks the cancel button
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //This method validates expense with the ValidExpense method and updates the expense if the input is valid
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if(ValidExpense())
            {
                //gets the values necessary to update the expense
                DateTime date = dpDate.SelectedDate.Value.Date;
                Category cat = (Category) cmbCategory.SelectedItem;
                double.TryParse(txtAmount.Text, out double amount);
                string description = txtDescription.Text;
                String categoryType = Convert.ToString(cat.Type).ToLower();

                //Checks if the category the user selected is expense or savings to turn it into a negative number if it isn't already one
                if(amount > 0 && (categoryType == "expense" || categoryType == "savings"))
                {
                    amount *= -1;
                }

                //in a try catch to catch any throws thrown by the homebudget or the database without breaking the application
                try
                {
                    //modifies the expense shows a message and closes the window
                    _homeBudget.expenses.UpdateProperties(_expense.Id, date, cat.Id, amount, description);
                    MessageBox.Show("Expense was updated successfully", "EXPENSE UPDATED", MessageBoxButton.OK);
                    ClearForm();
                }
                catch(Exception exception)
                {
                    MessageBox.Show(exception.Message, "EXPENSE COULD NOT BE UPDATED", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        //This method is used to clear the form after adding a valid expense to the database while keeping the Category and the Date the same
        //in case the user wants to keep adding expense of the same category or on the same date
        private void ClearForm()
        {
            txtAmount.Text = null;
            txtDescription.Text = null;
        }

        //This method validates the fields of the form and outputs a message box if anything is wrong with the user input
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

            //if the builder is emtpy meaning that the expense is valid returns true
            if (String.IsNullOrEmpty(builder.ToString()))
                return true;

            //output the builder on a message box to show the user what is wrong with the expense and return false;
            MessageBox.Show(builder.ToString(), "INVALID EXPENSE", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            _homeBudget.expenses.Delete(_expense.Id);
            Close();
        }
    }
}
