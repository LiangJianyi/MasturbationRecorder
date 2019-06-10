﻿using System;
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
        private static HashSet<Rectangle> _rectangleRegisteTable = null;

        public MainPage() {
#if DEBUG
            this._window.SizeChanged += Current_SizeChanged;
#endif
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
                ResetRectangleColor();  // 每次选择文件之后都要重置方块颜色

                StateBar.IsActive = true;
                ProgressBoard.OpenProgessBoard(StackCanvas, null);
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
                string text = await FileIO.ReadTextAsync(file);
#if DEBUG
                Debug.WriteLine(text);
                Debug.WriteLine($"line count:{(from t in text where t == '\n' select t).Count() + 1}");
#endif
                IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
                try {
                    LinkedList<StatistTotalByDateTime> dateTimes = MainPageViewModel.LinesConvertToStatistTotalByDateTimes(lines);
                    List<IGrouping<BigInteger, StatistTotalByDateTime>>[] res = MainPageViewModel.GroupDateTimesByTotal(dateTimes);
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
                    ProgressBoard.CloseProgessBoard(StackCanvas);
                }
            }
        }

        private void Rect_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            Rectangle rectangle = sender as Rectangle;
            Bubble.CreateBubbleRectangle(
                canvas: RectanglesCanvas,
                hostRect: sender as Rectangle,
                bubbleName: RectanglesCanvas.Resources["OhhohoRect"] as string,
                zoom: (minWidth: (double)RectanglesCanvas.Resources["MinWidth"],
                       minHeight: (double)RectanglesCanvas.Resources["MinHeight"],
                       maxWidth: (double)RectanglesCanvas.Resources["MaxWidth"],
                       maxHeight: (double)RectanglesCanvas.Resources["MaxHeight"])
            );
            Bubble.CreateBubbleStoryboard(
                hostPosition: (left: Canvas.GetLeft(rectangle), top: Canvas.GetTop(rectangle)),
                storyboard_Completed: RectangleBubbleAnimation_Completed);
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
            rect.PointerReleased += Rect_PointerReleased;
#if DEBUG
            ToolTip toolTip = new ToolTip {
                Content = rect.Name + $"  Level:0  Total:0  Color:{(rect.Fill as SolidColorBrush).Color}"
            };
#else
            ToolTip toolTip = new ToolTip {
                Content = dateTime.ToShortDateString()
            }; 
#endif
            ToolTipService.SetToolTip(rect, toolTip);
            RectanglesCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, canvasLeft);
            Canvas.SetTop(rect, canvasTop);
        }

#if DEBUG
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
            Debug.WriteLine($"{this._window.Bounds.Width} , {this._window.Bounds.Height}");
        }
#endif
        /*
         * 气泡动画结束后从 Canvas 移除气泡方块
         */
        private void RectangleBubbleAnimation_Completed(object sender, object e) {
            RectanglesCanvas.Children.Remove(Bubble._bubble);
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

        /// <summary>
        /// 重置方块的颜色
        /// </summary>
        private void ResetRectangleColor() {
            // 程序首次执行时 -rectangleRegisteTable 为 null
            if (_rectangleRegisteTable != null) {
                foreach (Rectangle rect in this.RectanglesCanvas.Children) {
                    if (_rectangleRegisteTable.Contains(rect)) {
                        rect.Fill = new SolidColorBrush(MainPageViewModel.LightGray);
#if DEBUG
                        (ToolTipService.GetToolTip(rect) as ToolTip).Content = rect.Name;
#endif
                    }
                }
            }
            // 充值方块颜色之后要紧接着重新初始化该表
            _rectangleRegisteTable = new HashSet<Rectangle>();
        }
    }
}
