using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    enum SaveMode {
        NewFile,
        OrginalFile
    }
    static class SaveFileForm {
        /// <summary>
        /// 修改原始文件
        /// </summary>
        /// <param name="parent">承载表单的父级容器</param>
        public static void Open(Canvas parent) {
            Grid rectangle = new Grid() {
                Name = "SaveFileForm",
                Width = parent.ActualWidth * 0.8,
                Height = parent.ActualHeight * 0.8,
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.BlueViolet)
            };
        }
        /// <summary>
        /// 作为新文件保存
        /// </summary>
        /// <param name="parent">承载表单的父级容器</param>
        /// <param name="fileName">文件名</param>
        public static void Open(Canvas parent, string fileName) {

        }
    }
}
