
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			const string testDataFile = "ms-appx:///Test Data/masturbation test.txt";
			const string exceptedText = "May 27 2018 x20\r\nMay 29 2018\r\nMay 31 2018\r\nJun 12 2018\r\nJun 15 2018\r\nJun 17 2018\r\nJun 20 2018\r\nJun 24 2018\r\nJun 26 2018\r\nJul 04 2018\r\nJul 05 2018\r\nJul 10 2018 x2\r\nJul 11 2018\r\nJul 13 2018\r\nJul 16 2018\r\nJul 17 2018\r\nJul 18 2018\r\nJul 19 2018\r\nJul 22 2018 x2\r\nJul 24 2018\r\nJul 29 2018\r\nJul 31 2018\r\nAug 2 2018\r\nAug 6 2018\r\nAug 8 2018\r\nAug 9 2018\r\nAug 11 2018\r\nAug 12 2018\r\nAug 15 2018\r\nAug 16 2018\r\nAug 21 2018 x2\r\nAug 22 2018\r\nAug 24 2018\r\nAug 25 2018\r\nAug 29 2018 x2\r\nSep 1 2018\r\nSep 3 2018\r\nSep 6 2018\r\nSep 7 2018\r\nSep 9 2018\r\nSep 11 2018\r\nSep 12 2018\r\nSep 14 2018 x2\r\nSep 15 2018\r\nSep 16 2018\r\nSep 19 2018\r\nSep 20 2018\r\nSep 22 2018\r\nSep 25 2018\r\nSep 26 2018\r\nSep 27 2018 x2\r\nSep 30 2018\r\nOct 1 2018\r\nOct 4 2018\r\nOct 5 2018";
			Uri uri = new Uri(testDataFile);
			StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
			string text = await FileIO.ReadTextAsync(file);
			Assert.AreEqual<string>(exceptedText, text);
			IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
		}
	}
}
