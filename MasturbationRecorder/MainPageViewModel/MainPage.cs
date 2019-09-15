﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    using Debug = System.Diagnostics.Debug;

    /// <summary>
    /// 文件保存模式
    /// </summary>
    enum SaveMode {
        NewFile,
        OrginalFile
    }

    /// <summary>
    /// 控制 MainPage.xaml 的视图逻辑
    /// </summary>
    public sealed partial class MainPage {
        private static Windows.UI.Color LightGray => new Windows.UI.Color() { A = 255, R = 214, G = 218, B = 215 };
        private static Windows.UI.Color OneLevelColor => new Windows.UI.Color() { A = 255, R = 203, G = 229, B = 146 };
        private static Windows.UI.Color TwoLevelColor => new Windows.UI.Color() { A = 255, R = 142, G = 230, B = 107 };
        private static Windows.UI.Color ThreeLevelColor => new Windows.UI.Color() { A = 255, R = 78, G = 154, B = 67 };
        private static Windows.UI.Color FourLevelColor => new Windows.UI.Color() { A = 255, R = 11, G = 110, B = 0 };
        private static Windows.UI.Color FiveLevelColor => new Windows.UI.Color() { A = 255, R = 0, G = 58, B = 6 };

        private static IDictionary<int, SolidColorBrush> ClassifyColorByLevelScore(int groups) {
            switch (groups) {
                case 0:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) }
                            };
                case 1:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(FiveLevelColor) }
                            };
                case 2:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(FourLevelColor) },
                                { 2, new SolidColorBrush(FiveLevelColor) }
                            };
                case 3:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(ThreeLevelColor) },
                                { 2, new SolidColorBrush(FourLevelColor) },
                                { 3, new SolidColorBrush(FiveLevelColor) }
                            };
                case 4:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(TwoLevelColor) },
                                { 2, new SolidColorBrush(ThreeLevelColor) },
                                { 3, new SolidColorBrush(FourLevelColor) },
                                { 4, new SolidColorBrush(FiveLevelColor) }
                            };
                case 5:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(OneLevelColor) },
                                { 2, new SolidColorBrush(TwoLevelColor) },
                                { 3, new SolidColorBrush(ThreeLevelColor) },
                                { 4, new SolidColorBrush(FourLevelColor) },
                                { 5, new SolidColorBrush(FiveLevelColor) }
                            };
                default:
                    throw new System.ArgumentOutOfRangeException($"levelRange out of range: {groups}");
            }
        }

        /// <summary>
        /// 给相应的 Rectangle 标记上日期和星期数
        /// </summary>
        /// <param name="canvas"></param>
        private static void DateTag(Canvas canvas) {
            /*
             * 下面的查询语句用来提取画布中最左侧的一列方块(res1)和最上侧的一行方块(res2)，
             * 然后将这些方块标记上星期数和月份
             */
            IEnumerable<IGrouping<double, UIElement>> leftGroup = from rect in canvas.Children
                                                                  group rect by Canvas.GetLeft(rect);
            IEnumerable<IGrouping<double, UIElement>> topGroup = from rect in canvas.Children
                                                                 group rect by Canvas.GetTop(rect);
            double minLeftGroupKey = leftGroup.Min(group => group.Key);
            double minTopGroupKey = topGroup.Min(group => group.Key);
            IGrouping<double, UIElement> res1 = (from g in leftGroup
                                                 where g.Key == minLeftGroupKey
                                                 select g).First();
            IGrouping<double, UIElement> res2 = (from g in topGroup
                                                 where g.Key == minTopGroupKey
                                                 select g).First();

            /*
             * 给周一、周三、周五的方块打上标记 Mon、Wed、Fri
             */
            foreach (Rectangle rect in res1) {
                if (DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == DayOfWeek.Monday ||
                    DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == DayOfWeek.Wednesday ||
                    DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == DayOfWeek.Friday) {
                    var tbx = new TextBlock() {
                        Text = DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek.ToString().Substring(0, 3),
                        FontSize = 10,
                        Foreground = new SolidColorBrush(Windows.UI.Colors.Gray)
                    };
                    Canvas.SetLeft(tbx, Canvas.GetLeft(rect) - 30);
                    Canvas.SetTop(tbx, Canvas.GetTop(rect));
                    canvas.Children.Add(tbx);
                }
            }

            /*
             * 给每个月份开头的方块打上标记，从 Jan 到 Dec
             */
            Rectangle previous = null;
            foreach (Rectangle rect in res2.Reverse()) {
                void setTopTag(string text) {
                    var tbx = new TextBlock() {
                        Text = text,
                        FontSize = 10,
                        Foreground = new SolidColorBrush(Windows.UI.Colors.Gray)
                    };
                    Canvas.SetLeft(tbx, Canvas.GetLeft(rect));
                    Canvas.SetTop(tbx, Canvas.GetTop(rect) - 15);
                    canvas.Children.Add(tbx);
                }
                if (previous == null) {
                    setTopTag(DatetimeParser.NumberToMonth(DatetimeParser.ParseExpressToDateTime(rect.Name, DateMode.DateWithSlash).Month));
                }
                else {
                    int monthOfPreviousRectangle = DatetimeParser.ParseExpressToDateTime(previous.Name, DateMode.DateWithSlash).Month;
                    int monthOfCurrentRectangle = DatetimeParser.ParseExpressToDateTime(rect.Name, DateMode.DateWithSlash).Month;
                    if (monthOfCurrentRectangle != monthOfPreviousRectangle) {
                        setTopTag(DatetimeParser.NumberToMonth(DatetimeParser.ParseExpressToDateTime(rect.Name, DateMode.DateWithSlash).Month));
                    }
                }
                previous = rect;
            }
        }


        private void RectanglesLayout() {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime todayOfLastyear = new DateTime(today.Year - 1, today.Month, today.Day);
            TimeSpan pastDay = today - todayOfLastyear;
            const int rectWidth = 10;
            const int rectHeight = 10;
            const int columnDistance = 3;
            const int rowDistance = 3;
            const int monthTitleSpace = 40;
            const int bottomSpace = 20;
            const int leftSpace = 80;
            const int topSpace = 37;
            const int rightSpace = leftSpace;
            int rectCount = pastDay.Days;
            int totalWeek = pastDay.Days / 7;
            this.RectanglesCanvas.Width = totalWeek * columnDistance + leftSpace + rightSpace + totalWeek * rectWidth + rectWidth;
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
                Fill = new SolidColorBrush(LightGray),
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

        /// <summary>
        /// 初始化页面和改变窗体大小都需要调用该方法对 UI 布局进行变更，
        /// 该方法为设计自适应界面而准备。
        /// </summary>
        private void UpdateMainPageLayout() {
            Menu.Width = this.Window.Bounds.Width;
            RootGrid.Width = this.Window.Bounds.Width;
            RootGrid.Height = this.Window.Bounds.Height - ((double)RootCanvas.Resources["CanvasTopForRootGrid"]);
            Canvas.SetTop(AvatarStack, 80);
            Canvas.SetLeft(AvatarStack, this.Window.Bounds.Width - AvatarStack.ActualWidth - 50);
        }

        /// <summary>
        /// 绘制方块颜色
        /// </summary>
        /// <param name="entries">分级后条目列表</param>
        /// <param name="haveProgressBoard">是否开启进度条面板，true 为开启，反之不开启</param>
        private void DrawRectangleColor(List<IGrouping<BigInteger, StatistTotalByDateTime>>[] entries, bool haveProgressBoard) {
#if DEBUG
            Debug.WriteLine($"Executing DrawRectangleColor:");
#endif
            if (haveProgressBoard) {
                ProgressBoard.Slide(RectanglesCanvas);
            }
            IDictionary<int, SolidColorBrush> colorDic = ClassifyColorByLevelScore(entries.Length);

            Windows.Foundation.IAsyncAction action = Windows.System.Threading.ThreadPool.RunAsync(
                async (asyncAction) => {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    priority: Windows.UI.Core.CoreDispatcherPriority.Normal,
                    agileCallback: () => {
                        // level 作为 entries 的索引值，值越小对应的 Total 越小
                        for (int level = 0; level < entries.Length; level++) {
                            List<IGrouping<BigInteger, StatistTotalByDateTime>> groups = entries[level];
                            IGrouping<BigInteger, StatistTotalByDateTime> group = null;
                            for (int groupsIncre = 0; groupsIncre < groups.LongCount(); groupsIncre++) {
                                group = groups[groupsIncre];
                                foreach (StatistTotalByDateTime entry in group) {
                                    // 过滤掉非 Rectangle 的元素（比如 ProgressBoard）
                                    var rectangles = from rect in RectanglesCanvas.Children
                                                     where rect is Rectangle
                                                     select rect;
                                    foreach (Rectangle rect in rectangles) {
                                        if (rect.Name == entry.DateTime.ToShortDateString()) {
                                            rect.Fill = colorDic[level + 1];
#if DEBUG
                                            ToolTip toolTip = new ToolTip {
                                                Content = rect.Name + $"  Level:{level + 1}  Total:{entry.Total}  Color:{(rect.Fill as SolidColorBrush).Color}"
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
                    });
                });

        }

        /// <summary>
        /// 重置方块的颜色和闪烁状态
        /// </summary>
        private void ResetRectangle() {
            foreach (var rect in _rectangleRegisteTable) {
                rect.Fill = new SolidColorBrush(LightGray);
            }
            // 重置方块颜色之后要紧接着重新初始化该表
            _rectangleRegisteTable = new HashSet<Rectangle>();
            // 停止所有闪烁状态的方块
            foreach (var rect in Blink.BlinkedRectangles) {
                Blink.StopBlink(rect.Value.rectangle);
            }
        }

        /// <summary>
        /// 将变更作为新文件存储
        /// </summary>
        /// <returns>
        /// 返回一个元组，Status 字段代表文件的更新状态，FileIsPick 字段代表用户是否在文件选取器上选取文件，true 为已选取，false 为用户关闭了文件选取器
        /// </returns>
        private async Task SaveNewFileAsync() {
            FileSavePicker savePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "New Record"
            };
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt", ".mast" });
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null) {
                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                switch (status) {
                    case Windows.Storage.Provider.FileUpdateStatus.Complete:
                        await FileIO.WriteLinesAsync(file, _model.ToArray());
                        break;
                    case Windows.Storage.Provider.FileUpdateStatus.Incomplete:
                    case Windows.Storage.Provider.FileUpdateStatus.UserInputNeeded:
                    case Windows.Storage.Provider.FileUpdateStatus.CurrentlyUnavailable:
                    case Windows.Storage.Provider.FileUpdateStatus.Failed:
                    case Windows.Storage.Provider.FileUpdateStatus.CompleteAndRenamed:
                    default:
                        throw new FilePickFaildException($"Pick a file faild! Windows.Storage.Provider.FileUpdateStatus = {status}");
                }
            }
            DrawRectangleColor(_model?.GroupDateTimesByTotal(), true);
        }

        /// <summary>
        /// 将变更覆盖原有文件
        /// </summary>
        /// <returns></returns>
        private async Task SaveOrginalFileAsync() {
            CachedFileManager.DeferUpdates(_file);
            await FileIO.WriteLinesAsync(_file, _model.ToArray());
            Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(_file);
            if (status == Windows.Storage.Provider.FileUpdateStatus.Complete) {
                Debug.WriteLine("File " + _file.Name + " was saved.");
            }
            else {
                throw new FileNotSaveException($"File {_file.Name} couldn't be saved.");
            }
            DrawRectangleColor(_model?.GroupDateTimesByTotal(), true);
        }
    }
}
