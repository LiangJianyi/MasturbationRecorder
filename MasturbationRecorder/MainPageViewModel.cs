﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Media;
using System.Numerics;
using Janyee.Utilty;

namespace MasturbationRecorder {
    /// <summary>
    /// 控制 MainPage.xaml 的视图逻辑
    /// </summary>
    static class MainPageViewModel {
        /// <summary>
        /// 根据时间链表中的每个节点的 Total 分别计算它们的差值（Diff），
        /// 根据 Diff 对节点分组
        /// </summary>
        /// <param name="dateTimes">接收一个时间链表</param>
        /// <returns>
        /// 返回一个元组列表，其根据时间链表（LinkeList<StatistTotalByDateTime>）
        /// 中每个相邻元素之间的 Total 差值进行分组和排序，每个元组之间呈升序排列（由 Ordinal 属性决定），元组之间的 Diff 不一定
        /// 呈线性增长，其取决于对应的 StaticsList ，可以肯定的是，Ordinal 值越大，
        /// 其对应的 StaticsList 中的 StatistTotalByDateTime 的 Total 属性值也越大，
        /// 该 Total 属性值用作 StaticsList 的 Key。
        /// </returns>
        public static List<(BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList)>
            GroupDateTimesByDiff(LinkedList<StatistTotalByDateTime> dateTimes) {
            List<(BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList)> datetimesDiffTable =
                new List<(BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList)>();
            BigInteger tempDiff = 0L;
            BigInteger ordinal = 0UL;
            SortedList<BigInteger, StatistTotalByDateTime> values = new SortedList<BigInteger, StatistTotalByDateTime>();

            /// <summary>
            /// 给有序列表 values 添加元素，同时检测节点在 SortedList 的唯一性
            /// </summary>
			void AddUniqueToValues(SortedList<BigInteger, StatistTotalByDateTime> sortList, LinkedListNode<StatistTotalByDateTime> node, bool nullCheck = true) {
                // 这个地方可以考虑优化，看看有无替代方法而不用异常检测
                try {
                    sortList.Add(node.Value.Total, node.Value);
                }
                catch (ArgumentException) { }   // 如果被添加的节点已存在，直接忽略
                catch (NullReferenceException) {
                    if (nullCheck) {
                        throw;
                    }
                    else { } // 如果null检测关闭，直接忽略
                }
            }

            for (var current = dateTimes.First; current != null; current = current.Next) {
#if DEBUG
                Debug.WriteLine($"Current node: {current.Value} in GroupDateTimesByDiff.");

#endif
                bool dateTimesCountBiggerThanOne = dateTimes.Count > 1;
                if (dateTimesCountBiggerThanOne) {
                    if (current.Next != null) {
                        tempDiff = BigInteger.Abs(current.Next.Value.Total - current.Value.Total);
                    }
                    else {
                        tempDiff = current.Value.Total;
                    }
                }
                else {
                    tempDiff = 0L;
                }
                if (values.Count == 0 && datetimesDiffTable.Count == 0) {
                    AddUniqueToValues(values, current, dateTimesCountBiggerThanOne);
                    if (current.Next != null) {
                        AddUniqueToValues(values, current.Next, dateTimesCountBiggerThanOne);
                    }
                    datetimesDiffTable.Add((Ordinal: ++ordinal, Diff: tempDiff, StaticsList: values));
                }
                else if (values.Count > 0) {
                    if (datetimesDiffTable[ordinal.BigIntegerToInt32() - 1].Diff == tempDiff && tempDiff > 0L) {
                        AddUniqueToValues(datetimesDiffTable[ordinal.BigIntegerToInt32() - 1].StaticsList, current.Next);
                    }
                    else {
                        values = new SortedList<BigInteger, StatistTotalByDateTime>();
                        AddUniqueToValues(values, current, dateTimesCountBiggerThanOne);
                        if (current.Next != null) {
                            AddUniqueToValues(values, current.Next, dateTimesCountBiggerThanOne);
                        }
                        datetimesDiffTable.Add((Ordinal: ++ordinal, Diff: tempDiff, StaticsList: values));
                    }
                }
                else {
                    throw new Exception($"Unknow error: res.Count = {datetimesDiffTable.Count}, values.Count = {values.Count}");
                }
            }
            return datetimesDiffTable;
        }


