using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasturbationRecorder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasturbationRecorderTest {
    [TestClass]
    public class ConfigurationTest {
        [TestMethod]
        public async Task SerializeToBytesAsyncTest() {
            Configuration configuration = await CommonTestResource.GenerateConfigurationAsync();
            byte[] bytes = await Configuration.SerializeToBytesAsync(configuration);
            configuration = await Configuration.DeserializeObjectAsync(bytes);
            string acutalname = configuration.Avatar.Name;
            Assert.AreEqual<string>("wangyuyan.png", configuration.Avatar.Name);
        }

        [TestMethod]
        public async Task FuckTestAsync() => await Task.Factory.StartNew(()=> Assert.AreEqual(1, 1));
    }
}
