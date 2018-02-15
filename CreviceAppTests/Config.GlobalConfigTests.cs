using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace CreviceTests
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
            Assert.IsTrue(globalConfig.DefaultUserDirectory.EndsWith("\\AppData\\Roaming\\Crevice4"));
        }

        [TestMethod()]
        public void UserScriptFile0Test()
        {
            var globalConfig = new GlobalConfig();
            Assert.IsTrue(globalConfig.UserScriptFile.EndsWith("\\AppData\\Roaming\\Crevice4\\default.csx"));
        }

        [TestMethod()]
        public void UserScriptFile1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.IsTrue(globalConfig.UserScriptFile.EndsWith("\\AppData\\Roaming\\Crevice4\\hoge.csx"));
        }
        
        [TestMethod()]
        public void UserScriptFile2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.IsTrue(globalConfig.UserScriptFile.Equals("C:\\hoge.csx"));
        }

        [TestMethod()]
        public void UserDirectory0Test()
        {
            var globalConfig = new GlobalConfig();
            Assert.IsTrue(globalConfig.UserDirectory.EndsWith("\\AppData\\Roaming\\Crevice4"));
        }

        [TestMethod()]
        public void UserDirectory1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.IsTrue(globalConfig.UserDirectory.EndsWith("\\AppData\\Roaming\\Crevice4"));
        }

        [TestMethod()]
        public void UserDirectory2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.IsTrue(globalConfig.UserDirectory.Equals("C:\\"));
        }

        [TestMethod()]
        public void GetOrSetDefaultUserScriptFile0Test()
        {
            var directory = TestHelpers.GetTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.IsTrue(globalConfig.GetOrSetDefaultUserScriptFile("").Length == 0);
        }

        [TestMethod()]
        public void GetOrSetDefaultUserScriptFile1Test()
        {
            var directory = TestHelpers.GetTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            Assert.IsTrue(globalConfig.GetOrSetDefaultUserScriptFile("hoge").Length > 0);
        }


        [TestMethod()]
        public void GetUserScriptCacheFileTest()
        {
            var globalConfig = new GlobalConfig();
            Assert.IsTrue(globalConfig.UserScriptCacheFile.EndsWith("\\AppData\\Roaming\\Crevice4\\default.csx.cache"));
        }
    }
}
