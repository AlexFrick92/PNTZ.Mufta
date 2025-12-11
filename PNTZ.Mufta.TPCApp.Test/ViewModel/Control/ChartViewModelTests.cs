using NUnit.Framework;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using System.Collections.ObjectModel;

namespace PNTZ.Mufta.TPCApp.Test.ViewModel.Control
{
    [TestFixture]
    public class ChartViewModelTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_InitializesDefaultValues()
        {
            var vm = new ChartViewModel();

            Assert.That(vm.ArgumentMember, Is.EqualTo("Аргумент"));
            Assert.That(vm.XMin, Is.EqualTo(0.0));
            Assert.That(vm.XMax, Is.EqualTo(100.0));
            Assert.That(vm.YMin, Is.EqualTo(0.0));
            Assert.That(vm.YMax, Is.EqualTo(100.0));
            Assert.That(vm.XGridSpacing, Is.EqualTo(10.0));
            Assert.That(vm.YGridSpacing, Is.EqualTo(10.0));
        }

        [Test]
        public void Constructor_InitializesCollections()
        {
            var vm = new ChartViewModel();

            Assert.That(vm.XConstantLines, Is.Not.Null);
            Assert.That(vm.YConstantLines, Is.Not.Null);
            Assert.That(vm.XStrips, Is.Not.Null);
            Assert.That(vm.YStrips, Is.Not.Null);
            Assert.That(vm.Series, Is.Not.Null);

            Assert.That(vm.XConstantLines, Is.InstanceOf<ObservableCollection<ConstantLineViewModel>>());
            Assert.That(vm.YConstantLines, Is.InstanceOf<ObservableCollection<ConstantLineViewModel>>());
            Assert.That(vm.XStrips, Is.InstanceOf<ObservableCollection<StripViewModel>>());
            Assert.That(vm.YStrips, Is.InstanceOf<ObservableCollection<StripViewModel>>());
            Assert.That(vm.Series, Is.InstanceOf<ObservableCollection<ChartSeriesViewModel>>());
        }

        #endregion

        #region String Properties

        [Test]
        public void ChartTitle_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = "Test Chart Title";

            vm.ChartTitle = testValue;

