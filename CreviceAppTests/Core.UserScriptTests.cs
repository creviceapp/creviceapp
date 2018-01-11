using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp
{
    [TestClass()]
    public class UserScriptTests
    {
        [TestMethod()]
        public void DefaultUserDirectoryTest()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.DefaultUserDirectory.EndsWith("\\AppData\\Roaming\\Crevice\\CreviceApp"));
        }

        [TestMethod()]
        public void UserScriptFile0Test()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.UserScriptFile.EndsWith("\\AppData\\Roaming\\Crevice\\CreviceApp\\default.csx"));
        }

        [TestMethod()]
        public void UserScriptFile1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var global = new AppGlobal(cliOption);
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.UserScriptFile.EndsWith("\\AppData\\Roaming\\Crevice\\CreviceApp\\hoge.csx"));
        }
        
        [TestMethod()]
        public void UserScriptFile2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var global = new AppGlobal(cliOption);
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.UserScriptFile.Equals("C:\\hoge.csx"));
        }

        [TestMethod()]
        public void UserDirectory0Test()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.UserDirectory.EndsWith("\\AppData\\Roaming\\Crevice\\CreviceApp"));
        }

        [TestMethod()]
        public void UserDirectory1Test()
        {
            // When relative user script path is given.
            string[] args = { "--script", "hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var global = new AppGlobal(cliOption);
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.UserDirectory.EndsWith("\\AppData\\Roaming\\Crevice\\CreviceApp"));
        }

        [TestMethod()]
        public void UserDirectory2Test()
        {
            // When absolute user script path is given.
            string[] args = { "--script", "C:\\hoge.csx" };
            var cliOption = CLIOption.Parse(args);
            var global = new AppGlobal(cliOption);
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.UserDirectory.Equals("C:\\"));
        }

        [TestMethod()]
        public void GetUserScriptCodeTest()
        {
            string[] args = { "--script", "test.csx" };
            var cliOption = CLIOption.Parse(args);
            var global = new AppGlobal(cliOption);
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.GetUserScriptCode().Length > 0);
        }

        [TestMethod()]
        public void GetUserScriptCacheFileTest()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            Assert.IsTrue(userScript.UserScriptCacheFile.EndsWith("\\AppData\\Roaming\\Crevice\\CreviceApp\\default.csx.cache"));
        }

        [TestMethod()]
        public void ParseScriptTest()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "dummy_script";
            var parsedScript = userScript.ParseScript(userScriptCode);
            Assert.AreEqual(parsedScript.Code, userScriptCode);
            Assert.AreEqual(parsedScript.GlobalsType.Name, "UserScriptExecutionContext");
            Assert.AreEqual(parsedScript.GlobalsType.FullName, "CreviceApp.Core.UserScriptExecutionContext");
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("mscorlib.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("System.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("System.Core.dll")));
            Assert.IsTrue(parsedScript.Options.MetadataReferences.Any(v => v.Display.Contains("CreviceApp.exe")));
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_0Test()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "var hoge = 1;";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var errors = userScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScript(parsedScript, ctx);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_1Test()
        {
            // When the user script containing dynamic expression is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "dynamic hoge = 1;";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var errors = userScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScript(parsedScript, ctx);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_2Test()
        {
            // When the user script which defines a mouse gesture is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "var Whenever = @when((ctx) => {return true;});";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var errors = userScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScript(parsedScript, ctx);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_3Test()
        {
            // When the user script which using CreviceApp API is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "using static CreviceApp.WinAPI.Constants.WindowsMessages;";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var errors = userScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() == 0);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScript(parsedScript, ctx);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript0_4Test()
        {
            // When the user script containing a error is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "undefined_variable";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var errors = userScript.CompileUserScript(parsedScript);
            Assert.IsTrue(errors.Count() > 0);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_0Test()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "var hoge = 1;";
            var parsedScript = userScript.ParseScript(userScriptCode);
            userScript.CompileUserScript(userScriptCode, parsedScript);
            var userScriptCache = userScript.CompileUserScript(userScriptCode, parsedScript);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScriptAssembly(userScriptCache, ctx);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_1Test()
        {
            // When the user script containing dynamic expression is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "dynamic hoge = 1;";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var userScriptCache = userScript.CompileUserScript(userScriptCode, parsedScript);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScriptAssembly(userScriptCache, ctx);
        }

        [TestMethod()]
        public void CompileAndEvaluateUserScript1_2Test()
        {
            // When the user script which defines a mouse gesture is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "var Whenever = @when((ctx) => {return true;});";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var userScriptCache = userScript.CompileUserScript(userScriptCode, parsedScript);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScriptAssembly(userScriptCache, ctx);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.BadImageFormatException))]
        public void CompileAndEvaluateUserScript1_3Test()
        {
            // When the user script which using CreviceApp API is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "using static CreviceApp.WinAPI.Constants.WindowsMessages;";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var userScriptCache = userScript.CompileUserScript(userScriptCode, parsedScript);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScriptAssembly(userScriptCache, ctx);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.BadImageFormatException))]
        public void CompileAndEvaluateUserScript1_4Test()
        {
            // When the user script containing a error is given.
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            var userScriptCode = "undefined_variable";
            var parsedScript = userScript.ParseScript(userScriptCode);
            var userScriptCache = userScript.CompileUserScript(userScriptCode, parsedScript);
            var ctx = new Core.UserScriptExecutionContext(global);
            userScript.EvaluateUserScriptAssembly(userScriptCache, ctx);
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

        public void CleanupUserDirectory(UserScript us)
        {
            foreach (var file in Directory.GetFiles(us.UserDirectory))
            {
                File.Delete(file);
            }
        }

        [TestMethod()]
        public void WatcherTest()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            CleanupUserDirectory(userScript);
            var mock = new SynchronizeInvokeMock();
            var watcher = userScript.GetWatcher(mock);
            var watcherTestfile = Path.Combine(userScript.UserDirectory, "watcher_test.csx");
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
        }

        [TestMethod()]
        public void GetGestureDefTest()
        {
            var global = new AppGlobal();
            var userScript = new UserScript(global);
            CleanupUserDirectory(userScript);
            var ctx = new Core.UserScriptExecutionContext(global);
            var gestureDef = userScript.GetGestureDef(ctx);
            Assert.IsTrue(gestureDef.Count() > 0);
        }
    }
}
