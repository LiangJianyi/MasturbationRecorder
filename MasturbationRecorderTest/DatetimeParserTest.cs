using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
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
                (ushort)stringToUInt16.Invoke(null, new object[] { "mon" }),
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
        public async Task SplitByLineTest() {
            const string exceptedText =
                "May 27 2018 x20\r\n" +
                "May 29 2018\r\n" +
                "May 31 2018\r\n" +
                "Jun 12 2018\r\n" +
                "Jun 15 2018\r\n" +
                "Jun 17 2018\r\n" +
                "Jun 20 2018\r\n" +
                "Jun 24 2018\r\n" +
                "Jun 26 2018\r\n" +
                "Jul 04 2018\r\n" +
                "Jul 05 2018\r\n" +
                "Jul 10 2018 x2\r\n" +
                "Jul 11 2018\r\n" +
                "Jul 13 2018\r\n" +
                "Jul 16 2018\r\n" +
                "Jul 17 2018\r\n" +
                "Jul 18 2018\r\n" +
                "Jul 19 2018\r\n" +
                "Jul 22 2018 x2\r\n" +
                "Jul 24 2018\r\n" +
                "Jul 29 2018\r\n" +
                "Jul 31 2018\r\n" +
                "Aug 2 2018\r\n" +
                "Aug 6 2018\r\n" +
                "Aug 8 2018\r\n" +
                "Aug 9 2018\r\n" +
                "Aug 11 2018\r\n" +
                "Aug 12 2018\r\n" +
                "Aug 15 2018\r\n" +
                "Aug 16 2018\r\n" +
                "Aug 21 2018 x2\r\n" +
                "Aug 22 2018\r\n" +
                "Aug 24 2018\r\n" +
                "Aug 25 2018\r\n" +
                "Aug 29 2018 x2\r\n" +
                "Sep 1 2018\r\n" +
                "Sep 3 2018\r\n" +
                "Sep 6 2018\r\n" +
                "Sep 7 2018\r\n" +
                "Sep 9 2018\r\n" +
                "Sep 11 2018\r\n" +
                "Sep 12 2018\r\n" +
                "Sep 14 2018 x2\r\n" +
                "Sep 15 2018\r\n" +
                "Sep 16 2018\r\n" +
                "Sep 19 2018\r\n" +
                "Sep 20 2018\r\n" +
                "Sep 22 2018\r\n" +
                "Sep 25 2018\r\n" +
                "Sep 26 2018\r\n" +
                "Sep 27 2018 x2\r\n" +
                "Sep 30 2018\r\n" +
                "Oct 1 2018\r\n" +
                "Oct 4 2018\r\n" +
                "Oct 5 2018";

            IEnumerable<string> actualLines = DatetimeParser.SplitByLine(exceptedText);
            if (CommonTestResource.TestTextLine.Count<string>() == actualLines.Count<string>()) {
                int index = 0;
                foreach (string actual in actualLines) {
                    Assert.AreEqual<string>(CommonTestResource.TestTextLine[index], actual);
                    index += 1;
                }
            }
            else {
                Assert.Fail("exceptedLines.Count() not equal to actualLines.Count().");
            }
        }

        // 此函数会让测试工具故障，原因未知。
        public async Task SplitByLineTest2() {
            //const string testDataFile = "ms-appx:///Test Data/masturbation test.txt";
            const string testDataFile = @"C:\Users\a124p\Documents\Test Data";
            const string exceptedText = "May 27 2018 x20\r\nMay 29 2018\r\nMay 31 2018\r\nJun 12 2018\r\nJun 15 2018\r\nJun 17 2018\r\nJun 20 2018\r\nJun 24 2018\r\nJun 26 2018\r\nJul 04 2018\r\nJul 05 2018\r\nJul 10 2018 x2\r\nJul 11 2018\r\nJul 13 2018\r\nJul 16 2018\r\nJul 17 2018\r\nJul 18 2018\r\nJul 19 2018\r\nJul 22 2018 x2\r\nJul 24 2018\r\nJul 29 2018\r\nJul 31 2018\r\nAug 2 2018\r\nAug 6 2018\r\nAug 8 2018\r\nAug 9 2018\r\nAug 11 2018\r\nAug 12 2018\r\nAug 15 2018\r\nAug 16 2018\r\nAug 21 2018 x2\r\nAug 22 2018\r\nAug 24 2018\r\nAug 25 2018\r\nAug 29 2018 x2\r\nSep 1 2018\r\nSep 3 2018\r\nSep 6 2018\r\nSep 7 2018\r\nSep 9 2018\r\nSep 11 2018\r\nSep 12 2018\r\nSep 14 2018 x2\r\nSep 15 2018\r\nSep 16 2018\r\nSep 19 2018\r\nSep 20 2018\r\nSep 22 2018\r\nSep 25 2018\r\nSep 26 2018\r\nSep 27 2018 x2\r\nSep 30 2018\r\nOct 1 2018\r\nOct 4 2018\r\nOct 5 2018";
            Uri uri = new Uri(testDataFile);
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(testDataFile);
            StorageFile file = await folder.GetFileAsync("masturbation test.txt");
            string text = await FileIO.ReadTextAsync(file);
            Assert.AreEqual<string>(exceptedText, text);
            IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
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
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 10 , 5) , Total = 1 }
            };
            int index = 0;
            foreach (var item in exceptedStatistTotalByDateTimes) {
                Assert.IsTrue(item.Equivalent(DatetimeParser.ParseExpr(CommonTestResource.TestTextLine[index])));
                index += 1;
            }
        }
    }
}
