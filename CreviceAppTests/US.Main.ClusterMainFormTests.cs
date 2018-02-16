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
    using System.Windows.Forms;

    using Crevice.Config;
    using Crevice.UserScript;
    using Crevice.GestureMachine;
    using Crevice.UI;
    using Crevice.UserScript.Keys;

    [TestClass()]
    public class ClusterMainFormTests
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
        public void ConstructorTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            TestHelpers.SetupUserDirectory(binaryDir, tempDir);

            string[] args = { "-s", userScriptFile };
            var cliOption = CLIOption.Parse(args);
            var config = new GlobalConfig(cliOption);
            var ctx = new UserScriptExecutionContext(config);
            ctx.When(x => { return true; })
            .On(SupportedKeys.Keys.WheelUp)
            .Do(x => { });
            var gestureMachineCluster = new GestureMachineCluster(ctx.Profiles);
            using (var form = new ClusterMainForm(cliOption, gestureMachineCluster))
            {
                Assert.AreEqual(form.Config.CLIOption, cliOption);
                Assert.AreEqual(form._gestureMachineCluster.Profiles, ctx.Profiles);
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
            var config = new GlobalConfig(cliOption);
            var ctx = new UserScriptExecutionContext(config);
            using (var cde = new CountdownEvent(1))
            {
                var when = ctx.When(x => { return true; });
                when.On(SupportedKeys.Keys.WheelUp)
                   .Do(x => { cde.Signal(); });
                when.On(SupportedKeys.Keys.RButton)
                   .Do(x => { cde.Signal(); });
                var gestureMachineCluster = new GestureMachineCluster(ctx.Profiles);
                using (var form = new ClusterMainForm(cliOption, gestureMachineCluster))
                {
                    form.Shown += new EventHandler((sender, e) => {
                        cde.Signal();
                    });
                    var task = Task.Run(() => {
                        Application.Run(form);
                    });
                    Assert.AreEqual(cde.Wait(100000), true);
                    cde.Reset();
                    Thread.Sleep(10000);
                    Assert.AreEqual(form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState.Depth, 0);
                    form.GestureMachine.Input(SupportedKeys.PhysicalKeys.WheelUp.FireEvent);
                    Assert.AreEqual(cde.Wait(100000), true);
                    cde.Reset();
                    form.GestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState.Depth, 1);
                    form.GestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent);
                    Assert.AreEqual(cde.Wait(100000), true);
                    form.Close();
                    Application.ExitThread();
                }
            }
        }
    }
}
