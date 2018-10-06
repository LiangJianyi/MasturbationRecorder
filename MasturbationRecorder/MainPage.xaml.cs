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
			//Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
			this.RectanglesLayout();
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
			for (int column = totalWeek; column >= 0; column--) {
				if (column == totalWeek) {
					for (int row = Convert.ToInt32(today.DayOfWeek); row >= 0; row--) {
						CreateRectangle(
							rectWidth: rectWidth,
							rectHeight: rectHeight,
							canvasLeft: column * rectWidth + columnDistance * (column - 1) + leftSpace,
							canvasTop: row * rectHeight + row * rowDistance + topSpace,
							dateTime: DateTime.Now
						);
					}
				}
				else {
					for (int row = 6; row >= 0; row--) {
						CreateRectangle(
							rectWidth: rectWidth,
							rectHeight: rectHeight,
							canvasLeft: column * rectWidth + columnDistance * (column - 1) + leftSpace,
							canvasTop: row * rectHeight + row * rowDistance + topSpace,
							dateTime: DateTime.Now
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
			rect.PointerEntered += Rect_PointerEntered;
			rect.PointerExited += Rect_PointerExited;
			RectanglesCanvas.Children.Add(rect);
			Canvas.SetLeft(rect, canvasLeft);
			Canvas.SetTop(rect, canvasTop);
		}

		private void Rect_PointerExited(object sender, PointerRoutedEventArgs e) {
			Debug.WriteLine($"Pointer exit from the {(sender as Rectangle).Name}");
		}

		private void Rect_PointerEntered(object sender, PointerRoutedEventArgs e) {
			Debug.WriteLine($"Pointer enter to the {(sender as Rectangle).Name}");
		}

		private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
			Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
		}

		private SolidColorBrush GetBackgroundOfRectanglesByDateTime(LinkedList<DateTime> dateTimes, DateTime currentDateTime) {
			if (dateTimes == null || dateTimes.Count == 0) {
				return new SolidColorBrush(Windows.UI.Colors.YellowGreen);
			}
			else {
				var groupDateTimeByTotal = from k in dateTimes group k by k;
				var classifyLevelBaseOnTotal = from dt in groupDateTimeByTotal
											   select new StatistTotalByDateTime { DateTime = dt.Key, Total = dt.Count() };
				//var classifyLevelBaseOnTotal2=from k in _dateTimes group
				foreach (var statist in classifyLevelBaseOnTotal) {
					Debug.WriteLine($"DateTime: {statist.DateTime}  Total: {statist.Total}");
				}
				var moreLess = classifyLevelBaseOnTotal.Min().Total;
				var moreBiger = classifyLevelBaseOnTotal.Max().Total;
				int GetLevelScore() {
					if (moreBiger == 0 && moreLess == 0) {
						return 0;
					}
					else {
						return moreBiger - moreLess >= 4 ? 5 : (moreBiger - moreLess) + 1;
					}
				}
				var levelScore = GetLevelScore();
				IDictionary<int, SolidColorBrush> classifyColorByLevelScore() {
					switch (levelScore) {
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
							throw new InvalidDataException($"levelRange out of range: {levelScore}");
					}
				}
				IDictionary<int, SolidColorBrush> classifyLevelColor = classifyColorByLevelScore();
				var totalOfCurrentDateTime = 0;
				try {
					totalOfCurrentDateTime = (from item in classifyLevelBaseOnTotal
											  where item.DateTime == currentDateTime
											  select item.Total).First();
				}
				catch (InvalidOperationException ex) {
					Debug.WriteLine(ex.Message);
				}
				return classifyLevelColor[totalOfCurrentDateTime];
			}
		}

		private async void Button_Click(object sender, RoutedEventArgs e) {
			FileOpenPicker openPicker = new FileOpenPicker();
			openPicker.ViewMode = PickerViewMode.Thumbnail;
			openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
			openPicker.FileTypeFilter.Add(".txt");
			openPicker.FileTypeFilter.Add(".mast");

			StorageFile file = await openPicker.PickSingleFileAsync();
			if (file != null) {
				// Application now has read/write access to the picked file
				Debug.WriteLine("Picked document: ");
				//Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
				string text = await FileIO.ReadTextAsync(file);
				Debug.WriteLine(text);
				Debug.WriteLine((from t in text where t == '\n' select t).Count() + 1);
				IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
				LinkedList<DateTime> dateTimes = new LinkedList<DateTime>();
				foreach (var line in lines) {
					if (line != "") {   // 忽略空行
						(DateTime dateTime, ulong count) = DatetimeParser.ParseALine(line);
						LinkedList<DateTime> sublik = new LinkedList<DateTime>();
						for (ulong i = 1; i <= count; i++) {
							if (i == 1) {
								sublik.AddFirst(dateTime);
							}
							else {
								sublik.AddAfter(sublik.Last, dateTime);
							}
						}
						dateTimes = new LinkedList<DateTime>(dateTimes.Concat(sublik));
					}
				}
				this.Tintor(dateTimes);
				Debug.WriteLine("Outputing datetime file content:");
				foreach (var item in dateTimes) {
					Debug.WriteLine(item.ToShortDateString());
				}
			}
			else {
				Debug.WriteLine("Operation cancelled.");
			}
		}

		/// <summary>
		/// 遍历所有 Rectangle 根据 _datetimes 进行着色
		/// </summary>
		private void Tintor(LinkedList<DateTime> dateTimes) {
			foreach (Rectangle rect in RectanglesCanvas.Children) {
				rect.Fill = GetBackgroundOfRectanglesByDateTime(dateTimes, DateTime.Parse(rect.Name));
			}
		}

		class StatistTotalByDateTime : IComparable<StatistTotalByDateTime> {
			public DateTime DateTime;
			public int Total;
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
}
