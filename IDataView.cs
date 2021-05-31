using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{
    public interface IDataView
    {
        DataPresenter presenter { get; set; }
        List<object> DataSource { get; set; }
        void ResetFocusAfterUpdate(int itemIndex);
        void DataClear();
        void InitializeStandardDisplay();
        void InitializeByMonthDisplay();
        void InitializeByCategoryDisplay();
        void InitializeByCategoryAndMonthDisplay(
        List<String> usedCategoryList);
        object DataSelectedItem { get; set; }
        int DataSelectedIndex { get; set; }
    }
}