        public static List<IGrouping<BigInteger, StatistTotalByDateTime>>[] GroupDateTimesByDiff2(LinkedList<StatistTotalByDateTime> dateTimes) {
            Debug.WriteLine("Executing GroupDateTimesByDiff2...");
            var groupingForTotal = from e in dateTimes
                                   group e by e.Total into newgroup
                                   orderby newgroup.Key
                                   select newgroup;

#if DEBUG
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
                BigInteger keysForEachLevel = groups > 5 ? groups / 5 : 1;
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
                        keyIncre = 0;
                        levels.Add(item);
                        entries[entriesIncre] = levels;
                        entriesIncre += 1;
                        levels = null;
                    }
                    else if (levels == null) {
                        levels = new List<IGrouping<BigInteger, StatistTotalByDateTime>> {
                            item
                        };
                    }
                    else {
                        levels.Add(item);
                    }
                }
                if (levels != null && levels.LongCount() > 1) {
                    entries[entriesIncre] = levels;
                    entriesIncre += 1;
                }
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

        public static IDictionary<int, SolidColorBrush> ClassifyColorByLevelScore(int groups) {
            switch (groups) {
                case 0:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(Windows.UI.Colors.LightGray) }
                            };
                case 1:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { 1, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case 2:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { 1, new SolidColorBrush(Windows.UI.Colors.Green) },
                                { 2, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case 3:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { 1, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
                                { 2, new SolidColorBrush(Windows.UI.Colors.Green) },
                                { 3, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case 4:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { 1, new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
                                { 2, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
                                { 3, new SolidColorBrush(Windows.UI.Colors.Green) },
                                { 4, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case 5:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { 1, new SolidColorBrush(Windows.UI.Colors.YellowGreen) },
                                { 2, new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
                                { 3, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
                                { 4, new SolidColorBrush(Windows.UI.Colors.Green) },
                                { 5, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                default:
                    throw new System.IO.InvalidDataException($"levelRange out of range: {groups}");
            }
        }

        /// <summary>
        /// 将 IEnumerable<string> 转换为 LinkedList<StatistTotalByDateTime>
        /// 由于该方法主要使用 DatetimeParser.ParseExpr(string)，而 DatetimeParser.ParseExpr(string) 已经编写单元测试，
        /// 所以无需为该方法另写单元测试。
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static LinkedList<StatistTotalByDateTime> LinesConvertToStatistTotalByDateTimes(IEnumerable<string> lines) {
#if DEBUG
            Debug.WriteLine("Invoking LinesConvertToStatistTotalByDateTimes.");
#endif
            LinkedList<StatistTotalByDateTime> dateTimes = new LinkedList<StatistTotalByDateTime>();
            foreach (var line in lines) {
#if DEBUG
                Debug.WriteLine($"Current line: {line}");
                if (line[0] == 'M' && line[1] == 'a' && line[2] == 'r' && line[3] == ' ' && line[4] == '2' && line[5] == '8') {
                    ;
                }
#endif
                if (line != "" && line != "\r") {   // 忽略空行
                    StatistTotalByDateTime statist = DatetimeParser.ParseExpr(line);
                    dateTimes.AddLast(statist);
                }
            }
            return dateTimes;
        }

        /// <summary>
        /// classifiedDateTimes 的测试用例，不要删除
        /// </summary>
        /// <param name="classifiedDateTimes"></param>
        private static void Test_classifiedDateTimes(SortedList<BigInteger, StatistTotalByDateTime>[][] classifiedDateTimes) {
#if DEBUG
            var level2 = 0;
            foreach (var sortListArr in classifiedDateTimes) {
                Debug.WriteLine($"Level: {++level2}");
                foreach (var sortList in sortListArr) {
                    foreach (var item in sortList) {
                        Debug.Write($"{item.Key} ");
                    }
                }
                Debug.WriteLine("");
            }
#endif
        }
    }
}