            Assert.That(vm.ChartTitle, Is.EqualTo(testValue));
        }

        [Test]
        public void ArgumentMember_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = "TestArgument";

            vm.ArgumentMember = testValue;

            Assert.That(vm.ArgumentMember, Is.EqualTo(testValue));
        }

        [Test]
        public void XAxisTitle_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = "X Axis Title";

            vm.XAxisTitle = testValue;

            Assert.That(vm.XAxisTitle, Is.EqualTo(testValue));
        }

        [Test]
        public void YAxisTitle_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = "Y Axis Title";

            vm.YAxisTitle = testValue;

            Assert.That(vm.YAxisTitle, Is.EqualTo(testValue));
        }

        #endregion

        #region Axis Bounds

        [Test]
        public void XMin_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = -50.0;

            vm.XMin = testValue;

            Assert.That(vm.XMin, Is.EqualTo(testValue));
        }

        [Test]
        public void XMax_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = 200.0;

            vm.XMax = testValue;

            Assert.That(vm.XMax, Is.EqualTo(testValue));
        }

        [Test]
        public void YMin_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = -100.0;

            vm.YMin = testValue;

            Assert.That(vm.YMin, Is.EqualTo(testValue));
        }

        [Test]
        public void YMax_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = 500.0;

            vm.YMax = testValue;

            Assert.That(vm.YMax, Is.EqualTo(testValue));
        }

        #endregion

        #region Grid Spacing

        [Test]
        public void XGridSpacing_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = 5.0;

            vm.XGridSpacing = testValue;

            Assert.That(vm.XGridSpacing, Is.EqualTo(testValue));
        }

        [Test]
        public void YGridSpacing_SetAndGet_ReturnsCorrectValue()
        {
            var vm = new ChartViewModel();
            var testValue = 20.0;

            vm.YGridSpacing = testValue;

            Assert.That(vm.YGridSpacing, Is.EqualTo(testValue));
        }

        #endregion

        #region PropertyChanged Events

        [Test]
        public void ChartTitle_WhenChanged_RaisesPropertyChangedEvent()
        {
            var vm = new ChartViewModel();
            var eventRaised = false;
            string propertyName = null;

            vm.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                propertyName = args.PropertyName;
            };

            vm.ChartTitle = "New Title";

            Assert.That(eventRaised, Is.True);
            Assert.That(propertyName, Is.EqualTo(nameof(ChartViewModel.ChartTitle)));
        }

        [Test]
        public void XMin_WhenChanged_RaisesPropertyChangedEvent()
        {
            var vm = new ChartViewModel();
            var eventRaised = false;
            string propertyName = null;

            vm.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                propertyName = args.PropertyName;
            };

            vm.XMin = 42.0;

            Assert.That(eventRaised, Is.True);
            Assert.That(propertyName, Is.EqualTo(nameof(ChartViewModel.XMin)));
        }

        [Test]
        public void Series_WhenChanged_RaisesPropertyChangedEvent()
        {
            var vm = new ChartViewModel();
            var eventRaised = false;
            string propertyName = null;

            vm.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                propertyName = args.PropertyName;
            };

            vm.Series = new ObservableCollection<ChartSeriesViewModel>();

            Assert.That(eventRaised, Is.True);
            Assert.That(propertyName, Is.EqualTo(nameof(ChartViewModel.Series)));
        }

        #endregion

        #region Collections

        [Test]
        public void Series_SetNewCollection_ReturnsCorrectCollection()
        {
            var vm = new ChartViewModel();
            var newCollection = new ObservableCollection<ChartSeriesViewModel>
            {
                new ChartSeriesViewModel()
            };

            vm.Series = newCollection;

            Assert.That(vm.Series, Is.SameAs(newCollection));
            Assert.That(vm.Series.Count, Is.EqualTo(1));
        }

        [Test]
        public void XConstantLines_SetNewCollection_ReturnsCorrectCollection()
        {
            var vm = new ChartViewModel();
            var newCollection = new ObservableCollection<ConstantLineViewModel>
            {
                new ConstantLineViewModel()
            };

            vm.XConstantLines = newCollection;

            Assert.That(vm.XConstantLines, Is.SameAs(newCollection));
            Assert.That(vm.XConstantLines.Count, Is.EqualTo(1));
        }

        [Test]
        public void YConstantLines_SetNewCollection_ReturnsCorrectCollection()
        {
            var vm = new ChartViewModel();
            var newCollection = new ObservableCollection<ConstantLineViewModel>
            {
                new ConstantLineViewModel()
            };

            vm.YConstantLines = newCollection;

            Assert.That(vm.YConstantLines, Is.SameAs(newCollection));
            Assert.That(vm.YConstantLines.Count, Is.EqualTo(1));
        }

        [Test]
        public void XStrips_SetNewCollection_ReturnsCorrectCollection()
        {
            var vm = new ChartViewModel();
            var newCollection = new ObservableCollection<StripViewModel>
            {
                new StripViewModel()
            };

            vm.XStrips = newCollection;

            Assert.That(vm.XStrips, Is.SameAs(newCollection));
            Assert.That(vm.XStrips.Count, Is.EqualTo(1));
        }

        [Test]
        public void YStrips_SetNewCollection_ReturnsCorrectCollection()
        {
            var vm = new ChartViewModel();
            var newCollection = new ObservableCollection<StripViewModel>
            {
                new StripViewModel()
            };

            vm.YStrips = newCollection;

            Assert.That(vm.YStrips, Is.SameAs(newCollection));
            Assert.That(vm.YStrips.Count, Is.EqualTo(1));
        }

        #endregion

        #region Special Properties

        [Test]
        public void ResetZoomTrigger_SetNewValue_UpdatesProperty()
        {
            var vm = new ChartViewModel();
            var testValue = new object();

            vm.ResetZoomTrigger = testValue;

            Assert.That(vm.ResetZoomTrigger, Is.SameAs(testValue));
        }

        [Test]
        public void ChartData_SetEnumerable_ReturnsCorrectData()
        {
            var vm = new ChartViewModel();
            var testData = new[] { 1, 2, 3, 4, 5 };

            vm.ChartData = testData;

            Assert.That(vm.ChartData, Is.SameAs(testData));
        }

        #endregion
    }
}
