using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Janyee.Utilty;

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

    class StatistTotalByDateTimeModel {
        private LinkedList<StatistTotalByDateTime> _entries;
        public LinkedList<StatistTotalByDateTime> Entries => _entries;
        public StatistTotalByDateTimeModel(IEnumerable<string> lines) {
            LinkedList<StatistTotalByDateTime> dateTimes = new LinkedList<StatistTotalByDateTime>();
            foreach (var line in lines) {
                if (line != "" && line != "\r") {   // 忽略空行
                    StatistTotalByDateTime statist = DatetimeParser.ParseExpr(line);
                    dateTimes.AddLast(statist);
                }
            }
            this._entries = dateTimes;
        }
        public List<IGrouping<BigInteger, StatistTotalByDateTime>>[] GroupDateTimesByTotal() {
            IOrderedEnumerable<IGrouping<BigInteger, StatistTotalByDateTime>> groupingForTotal = from e in _entries
                                                                                                 group e by e.Total into newgroup
                                                                                                 orderby newgroup.Key
                                                                                                 select newgroup;

#if DEBUG
            Debug.WriteLine("Executing GroupDateTimesByDiff...");
            foreach (var item in groupingForTotal) {
                Debug.WriteLine(item.Key);
                foreach (var subitem in item) {
                    Debug.WriteLine($"  {subitem}");
                }
            }
#endif
            // 一个级别有若干个Key；一个Key有若干条记录
            // levelByTotal 指示每个级别有多少个 Key（groupingForTotal根据Total分组出来的Key）
            List<IGrouping<BigInteger, StatistTotalByDateTime>> levels = null;

            // groups 代表 dateTimes 根据每个元素的 Total 分组之后 groups（item.Key） 的总数
            BigInteger groups = groupingForTotal.LongCount();

            if (groups > 5) {
                // keysForEachLevel 表示每个级别应包含多少个 item.Key
                BigInteger keysForEachLevel = groups / 5;
                BigInteger remain = groups % 5;

#if DEBUG
                Debug.WriteLine($"groups: {groups}");
                Debug.WriteLine($"keysForEachLevel: {keysForEachLevel}");
                Debug.WriteLine($"remain: {remain}");
#endif

                List<IGrouping<BigInteger, StatistTotalByDateTime>>[] entries = new List<IGrouping<BigInteger, StatistTotalByDateTime>>[5];
                BigInteger keyIncre = 0;
                // entriesIncre 没有必要使用 BigInteger，因为不管有多少个 Key，分成多少个 group，
                // entries 的长度永远为 5，因为纪录器最多只能分五级
                int entriesIncre = 0;
                foreach (var item in groupingForTotal) {
                    keyIncre += 1;
                    if (keyIncre == keysForEachLevel) {
                        if (entriesIncre < entries.Length - 1) {
                            keyIncre = 0;
                            levels.Add(item);
                            entries[entriesIncre] = levels;
                            entriesIncre += 1;
                            levels = null;
                        }
                        else if (entriesIncre == entries.Length - 1) {
                            if (remain != 0) {
                                levels.Add(item);
                            }
                            // 这么做迫使下次循环 keyIncre 仍然为 5，
                            // 这样能再次进入 keyIncre == keysForEachLevel 语句块；
                            keyIncre -= 1;
                        }
                    }
                    else if (levels == null) {
                        levels = new List<IGrouping<BigInteger, StatistTotalByDateTime>> { item };
                    }
                    else {
                        levels.Add(item);
                    }
                }
                entries[entriesIncre] = levels;
                return entries;
            }
            else if (groups <= 5 && groups > 0) {
                List<IGrouping<BigInteger, StatistTotalByDateTime>>[] entries = new List<IGrouping<BigInteger, StatistTotalByDateTime>>[groups.BigIntegerToInt32()];
                List<IGrouping<BigInteger, StatistTotalByDateTime>> temp = groupingForTotal.ToList();
                for (int i = 0; i < entries.Length; i++) {
                    entries[i] = new List<IGrouping<BigInteger, StatistTotalByDateTime>>(1) {
                        temp[i]
                    };
                }
                return entries;
            }
            else {
                throw new Exception($"Unkown error. Groups is {groups}");
            }
        }
        public void AddEntry(StatistTotalByDateTime statistTotalByDateTime) {

        }
    }
}
