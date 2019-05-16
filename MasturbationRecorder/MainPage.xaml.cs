using System;
using System.Numerics;
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
            //this.StateBar.Height = this.Menu.ActualHeight;
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e) {
            FileOpenPicker openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.FileTypeFilter.Add(".mast");

            StorageFile file = await openPicker.PickSingleFileAsync();
            StateBar.IsActive = true;

            if (file != null) {
                //Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
                string text = await FileIO.ReadTextAsync(file);
#if DEBUG
                Debug.WriteLine(text);
                Debug.WriteLine($"line count:{(from t in text where t == '\n' select t).Count() + 1}");
#endif
                IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
                try {
                    LinkedList<StatistTotalByDateTime> dateTimes = MainPageViewModel.LinesConvertToStatistTotalByDateTimes(lines);
                    var res = MainPageViewModel.GroupDateTimesByDiff(dateTimes);
                    var classifyDateTimes = MainPageViewModel.GetClassifyDateTimesTable(res);

                    // 遍历所有 Rectangle 根据 _datetimes 进行着色
                    foreach (Rectangle rect in RectanglesCanvas.Children) {
#if DEBUG
                        Debug.WriteLine(DateTime.Parse(rect.Name));
#endif
                        rect.Fill = GetFillOfRectanglesByDifferentOfDateTimesTotal(
                            currentDateTime: DateTime.Parse(rect.Name),
                            classifyLevelColor: MainPageViewModel.ClassifyColorByLevelScore(classifyDateTimes.LongLength),
                            classifiedDateTimes: classifyDateTimes
                        );
                    }

                    StateBar.IsActive = false;
                }
                catch (ArgumentException e) {
                    DisplayErrorDialog(e.Message);
                }
            }
            else {
                DisplayErrorDialog("File open fail!");
            }
        }


        private static async void DisplayErrorDialog(string content) {
            ContentDialog fileOpenFailDialog = new ContentDialog {
                Title = "Error",
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await fileOpenFailDialog.ShowAsync();
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
#if DEBUG
            Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
#endif
        }

        private static SolidColorBrush GetFillOfRectanglesByDifferentOfDateTimesTotal(
            DateTime currentDateTime,
            IDictionary<BigInteger, SolidColorBrush> classifyLevelColor,
            SortedList<BigInteger, StatistTotalByDateTime>[][] classifiedDateTimes) {
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

    }
}
