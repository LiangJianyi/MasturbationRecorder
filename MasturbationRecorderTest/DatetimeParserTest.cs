﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using MasturbationRecorder;

namespace UnitTestMasturbationRecorder {
    [TestClass]
    public class DatetimeParserTest {
        private Type datetimeParser = typeof(DatetimeParser);

        [TestMethod]
        public void StringToUInt16Test() {
            MethodInfo stringToUInt16 = datetimeParser.GetMethod("StringToUInt16", BindingFlags.NonPublic | BindingFlags.Static); ;

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

            string[] exceptedLines = new string[] {
                "May 27 2018 x20\r",
                "May 29 2018\r",
                "May 31 2018\r",
                "Jun 12 2018\r",
                "Jun 15 2018\r",
                "Jun 17 2018\r",
                "Jun 20 2018\r",
                "Jun 24 2018\r",
                "Jun 26 2018\r",
                "Jul 04 2018\r",
                "Jul 05 2018\r",
                "Jul 10 2018 x2\r",
                "Jul 11 2018\r",
                "Jul 13 2018\r",
                "Jul 16 2018\r",
                "Jul 17 2018\r",
                "Jul 18 2018\r",
                "Jul 19 2018\r",
                "Jul 22 2018 x2\r",
                "Jul 24 2018\r",
                "Jul 29 2018\r",
                "Jul 31 2018\r",
                "Aug 2 2018\r",
                "Aug 6 2018\r",
                "Aug 8 2018\r",
                "Aug 9 2018\r",
                "Aug 11 2018\r",
                "Aug 12 2018\r",
                "Aug 15 2018\r",
                "Aug 16 2018\r",
                "Aug 21 2018 x2\r",
                "Aug 22 2018\r",
                "Aug 24 2018\r",
                "Aug 25 2018\r",
                "Aug 29 2018 x2\r",
                "Sep 1 2018\r",
                "Sep 3 2018\r",
                "Sep 6 2018\r",
                "Sep 7 2018\r",
                "Sep 9 2018\r",
                "Sep 11 2018\r",
                "Sep 12 2018\r",
                "Sep 14 2018 x2\r",
                "Sep 15 2018\r",
                "Sep 16 2018\r",
                "Sep 19 2018\r",
                "Sep 20 2018\r",
                "Sep 22 2018\r",
                "Sep 25 2018\r",
                "Sep 26 2018\r",
                "Sep 27 2018 x2\r",
                "Sep 30 2018\r",
                "Oct 1 2018\r",
                "Oct 4 2018\r",
                "Oct 5 2018"
            };
            IEnumerable<string> actualLines = DatetimeParser.SplitByLine(exceptedText);
            if (exceptedLines.Count<string>() == actualLines.Count<string>()) {
                int index = 0;
                foreach (string actual in actualLines) {
                    Assert.AreEqual<string>(exceptedLines[index], actual);
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
    }
}