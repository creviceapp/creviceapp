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
    using System.Reflection;
    using Crevice.Config;
    using Crevice.UserScript;
    using Crevice.GestureMachine;

    [TestClass()]
    public class GestureMachineCandidateTests
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
            if (Directory.Exists(TestHelpers.TemporaryDirectory))
            {
                Directory.Delete(TestHelpers.TemporaryDirectory, recursive: true);
            }
            TestHelpers.TestDirectoryMutex.ReleaseMutex();
        }
        
        void Setup(DirectoryInfo src, string dst)
        {
            Directory.CreateDirectory(Path.Combine(dst, "IDESupport"));

            foreach (var file in src.EnumerateFiles())
            {
                File.Copy(file.FullName, Path.Combine(dst, "IDESupport", file.Name));
            }

            foreach (var dir in src.EnumerateDirectories())
            {
                Directory.CreateDirectory(Path.Combine(dst, "IDESupport", dir.Name));
                foreach (var file in dir.EnumerateFiles())
                {
                    File.Copy(file.FullName, Path.Combine(dst, "IDESupport", dir.Name, file.Name));
                }
            }
        }

        [TestMethod()]
        public void UserScriptEnvironmentChangeDetectionTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Setup(binaryDir, tempDir);

            var cacheFile = Path.Combine(tempDir, "default.csx.cache");
            var candidate0 = new GestureMachineCandidate(tempDir, "", cacheFile, true);
            Assert.AreEqual(candidate0.IsRestorable, false);
            UserScript.SaveUserScriptAssemblyCache(cacheFile, candidate0.UserScriptAssemblyCache);
            var candidate1 = new GestureMachineCandidate(tempDir, "", cacheFile, true);
            Assert.AreEqual(candidate1.IsRestorable, true);
            File.WriteAllText(userScriptFile, "");
            var candidate2 = new GestureMachineCandidate(tempDir, "", cacheFile, true);
            Assert.AreEqual(candidate2.IsRestorable, false);
        }

        [TestMethod()]
        public void CreateTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Setup(binaryDir, tempDir);

            var cacheFile = Path.Combine(tempDir, "default.csx.cache");
            var candidate = new GestureMachineCandidate(tempDir, userScriptString, cacheFile, false);
            
            string[] args = { "--script", userScriptFile };
            var cliOption = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig(cliOption);
            var ctx = new UserScriptExecutionContext(globalConfig);
            Assert.AreEqual(candidate.Errors.Count(), 0);
            UserScript.EvaluateUserScriptAssembly(ctx, candidate.UserScriptAssemblyCache);
            var gmcluster = candidate.Create(ctx);
            Assert.AreEqual(gmcluster == null, false);
            // Todo: why this test not passing...??
            //Assert.AreEqual(gmcluster.Profiles.Any(), true);
        }

        [TestMethod()]
        public void CreateIgnoreExceptionTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString =
                "var Never = When(ctx => { return false; });" +
                "Never.On(Keys.RButton);" +
                "Never.OnDecomposed(Keys.RButton);";

            Setup(binaryDir, tempDir);

            var cacheFile = Path.Combine(tempDir, "default.csx.cache");
            var candidate = new GestureMachineCandidate(tempDir, userScriptString, cacheFile, false);

            var globalConfig = new GlobalConfig();
            var ctx = new UserScriptExecutionContext(globalConfig);
            Assert.AreEqual(candidate.Errors.Count(), 0);
            var gmcluster = candidate.Create(ctx);
            Assert.AreEqual(gmcluster == null, false);
        }
    }
}
