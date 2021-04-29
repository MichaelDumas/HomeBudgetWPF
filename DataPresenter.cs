
using Budget;
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
    public class DataPresenter
    {
        IDataView dataView;
        HomeBudget budget;


        public DataPresenter(IDataView view, HomeBudget bud )
        {
            dataView = view;
            budget = bud;
        }

        public void GetStandardDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            List<BudgetItem> bi = budget.GetBudgetItems(start, end, filter, catId);
            dataView.DataSource = bi.Cast<object>().ToList();
        }

        public void GetByMonthDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            List<BudgetItemsByMonth> bi = budget.GetBudgetItemsByMonth(start, end, filter, catId);
            dataView.DataSource = bi.Cast<object>().ToList();
        }

        public void GetByCategoryDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            List<BudgetItemsByCategory> bi = budget.GetBudgetItemsByCategory(start, end, filter, catId);
            dataView.DataSource = bi.Cast<object>().ToList();
        }

        public void GetByMonthAndCategoryDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            List<Dictionary<string, object>> bi = budget.GetBudgetDictionaryByCategoryAndMonth(start, end, filter, catId);
            dataView.DataSource = bi.Cast<object>().ToList();
        }

        public List<String> GetCategoryDescriptions()
        {
            List<String> names = new List<string>();

            foreach(Category cat in budget.categories.List())
            {
                names.Add(cat.Description);
            }

            return names;

        }
    }
}