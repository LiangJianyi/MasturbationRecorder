using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MasturbationRecorder {
    static class ProgressBoard {
        static ProgressBoard() {
            _processingCanvas.Children.Add(_processingRing);
        }

        private static readonly ProgressRing _processingRing = new ProgressRing() {
            Width = 30,
            Height = 30,
            FontSize = 15,
            Foreground = new SolidColorBrush(Windows.UI.Colors.Blue),
            FontStretch = Windows.UI.Text.FontStretch.SemiCondensed,
            Margin = new Windows.UI.Xaml.Thickness(100, 100, 100, 100)
        };
        private static readonly Canvas _processingCanvas = new Canvas() { Name = "ProcessingCanvas" };

        public static void OpenProgessBoard(Panel parent, EventHandler<object> storyboard_Completed) {
            parent.Children.Add(_processingCanvas);
            Canvas.SetLeft(_processingCanvas, (parent.ActualWidth - _processingCanvas.ActualWidth) / 2);
            Canvas.SetTop(_processingCanvas, -1);
            Canvas.SetZIndex(_processingCanvas, 2);

            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));

            Storyboard storyboard = new Storyboard();
            if (storyboard_Completed != null) {
                storyboard.Completed += storyboard_Completed;
            }
            DoubleAnimationUsingKeyFrames canvasTop_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = true };
            canvasTop_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetTop(_processingCanvas),
                KeyTime = startTime
            });
            canvasTop_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetTop(_processingCanvas) + _processingCanvas.ActualHeight,
                KeyTime = endTime
            });
            storyboard.Children.Add(canvasTop_DoubleAnimationUsingKeyFrames);
            Storyboard.SetTarget(canvasTop_DoubleAnimationUsingKeyFrames, _processingCanvas);
            Storyboard.SetTargetName(canvasTop_DoubleAnimationUsingKeyFrames, _processingCanvas.Name);
            Storyboard.SetTargetProperty(canvasTop_DoubleAnimationUsingKeyFrames, "(Canvas.Top)");
            _processingRing.IsActive = true;
            storyboard.Begin();
        }

        public static void CloseProgessBoard(Panel parent) {
            _processingRing.IsActive = false;
            parent.Children.Remove(_processingCanvas);
        }
    }
}
