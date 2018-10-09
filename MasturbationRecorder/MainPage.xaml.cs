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

		private async void Button_Click(object sender, RoutedEventArgs e) {
			FileOpenPicker openPicker = new FileOpenPicker();
			openPicker.ViewMode = PickerViewMode.Thumbnail;
			openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
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
					if (line != "") {   // 忽略空行
						StatistTotalByDateTime statist = DatetimeParser.ParseALine(line);
						dateTimes.AddLast(statist);
					}
				}

				// 遍历所有 Rectangle 根据 _datetimes 进行着色
				foreach (Rectangle rect in RectanglesCanvas.Children) {
					Debug.WriteLine(DateTime.Parse(rect.Name));
					rect.Fill = GetBackgroundOfRectanglesByDateTime(dateTimes, DateTime.Parse(rect.Name));
				}

				Debug.WriteLine("Outputing datetime file content:");
				foreach (var item in dateTimes) {
					Debug.WriteLine(item.DateTime.ToShortDateString());
				}
			}
			else {
				Debug.WriteLine("Operation cancelled.");
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

		private SolidColorBrush GetBackgroundOfRectanglesByDateTime(LinkedList<StatistTotalByDateTime> dateTimes, DateTime currentDateTime) {
			if (dateTimes == null) {
				throw new ArgumentNullException("DateTimes cannot be empty.");
			}
			else if (dateTimes.Count==0) {
				return new SolidColorBrush(Windows.UI.Colors.LightGray);
			}
			else {
				//var groupDateTimeByTotal = from k in dateTimes group k by k;
				//var classifyLevelBaseOnTotal = from dt in groupDateTimeByTotal
				//							   select new StatistTotalByDateTime { DateTime = dt.Key, Total = dt.Count() };
				//foreach (var statist in classifyLevelBaseOnTotal) {
				//	Debug.WriteLine($"DateTime: {statist.DateTime}  Total: {statist.Total}");
				//}
				var moreLess = dateTimes.Min().Total;
				var moreBiger = dateTimes.Max().Total;
				ulong GetLevelScore() {
					if (moreBiger == 0 && moreLess == 0) {
						return 0;
					}
					else {
						return moreBiger - moreLess >= 4 ? 5 : (moreBiger - moreLess) + 1;
					}
				}
				IDictionary<ulong, SolidColorBrush> classifyColorByLevelScore() {
					switch (GetLevelScore()) {
						case 0:
							return new Dictionary<ulong, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) }
							};
						case 1:
							return new Dictionary<ulong, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
						case 2:
							return new Dictionary<ulong, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
						case 3:
							return new Dictionary<ulong, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 3, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
						case 4:
							return new Dictionary<ulong, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
								{ 3, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 4, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
						case 5:
							return new Dictionary<ulong, SolidColorBrush>() {
								{ 0, new SolidColorBrush(Windows.UI.Colors.LightGray) },
								{ 1, new SolidColorBrush(Windows.UI.Colors.YellowGreen) },
								{ 2, new SolidColorBrush(Windows.UI.Colors.GreenYellow) },
								{ 3, new SolidColorBrush(Windows.UI.Colors.LawnGreen) },
								{ 4, new SolidColorBrush(Windows.UI.Colors.Green) },
								{ 5, new SolidColorBrush(Windows.UI.Colors.DarkGreen) }
							};
						default:
							throw new InvalidDataException($"levelRange out of range: {GetLevelScore()}");
					}
				}
				IDictionary<ulong, SolidColorBrush> classifyLevelColor = classifyColorByLevelScore();
				var totalOfCurrentDateTime = 0UL;
				try {
					totalOfCurrentDateTime = (from item in dateTimes
											  where item.DateTime == currentDateTime
											  select item.Total).First();
				}
				catch (InvalidOperationException ex) {
					Debug.WriteLine(ex.Message);
				}
				return classifyLevelColor[totalOfCurrentDateTime];
			}
		}

		/// <summary>
		/// 根据时间链表中的每个节点的 Total 分别计算它们的差值（Diff），根据 Diff
		/// 对节点分组
		/// </summary>
		/// <param name="dateTimes"></param>
		/// <returns></returns>
		private List<(ulong Ordinal, ulong Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList)>
			GroupDateTimesByDiff(LinkedList<StatistTotalByDateTime> dateTimes) {
			List<(ulong Ordinal, ulong Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList)> res =
				new List<(ulong Ordinal, ulong Diff, SortedList<ulong, StatistTotalByDateTime> StaticsList)>();
			ulong tempDiff = 0UL;
			ulong ordinal = 0UL;
			SortedList<ulong, StatistTotalByDateTime> values = new SortedList<ulong, StatistTotalByDateTime>();
			void AddUniqueToValues(LinkedListNode<StatistTotalByDateTime> node) {
				try {
					values.Add(node.Value.Total, node.Value);
				}
				catch (ArgumentException) { }	// 如果被添加的节点已存在，直接忽略
			}
			for (var current = dateTimes.First; current.Next.Next != null; current = current.Next) {
				if (tempDiff == 0) {
					tempDiff = current.Next.Value.Total - current.Value.Total;
					AddUniqueToValues(current);
					AddUniqueToValues(current.Next);
				}
				else {
					if (tempDiff == current.Next.Value.Total - current.Value.Total) {
						AddUniqueToValues(current);
						AddUniqueToValues(current.Next);
					}
					else {
						res.Add((Ordinal: ++ordinal, Diff: tempDiff, StaticsList: values));
						/*
						 * 请勿使用 values.Clear() 对其进行重置，因为 values 按引用传递给 StaticsList,
						 * 修改 values 的元素会影响到 StaticsList，所以这里给 values 分配新的 SortedList 实例
						 */
						values = new SortedList<ulong, StatistTotalByDateTime>();
					}
				}
			}
			return res;
		}
	}
}
