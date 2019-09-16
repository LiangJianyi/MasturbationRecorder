using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Linq;
using MasturbationRecorder;

namespace MasturbationRecorderTest {
    [TestClass]
    public class DatetimeParserTest {
        private Type _datetimeParser = typeof(DatetimeParser);

        [TestMethod]
        public void StringToUInt16Test() {
            MethodInfo stringToUInt16 = _datetimeParser.GetMethod("StringToUInt16", BindingFlags.NonPublic | BindingFlags.Static); ;

            ushort[] expected = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ushort[] actual = {
                (ushort)stringToUInt16.Invoke(null, new object[] { "jan" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "feb" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "mar" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "apr" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "may" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "jun" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "jul" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "aug" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "sep" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "oct" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "nov" }),
                (ushort)stringToUInt16.Invoke(null, new object[] { "dec" }),
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void SplitByLineTest() {
            string[] actualLines = DatetimeParser.SplitByLine(CommonTestResource.EXCEPTED_TEXT).ToArray();
            if (actualLines.Count<string>() == CommonTestResource.EXCEPTED_LINE.Count<string>()) {
                int index = 0;
                foreach (string excepted in CommonTestResource.EXCEPTED_LINE) {
                    Assert.AreEqual<string>(actualLines[index], excepted);
                    index += 1;
                }
            }
            else {
                Assert.Fail("exceptedLines.Count() not equal to actualLines.Count().");
            }
        }

        [TestMethod]
        public void ParseExprTest() {
            StatistTotalByDateTime[] exceptedStatistTotalByDateTimes = new StatistTotalByDateTime[] {
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 5 , 27) , Total = 20 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 5 , 29) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 5 , 31) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 17) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 20) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 24) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 26) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 4) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 5) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 10) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 13) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 17) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 18) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 19) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 22) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 24) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 29) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 31) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 2) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 6) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 8) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 9) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 21) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 22) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 24) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 25) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 29) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 1) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 3) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 6) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 7) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 9) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 14) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 19) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 20) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 22) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 25) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 26) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 27) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 30) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 10 , 1) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 10 , 4) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 10 , 5) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 4) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 14) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 14) , Total = 3 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 24) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 25) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 26) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 27) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 30) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 31) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 10) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 16) , Total = 1 }
            };
            int index = 0;
            foreach (var item in exceptedStatistTotalByDateTimes) {
                Assert.IsTrue(item.Equivalent(DatetimeParser.ParseExpressToStatistTotalByDateTime(CommonTestResource.EXCEPTED_LINE[index])));
                index += 1;
            }
        }
    }
}
