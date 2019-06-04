using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasturbationRecorder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasturbationRecorderTest {
    class MainPageViewModelTest {
        [TestMethod]
        public void GroupDateTimesByDiffTest() {
            LinkedList<StatistTotalByDateTime> dateTimes = MainPageViewModel.LinesConvertToStatistTotalByDateTimes(CommonTestResource.TestTextLine);
            var res = MainPageViewModel.GroupDateTimesByTotal(dateTimes);
        }
    }
}
