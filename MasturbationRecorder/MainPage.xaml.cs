using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
	using Debug = System.Diagnostics.Debug;

	public sealed partial class MainPage : Page {
		private Window _window = Window.Current;

		public MainPage() {
			this._window.SizeChanged += Current_SizeChanged;
			this.InitializeComponent();
			this.RectanglesLayout();
		}

		private async void OpenFileButton_Click(object sender, RoutedEventArgs e) {
			FileOpenPicker openPicker = new FileOpenPicker {
				ViewMode = PickerViewMode.Thumbnail,
				SuggestedStartLocation = PickerLocationId.ComputerFolder
			};
			openPicker.FileTypeFilter.Add(".txt");
			openPicker.FileTypeFilter.Add(".mast");

			StorageFile file = await openPicker.PickSingleFileAsync();
			if (file != null) {
				//Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
				string text = await FileIO.ReadTextAsync(file);
				Debug.WriteLine(text);
				Debug.WriteLine($"line count:{(from t in text where t == '\n' select t).Count() + 1}");
				IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
				LinkedList<StatistTotalByDateTime> dateTimes = new LinkedList<StatistTotalByDateTime>();
				foreach (var line in lines) {
					if (line != "" && line != "\r") {   // 忽略空行
						StatistTotalByDateTime statist = DatetimeParser.ParseALine(line);
						dateTimes.AddLast(statist);
					}
				}

				var res = this.GroupDateTimesByDiff(dateTimes);
				var classifyDateTimes = GetClassifyDateTimesTable(res);

				// 遍历所有 Rectangle 根据 _datetimes 进行着色
				foreach (Rectangle rect in RectanglesCanvas.Children) {
					Debug.WriteLine(DateTime.Parse(rect.Name));
					rect.Fill = GetFillOfRectanglesByDifferentOfDateTimesTotal(
						currentDateTime: DateTime.Parse(rect.Name),
						classifyLevelColor: ClassifyColorByLevelScore(classifyDateTimes.LongLength),
						classifiedDateTimes: classifyDateTimes
					);
				}


			}
			else {
				Debug.WriteLine("Operation cancelled.");
			}
		}

		private void TempTest(LinkedList<StatistTotalByDateTime> dateTimes) {
			var res = this.GroupDateTimesByDiff(dateTimes);
			string PrintStaticsList(SortedList<ulong, StatistTotalByDateTime> list) {
				string text = null;
				foreach (var statics in list) {
					if (text == null) {
						text = $"[\"{statics}\", ";
					}
					else {
						text += $"\"{statics}\", ";
					}
				}
				text = text.Remove(text.Length - 2, 2);
				return text;
			}
			foreach (var item in res) {
				Debug.WriteLine($"{item.Ordinal}  {item.Diff}  {PrintStaticsList(item.StaticsList)}");
			}
		}

		private void RectanglesLayout() {
			DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
			DateTime todayOfLastyear = new DateTime(today.Year - 1, today.Month, today.Day);
			TimeSpan fuck = today - todayOfLastyear;
			const int rectWidth = 10;
			const int rectHeight = 10;
			const int columnDistance = 3;
			const int rowDistance = 3;
			const int monthTitleSpace = 40;
			const int bottomSpace = 20;
			const int leftSpace = 80;
			const int topSpace = 37;
			const int rightSpace = leftSpace;
			int rectCount = fuck.Days;
			int totalWeek = fuck.Days / 7;
			int remainDaysOfYear = fuck.Days % 7;
			if (remainDaysOfYear == 0) {
				this.RectanglesCanvas.Width = (totalWeek - 1) * columnDistance + leftSpace + rightSpace + totalWeek * rectWidth;
			}
			else {
				this.RectanglesCanvas.Width = totalWeek * columnDistance + leftSpace + rightSpace + totalWeek * rectWidth + rectWidth;
			}
			this.RectanglesCanvas.Height = rowDistance * 6 + bottomSpace + monthTitleSpace + 7 * rectHeight;
			DateTime dateOfEachRectangle = today;
			for (int column = totalWeek; column >= 0; column--) {
				if (column == totalWeek) {
					for (int row = Convert.ToInt32(today.DayOfWeek); row >= 0; row--, dateOfEachRectangle = dateOfEachRectangle.AddDays(-1)) {
						CreateRectangle(
							rectWidth: rectWidth,
							rectHeight: rectHeight,
							canvasLeft: column * rectWidth + columnDistance * (column - 1) + leftSpace,
							canvasTop: row * rectHeight + row * rowDistance + topSpace,
							dateTime: dateOfEachRectangle
						);
					}
				}
				else {
					for (int row = 6; row >= 0; row--, dateOfEachRectangle = dateOfEachRectangle.AddDays(-1)) {
						CreateRectangle(
							rectWidth: rectWidth,
							rectHeight: rectHeight,
							canvasLeft: column * rectWidth + columnDistance * (column - 1) + leftSpace,
							canvasTop: row * rectHeight + row * rowDistance + topSpace,
							dateTime: dateOfEachRectangle
						);
					}
				}
			}
		}

		private void CreateRectangle(int rectWidth, int rectHeight, int canvasLeft, int canvasTop, DateTime dateTime) {
			Rectangle rect = new Rectangle {
				Name = dateTime.ToShortDateString(),
				Width = rectWidth,
				Height = rectHeight,
				Fill = new SolidColorBrush(Windows.UI.Colors.LightGray),
			};
			ToolTip toolTip = new ToolTip {
				Content = dateTime.ToShortDateString()
			};
			ToolTipService.SetToolTip(rect, toolTip);
			RectanglesCanvas.Children.Add(rect);
			Canvas.SetLeft(rect, canvasLeft);
			Canvas.SetTop(rect, canvasTop);
		}

		private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
			Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
		}

		private static IDictionary<long, SolidColorBrush> ClassifyColorByLevelScore(long level) {
			switch (level) {
				case 0:
					return new Dictionary<long, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) }
							};
				case 1:
					return new Dictionary<long, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
				case 2:
					return new Dictionary<long, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
				case 3:
					return new Dictionary<long, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 3, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
				case 4:
					return new Dictionary<long, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
								{ 3, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 4, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
				case 5:
					return new Dictionary<long, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.YellowGreen) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
								{ 3, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
								{ 4, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 5, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
				default:
					throw new InvalidDataException($"levelRange out of range: {level}");
			}
		}

		/// <summary>
		/// 为 dateTimesDiffTable 分级并返回一个交错数组
		/// </summary>
		/// <param name="dateTimesDiffTable">
		/// 接收一个元组列表，该列表由方法 GroupDateTimesByDiff 产生，其根据时间链表（LinkeList<StatistTotalByDateTime>）
		/// 中每个相邻元素之间的 Total 差值进行分组和排序，每个元组之间呈升序排列（由 Ordinal 属性决定），元组之间的 Diff 不一定
		/// 呈线性增长，其取决于对应的 StaticsList ，可以肯定的是，Ordinal 值越大，其对应的 StaticsList 中的 StatistTotalByDateTime
		/// 的 Total 属性值也越大
		/// </param>
		/// <returns></returns>
		private SortedList<ulong, StatistTotalByDateTime>[][] GetClassifyDateTimesTable(List<(ulong Ordinal, long Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList)> dateTimesDiffTable) {
			if (dateTimesDiffTable == null) {
				throw new ArgumentNullException("DateTimes cannot be empty.");
			}
			else if (dateTimesDiffTable.Count == 0) {
				throw new ArgumentException("dateTimesDiffTable must have content.");
			}
			else {
				/// 根据 dateTimesDiffTable 的长度决定到底应该分多少级
				long GetLevelScore() => dateTimesDiffTable.LongCount() == 0 ? 0 :
					dateTimesDiffTable.LongCount() >= 4 ? 5 : dateTimesDiffTable.LongCount() + 1;

				IDictionary<long, SolidColorBrush> classifyLevelColor = ClassifyColorByLevelScore(GetLevelScore());

				// dateTimesDiffTable 每个级别有 classifyLevelByDiff 个元素((ulong Ordinal, long Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList))，
				// 如果 diffRemain 大于 0，则最后一个级别有 classifyLevelByDiff + diffRemain 个元素
				var classifyLevelByDiff = dateTimesDiffTable.LongCount() / GetLevelScore();
				var diffRemain = dateTimesDiffTable.LongCount() % GetLevelScore();

				// 存放已经分级的 (ulong Ordinal, long Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList) 对象，
				// 长度由 GetLevelScore 决定，每个元素是另一个数组，长度由每一级元组对象的总和决定，即 classifyLevelByDiff，
				// 如果 diffRemain > 0，那么最后一个元素包含的数组有 classifyLevelByDiff + diffRemain
				SortedList<ulong, StatistTotalByDateTime>[][] classifiedDateTimes =
					new SortedList<ulong, StatistTotalByDateTime>[GetLevelScore()][];

				/*
				 *	无法更改List <T>索引参数的类型，它必须是int。 将来可以考虑创建一个需要 ulong 索引的自定义类型。
				 *	你不能创建一个足够大的数组来要求64位索引。如果要创建可以存储这么多内容的列表，则必须使用某种树结构或交错数组。 
				 *	这需要大量的内存！
				 */
				int dateTimesDiffTableIndex = -1;

				for (var level = 0L; level < classifiedDateTimes.LongLength; level++) {
					if (diffRemain == 0) {
						classifiedDateTimes[level] = new SortedList<ulong, StatistTotalByDateTime>[classifyLevelByDiff];
						for (var incre = 0L; incre < classifyLevelByDiff; incre++) {
							classifiedDateTimes[level][incre] = dateTimesDiffTable[++dateTimesDiffTableIndex].StaticsList;
						}
					}
					else {
						if (level != classifiedDateTimes.LongLength - 1) {
							classifiedDateTimes[level] = new SortedList<ulong, StatistTotalByDateTime>[classifyLevelByDiff];
							for (var incre = 0L; incre < classifyLevelByDiff; incre++) {
								classifiedDateTimes[level][incre] = dateTimesDiffTable[++dateTimesDiffTableIndex].StaticsList;
							}
						}
						else {  // 遍历到最后一个级别，classifiedDateTimes 最后一个元素会长一些
							classifiedDateTimes[level] = new SortedList<ulong, StatistTotalByDateTime>[classifyLevelByDiff + diffRemain];
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

		private static SolidColorBrush GetFillOfRectanglesByDifferentOfDateTimesTotal(
			DateTime currentDateTime,
			IDictionary<long, SolidColorBrush> classifyLevelColor,
			SortedList<ulong, StatistTotalByDateTime>[][] classifiedDateTimes) {
			for (var level = 0L; level < classifiedDateTimes.LongLength; level++) {
				foreach (var sortList in classifiedDateTimes[level]) {
					foreach (var date in sortList) {
						if (date.Value.DateTime == currentDateTime) {
							return classifyLevelColor[level + 1];
						}
					}
				}
			}
			return classifyLevelColor[0L];
		}

		/// <summary>
		/// classifiedDateTimes 的测试用例，不要删除
		/// </summary>
		/// <param name="classifiedDateTimes"></param>
		private static void Test_classifiedDateTimes(SortedList<ulong, StatistTotalByDateTime>[][] classifiedDateTimes) {
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

		/// <summary>
		/// 根据时间链表中的每个节点的 Total 分别计算它们的差值（Diff），
		/// 根据 Diff 对节点分组
		/// </summary>
		/// <param name="dateTimes">接收一个时间链表</param>
		/// <returns>
		/// 返回一个元组列表，其根据时间链表（LinkeList<StatistTotalByDateTime>）
		/// 中每个相邻元素之间的 Total 差值进行分组和排序，每个元组之间呈升序排列（由 Ordinal 属性决定），元组之间的 Diff 不一定
		/// 呈线性增长，其取决于对应的 StaticsList ，可以肯定的是，Ordinal 值越大，
		/// 其对应的 StaticsList 中的 StatistTotalByDateTime 的 Total 属性值也越大
		/// </returns>
		private List<(ulong Ordinal, long Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList)>
			GroupDateTimesByDiff(LinkedList<StatistTotalByDateTime> dateTimes) {
			List<(ulong Ordinal, long Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList)> datetimesDiffTable =
				new List<(ulong Ordinal, long Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList)>();
			long tempDiff = 0L;
			ulong ordinal = 0UL;
			SortedList<ulong, StatistTotalByDateTime> values = new SortedList<ulong, StatistTotalByDateTime>();
			void AddUniqueToValues(LinkedListNode<StatistTotalByDateTime> node) {
				try {
					values.Add(node.Value.Total, node.Value);
				}
				catch (ArgumentException) { }   // 如果被添加的节点已存在，直接忽略
			}
			for (var current = dateTimes.First; current.Next != null; current = current.Next) {
				tempDiff = Convert.ToInt64(current.Next.Value.Total - current.Value.Total);
				if (values.Count == 0 && datetimesDiffTable.Count == 0) {
					AddUniqueToValues(current);
					AddUniqueToValues(current.Next);
					datetimesDiffTable.Add((Ordinal: ++ordinal, Diff: tempDiff, StaticsList: values));
				}
				else if (values.Count > 0) {
					if (datetimesDiffTable[Convert.ToInt32(ordinal - 1)].Diff == tempDiff) {
						datetimesDiffTable[Convert.ToInt32(ordinal - 1)].StaticsList.Add(current.Next.Value.Total, current.Next.Value);
					}
					else {
						values = new SortedList<ulong, StatistTotalByDateTime>();
						AddUniqueToValues(current);
						AddUniqueToValues(current.Next);
						datetimesDiffTable.Add((Ordinal: ++ordinal, Diff: tempDiff, StaticsList: values));
					}
				}
				else {
					throw new Exception($"Unknow error: res.Count = {datetimesDiffTable.Count}, values.Count = {values.Count}");
				}
			}
			return datetimesDiffTable;
		}
	}
}
