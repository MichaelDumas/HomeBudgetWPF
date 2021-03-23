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
using System.Data.SQLite;


namespace WPF
{
    /// <summary>
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        private HomeBudget budget;
        private string fileName = "C:\\sqlite\\testDBInput.db";

        public AddCategoryWindow()
        {
            InitializeComponent();
            budget = new HomeBudget(fileName);
            initComboBox();
        }

        public AddCategoryWindow(HomeBudget homeBudget)
        {
            budget = homeBudget;
            initComboBox();
        }

        private void initComboBox()
        {
            TypeId.Items.Add("Income");
            TypeId.Items.Add("Expenses");
            TypeId.Items.Add("Credit");
            TypeId.Items.Add("Savings");
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            string description = Description.Text;
            int typeId = TypeId.SelectedIndex;

            // validate description and category
            if (description == "" || typeId == -1)
            {
                MessageBox.Show("Description and TypeId cannot be blank", "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                // add it to the database
                budget.categories.Add(description, (Category.CategoryType)typeId);

                MessageBox.Show("Category was added successfully to the database", "CATEGORY ADDED", MessageBoxButton.OK);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Category could not be added", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
