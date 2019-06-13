using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;

namespace MasturbationRecorder {
    static class ProgressBoard {
        private static Canvas _progressBoard;

        private static Canvas CreateProgressBoard(string name) {
            Canvas canvas = new Canvas() {
                Name = name,
                Width = 100,
                Height = 100,
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.LightGray),
                Opacity = 0.7
            };
            Canvas.SetZIndex(canvas, 2);
            ProgressRing processingRing = new ProgressRing() {
                Width = 80,
                Height = 80,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black),
                IsActive = true
            };
            canvas.Children.Add(processingRing);
            HorizontalCenterOnCanvas(processingRing, canvas);
            VerticalCenterOnCanvas(processingRing, canvas);
            return canvas;
        }

        private static void HorizontalCenterOnCanvas(Canvas subCanvas, Canvas parentCanvas) {
            Canvas.SetLeft(subCanvas, (parentCanvas.Width - subCanvas.Width) / 2);
        }

        private static void HorizontalCenterOnCanvas(Control control, Canvas panel) {
            Canvas.SetLeft(control, (panel.ActualWidth - control.Width) / 2);
        }

        private static void VerticalCenterOnCanvas(Canvas subCanvas, Canvas parentCanvas) {
            Canvas.SetTop(subCanvas, -subCanvas.Height);
        }

        private static void VerticalCenterOnCanvas(Control control, Canvas panel) {
            Canvas.SetTop(control, (panel.ActualHeight - control.Width) / 2);
        }

        /// <summary>
        /// 播放进度条模块动画
        /// </summary>
        /// <param name="parentCanvas">承载进度条模块的容器</param>
        /// <param name="isActive">true 为进度条模块下降，false 为进度条模块上升</param>
        public static void Slide(Canvas parentCanvas, bool isActive) {
            if (ProgressBoard._progressBoard == null) {
                ProgressBoard._progressBoard = ProgressBoard.CreateProgressBoard("ProgressBoard");
            }
            if (parentCanvas.Children.Contains(ProgressBoard._progressBoard) == false) {
                parentCanvas.Children.Add(ProgressBoard._progressBoard);
            }
            ProgressBoard.HorizontalCenterOnCanvas(ProgressBoard._progressBoard, parentCanvas);

            Storyboard startStoryboard = new Storyboard();
            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
            DoubleAnimationUsingKeyFrames top_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();

            if (isActive) {
                top_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                    Value = Canvas.GetTop(ProgressBoard._progressBoard),
                    KeyTime = startTime
                });
                top_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                    Value = (parentCanvas.ActualHeight - ProgressBoard._progressBoard.Height) / 2,
                    KeyTime = endTime
                });
            }
            else {
                top_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                    Value = Canvas.GetTop(ProgressBoard._progressBoard),
                    KeyTime = startTime
                });
                top_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                    Value = -100,
                    KeyTime = endTime
                });
                /*
                 * 把 startStoryboard.Completed 事件的 Handler 作为局部函数能让它的作用域捕获 ProgressBoard._progressBoard,
                 * 从而使得 Completed 事件能够在动画播放结束时为 parentCanvas 移除  ProgressBoard._progressBoard
                 */
                void StartStoryboard_Completed(object sender, object e) {
                    parentCanvas.Children.Remove(ProgressBoard._progressBoard);
                }
                startStoryboard.Completed += StartStoryboard_Completed;
            }
            startStoryboard.Children.Add(top_DoubleAnimationUsingKeyFrames);
            Storyboard.SetTarget(top_DoubleAnimationUsingKeyFrames, ProgressBoard._progressBoard);
            Storyboard.SetTargetName(top_DoubleAnimationUsingKeyFrames, ProgressBoard._progressBoard.Name);
            Storyboard.SetTargetProperty(top_DoubleAnimationUsingKeyFrames, "(Canvas.Top)");
            startStoryboard.Begin();
        }

    }
}
