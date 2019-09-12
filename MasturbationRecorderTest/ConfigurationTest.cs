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
        public async void SerializeToBytesAsyncTest() {
            Configuration configuration = await CommonTestResource.GenerateConfigurationAsync();
            var bytes = await Configuration.SerializeToBytesAsync(configuration);
            configuration = await Configuration.DeserializeObjectAsync(bytes);
            Assert.AreEqual<string>("wangyuyan.png", configuration.Avatar.Name);
        }

        [TestMethod]
        public void FuckTest() => Assert.AreEqual(1, 1);
    }
}
