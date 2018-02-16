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
    using Crevice.UI;
    using Crevice.UserScript.Keys;

    [TestClass()]
    public class ReloadableMainFormTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestHelpers.MouseMutex.WaitOne();
            TestHelpers.KeyboardMutex.WaitOne();
            TestHelpers.TestDirectoryMutex.WaitOne();
            Directory.CreateDirectory(TestHelpers.TemporaryDirectory);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Directory.Delete(TestHelpers.TemporaryDirectory, recursive: true);
            TestHelpers.MouseMutex.ReleaseMutex();
            TestHelpers.KeyboardMutex.ReleaseMutex();
            TestHelpers.TestDirectoryMutex.ReleaseMutex();
        }

        static readonly Mutex mutex = new Mutex(true);

        [TestInitialize()]
        public void TestInitialize()
        {
            mutex.WaitOne();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            mutex.ReleaseMutex();
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            TestHelpers.SetupUserDirectory(binaryDir, tempDir);

            string[] args = { "-s", userScriptFile };
            var cliOption = CLIOption.Parse(args);
            using(var form = new ReloadableMainForm(cliOption))
            {
                Assert.AreEqual(form.Config.CLIOption, cliOption);
            }
        }
        
        [TestMethod()]
        public void InputTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            TestHelpers.SetupUserDirectory(binaryDir, tempDir);

            string[] args = { "-s", userScriptFile };
            var cliOption = CLIOption.Parse(args);
            using (var cde = new CountdownEvent(1))
            {
                using (var form = new ReloadableMainForm(cliOption))
                {
                    form._reloadableGestureMachine.Reloaded += new EventHandler((sender, e) => {
                        cde.Signal();
                    });
                    Task.Run(() => {
                        form.ShowDialog();
                    });
                    Assert.AreEqual(cde.Wait(10000), true);
                    cde.Reset();
                    Assert.AreEqual(form._reloadableGestureMachine._instance.Profiles.Count > 0, true);
                    Assert.AreEqual(form._reloadableGestureMachine._instance.Profiles[0].RootElement.GestureCount > 0, true);
                    Assert.AreEqual(form._reloadableGestureMachine.Input(SupportedKeys.PhysicalKeys.WheelUp.FireEvent), false);
                    Assert.AreEqual(form._reloadableGestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent), false);
                    form.Close();
                }
            }
        }
    }
}
