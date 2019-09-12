using MasturbationRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace MasturbationRecorderTest {
    static class CommonTestResource {
        /// <summary>
        /// 获取指定路径下的文本文件的内容
        /// </summary>
        /// <param name="path">路径字符串</param>
        /// <returns>返回一个字符串数组，每行文本代表一个string</returns>
        private static string[] GetTextFile(string path) {
            List<string> textList = new List<string>();
            using (System.IO.StreamReader reader = System.IO.File.OpenText(path)) {
                for (string s = reader.ReadLine(); s != null; s = reader.ReadLine()) {
                    textList.Add(s);
                }
            }
            return textList.ToArray();
        }

        /// <summary>
        /// 获取测试文件
        /// </summary>
        public static string[] TestTextLine => GetTextFile(@"C:\Users\a124p\Documents\Test Data\masturbation test.txt");

        /// <summary>
        /// 生成一个 Configuration
        /// </summary>
        public async static Task<Configuration> GenerateConfigurationAsync() =>
            new Configuration(
                username: KeyGenerator.GetUniqueKey(10),
                password: KeyGenerator.GetUniqueKey(256),
                title: KeyGenerator.GetUniqueKey(12),
                theme: Theme.Light,
                avatar: await KnownFolders.PicturesLibrary.GetFileAsync("wangyuyan.png")
            );
    }
}
