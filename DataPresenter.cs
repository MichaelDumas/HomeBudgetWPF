
using Budget;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using System.Linq;

namespace WPF
{
    public class DataPresenter
    {
        IDataView dataView;
        HomeBudget budget;


        public DataPresenter(IDataView view, HomeBudget bud)
        {
            dataView = view;
            budget = bud;
        }

        public void GetStandardDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            dataView.DataClear();
            List<BudgetItem> bi = budget.GetBudgetItems(start, end, filter, catId);
            dataView.DataSource = bi.Cast<object>().ToList();
        }

        public void GetByMonthDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            dataView.DataClear();
            List<BudgetItemsByMonth> bi = budget.GetBudgetItemsByMonth(start, end, filter, catId);
            dataView.DataSource = bi.Cast<object>().ToList();
        }

        public void GetByCategoryDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            dataView.DataClear();
            List<BudgetItemsByCategory> bi = budget.GetBudgetItemsByCategory(start, end, filter, catId);
            dataView.DataSource = bi.Cast<object>().ToList();
        }

        public void GetByMonthAndCategoryDisplayValues(DateTime? start, DateTime? end, bool filter, int catId)
        {
            dataView.DataClear();
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

        public bool Search(int index, string txtSearch)
        {
            if (dataView.DataSource == null)
            {
                return false;
            }

            int idx = index;
            List<BudgetItem> list = dataView.DataSource.Cast<BudgetItem>().ToList();

            //start for loop at to 0 if no index selected (idx is negative) else start at idx + 1
            int c = idx + 1 > 0 ? idx + 1 : 0;

            for (int i = c; i < list.Count(); i++)
            {
                if (list[i].ShortDescription.ToLower().Contains(txtSearch.ToLower()) || list[i].Amount.ToString().Contains((txtSearch)))
                {
                    dataView.ResetFocusAfterUpdate(i);
                    return true;
                }
            }

            //second for loop to wrap
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].ShortDescription.ToLower().Contains(txtSearch.ToLower()) || list[i].Amount.ToString().Contains((txtSearch)))
                {
                    dataView.ResetFocusAfterUpdate(i);
                    return true;
                }
            }

            return false;

        }

    }
}