using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

namespace WPF
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class DataChartControl : UserControl, IDataView
    {
        public DataChartControl()
        {
            InitializeComponent();
        }
        private enum ChartType
        {
            Standard,
            ByCategory,
            ByMonth,
            ByMonthAndCategory
        }
        private ChartType chartType = ChartType.Standard;
        private List<string> usedCategories;

        private void ShowChart(ChartType type)
        {
            switch (type)
            {
                case ChartType.Standard:
                    chPie.Visibility = Visibility.Hidden;
                    chBar.Visibility = Visibility.Hidden;
                    txtInvalid.Visibility = Visibility.Visible;
                    break;
                case ChartType.ByCategory:
                    chPie.Visibility = Visibility.Hidden;
                    chBar.Visibility = Visibility.Hidden;
                    txtInvalid.Visibility = Visibility.Visible;
                    break;
                default:
                    chPie.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public DataPresenter presenter { get; set; }
        public List<object> DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value;
                if (chartType == ChartType.ByMonthAndCategory) drawByMonthPieChart();
                if (chartType == ChartType.ByMonth) drawByMonthLineChart();
            }
        }

        public object DataSelectedItem { get { return null; } set { _ = value; } }

        public int DataSelectedIndex { get { return -1; } set { _ = value; } }

        public List<object> _dataSource;

        private void drawByMonthLineChart()
        {
            /*
            List<String> months = new List<String>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                if (item != null)
                {
                    months.Add(item["Month"].ToString());
                }
            }
            cbMonths.ItemsSource = months;
            int oldIndex = cbMonths.SelectedIndex;
            cbMonths.SelectedIndex = months.Count - 1;
            if (oldIndex == cbMonths.SelectedIndex)
                set_MonthCategory_Data();
            */
        }



        #region byMonthAndCategory

        public void InitializeByCategoryAndMonthDisplay(List<string> usedCategoryList)
        {
            txtTitle.Text = "By Month";
            chartType = ChartType.ByMonthAndCategory;
            chPie.Visibility = Visibility.Visible;
            txtInvalid.Visibility = Visibility.Hidden;
            this.usedCategories = usedCategoryList;
        }

        private void drawByMonthPieChart()
        {
            List<String> months = new List<String>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                if (item != null)
                {
                    months.Add(item["Month"].ToString());
                }
            }
            cbMonths.ItemsSource = months;
            int oldIndex = cbMonths.SelectedIndex;
            cbMonths.SelectedIndex = months.Count - 1;
           if (oldIndex == cbMonths.SelectedIndex)
            set_MonthCategory_Data();
        }

        private void set_MonthCategory_Data()
        {
            // DataClear();
            if (cbMonths.Items.Count == 0) return;
            if (cbMonths.SelectedIndex < 0) cbMonths.SelectedIndex = 0;
            var DisplayData = new List<KeyValuePair<String, double>>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                if (item != null && (string)item["Month"] == cbMonths.SelectedItem.ToString())
                {
                    foreach (var pair in item)
                    {
                        if (!usedCategories.Contains(pair.Key)) continue;
                        var amount = 0.0;
                        double.TryParse(pair.Value.ToString(), out amount);
                        if (amount < 0)
                        {
                            DisplayData.Add(new KeyValuePair<String, double>(pair.Key, -amount));
                        }
                    }
                    break;
                }
            }
            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }

        #endregion

        public void DataClear()
        {
              ((PieSeries)chPie.Series[0]).ItemsSource = null;
        }

        public int GetIndexOfItem(DateTime dateOfExpense)
        {
            return 0;
        }

        public void InitializeByCategoryDisplay()
        {
            chPie.Visibility = Visibility.Hidden;
            txtInvalid.Visibility = Visibility.Visible;
        }

        public void InitializeByMonthDisplay()
        {
            chPie.Visibility = Visibility.Hidden;
            txtInvalid.Visibility = Visibility.Visible;
        }

        public void InitializeStandardDisplay()
        {
            chPie.Visibility = Visibility.Hidden;
            txtInvalid.Visibility = Visibility.Visible;
        }

        public void ResetFocusAfterUpdate(int itemIndex)
        {
            return;
        }

        public void ResetSearchString(string text)
        {
            return;
        }

        private void cbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataClear();
            if (cbMonths.SelectedIndex < 0) return;
            var DisplayData = new List<KeyValuePair<String, int>>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<String, object>;
                if (item != null && (string)item["Month"] == cbMonths.SelectedItem.ToString())
                {
                    foreach (var pair in item)
                    {
                        if (pair.Key == "Month") continue;
                        if (pair.Key.Contains("details:")) continue;
                        if (pair.Key == "Total") continue;
                        var amount = 0.0;
                        double.TryParse(pair.Value.ToString(), out amount);
                        DisplayData.Add(new KeyValuePair<String, int>( pair.Key, Convert.ToInt32(amount)));
                    }
                    break;
                }
            }

            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }
    }
}
