﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MasturbationRecorder.SqlDbHelper;
using System.Threading.Tasks;

namespace MasturbationRecorder {
    using Debug = System.Diagnostics.Debug;

    public sealed partial class GuidancePage : Page {
        public GuidancePage() {
            this.InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(AccountTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password)) {
                PopErrorDialogAsync("账户和密码不能为空");
            }
            else if (string.IsNullOrWhiteSpace(AccountTextBox.Text) ||
                     string.IsNullOrWhiteSpace(PasswordBox.Password)) {
                PopErrorDialogAsync("账户和密码不能包含空格");
            }
            else {
                Configuration configuration = new Configuration(
                    username: AccountTextBox.Text,
                    password: PasswordBox.Password,
                    title: TitleBox.Text,
                    theme: Theme.Light
                );

                if (await AzureSqlDbHelper.LoginAsync(configuration)) {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage), configuration);
                }
                else {
                    PopErrorDialogAsync("账号或密码错误");
                }
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

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            AccountTextBox.Width = TitleBox.ActualWidth;
            PasswordBox.Width = TitleBox.ActualWidth;
            Debug.WriteLine(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
        }

        private void Register_Click(object sender, RoutedEventArgs e) {

        }
    }
}
