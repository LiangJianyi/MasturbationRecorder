using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Media;
using System.Numerics;

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
                Debug.WriteLine($"Current node: {current.Value} in GroupDateTimesByDiff.");

                bool dateTimesCountBiggerThanOne = dateTimes.Count > 1;
                if (dateTimesCountBiggerThanOne) {
                    tempDiff = current.Next.Value.Total - current.Value.Total;
                }
                else {
                    tempDiff = 0L;
                }
                if (values.Count == 0 && datetimesDiffTable.Count == 0) {
                    AddUniqueToValues(values, current, dateTimesCountBiggerThanOne);
                    AddUniqueToValues(values, current.Next, dateTimesCountBiggerThanOne);
                    datetimesDiffTable.Add((Ordinal: ++ordinal, Diff: tempDiff, StaticsList: values));
                }
                else if (values.Count > 0) {
                    if (datetimesDiffTable[Convert.ToInt32(ordinal - 1)].Diff == tempDiff && tempDiff > 0L) {
                        //datetimesDiffTable[Convert.ToInt32(ordinal - 1)].StaticsList.Add(current.Next.Value.Total, current.Next.Value);
                        AddUniqueToValues(datetimesDiffTable[Convert.ToInt32(ordinal - 1)].StaticsList, current.Next);
                    }
                    else {
                        values = new SortedList<BigInteger, StatistTotalByDateTime>();
                        AddUniqueToValues(values, current, dateTimesCountBiggerThanOne);
                        AddUniqueToValues(values, current.Next, dateTimesCountBiggerThanOne);
                        datetimesDiffTable.Add((Ordinal: ++ordinal, Diff: tempDiff, StaticsList: values));
                    }
                }
                else {
                    throw new Exception($"Unknow error: res.Count = {datetimesDiffTable.Count}, values.Count = {values.Count}");
                }
            }
            return datetimesDiffTable;
        }

        /// <summary>
        /// 为 dateTimesDiffTable 分级并返回一个交错数组
        /// </summary>
        /// <param name="dateTimesDiffTable">
        /// 接收一个元组列表，该列表由方法 MainPageViewModel.GroupDateTimesByDiff 产生，其根据时间链表（LinkeList<StatistTotalByDateTime>）
        /// 中每个相邻元素之间的 Total 差值进行分组和排序，每个元组之间呈升序排列（由 Ordinal 属性决定），元组之间的 Diff 不一定
        /// 呈线性增长，其取决于对应的 StaticsList ，可以肯定的是，Ordinal 值越大，其对应的 StaticsList 中的 StatistTotalByDateTime
        /// 的 Total 属性值也越大
        /// </param>
        /// <returns></returns>
        public static SortedList<BigInteger, StatistTotalByDateTime>[][] GetClassifyDateTimesTable(List<(BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList)> dateTimesDiffTable) {
            if (dateTimesDiffTable == null) {
                throw new ArgumentNullException("DateTimes cannot be empty.");
            }
            else if (dateTimesDiffTable.Count == 0) {
                throw new ArgumentException("dateTimesDiffTable must have content.");
            }
            else {
                /// 根据 dateTimesDiffTable 的长度决定到底应该分多少级
                BigInteger GetLevelScore() => dateTimesDiffTable.LongCount() == 0 ? 0 :
                    dateTimesDiffTable.LongCount() >= 4 ? 5 : dateTimesDiffTable.LongCount() + 1;

                IDictionary<BigInteger, SolidColorBrush> classifyLevelColor = ClassifyColorByLevelScore(GetLevelScore());

                // dateTimesDiffTable 每个级别有 classifyLevelByDiff 个元素((BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList))，
                // 如果 diffRemain 大于 0，则最后一个级别有 classifyLevelByDiff + diffRemain 个元素
                var classifyLevelByDiff = dateTimesDiffTable.LongCount() / GetLevelScore();
                var diffRemain = dateTimesDiffTable.LongCount() % GetLevelScore();

                // 存放已经分级的 (BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList) 对象，
                // 长度由 GetLevelScore 决定，每个元素是另一个数组，长度由每一级元组对象的总和决定，即 classifyLevelByDiff，
                // 如果 diffRemain > 0，那么最后一个元素包含的数组有 classifyLevelByDiff + diffRemain
                SortedList<BigInteger, StatistTotalByDateTime>[][] classifiedDateTimes =
                    new SortedList<BigInteger, StatistTotalByDateTime>[(int)GetLevelScore()][];

                /*
				 *	无法更改List <T>索引参数的类型，它必须是int。 将来可以考虑创建一个需要 BigInteger 索引的自定义类型。
				 *	你不能创建一个足够大的数组来要求64位索引。如果要创建可以存储这么多内容的列表，则必须使用某种树结构或交错数组。 
				 *	这需要大量的内存！
				 */
                int dateTimesDiffTableIndex = -1;

                for (var level = 0L; level < classifiedDateTimes.LongLength; level++) {
                    if (diffRemain == 0) {
                        classifiedDateTimes[level] = new SortedList<BigInteger, StatistTotalByDateTime>[(int)classifyLevelByDiff];
                        for (var incre = 0L; incre < classifyLevelByDiff; incre++) {
                            classifiedDateTimes[level][incre] = dateTimesDiffTable[++dateTimesDiffTableIndex].StaticsList;
                        }
                    }
                    else {
                        if (level != classifiedDateTimes.LongLength - 1) {
                            classifiedDateTimes[level] = new SortedList<BigInteger, StatistTotalByDateTime>[(int)classifyLevelByDiff];
                            for (var incre = 0L; incre < classifyLevelByDiff; incre++) {
                                classifiedDateTimes[level][incre] = dateTimesDiffTable[++dateTimesDiffTableIndex].StaticsList;
                            }
                        }
                        else {  // 遍历到最后一个级别，classifiedDateTimes 最后一个元素会长一些
                            classifiedDateTimes[level] = new SortedList<BigInteger, StatistTotalByDateTime>[(int)(classifyLevelByDiff + diffRemain)];
                            for (var incre = 0L; incre < classifyLevelByDiff + diffRemain; incre++) {
                                classifiedDateTimes[level][incre] = dateTimesDiffTable[++dateTimesDiffTableIndex].StaticsList;
                            }
                        }
                    }
                }

                //test
                Test_classifiedDateTimes(classifiedDateTimes);

                // 除了 Ordinal 为 1 的元组（即第一级第一行），其它元组的 StaticsList 的首元素均要移除
                for (var level = 0L; level < classifiedDateTimes.LongLength; level++) {
                    if (level == 0L) {
                        for (var incre = 0L; incre < classifiedDateTimes[level].LongLength; incre++) {
                            if (incre > 0L) {
                                classifiedDateTimes[level][incre].RemoveAt(0);
                            }
                        }
                    }
                    else {
                        foreach (var sortList in classifiedDateTimes[level]) {
                            sortList.RemoveAt(0);
                        }
                    }
                }

                //test
                Test_classifiedDateTimes(classifiedDateTimes);

                return classifiedDateTimes;
            }
        }

        public static IDictionary<BigInteger, SolidColorBrush> ClassifyColorByLevelScore(BigInteger level) {
            switch (level.ToString()) {
                case "0":
                    return new Dictionary<BigInteger, SolidColorBrush>() {
                                { new BigInteger(0), new SolidColorBrush(Windows.UI.Colors.LightGray) }
                            };
                case "1":
                    return new Dictionary<BigInteger, SolidColorBrush>() {
                                { new BigInteger(0), new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { new BigInteger(1), new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case "2":
                    return new Dictionary<BigInteger, SolidColorBrush>() {
                                { new BigInteger(0), new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { new BigInteger(1), new SolidColorBrush(Windows.UI.Colors.Green) },
                                { new BigInteger(2), new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case "3":
                    return new Dictionary<BigInteger, SolidColorBrush>() {
                                { new BigInteger(0), new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { new BigInteger(1), new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
                                { new BigInteger(2), new SolidColorBrush(Windows.UI.Colors.Green) },
                                { new BigInteger(3), new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case "4":
                    return new Dictionary<BigInteger, SolidColorBrush>() {
                                { new BigInteger(0), new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { new BigInteger(1), new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
                                { new BigInteger(2), new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
                                { new BigInteger(3), new SolidColorBrush(Windows.UI.Colors.Green) },
                                { new BigInteger(4), new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                case "5":
                    return new Dictionary<BigInteger, SolidColorBrush>() {
                                { new BigInteger(0), new SolidColorBrush(Windows.UI.Colors.LightGray) },
                                { new BigInteger(1), new SolidColorBrush(Windows.UI.Colors.YellowGreen) },
                                { new BigInteger(2), new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
                                { new BigInteger(3), new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
                                { new BigInteger(4), new SolidColorBrush(Windows.UI.Colors.Green) },
                                { new BigInteger(5), new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
                            };
                default:
                    throw new System.IO.InvalidDataException($"levelRange out of range: {level}");
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
            LinkedList<StatistTotalByDateTime> dateTimes = new LinkedList<StatistTotalByDateTime>();
            foreach (var line in lines) {
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
        }
    }
}
