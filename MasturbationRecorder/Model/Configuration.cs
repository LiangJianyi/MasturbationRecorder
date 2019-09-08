using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Storage;
using MasturbationRecorder.CommonTool;
using System.Threading.Tasks;

namespace MasturbationRecorder {
    [Serializable]
    enum Theme {
        Light,
        Dark
    }

    class Configuration {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Theme Theme { get; set; }
        public string Title { get; set; }
        public StorageFile Avatar { get; set; }
        public StorageFile RecordFile { get; set; }

        public Configuration(string username, string password, string title = "", Theme theme = Theme.Light, StorageFile avatar = null, StorageFile record = null) {
            this.UserName = username;
            this.Password = password;
            this.Title = title;
            this.Theme = theme;
            if (avatar != null) {
                this.Avatar = avatar;
            }
            if (record != null) {
                this.RecordFile = record;
            }
        }

        private async Task<SerializationConfiguration> AsSerializationConfigurationAsync()
            => new SerializationConfiguration() {
                UserName = this.UserName,
                Password = this.Password,
                Title = this.Title,
                Theme = this.Theme,
                Avatar = await this.Avatar?.ToBytesAsync(),
                RecordFile = await this.RecordFile?.ToBytesAsync()
            };

        public static byte[] SerializeToBytes(Configuration configuration) {
            //内存实例
            MemoryStream ms = new MemoryStream();
            //创建序列化的实例
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, configuration);//序列化对象，写入内存流中  
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            return bytes;
        }

        public static Configuration DeserializeObject(byte[] bytes) {
            //利用传来的byte[]创建一个内存流
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            Configuration obj = formatter.Deserialize(ms) as Configuration;//把内存流反序列成对象  
            ms.Close();
            return obj;
        }

        public override string ToString() {
            return $"Title: {this.Title}\n" +
                   $"Theme: {this.Theme}\n" +
                   $"Avatar file name: {this.Avatar?.Name}\n" +
                   $"Record file name: {this.RecordFile?.Name}";
        }
    }

    [Serializable]
    class SerializationConfiguration {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Theme Theme { get; set; }
        public string Title { get; set; }
        public byte[] Avatar { get; set; }
        public byte[] RecordFile { get; set; }

        public static byte[] SerializeToBytes(Configuration configuration) {
            //内存实例
            MemoryStream ms = new MemoryStream();
            //创建序列化的实例
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, configuration);//序列化对象，写入内存流中  
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            return bytes;
        }

        public static Configuration DeserializeObject(byte[] bytes) {
            //利用传来的byte[]创建一个内存流
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            Configuration obj = formatter.Deserialize(ms) as Configuration;//把内存流反序列成对象  
            ms.Close();
            return obj;
        }
    }
}
