using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    using Debug = System.Diagnostics.Debug;

    public sealed partial class MainPage : Page {
        private Window _window = Window.Current;
        /// <summary>
        /// 对已经填充颜色的 Rectangle 进行登记
        /// </summary>
        private static HashSet<Rectangle> _rectangleRegisteTable = new HashSet<Rectangle>();

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

            if (file != null) {
                StateBar.IsActive = true;
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
                string text = await FileIO.ReadTextAsync(file);
#if DEBUG
                Debug.WriteLine(text);
                Debug.WriteLine($"line count:{(from t in text where t == '\n' select t).Count() + 1}");
#endif
                IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
                try {
                    LinkedList<StatistTotalByDateTime> dateTimes = MainPageViewModel.LinesConvertToStatistTotalByDateTimes(lines);
                    List<IGrouping<BigInteger, StatistTotalByDateTime>>[] res = MainPageViewModel.GroupDateTimesByDiff(dateTimes);
#if DEBUG
                    for (int level = 0; level < res.Length; level++) {
                        Debug.WriteLine($"level: {level + 1}");
                        Debug.WriteLine($"  List res[{level}]:");
                        foreach (var group in res[level]) {
                            Debug.WriteLine($"    Total: {group.Key}");
                            foreach (var item in group) {
                                Debug.WriteLine($"      {item}");
                            }
                        }
                    }
#endif
                    DrawRectangleColor(res);
                }
                catch (ArgumentException err) {
                    DisplayErrorDialog(err.Message);
                }
                finally {
                    StateBar.IsActive = false;
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
                Fill = new SolidColorBrush(MainPageViewModel.LightGray),
            };
#if DEBUG
            ToolTip toolTip = new ToolTip {
                Content = rect.Name + $"  Level:0  Total:0  Color:{(rect.Fill as SolidColorBrush).Color}"
            };
#endif
#if RELEASE
            ToolTip toolTip = new ToolTip {
                Content = dateTime.ToShortDateString()
            }; 
#endif
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

        private void DrawRectangleColor(List<IGrouping<BigInteger, StatistTotalByDateTime>>[] entries) {
#if DEBUG
            Debug.WriteLine($"Executing DrawRectangleColor:");
#endif
            IDictionary<int, SolidColorBrush> colorDic = MainPageViewModel.ClassifyColorByLevelScore(entries.Length);
            // level 作为 entries 的索引值，值越小对应的 Total 越小
            for (int level = 0; level < entries.Length; level++) {
                List<IGrouping<BigInteger, StatistTotalByDateTime>> groups = entries[level];
                IGrouping<BigInteger, StatistTotalByDateTime> group = null;
                for (int groupsIncre = 0; groupsIncre < groups.LongCount(); groupsIncre++) {
                    group = groups[groupsIncre];
                    foreach (var item in group) {
                        foreach (Rectangle rect in RectanglesCanvas.Children) {
#if DEBUG
                            Debug.WriteLine($"rect: {rect.Name}");
#endif
                            if (rect.Name == item.DateTime.ToShortDateString()) {
                                rect.Fill = colorDic[level + 1];
#if DEBUG
                                ToolTip toolTip = new ToolTip {
                                    Content = rect.Name + $"  Level:{level + 1}  Total:{item.Total}  Color:{(rect.Fill as SolidColorBrush).Color}"
                                };
                                ToolTipService.SetToolTip(rect, toolTip);
#endif
                                _rectangleRegisteTable.Add(rect);
                                break;
                            }
                            else {
                                if (!_rectangleRegisteTable.Contains(rect)) {
                                    rect.Fill = colorDic[0];
#if DEBUG
                                    ToolTip toolTip = new ToolTip {
                                        Content = rect.Name + $"  Level:0  Total:0  Color:{(rect.Fill as SolidColorBrush).Color}"
                                    };
                                    ToolTipService.SetToolTip(rect, toolTip);
#endif 
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
