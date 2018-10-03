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
			CalculateDateTime();
			this.RectanglesLayout2();
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
			Debug.WriteLine($"RectanglesCanvas widht and height: {this.RectanglesCanvas.ActualWidth}, {this.RectanglesCanvas.ActualHeight}");
			(Root.Children.First() as Border).Width = this.RectanglesCanvas.Width;
			(Root.Children.First() as Border).Height = this.RectanglesCanvas.Height + 50;
			Rectangle[][] rectangles = new Rectangle[53][];
			for (int column = 0, canvasLeft = 0; column <= 52; column++) {
				if (column == 0) {
					canvasLeft = leftSpace;
				}
				else {
					canvasLeft += (columnDistance + rectWidth);
				}
				for (int row = 0, canvasTop = 0; row <= 6; row++) {
					if (row == 0) {
						canvasTop = topSpace;
					}
					else {
						canvasTop += (rowDistance + rectHeight);
					}
					if (column == 52) {
						if (row > Convert.ToInt32(today.DayOfWeek)) {
							goto Jump;
						}
						else {
							CreateRectangle(rectWidth, rectHeight, canvasLeft, canvasTop);
						}
					}
					else {
						CreateRectangle(rectWidth, rectHeight, canvasLeft, canvasTop);
					}

				}
				Jump: continue;
			}
		}

		private void RectanglesLayout2() {
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
							canvasTop: row * rectHeight + row * rowDistance + topSpace
						);
					}
				}
				else {
					for (int row = 6; row >= 0; row--) {
						CreateRectangle(
							rectWidth: rectWidth,
							rectHeight: rectHeight,
							canvasLeft: column * rectWidth + columnDistance * (column - 1) + leftSpace,
							canvasTop: row * rectHeight + row * rowDistance + topSpace
						);
					}
				}
			}
		}

		private void CreateRectangle(int rectWidth, int rectHeight, int canvasLeft, int canvasTop, DateTime dateTime, LinkedList<DateTime> lik, Func<LinkedList<DateTime>, DateTime, SolidColorBrush> func) {
			Rectangle rect = new Rectangle {
				Name = dateTime.ToShortDateString(),
				Width = rectWidth,
				Height = rectHeight,
				//Fill = new SolidColorBrush(Windows.UI.Colors.YellowGreen)
				Fill = func(lik, dateTime)
			};
			RectanglesCanvas.Children.Add(rect);
			Canvas.SetLeft(rect, canvasLeft);
			Canvas.SetTop(rect, canvasTop);
		}

		private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
			Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
		}

		private static SolidColorBrush GetBackgroundOfRectanglesByDateTime(LinkedList<DateTime> lik, DateTime dateTime) {
			if (lik == null) {
				return new SolidColorBrush(Windows.UI.Colors.YellowGreen);
			}
			else {
				var groupDateTimeByTotal = from k in lik group k by k;
				var classifyColorBaseOnTotal = from dt in groupDateTimeByTotal select new { @DateTime = dt.Key, Total = dt.Count() };
				var moreLess = classifyColorBaseOnTotal.Min().Total;
				var moreBiger = classifyColorBaseOnTotal.Max().Total;
				var levelRange = moreBiger - moreLess >= 4 ? 5 : (moreBiger - moreLess) + 1;
				
			}
		}

		private async void Button_ClickAsync(object sender, RoutedEventArgs e) {
			FileOpenPicker openPicker = new FileOpenPicker();
			openPicker.ViewMode = PickerViewMode.Thumbnail;
			openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
			openPicker.FileTypeFilter.Add(".txt");
			openPicker.FileTypeFilter.Add(".mast");

			StorageFile file = await openPicker.PickSingleFileAsync();
			if (file != null) {
				// Application now has read/write access to the picked file
				Debug.WriteLine("Picked document: ");
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
	}
}
