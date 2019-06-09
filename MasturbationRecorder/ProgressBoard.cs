using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MasturbationRecorder {
    class ProgressBoard {
        private static readonly ProgressRing _processingRing = new ProgressRing() {
            Width = 30,
            Height = 30,
            FontSize = 15,
            Foreground = new SolidColorBrush(Windows.UI.Colors.Blue),
            //FontStretch=new Windows.UI.Text.FontStretch.
            Margin = new Windows.UI.Xaml.Thickness(100, 100, 100, 100)
        };
        private static readonly Canvas _processingCanvas = new Canvas();
        
        public static Canvas CreateProgessBoard(EventHandler<object> storyboard_Completed) {
            _processingCanvas.Children.Add(_processingRing);
            Storyboard storyboard = new Storyboard();
            if (storyboard_Completed != null) {
                storyboard.Completed += storyboard_Completed;
            }
            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
            return _processingCanvas;
        }
    }
}
