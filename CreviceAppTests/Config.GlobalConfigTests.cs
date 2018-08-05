using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace Crevice4Tests
{
    using Crevice.Config;

    [TestClass()]
    public class GlobalConfigTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestHelpers.TestDirectoryMutex.WaitOne();
            Directory.CreateDirectory(TestHelpers.TemporaryDirectory);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Directory.Delete(TestHelpers.TemporaryDirectory, recursive: true);
            TestHelpers.TestDirectoryMutex.ReleaseMutex();
        }

        [TestMethod()]
        public void DefaultUserDirectoryTest()
        {
            var globalConfig = new GlobalConfig();
            Assert.AreEqual(globalConfig.DefaultUserDirectory.EndsWith("\\Crevice4"), true);
        }

        [TestMethod()]
        public void UserScriptFile0Test()
        {
            var globalConfig = new GlobalConfig();
            Assert.AreEqual(globalConfig.UserScriptFile.EndsWith("\\Crevice4\\default.csx"), true);
        }

        [TestMethod()]
        public void UserScriptFile1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(globalConfig.UserScriptFile.EndsWith("\\Crevice4\\hoge.csx"), true);
        }
        
        [TestMethod()]
        public void UserScriptFile2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(globalConfig.UserScriptFile.Equals("C:\\hoge.csx"), true);
        }

        [TestMethod()]
        public void UserDirectory0Test()
        {
            var globalConfig = new GlobalConfig();
            Assert.AreEqual(globalConfig.UserDirectory.EndsWith("\\Crevice4"), true);
        }

        [TestMethod()]
        public void UserDirectory1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(globalConfig.UserDirectory.EndsWith("\\Crevice4"), true);
        }

        [TestMethod()]
        public void UserDirectory2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(globalConfig.UserDirectory.Equals("C:\\"), true);
        }

        [TestMethod()]
        public void GetOrSetDefaultUserScriptFile0Test()
        {
            var directory = TestHelpers.GetTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(globalConfig.GetOrSetDefaultUserScriptFile("").Length == 0, true);
        }

        [TestMethod()]
        public void GetOrSetDefaultUserScriptFile1Test()
        {
            var directory = TestHelpers.GetTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(globalConfig.GetOrSetDefaultUserScriptFile("hoge").Length > 0, true);
        }

        [TestMethod()]
        public void DisableIDESupportLoadDirectives0Test()
        {
            var directory = TestHelpers.GetTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(globalConfig.GetOrSetDefaultUserScriptFile("#load \"IDESupport"), "//#load \"IDESupport");
        }

        [TestMethod()]
        public void DisableIDESupportLoadDirectives1Test()
        {
            var directory = TestHelpers.GetTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.AreEqual(
                globalConfig.GetOrSetDefaultUserScriptFile("#load \"IDESupport", disableIDESupport: false), "#load \"IDESupport");
        }


        [TestMethod()]
        public void GetUserScriptCacheFileTest()
        {
            var globalConfig = new GlobalConfig();
            Assert.AreEqual(globalConfig.UserScriptCacheFile.EndsWith("Crevice4\\default.csx.cache"), true);
        }
    }
}
