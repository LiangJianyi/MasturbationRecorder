using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    /// <summary>
    /// 控制 MainPage.xaml 的视图逻辑
    /// </summary>
    static class MainPageViewModel {
        internal static Windows.UI.Color LightGray => new Windows.UI.Color() { A = 255, R = 214, G = 218, B = 215 };
        internal static Windows.UI.Color OneLevelColor => new Windows.UI.Color() { A = 255, R = 203, G = 229, B = 146 };
        internal static Windows.UI.Color TwoLevelColor => new Windows.UI.Color() { A = 255, R = 142, G = 230, B = 107 };
        internal static Windows.UI.Color ThreeLevelColor => new Windows.UI.Color() { A = 255, R = 78, G = 154, B = 67 };
        internal static Windows.UI.Color FourLevelColor => new Windows.UI.Color() { A = 255, R = 11, G = 110, B = 0 };
        internal static Windows.UI.Color FiveLevelColor => new Windows.UI.Color() { A = 255, R = 0, G = 58, B = 6 };

        public static IDictionary<int, SolidColorBrush> ClassifyColorByLevelScore(int groups) {
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
        public static void DateTag(Canvas canvas) {
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

        /// <summary>
        /// 从流中提取照片设置传递进去的 Image 控件
        /// </summary>
        /// <param name="imageControl">要设置照片的控件</param>
        /// <param name="file">文件流</param>
        /// <param name="decodePixelWidth">照片的宽度</param>
        /// <param name="decodePixelHeight">照片的高度</param>
        public static async Task LoadImageFromStreamAsync(Image imageControl, StorageFile file, int decodePixelWidth, int decodePixelHeight) {
            System.Diagnostics.Debug.WriteLine("Invoking LoadImageFromStreamAsync...");
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read)) {
                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage {
                    DecodePixelHeight = decodePixelHeight,
                    DecodePixelWidth = decodePixelWidth
                };
                await bitmapImage.SetSourceAsync(fileStream);
                imageControl.Source = bitmapImage;
            }
        }

        /// <summary>
        /// 给 Configuration.Avatar 设置默认头像
        /// </summary>
        /// <param name="res">用于设置 Avatar 属性的 Configuration 实例</param>
        public static async Task GetDefaultAvatarForConfigurationAsync(Configuration res) {
            System.Diagnostics.Debug.WriteLine("Invoking GetDefaultAvatarForConfigurationAsync...");
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assetsFolder = await installFolder.GetFolderAsync("Assets");
            res.Avatar = await assetsFolder.GetFileAsync("avatar_icon.png");
        }

        public static async Task GetAvatarAsync(Image image, int decodePixelWidth, int decodePixelHeight) {
            System.Diagnostics.Debug.WriteLine("Invoking GetAvatarAsync...");
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assetsFolder = await installFolder.GetFolderAsync("Assets");
            StorageFile avatar = await assetsFolder.GetFileAsync("avatar_icon.png");
            using (IRandomAccessStream fileStream = await avatar.OpenAsync(FileAccessMode.Read)) {
                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage {
                    DecodePixelHeight = decodePixelHeight,
                    DecodePixelWidth = decodePixelWidth
                };

                await bitmapImage.SetSourceAsync(fileStream);
                image.Source = bitmapImage;
            }
        }

        public static async Task GetAvatarAsync(Image image, Configuration configuration, int decodePixelWidth, int decodePixelHeight) {
            System.Diagnostics.Debug.WriteLine("Invoking GetAvatarAsync...");
            using (IRandomAccessStream fileStream = await configuration.Avatar.OpenAsync(FileAccessMode.Read)) {
                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage {
                    DecodePixelHeight = decodePixelHeight,
                    DecodePixelWidth = decodePixelWidth
                };

                await bitmapImage.SetSourceAsync(fileStream);
                image.Source = bitmapImage;
            }
        }

        /// <summary>
        /// 将字节数组转换为 StorageFile
        /// </summary>
        /// <param name="byteArray">接收一个字节数组</param>
        /// <param name="fileName">要创建的 StorageFile 名称</param>
        /// <returns></returns>
        /// <remarks>https://social.msdn.microsoft.com/Forums/en-US/3c70c644-df5d-419f-9d19-55a9414c36dd/uwp-how-to-covert-back-byte-array-to-storage-file-c?forum=wpdevelop</remarks>
        public static async Task<StorageFile> AsStorageFile(this byte[] byteArray, string fileName) {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, byteArray);
            return file;
        }
    }
}
