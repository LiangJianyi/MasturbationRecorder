using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MasturbationRecorder {
	using Debug = System.Diagnostics.Debug;

	public sealed partial class MainPage : Page {
		private Window _window = Window.Current;

		public MainPage() {
			this._window.SizeChanged += Current_SizeChanged;
			this.InitializeComponent();
			Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
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
			int columnDistance = 10;
			int rowDistance = 10;
			int monthTitleSpace = 40;
			int bottomSpace = 20;
			int leftSpace = 133;
			int rightSpace = leftSpace;
			int rectCount = fuck.Days;
			int totalWeek = fuck.Days / 7;
			int remainDaysOfYear = fuck.Days % 7;
			if (remainDaysOfYear == 0) {
				this.RectanglesCanvas.Width = (totalWeek - 1) * columnDistance;
			}
			else {
				this.RectanglesCanvas.Width = totalWeek * columnDistance;
			}
			this.RectanglesCanvas.Height = rowDistance * 6;
		}

		private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
			Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
		}

		private static void SetGridLayout(Grid grid, int rowCount = 0, int columnCount = 0) {
			for (int c = 0; c < columnCount; c++) {
				grid.ColumnDefinitions.Add(new ColumnDefinition());
			}
			for (int r = 0; r < rowCount; r++) {
				grid.RowDefinitions.Add(new RowDefinition());
			}
		}
	}
}
