using System;
using System.Numerics;

namespace MasturbationRecorder {
	class StatistTotalByDateTime : IComparable<StatistTotalByDateTime> {
		public DateTime DateTime;
		public BigInteger Total;
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
		public override string ToString() => $"DateTime: {DateTime}, Total: {Total}";
        public bool Equivalent(StatistTotalByDateTime other) => this.DateTime.Equals(other.DateTime) && this.Total.Equals(other.Total);
        // public bool EarlierThan(StatistTotalByDateTime other);
        // public bool LaterThan(StatistTotalByDateTime other);
		public static bool operator >(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) == 1;
		public static bool operator <(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) == -1;
		public static bool operator >=(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) >= 0;
		public static bool operator <=(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) <= 0;
	}
}
