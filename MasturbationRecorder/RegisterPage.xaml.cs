using MasturbationRecorder.SqlDbHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MasturbationRecorder {
    using Windows.Storage;
    using Debug = System.Diagnostics.Debug;

    public sealed partial class RegisterPage : Page {
        private byte[] _fileBytes;
        private Theme _theme;
        private StorageFile _file;

        public RegisterPage() {
            this.InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            UpdateRegisterPageLayout();
        }

        private async void RegisterButton_ClickAsync(object sender, RoutedEventArgs e) {
            if (await AzureSqlDbHelper.RegisterUserAsync(AccountTextBox.Text, PasswordBox.Password, _fileBytes) > 0) {
                Configuration configuration = new Configuration(
                    username: AccountTextBox.Text,
                    password: PasswordBox.Password,
                    title: TitleBox.Text,
                    theme: this._theme,
                    avatar: _file
                );
                if (await AzureSqlDbHelper.LoginAsync(configuration)) {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage));
                }
            }
            else {
                await PopErrorDialogAsync("注册发生错误！");
            }
        }

        private async void Avatar_PointerReleasedAsync(object sender, PointerRoutedEventArgs e) {
            var picker = new Windows.Storage.Pickers.FileOpenPicker {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            _file = await picker.PickSingleFileAsync();
            if (_file != null) {
                await StorageFileToBytesAsync(_file);
                Avatar.Source = await StorageFileToBitmapImageAsync(_file, (int)Avatar.Width, (int)Avatar.Height);
            }
        }

        private void LightRadioButton_Checked(object sender, RoutedEventArgs e) {
            this._theme = Theme.Light;
        }

        private void DarkRadioButton_Checked(object sender, RoutedEventArgs e) {
            this._theme = Theme.Dark;
        }
    }
}
