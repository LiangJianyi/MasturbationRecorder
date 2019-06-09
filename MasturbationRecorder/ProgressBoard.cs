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
        private static readonly Canvas _processingCanvas = new Canvas() { Name = "ProcessingCanvas" };

        public static void CreateProgessBoard(Panel parent, EventHandler<object> storyboard_Completed) {
            _processingCanvas.Children.Add(_processingRing);
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
            DoubleAnimationUsingKeyFrames width_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = true };
            width_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetTop(_processingCanvas),
                KeyTime = startTime
            });
            width_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetTop(_processingCanvas) + _processingCanvas.ActualHeight,
                KeyTime = endTime
            });
            storyboard.Children.Add(width_DoubleAnimationUsingKeyFrames);
            Storyboard.SetTarget(width_DoubleAnimationUsingKeyFrames, _processingCanvas);
            Storyboard.SetTargetName(width_DoubleAnimationUsingKeyFrames, _processingCanvas.Name);
            Storyboard.SetTargetProperty(width_DoubleAnimationUsingKeyFrames, "(Canvas.Top)");
        }
    }
}
