﻿using System;
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
			RectanglesCanvas.Children.Clear();

			DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
			DateTime todayOfLastyear = new DateTime(today.Year - 1, today.Month, today.Day);
			TimeSpan fuck = today - todayOfLastyear;
			const int rectWidth = 10;
			const int rectHeight = 10;
			const int columnDistance = 5;
			const int rowDistance = 5;
			const int monthTitleSpace = 40;
			const int bottomSpace = 20;
			const int leftSpace = 133;
			const int topSpace = 200;
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
			for (int i = 0, canvasLeft = 0; i < totalWeek; i++) {
				if (i == 0) {
					canvasLeft = leftSpace;
				}
				else {
					canvasLeft += (columnDistance + rectWidth);
				}
				for (int j = 0, canvasTop = 0; j < 7; j++) {
					if (j == 0) {
						canvasTop = topSpace;
					}
					else {
						canvasTop += (rowDistance + rectHeight);
					}
					Rectangle rect = new Rectangle {
						Width = rectWidth,
						Height = rectHeight,
						Fill = new SolidColorBrush(Windows.UI.Colors.YellowGreen)
					};
					RectanglesCanvas.Children.Add(rect);
					Canvas.SetLeft(rect, canvasLeft);
					Canvas.SetTop(rect, canvasTop);
				}
			}
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
