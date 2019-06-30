using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    using Debug = System.Diagnostics.Debug;

    enum SaveMode {
        NewFile,
        OrginalFile
    }

    public sealed partial class MainPage : Page {
        /// <summary>
        /// 获取当前 Window 对象
        /// </summary>
        private Window Window => Window.Current;
        /// <summary>
        /// 注册已经填充颜色的 Rectangle，每个 Rectangle 只能注册一次
        /// </summary>
        private static HashSet<Rectangle> _rectangleRegisteTable = new HashSet<Rectangle>();
        /// <summary>
        /// 暂存当前页面的 StatistTotalByDateTime 集合
        /// </summary>
        private static StatistTotalByDateTimeModel _model = null;
        /// <summary>
        /// 文件的保存模式
        /// </summary>
        private static SaveMode _saveMode = SaveMode.NewFile;
        /// <summary>
        /// 保存从文件选择器选取的文件
        /// </summary>
        private static StorageFile _file = null;

        public MainPage() {
#if DEBUG
            this.Window.SizeChanged += Current_SizeChanged;
#endif
            this.InitializeComponent();
            this.RectanglesLayout();
        }

        private async void OpenFileButtonAsync_Click(object sender, RoutedEventArgs e) {
            FileOpenPicker openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.FileTypeFilter.Add(".mast");

            _file = await openPicker.PickSingleFileAsync();

            if (_file != null) {
                ResetRectangle();  // 每次选择文件之后都要重置方块颜色

                ProgressBoard.Slide(RectanglesCanvas);
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(_file);
                string text = await FileIO.ReadTextAsync(_file);
#if DEBUG
                Debug.WriteLine(text);
                Debug.WriteLine($"line count:{(from t in text where t == '\n' select t).Count() + 1}");
#endif
                try {
                    IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
                    _model = new StatistTotalByDateTimeModel(lines);
                    List<IGrouping<BigInteger, StatistTotalByDateTime>>[] res = _model.GroupDateTimesByTotal();
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
                    DrawRectangleColor(res, false);
                }
                catch (ArgumentException err) {
                    PopErrorDialogAsync(err.Message);
                }
                _saveMode = SaveMode.OrginalFile; // 表示当前的操作基于磁盘上已有的文件
            }
        }

        private void Rect_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            Rectangle rectangle = sender as Rectangle;
            Bubble.CreateBubbleRectangle(
                canvas: RectanglesCanvas,
                hostRect: rectangle,
                bubbleName: RectanglesCanvas.Resources["OhhohoRect"] as string,
                zoom: (minWidth: (double)RectanglesCanvas.Resources["MinWidth"],
                       minHeight: (double)RectanglesCanvas.Resources["MinHeight"],
                       maxWidth: (double)RectanglesCanvas.Resources["MaxWidth"],
                       maxHeight: (double)RectanglesCanvas.Resources["MaxHeight"])
            );
            Bubble.CreateBubbleStoryboard(
                hostPosition: (left: Canvas.GetLeft(rectangle), top: Canvas.GetTop(rectangle)),
                storyboard_Completed: RectangleBubbleAnimation_Completed
            );
            // 闪烁动画，提示用户该方块有未保存的变更；
            Blink.PlayBlink(rectangle);
            if (_model != null) {
                // 检测用户点击的方块对应的日期在之前打开的记录表中是否存在。
                // 如果 x.Count() > 0 为 true 证明存在，否则添加新条目。
                // 注意：x.Count() 和 x.First() 可能会导致两次查询，具体详情参见 MSDN
                var x = from entry in _model.Entries
                        where entry.DateTime.ToShortDateString() == rectangle.Name
                        select entry;
                if (x.Count() > 0) { // 点击绿色方块
                    x.First().Total += 1;
#if DEBUG
                    ToolTip toolTip = new ToolTip {
                        Content = rectangle.Name + $"  Level:0  Total:{x.First().Total}  Color:{(rectangle.Fill as SolidColorBrush).Color}"
                    };
                    ToolTipService.SetToolTip(rectangle, toolTip);
#endif
                }
                else {  // 点击灰色方块
                    _model.AddEntry(rectangle.Name);
#if DEBUG
                    ToolTip toolTip = new ToolTip {
                        Content = rectangle.Name + $"  Level:0  Total:1  Color:{(rectangle.Fill as SolidColorBrush).Color}"
                    };
                    ToolTipService.SetToolTip(rectangle, toolTip);
#endif
                }
            }
            else {
                // _model 为 null 证明用户在空白的状态下添加新条目
                _model = new StatistTotalByDateTimeModel(new string[] { rectangle.Name }, DateMode.DateWithSlash);
#if DEBUG
                ToolTip toolTip = new ToolTip {
                    Content = rectangle.Name + $"  Level:0  Total:1  Color:{(rectangle.Fill as SolidColorBrush).Color}"
                };
                ToolTipService.SetToolTip(rectangle, toolTip);
#endif
            }
            // 显示保存按钮，将变更添加到指定文件中
            if (SaveFileButton.Visibility == Visibility.Collapsed) {
                SaveFileButton.Visibility = Visibility.Visible;
            }
            // 显示刷新按钮，根据变更的时间频率对方块重新着色
            if (RefreshButton.Visibility == Visibility.Collapsed) {
                RefreshButton.Visibility = Visibility.Visible;
            }
            // 显示清空按钮
            if (ClearButton.Visibility == Visibility.Collapsed) {
                ClearButton.Visibility = Visibility.Visible;
            }
        }

        private static async void PopErrorDialogAsync(string content) {
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

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
#if DEBUG
            Debug.WriteLine($"{this.Window.Bounds.Width} , {this.Window.Bounds.Height}");
#endif
            Menu.Width = this.Window.Bounds.Width;
            RootGrid.Width = this.Window.Bounds.Width;
            RootGrid.Height = this.Window.Bounds.Height - ((double)RootCanvas.Resources["CanvasTopForRootGrid"]);
        }
        /*
         * 气泡动画结束后从 Canvas 移除气泡方块
         */
        private void RectangleBubbleAnimation_Completed(object sender, object e) {
            RectanglesCanvas.Children.Remove(Bubble._bubble);
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
            IDictionary<int, SolidColorBrush> colorDic = MainPageViewModel.ClassifyColorByLevelScore(entries.Length);

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
                rect.Fill = new SolidColorBrush(MainPageViewModel.LightGray);
            }
            // 重置方块颜色之后要紧接着重新初始化该表
            _rectangleRegisteTable = new HashSet<Rectangle>();
            // 停止所有闪烁状态的方块
            foreach (var rect in Blink.BlinkedRectangles) {
                Blink.StopBlink(rect.Value.rectangle);
            }
        }

        private async void SaveFileButtonAsync_Click(object sender, RoutedEventArgs e) {
            /*
             * 把 DrawRectangleColor(_model?.GroupDateTimesByTotal()) 写进下面两个 Save 异步方法
             * 可以避免用户触发 CloseButtonClick 事件时对方块颜色重新渲染
             */
            async void saveDialog_PrimaryButtonClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args) {
                await SaveOrginalFileAsync();
            }
            async void saveDialog_SecondaryButtonClickAsync(ContentDialog dialog, ContentDialogButtonClickEventArgs args) {
                await SaveNewFileAsync();
            }
            void saveDialog_CloseButtonClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args) {
                dialog.Hide();
            }

            // 在弹出路径选择器之前应渲染一个悬浮表单，让用户选择
            // 保存方式、文件格式、文件名
            // 给用户提供两种保存方式：
            // 1、更新原有文件
            // 2、作为新文件存储
            switch (_saveMode) {
                case SaveMode.NewFile:
                    await SaveNewFileAsync();
                    _saveMode = SaveMode.OrginalFile;
                    break;
                case SaveMode.OrginalFile:
                    ContentDialog saveDialog = new ContentDialog() {
                        Title = "SaveMode",
                        Content = "选择一种保存方式：",
                        PrimaryButtonText = "更新原有文件",
                        SecondaryButtonText = "作为新文件存储",
                        CloseButtonText = "放弃更改"
                    };
                    saveDialog.PrimaryButtonClick += saveDialog_PrimaryButtonClick;
                    saveDialog.SecondaryButtonClick += saveDialog_SecondaryButtonClickAsync;
                    saveDialog.CloseButtonClick += saveDialog_CloseButtonClick;
                    await saveDialog.ShowAsync();
                    break;
                default:
                    throw new InvalidOperationException($"Unknown Error. SaveMode = {_saveMode.ToString()}");
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

        /// <summary>
        /// 页面加载完成后要对部分控件的视觉状态进行预设
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_Loaded(object sender, RoutedEventArgs e) {
            SaveFileButton.Visibility = Visibility.Collapsed;
            RefreshButton.Visibility = Visibility.Collapsed;
            ClearButton.Visibility = Visibility.Collapsed;
            Menu.Width = this.Window.Bounds.Width;
            RootGrid.Width = this.Window.Bounds.Width;
            RootGrid.Height = this.Window.Bounds.Height - ((double)RootCanvas.Resources["CanvasTopForRootGrid"]);
        }

        /// <summary>
        /// 刷新方块颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e) {
            /*
             * _rectangleRegisteTable 已在 ResetRectangleColor() 内部重新初始化，
             * 这里无需再次执行 _rectangleRegisteTable = new HashSet<Rectangle>()
             */
            ResetRectangle();
            DrawRectangleColor(_model?.GroupDateTimesByTotal(), true);
            Blink.BlinkedRectangles.Clear();
        }

        /// <summary>
        /// 清空所有记录，重置所有状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e) {
            /*
             * _rectangleRegisteTable 已在 ResetRectangleColor() 内部重新初始化，
             * 这里无需再次执行 _rectangleRegisteTable = new HashSet<Rectangle>()
             */
            ResetRectangle();
            _model = null;
            _file = null;
            _saveMode = SaveMode.NewFile;
            Blink.BlinkedRectangles.Clear();
            RefreshButton.Visibility = Visibility.Collapsed;
            SaveFileButton.Visibility = Visibility.Collapsed;
            ClearButton.Visibility = Visibility.Collapsed;
        }
    }
}
