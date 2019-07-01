using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
            var leftTaggingRectangles = from rect in res1
                                        where DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == System.DayOfWeek.Monday ||
                                              DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == System.DayOfWeek.Wednesday ||
                                              DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == System.DayOfWeek.Friday
                                        select rect;
            foreach (Rectangle rect in leftTaggingRectangles) {
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
    }
}
