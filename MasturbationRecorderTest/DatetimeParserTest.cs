using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Linq;
using MasturbationRecorder;

namespace MasturbationRecorderTest {
    [TestClass]
    public class DatetimeParserTest {
        [TestMethod]
        public void StringToUInt16Test() {
            MethodInfo stringToUInt16 = typeof(DatetimeParser).GetMethod("StringToUInt16", BindingFlags.NonPublic | BindingFlags.Static); ;

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
            int index = 0;
            foreach (var item in CommonTestResource.ExceptedStatistTotalByDateTimes) {
                Assert.IsTrue(item.Equivalent(DatetimeParser.ParseExpressToStatistTotalByDateTime(CommonTestResource.EXCEPTED_LINE[index])));
                index += 1;
            }
        }
    }
}
