
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasturbationRecorder;
using System.Reflection;

namespace UnitTestMasturbationRecorder {
	[TestClass]
	public class DatetimeParserTest {
		[TestMethod]
		public void StringToUInt16Test() {
			MethodInfo stringToUInt16 = typeof(DatetimeParser).GetMethod("StringToUInt16", BindingFlags.NonPublic | BindingFlags.Static); ;

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
	}
}
