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

    [TestClass()]
    public class ScriptsTests
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
        
        void Setup(DirectoryInfo src, string dst)
        {
            var userScriptFile = Path.Combine(dst, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(src.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

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
        public void DefaultUserScriptTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Setup(binaryDir, tempDir);
            
            string[] args = { "-s", userScriptFile };
            var result = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig();
            var appEnvUserScriptString = globalConfig.GetOrSetDefaultUserScriptFile(userScriptString);
            var parsedScript = UserScript.ParseScript(appEnvUserScriptString, tempDir, tempDir);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.AreEqual(errors.Count() == 0, true);
            var cache = UserScript.GenerateUserScriptAssemblyCache(userScriptString, parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScriptAssembly(ctx, cache);
            Assert.AreEqual(ctx.Profiles[0].RootElement.GestureCount > 0, true);
        }

        [TestMethod()]
        public void DefaultUserScriptErrorTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Setup(binaryDir, tempDir);

            File.WriteAllText(userScriptFile, "hogehoge");

            string[] args = { "-s", userScriptFile };
            var result = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig();
            var appEnvUserScriptString = globalConfig.GetOrSetDefaultUserScriptFile(userScriptString);
            var parsedScript = UserScript.ParseScript(appEnvUserScriptString, tempDir, tempDir);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.AreEqual(errors.Count() == 0, true);
            var cache = UserScript.GenerateUserScriptAssemblyCache(userScriptString, parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScriptAssembly(ctx, cache);
            Assert.AreEqual(ctx.Profiles[0].RootElement.GestureCount > 0, true);
        }

        [TestMethod()]
        public void MockEnvTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Setup(binaryDir, tempDir);

            string[] args = { "-s", userScriptFile };
            var result = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig();
            var parsedScript = UserScript.ParseScript(userScriptString, tempDir, tempDir);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.AreEqual(errors.Count() == 0, true);
            var cache = UserScript.GenerateUserScriptAssemblyCache(userScriptString, parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScriptAssembly(ctx, cache);
            Assert.AreEqual(ctx.Profiles.Count == 0, true);
        }

        [TestMethod()]
        public void MockEnvErrorTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Setup(binaryDir, tempDir);

            var mockFile = Path.Combine(tempDir, "IDESupport", "Scripts", "MockEnv.csx");
            File.WriteAllText(mockFile, "hogehoge");

            string[] args = { "-s", userScriptFile };
            var result = CLIOption.Parse(args);
            var globalConfig = new GlobalConfig();
            var parsedScript = UserScript.ParseScript(userScriptString, tempDir, tempDir);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.AreEqual(errors.Count() == 1, true);
        }
    }
}
