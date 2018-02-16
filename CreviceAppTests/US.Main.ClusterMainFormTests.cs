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
    using Crevice.UI;
    using Crevice.UserScript.Keys;

    [TestClass()]
    public class ClusterMainFormTests
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
            var config = new GlobalConfig(cliOption);
            var ctx = new UserScriptExecutionContext(config);
            ctx.When(x => { return true; })
            .On(SupportedKeys.Keys.WheelUp)
            .Do(x => { });
            using (var form = new ClusterMainForm(cliOption, ctx.Profiles))
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
                using (var form = new ClusterMainForm(cliOption, ctx.Profiles))
                {
                    form.Shown += new EventHandler((sender, e) => {
                        cde.Signal();
                    });
                    Task.Run(() => {
                        form.ShowDialog();
                    });
                    Assert.AreEqual(cde.Wait(10000), true);
                    cde.Reset();

                    Console.WriteLine($"CurrentState: {form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState}");
                    Console.WriteLine($"IsRunning: {form._gestureMachineCluster.Profiles[0].GestureMachine.IsRunning}");
                    Console.WriteLine($"DoubleThrowTriggers: {form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState.ToState0().DoubleThrowTriggers}");
                    Console.WriteLine($"SingleThrowTriggers: {form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState.ToState0().SingleThrowTriggers}");
                    Console.WriteLine($"SupportedKeys.Keys.WheelUp.FireEvent.EventId: {SupportedKeys.Keys.WheelUp.FireEvent.EventId}");
                    Console.WriteLine($"SupportedKeys.PhysicalKeys.WheelUp.FireEvent.LogicalNormalized.EventId: {SupportedKeys.PhysicalKeys.WheelUp.FireEvent.LogicalNormalized.EventId}");
                    Assert.AreEqual(form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState.ToState0().SingleThrowTriggers.Contains(SupportedKeys.PhysicalKeys.WheelUp.FireEvent), true);
                    Assert.AreEqual(form._gestureMachineCluster.Profiles[0].RootElement.GestureCount, 2);
                    Assert.AreEqual(form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState.Depth, 0);
                    Assert.AreEqual(form._gestureMachineCluster.Input(SupportedKeys.PhysicalKeys.WheelUp.FireEvent), true);
                    Assert.AreEqual(cde.Wait(10000), true);
                    cde.Reset();
                    Assert.AreEqual(form._gestureMachineCluster.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent), true);
                    Assert.AreEqual(form._gestureMachineCluster.Profiles[0].GestureMachine.CurrentState.Depth, 1);
                    Assert.AreEqual(form._gestureMachineCluster.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent), true);
                    Assert.AreEqual(cde.Wait(10000), true);
                    form.Close();
                }
            }
        }
    }
}
