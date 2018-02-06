using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace CreviceApp
{
    using App;

    [TestClass()]
    public class UserScriptTests
    {
        [ClassInitialize()]
        public static void CreateApplicationDirectory(TestContext context)
        {
            var appConfig = new AppConfig();
            Directory.CreateDirectory(appConfig.DefaultUserDirectory);
        }

        private static string GetBaseTemporaryDirectory()
        {
            return Path.Combine(Path.GetTempPath(), "Crevice4Test");
        }

        private static string CreateTemporaryTestDirectory([CallerMemberName] string memberName = "")
        {
            var randomString = Guid.NewGuid().ToString("N");
            var directory = Path.Combine(GetBaseTemporaryDirectory(), randomString, memberName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        [ClassCleanup]
        public static void CleanupDirectory()
        {
            Directory.Delete(GetBaseTemporaryDirectory(), recursive: true);
        }

        [TestMethod()]
        public void DefaultUserDirectoryTest()
        {
            var appConfig = new AppConfig();
            Assert.IsTrue(appConfig.DefaultUserDirectory.EndsWith("\\AppData\\Roaming\\Crevice4"));
        }

        [TestMethod()]
        public void UserScriptFile0Test()
        {
            var appConfig = new AppConfig();
            Assert.IsTrue(appConfig.UserScriptFile.EndsWith("\\AppData\\Roaming\\Crevice4\\default.csx"));
        }

        [TestMethod()]
        public void UserScriptFile1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var appConfig = new AppConfig(cliOption);
            Assert.IsTrue(appConfig.UserScriptFile.EndsWith("\\AppData\\Roaming\\Crevice4\\hoge.csx"));
        }
        
        [TestMethod()]
        public void UserScriptFile2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var appConfig = new AppConfig(cliOption);
            Assert.IsTrue(appConfig.UserScriptFile.Equals("C:\\hoge.csx"));
        }

        [TestMethod()]
        public void UserDirectory0Test()
        {
            var appConfig = new AppConfig();
            Assert.IsTrue(appConfig.UserDirectory.EndsWith("\\AppData\\Roaming\\Crevice4"));
        }

        [TestMethod()]
        public void UserDirectory1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var appConfig = new AppConfig(cliOption);
            Assert.IsTrue(appConfig.UserDirectory.EndsWith("\\AppData\\Roaming\\Crevice4"));
        }

        [TestMethod()]
        public void UserDirectory2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var appConfig = new AppConfig(cliOption);
            Assert.IsTrue(appConfig.UserDirectory.Equals("C:\\"));
        }

        [TestMethod()]
        public void GetOrSetDefaultUserScriptFile0Test()
        {
            var directory = CreateTemporaryTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var appConfig = new AppConfig(cliOption);
            Assert.IsTrue(appConfig.GetOrSetDefaultUserScriptFile("").Length == 0);
        }

        [TestMethod()]
        public void GetOrSetDefaultUserScriptFile1Test()
        {
            var directory = CreateTemporaryTestDirectory();
            string[] args = { "--script", Path.Combine(directory, "test.csx") };
            var cliOption = CLIOption.Parse(args);
            var appConfig = new AppConfig(cliOption);
            Assert.IsTrue(appConfig.GetOrSetDefaultUserScriptFile("hoge").Length > 0);
        }


        [TestMethod()]
        public void GetUserScriptCacheFileTest()
        {
            var appConfig = new AppConfig();
            Assert.IsTrue(appConfig.UserScriptCacheFile.EndsWith("\\AppData\\Roaming\\Crevice4\\default.csx.cache"));
        }

        [TestMethod()]
        public void ParseScriptTest()
        {
            var appConfig = new AppConfig();
            var userScriptString = "dummy_script";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            Assert.AreEqual(parsedScript.Code, userScriptString);
            Assert.AreEqual(parsedScript.GlobalsType.Name, "UserScriptExecutionContext");
            Assert.AreEqual(parsedScript.GlobalsType.FullName, "CreviceApp.Core.UserScriptExecutionContext");
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("mscorlib.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("System.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("System.Core.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("Crevice4.exe")));
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_0Test()
        {
            var appConfig = new AppConfig();
            var userScriptString = "var hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_1Test()
        {
            // When the user script containing dynamic expression is given.
            var appConfig = new AppConfig();
            var userScriptString = "dynamic hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_2Test()
        {
            // When the user script which defines a mouse gesture is given.
            var appConfig = new AppConfig();
            var userScriptString = "var Whenever = When((ctx) => {return true;});";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_3Test()
        {
            // When the user script which using CreviceApp API is given.
            var appConfig = new AppConfig();
            var userScriptString = "using static CreviceApp.WinAPI.Constants.WindowsMessages;";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_4Test()
        {
            // When the user script containing a error is given.
            var appConfig = new AppConfig();
            var userScriptString = "undefined_variable";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() > 0);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_0Test()
        {
            var appConfig = new AppConfig();
            var userScriptString = "var hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_1Test()
        {
            // When the user script containing dynamic expression is given.
            var appConfig = new AppConfig();
            var userScriptString = "dynamic hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_2Test()
        {
            // When the user script which defines a mouse gesture is given.
            var appConfig = new AppConfig();
            var userScriptString = "var Whenever = When((ctx) => {return true;});";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_3Test()
        {
            // When the user script which using CreviceApp API is given.
            var appConfig = new AppConfig();
            var userScriptString = "using static CreviceApp.WinAPI.Constants.WindowsMessages;";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        [ExpectedException(typeof(Microsoft.CodeAnalysis.Scripting.CompilationErrorException))]
        public void CompileAndEvaluateUserScript1_4Test()
        {
            // When the user script containing a error is given.
            var appConfig = new AppConfig();
            var userScriptString = "undefined_variable";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new Core.UserScriptExecutionContext(appConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        public class SynchronizeInvokeMock : System.ComponentModel.ISynchronizeInvoke
        {
            public bool InvokeRequired
            {
                get { return true;  }
            }

            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                return Task.Run(() => {
                    method.DynamicInvoke(args);
                });
            }

            public object EndInvoke(IAsyncResult result)
            {
                return Task.Run(() => {});
            }

            public object Invoke(Delegate method, object[] args)
            {
                return new object();
            }
        }
        
        [TestMethod()]
        public void WatcherTest()
        {
            var tempDir = CreateTemporaryTestDirectory();
            Console.WriteLine(tempDir);
            var mock = new SynchronizeInvokeMock();
            var watcher = new UserScript.DirectoryWatcher(mock, tempDir, "*.csx");
            var watcherTestfile = Path.Combine(tempDir, "watcher_test.csx");
            var countdownEvent = new CountdownEvent(1);
            watcher.Created += new FileSystemEventHandler((s, e) =>
            {
                countdownEvent.Signal();
            });
            watcher.EnableRaisingEvents = true;
            Task.Run(() => 
            {
                File.WriteAllText(watcherTestfile, "watcher test text");
            });
            countdownEvent.Wait(TimeSpan.FromSeconds(4));
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        [TestMethod()]
        public void GetPrettyTextTest()
        {
            var appConfig = new AppConfig();
            var userScriptString = @"
var hoge = 1;

void foo() 
{ 
  undefined_variable
}

";
            var parsedScript = UserScript.ParseScript(userScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() > 0);
            var prettyText = UserScript.GetPrettyErrorMessage(errors);

            Assert.AreEqual(prettyText, @"--------------------------------------------------------------------------------
[Compiler Error] CS1002

; expected

Line 5, Pos 20
==========
0|
1|var hoge = 1;
2|
3|void foo() 
4|{ 
5|  undefined_variable
                      ~
6|}
7|
8|
[EOF]
==========
--------------------------------------------------------------------------------
");
        }
    }
}
