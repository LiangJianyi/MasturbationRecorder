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
    public sealed partial class RegisterPage : Page {
        private byte[] _fileBytes;

        public RegisterPage() {
            this.InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            // AzureSqlDbHelper.RegisterUser(Account,Password,_fileBytes)
        }

        private async void Avatar_PointerReleasedAsync(object sender, PointerRoutedEventArgs e) {
            var picker = new Windows.Storage.Pickers.FileOpenPicker {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                await StorageFileToBytesAsync(file);
                Avatar.Source = await StorageFileToBitmapImageAsync(file, (int)Avatar.Width, (int)Avatar.Height);
            }
        }
    }
}
