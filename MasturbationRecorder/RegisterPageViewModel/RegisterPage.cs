using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MasturbationRecorder {
    public sealed partial class RegisterPage {
        public async Task StorageFileToBytesAsync(StorageFile file) {
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync()) {
                _fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream)) {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(_fileBytes);
                }
            }
        }

        /// <summary>
        /// 从流中提取照片设置传递进去的 Image 控件
        /// </summary>
        /// <param name="imageControl">要设置照片的控件</param>
        /// <param name="file">接收一个文件</param>
        /// <param name="decodePixelWidth">照片的宽度</param>
        /// <param name="decodePixelHeight">照片的高度</param>
        /// <remarks>
        /// 如果字节解码失败，会抛出一个异常：System.Exception: 'The component cannot be found. (Exception from HRESULT: 0x88982F50)'
        /// 解码失败通常由两种原因：
        /// 1、字节码原本是由非图像数据转换而成；
        /// 2、字节码由一张遭遇损坏的图像文件转换而成；
        /// </remarks>
        public async Task<BitmapImage> StorageFileToBitmapImageAsync(StorageFile file, int decodePixelWidth, int decodePixelHeight, Windows.UI.Xaml.RoutedEventHandler imageOpenedEvent = null) {
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read)) {
                BitmapImage bitmapImage = new BitmapImage {
                    DecodePixelHeight = decodePixelHeight,
                    DecodePixelWidth = decodePixelWidth
                };
                if (imageOpenedEvent != null) {
                    bitmapImage.ImageOpened += imageOpenedEvent;
                }
                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
            }
        }

        /// <summary>
        /// 弹出错误提示
        /// </summary>
        /// <param name="content"></param>
        public async Task PopErrorDialogAsync(string content) {
            ContentDialog fileOpenFailDialog = new ContentDialog {
                Title = "Error",
                Content = content,
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await fileOpenFailDialog.ShowAsync();
        }

    }
}
