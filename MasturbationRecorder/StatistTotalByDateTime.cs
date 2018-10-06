using System;

namespace MasturbationRecorder {
	class StatistTotalByDateTime : IComparable<StatistTotalByDateTime> {
		public DateTime DateTime;
		public ulong Total;
		public int CompareTo(StatistTotalByDateTime other) {
			if (this.Total < other.Total) {
				return -1;
			}
			else if (this.Total == other.Total) {
				return 0;
			}
			else {
				return 1;
			}
		}
		public static bool operator >(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) == 1;
		public static bool operator <(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) == -1;
		public static bool operator >=(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) >= 0;
		public static bool operator <=(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) <= 0;
	}
}
