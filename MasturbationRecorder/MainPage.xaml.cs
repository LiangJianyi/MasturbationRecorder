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
		private LinkedList<DateTime> _dateTimes = new LinkedList<DateTime>();

		public MainPage() {
			this._window.SizeChanged += Current_SizeChanged;
			this.InitializeComponent();
			//Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
			CalculateDateTime();
			this.RectanglesLayout();
		}

		private static void CalculateDateTime() {
			DateTime now = DateTime.Now;
			DateTime today = new DateTime(now.Year, now.Month, now.Day);
			DateTime todayOfLastyear = new DateTime(today.Year - 1, today.Month, today.Day);
			Debug.WriteLine($"today is {today.DayOfWeek}");
			Debug.WriteLine($"todayOfLastyear is {todayOfLastyear.DayOfWeek}");
			TimeSpan fuck = today - todayOfLastyear;
			Debug.WriteLine(fuck);
			Debug.WriteLine(fuck.Days);
			Debug.WriteLine(fuck.TotalDays);
			Debug.WriteLine($"Week: {((uint)(fuck.TotalDays / 7))}   Remain days: {fuck.TotalDays % 7}");
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

		private SolidColorBrush GetBackgroundOfRectanglesByDateTime(LinkedList<DateTime> lik, DateTime dateTime) {
			if (lik == null) {
				return new SolidColorBrush(Windows.UI.Colors.YellowGreen);
			}
			else {
				var groupDateTimeByTotal = from k in lik group k by k;
				var classifyLevelBaseOnTotal = from dt in groupDateTimeByTotal
											   select new StatistTotalByDateTime { DateTime = dt.Key, Total = dt.Count() };
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
				var totalOfCurrentDateTime = (from item in classifyLevelBaseOnTotal
											  where item.DateTime == dateTime
											  select item.Total).First();
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
						this._dateTimes = new LinkedList<DateTime>(this._dateTimes.Concat(sublik));
					}
				}
				Debug.WriteLine("Outputing datetime file content:");
				foreach (var item in this._dateTimes) {
					Debug.WriteLine(item.ToShortDateString());
				}
			}
			else {
				Debug.WriteLine("Operation cancelled.");
			}
		}

		private bool EnsureUnsnapped() {
			// FilePicker APIs will not work if the application is in a snapped state.
			// If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
			bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
			//if (!unsnapped) {
			//	NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
			//}

			return unsnapped;
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
