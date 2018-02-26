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
    using Crevice.UserScript;

    [TestClass()]
    public class UserScriptTests
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
        public void ParseScriptTest()
        {
            var globalConfig = new GlobalConfig();
            var userScriptString = "dummy_script";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            Assert.AreEqual(parsedScript.Code, userScriptString);
            Assert.AreEqual(parsedScript.GlobalsType.Name, "UserScriptExecutionContext");
            Assert.AreEqual(parsedScript.GlobalsType.FullName, "Crevice.UserScript.UserScriptExecutionContext");
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("mscorlib.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("System.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("System.Core.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("crevice4.exe")));
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_0Test()
        {
            var globalConfig = new GlobalConfig();
            var userScriptString = "var hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_1Test()
        {
            // When the user script containing dynamic expression is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "dynamic hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_2Test()
        {
            // When the user script which defines a mouse gesture is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "var Whenever = When((ctx) => {return true;});";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_3Test()
        {
            // When the user script which using CreviceApp API is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "using static Crevice.WinAPI.Constants.WindowsMessages;";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_4Test()
        {
            // When the user script containing a error is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "undefined_variable";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var errors = UserScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() > 0);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_0Test()
        {
            var globalConfig = new GlobalConfig();
            var userScriptString = "var hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_1Test()
        {
            // When the user script containing dynamic expression is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "dynamic hoge = 1;";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_2Test()
        {
            // When the user script which defines a mouse gesture is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "var Whenever = When((ctx) => {return true;});";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_3Test()
        {
            // When the user script which using CreviceApp API is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "using static Crevice.WinAPI.Constants.WindowsMessages;";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        [ExpectedException(typeof(Microsoft.CodeAnalysis.Scripting.CompilationErrorException))]
        public void CompileAndEvaluateUserScript1_4Test()
        {
            // When the user script containing a error is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = "undefined_variable";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
            UserScript.EvaluateUserScript(ctx, parsedScript);
        }

        [TestMethod()]
        [ExpectedException(typeof(UserScript.EvaluationAbortedException))]
        public void CompileAndEvaluateUserScript_AnbiguousOnOnDecomposed_Test()
        {
            // When the user script containing a error is given.
            var globalConfig = new GlobalConfig();
            var userScriptString = 
                "var Never = When(ctx => { return false; });" +
                "Never.On(Keys.RButton);" +
                "Never.OnDecomposed(Keys.RButton);";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
            var userScriptCache = UserScript.CompileUserScript(parsedScript);
            var ctx = new UserScriptExecutionContext(globalConfig);
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
            var tempDir = TestHelpers.GetTestDirectory();
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
            var globalConfig = new GlobalConfig();
            var userScriptString = @"
var hoge = 1;

void foo() 
{ 
  undefined_variable
}

";
            var parsedScript = UserScript.ParseScript(userScriptString, globalConfig.UserDirectory, globalConfig.UserDirectory);
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
