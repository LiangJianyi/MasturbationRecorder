﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;

namespace MasturbationRecorder {
    class ProgressBoard {
        private Canvas _progressBoard;

        private Canvas CreateProgressBoard(string name) {
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

        private void HorizontalCenterOnCanvas(Canvas subCanvas, Canvas parentCanvas) {
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
        public void SlideOn(Canvas parentCanvas) {
            _progressBoard = CreateProgressBoard("ProgressBoard");
            if (parentCanvas.Children.Contains(_progressBoard) == false) {
                parentCanvas.Children.Add(_progressBoard);
            }
            HorizontalCenterOnCanvas(_progressBoard, parentCanvas);

            Storyboard startStoryboard = new Storyboard();
            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
            DoubleAnimationUsingKeyFrames slideAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();

            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetTop(_progressBoard),
                KeyTime = startTime
            });
            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = (parentCanvas.ActualHeight - _progressBoard.Height) / 2,
                KeyTime = endTime
            });

            /*
             * 把 startStoryboard.Completed 事件的 Handler 作为局部函数能让它的作用域捕获 ProgressBoard._progressBoard,
             * 从而使得 Completed 事件能够在动画播放结束时为 parentCanvas 移除  ProgressBoard._progressBoard
             */
            void StartStoryboard_Completed(object sender, object e) {
                parentCanvas.Children.Remove(_progressBoard);
            }
            startStoryboard.Completed += StartStoryboard_Completed;
            startStoryboard.Children.Add(slideAnimationUsingKeyFrames);
            Storyboard.SetTarget(slideAnimationUsingKeyFrames, _progressBoard);
            Storyboard.SetTargetName(slideAnimationUsingKeyFrames, _progressBoard.Name);
            Storyboard.SetTargetProperty(slideAnimationUsingKeyFrames, "(Canvas.Top)");
            startStoryboard.Begin();
        }

    }
}
