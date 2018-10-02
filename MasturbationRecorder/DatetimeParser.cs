using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasturbationRecorder {
	static class DatetimeParser {
		private static ushort StringToUInt16(string month) {
			switch (month.ToLower()) {
				case "mon":
					return 1;
				case "feb":
					return 2;
				case "mar":
					return 3;
				case "apr":
					return 4;
				case "may":
					return 5;
				case "jun":
					return 6;
				case "jul":
					return 7;
				case "aug":
					return 8;
				case "sep":
					return 9;
				case "oct":
					return 10;
				case "nov":
					return 11;
				case "dec":
					return 12;
				default:
					throw new ArgumentException("Month acronym error.");
			}
		}

		private static string[] GetToken(string text) {
			return text.Split(' ');
		}

		internal static DateTime ParseALine(string text) {
			string[] tokens = GetToken(text);
			if (tokens.Length == 3) {
				ushort monthValue = StringToUInt16(tokens[0]);
				ushort dayValue = Convert.ToUInt16(tokens[1]);
				ushort yearValue = Convert.ToUInt16(tokens[2]);
				return new DateTime(yearValue, monthValue, dayValue);
			}
			else if (tokens.Length == 4) {
				ushort monthValue = StringToUInt16(tokens[0]);
				ushort dayValue = Convert.ToUInt16(tokens[1]);
				ushort yearValue = Convert.ToUInt16(tokens[2]);
				return new DateTime(yearValue, monthValue, dayValue);
			}
			else {
				throw new ArgumentOutOfRangeException("Date time format error.");
			}
		}
	}
}
