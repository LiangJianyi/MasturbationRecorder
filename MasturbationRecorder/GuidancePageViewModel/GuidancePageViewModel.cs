using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MasturbationRecorder {
    static class GuidancePageViewModel {
        public static void OpenLoginProgressRing(StackPanel stackPanel) {
            // 移除“登录”和“注册”两个按钮
            stackPanel.Children.RemoveAt(0);
            stackPanel.Children.RemoveAt(1);
            // 添加进度条
            stackPanel.Children.Add(new ProgressRing() {
                FontSize = 25,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                IsActive = true,
                Margin = new Thickness(25)
            });
        }

        public static void CloseLoginProgressRing(StackPanel stackPanel, RoutedEventHandler loginClickEvent = null, RoutedEventHandler registerClickEvent = null) {
            // 移除进度条
            stackPanel.Children.RemoveAt(0);
            // 添加“登录”和“注册”两个按钮
            Button loginButton = new Button() {
                Content = "登录",
                Margin = new Thickness(0, 0, 20, 0)
            };
            loginButton.Click += loginClickEvent;
            Button registerButton = new Button() {
                Content = "注册",
                Margin = new Thickness(20, 0, 0, 0)
            };
            registerButton.Click += registerClickEvent;

            stackPanel.Children.Add(loginButton);
            stackPanel.Children.Add(registerButton);
        }
    }
}
