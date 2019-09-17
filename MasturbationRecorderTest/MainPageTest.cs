using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MasturbationRecorder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorderTest {
    [TestClass]
    public class MainPageTest {
        [TestMethod]
        public void EarlierThanEarliestRectangleTest() {
            StatistTotalByDateTimeModel statistTotalByDateTimeModel = new StatistTotalByDateTimeModel(CommonTestResource.EXCEPTED_LINE, DateMode.DateWithWhiteSpace);
            DateTime minDateTime = statistTotalByDateTimeModel.Entries.Min<StatistTotalByDateTime, DateTime>(s => s.DateTime);
            System.Diagnostics.Debug.WriteLine(minDateTime);
            Assert.AreEqual<DateTime>(minDateTime, new DateTime(2018, 5, 27));
        }
    }
}
