using Budget;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using WPF;
using System.IO;

namespace HomeBudgetUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        public class TestDataView : IDataView
        {
            public bool isCleared = false;
            public bool resetFocus = false;
            public bool initStandard = false;
            public bool initMonth = false;
            public bool initCat = false;
            public bool initCatMonth = false;

            private DataPresenter _presenter;
            private List<object> dataSource;

            DataPresenter IDataView.presenter
            {
                get { return _presenter; }
                set { _presenter = value; }
            }
            List<object> IDataView.DataSource
            {
                get { return dataSource; }
                set { dataSource = value; }
            }

            public TestDataView()
            {
            }

            public List<object> getDataSource()
            {
                return dataSource;
            }

            public void DataClear()
            {
                isCleared = true;
            }

            public void InitializeByCategoryAndMonthDisplay(List<string> usedCategoryList)
            {
                initCatMonth = true;
            }

            public void InitializeByCategoryDisplay()
            {
                initCat = true;
            }

            public void InitializeByMonthDisplay()
            {
                initMonth = true;
            }

            public void InitializeStandardDisplay()
            {
                initStandard = true;
            }

            public void ResetFocusAfterUpdate(int itemIndex)
            {
                resetFocus = true;
            }
        }

        [TestMethod]
        public void Test_GetStandardDisplayValues()
        {
            //Arrange
            HomeBudget model = new HomeBudget("./testDBInput.db", false);
            TestDataView view = new TestDataView();
            DataPresenter presenter = new DataPresenter(view, model);

            //Act
            presenter.GetStandardDisplayValues(null, null, false, -1);

            //Assert
            Assert.IsTrue(view.isCleared);
            Assert.IsNotNull(view.getDataSource());
        }

        [TestMethod]
        public void Test_GetByMonthDisplayValues()
        {
            //Arrange
            HomeBudget model = new HomeBudget("./testDBInput.db", false);
            TestDataView view = new TestDataView();
            DataPresenter presenter = new DataPresenter(view, model);

            //Act
            presenter.GetByMonthDisplayValues(null, null, false, -1);

            //Assert
            Assert.IsTrue(view.isCleared);
            Assert.IsNotNull(view.getDataSource());
        }

        [TestMethod]
        public void Test_GetByCategoryDisplayValues()
        {
            //Arrange
            HomeBudget model = new HomeBudget("./testDBInput.db", false);
            TestDataView view = new TestDataView();
            DataPresenter presenter = new DataPresenter(view, model);

            //Act
            presenter.GetByCategoryDisplayValues(null, null, false, -1);

            //Assert
            Assert.IsTrue(view.isCleared);
            Assert.IsNotNull(view.getDataSource());
        }

        [TestMethod]
        public void Test_GetByMonthAndCategoryDisplayValues()
        {
            //Arrange
            HomeBudget model = new HomeBudget("./testDBInput.db", false);
            TestDataView view = new TestDataView();
            DataPresenter presenter = new DataPresenter(view, model);

            //Act
            presenter.GetByMonthAndCategoryDisplayValues(null, null, false, -1);

            //Assert
            Assert.IsTrue(view.isCleared);
            Assert.IsNotNull(view.getDataSource());
        }

        [TestMethod]
        public void Test_GetCategoryDescriptions()
        {
            //Arrange
            HomeBudget model = new HomeBudget("./testDBInput.db", false);
            TestDataView view = new TestDataView();
            DataPresenter presenter = new DataPresenter(view, model);

            //Act
            List<string> list = presenter.GetCategoryDescriptions();

            //Assert
            Assert.IsNotNull(list);
            Assert.IsInstanceOfType(list, typeof(List<string>));
        }

        [TestMethod]
        public void Test_Search()
        {
            //Arrange
            HomeBudget model = new HomeBudget("./testDBInput.db", false);
            TestDataView view = new TestDataView();
            DataPresenter presenter = new DataPresenter(view, model);
            presenter.GetStandardDisplayValues(null, null, false, -1);


            //Act
            bool search1 = presenter.Search(0, "Wendys");
            bool search2 = presenter.Search(0, "xrdwg");

            //Assert
            Assert.IsTrue(search1);
            Assert.IsFalse(search2);
            Assert.IsTrue(view.resetFocus);
        }

    }
}
