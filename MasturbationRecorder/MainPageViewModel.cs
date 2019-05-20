using System;
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
        public static List<IGrouping<BigInteger, StatistTotalByDateTime>>[] GroupDateTimesByDiff(LinkedList<StatistTotalByDateTime> dateTimes) {
            Debug.WriteLine("Executing GroupDateTimesByDiff...");
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
                        levels = new List<IGrouping<BigInteger, StatistTotalByDateTime>> {
                            item
                        };
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
