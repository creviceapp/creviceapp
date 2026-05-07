using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Crevice4Tests
{
    using Crevice.Config;
    using Crevice.UserScript;
    using AppRootElement = Crevice.DSL.RootElement;

    [TestClass]
    public class GestureCoverageHarnessTests
    {
        [TestMethod]
        public void DefaultUserScriptGestureDefinitionsAreFullyEnumerated()
        {
            var root = LoadDefaultUserScriptRootElement();
            var cases = GestureCoverageHarness.Enumerate(root);

            Assert.AreEqual(root.GestureCount, cases.Count, "Coverage cases must match the DSL GestureCount.");
            Assert.AreEqual(ExpectedDefaultGestures.All.Count, cases.Count, "DefaultUserScript currently defines eight active browser gestures.");

            var signatures = cases.Select(c => c.Signature).OrderBy(x => x).ToList();
            var expected = ExpectedDefaultGestures.All.Select(c => c.Signature).OrderBy(x => x).ToList();

            CollectionAssert.AreEqual(expected, signatures);
        }

        [TestMethod]
        public void DefaultBrowserWheelGesturesReplayThroughSyntheticInput()
        {
            var gestures = ExpectedDefaultGestures.Wheel.ToList();
            var recorder = new ExecutionRecorder();

            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, gestures))
            {
                foreach (var gesture in gestures)
                {
                    var before = recorder.Count(gesture.Label);
                    var result = GestureReplayHarness.ReplayWheelGesture(gestureMachine, gesture);

                    Assert.IsTrue(result.PressConsumed, "RButton press should be consumed for " + gesture.Label);
                    Assert.IsTrue(result.TriggerConsumed, gesture.Label + " should be consumed while RButton is held.");
                    Assert.IsTrue(result.ReleaseConsumed, "RButton release should be consumed for " + gesture.Label);
                    Assert.AreEqual(before + 1, recorder.Count(gesture.Label), gesture.Label + " executor should run exactly once.");
                }
            }

            CollectionAssert.AreEqual(gestures.Select(g => g.Label).ToList(), recorder.Labels);
        }

        [TestMethod]
        public void DefaultBrowserStrokeGesturesReplayThroughSyntheticInput()
        {
            var gestures = ExpectedDefaultGestures.Stroke.ToList();
            var recorder = new ExecutionRecorder();

            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, gestures))
            {
                foreach (var gesture in gestures)
                {
                    var before = recorder.Count(gesture.Label);
                    var result = GestureReplayHarness.ReplayStrokeGesture(gestureMachine, gesture);

                    Assert.IsTrue(result.PressConsumed, "RButton press should be consumed for " + gesture.Label);
                    Assert.IsTrue(result.StrokeObservedBeforeRelease, "Stroke watcher should establish " + gesture.Label + " before release.");
                    Assert.IsTrue(result.ReleaseConsumed, "RButton release should be consumed for " + gesture.Label);
                    Assert.AreEqual(before + 1, recorder.Count(gesture.Label), gesture.Label + " executor should run exactly once.");
                }
            }

            CollectionAssert.AreEqual(gestures.Select(g => g.Label).ToList(), recorder.Labels);
        }

        [TestMethod]
        public void GeneratedDefaultBrowserStrokePatternsReplayThroughSyntheticInput()
        {
            var scenarios = MotionPatternGenerator.GeneratePositiveStrokeScenarios().ToList();
            var expectedScenarioCount =
                ExpectedDefaultGestures.Stroke.Count *
                MotionPatternGenerator.StartCoordinates.Count *
                MotionPatternGenerator.AboveThresholdDistances.Count *
                MotionPatternGenerator.PointDensities.Count *
                MotionPatternGenerator.JitterAmplitudes.Count;

            Assert.AreEqual(expectedScenarioCount, scenarios.Count, "Generated positive scenario inventory should match the Step 03 matrix.");

            var recorder = new ExecutionRecorder();
            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.Stroke))
            {
                foreach (var scenario in scenarios)
                {
                    var beforeGesture = recorder.Count(scenario.ExpectedStroke);
                    var beforeTotal = recorder.TotalCount;
                    var result = GestureReplayHarness.ReplayStrokeScenario(
                        gestureMachine,
                        scenario,
                        waitForStrokeBeforeRelease: true);

                    Assert.IsTrue(result.PressConsumed, scenario.Id + " should consume RButton press.");
                    Assert.IsTrue(result.StrokeObservedBeforeRelease, scenario.Id + " should observe " + scenario.ExpectedStroke + " before release; observed " + result.ObservedStroke + ".");
                    Assert.IsTrue(result.ReleaseConsumed, scenario.Id + " should consume RButton release.");
                    Assert.AreEqual(beforeGesture + 1, recorder.Count(scenario.ExpectedStroke), scenario.Id + " executor should run exactly once.");
                    Assert.AreEqual(beforeTotal + 1, recorder.TotalCount, scenario.Id + " should execute no extra handlers.");
                }
            }

            foreach (var gesture in ExpectedDefaultGestures.Stroke)
            {
                var count = scenarios.Count(s => s.ExpectedStroke == gesture.Label);
                Assert.AreEqual(expectedScenarioCount / ExpectedDefaultGestures.Stroke.Count, count, gesture.Label + " should have balanced generated coverage.");
            }
        }

        [TestMethod]
        public void NegativeGestureReplayCasesDoNotExecuteHandlers()
        {
            var recorder = new ExecutionRecorder();

            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.All))
            {
                var result = GestureReplayHarness.ReplayStrokeGesture(
                    gestureMachine,
                    DefaultGesture.Stroke("RL", "When[0] > On(Keys.RButton) > Stroke(RL) > Do[0]"));

                Assert.IsTrue(result.PressConsumed, "Registered RButton press should still be consumed.");
                Assert.IsTrue(result.StrokeObservedBeforeRelease, "Unregistered stroke should be fully observed before release.");
                Assert.IsTrue(result.ReleaseConsumed, "RButton release should be consumed after an unregistered stroke.");
                Assert.AreEqual(0, recorder.TotalCount, "Unregistered stroke RL must not execute any handler.");
            }

            recorder = new ExecutionRecorder();
            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.Wheel, whenResult: false))
            {
                var result = GestureReplayHarness.ReplayWheelGesture(gestureMachine, ExpectedDefaultGestures.WheelUp);

                Assert.IsFalse(result.PressConsumed, "False When condition should block the initial RButton press.");
                Assert.IsFalse(result.TriggerConsumed, "False When condition should prevent the nested wheel trigger.");
                Assert.AreEqual(0, recorder.TotalCount, "False When condition must not execute a handler.");
            }

            recorder = new ExecutionRecorder();
            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.Stroke))
            {
                var result = GestureReplayHarness.ReplayBelowThresholdMovement(gestureMachine);

                Assert.IsTrue(result.PressConsumed, "Registered RButton press should be consumed.");
                Assert.IsTrue(result.ReleaseConsumed, "RButton release should be consumed when a below-threshold gesture cancels.");
                Assert.AreEqual(0, recorder.TotalCount, "Below-threshold movement must not execute a stroke handler.");
            }
        }

        [TestMethod]
        public void GeneratedNegativeStrokePatternsDoNotExecuteHandlers()
        {
            var scenarios = MotionPatternGenerator.GenerateNegativeStrokeScenarios().ToList();

            Assert.IsTrue(scenarios.Any(s => s.Id.Contains("below-threshold")), "Generated negatives should include below-threshold movement.");
            Assert.IsTrue(scenarios.Any(s => s.ExpectedStroke == "RL"), "Generated negatives should include unknown RL.");
            Assert.IsTrue(scenarios.Any(s => s.ExpectedStroke == "LR"), "Generated negatives should include unknown LR.");
            Assert.IsTrue(scenarios.Any(s => s.Id.Contains("noisy")), "Generated negatives should include a stable noisy non-match.");

            var recorder = new ExecutionRecorder();
            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.All))
            {
                foreach (var scenario in scenarios)
                {
                    var result = GestureReplayHarness.ReplayStrokeScenario(
                        gestureMachine,
                        scenario,
                        waitForStrokeBeforeRelease: scenario.ExpectedStroke.Length > 0);

                    Assert.IsTrue(result.PressConsumed, scenario.Id + " should consume RButton press.");
                    if (scenario.ExpectedStroke.Length > 0)
                    {
                        Assert.IsTrue(result.StrokeObservedBeforeRelease, scenario.Id + " should observe " + scenario.ExpectedStroke + " before release; observed " + result.ObservedStroke + ".");
                    }
                    else
                    {
                        Assert.AreEqual("", result.ObservedStroke, scenario.Id + " should not establish a stroke.");
                    }
                    Assert.IsTrue(result.ReleaseConsumed, scenario.Id + " should consume RButton release.");
                    Assert.AreEqual(0, recorder.TotalCount, scenario.Id + " must not execute any default handler.");
                }
            }
        }

        [TestMethod]
        public void WheelAndStrokeGesturesDoNotCrossTriggerEachOther()
        {
            var recorder = new ExecutionRecorder();
            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.All))
            {
                var result = GestureReplayHarness.ReplayWheelGesture(gestureMachine, ExpectedDefaultGestures.WheelUp);

                Assert.IsTrue(result.PressConsumed);
                Assert.IsTrue(result.TriggerConsumed);
                Assert.IsTrue(result.ReleaseConsumed);
                Assert.AreEqual(1, recorder.Count(ExpectedDefaultGestures.WheelUp.Label));
                Assert.AreEqual(0, ExpectedDefaultGestures.Stroke.Sum(g => recorder.Count(g.Label)), "Wheel replay must not execute stroke handlers.");
            }

            recorder = new ExecutionRecorder();
            using (var gestureMachine = GestureReplayTestFactory.CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.All))
            {
                var result = GestureReplayHarness.ReplayStrokeGesture(gestureMachine, ExpectedDefaultGestures.StrokeUp);

                Assert.IsTrue(result.PressConsumed);
                Assert.IsTrue(result.StrokeObservedBeforeRelease);
                Assert.IsTrue(result.ReleaseConsumed);
                Assert.AreEqual(1, recorder.Count(ExpectedDefaultGestures.StrokeUp.Label));
                Assert.AreEqual(0, ExpectedDefaultGestures.Wheel.Sum(g => recorder.Count(g.Label)), "Stroke replay must not execute wheel handlers.");
            }
        }

        private static AppRootElement LoadDefaultUserScriptRootElement()
        {
            TestHelpers.TestDirectoryMutex.WaitOne();
            try
            {
                var tempDir = TestHelpers.GetTestDirectory();
                var binaryDir = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent;
                var userScriptFile = Path.Combine(tempDir, "default.csx");
                var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

                TestHelpers.SetupUserDirectory(binaryDir, tempDir);

                string[] args = { "-s", userScriptFile };
                var cliOption = CLIOption.Parse(args);
                var globalConfig = new GlobalConfig(cliOption);
                var appEnvUserScriptString = globalConfig.GetOrSetDefaultUserScriptFile(userScriptString);
                var parsedScript = UserScript.ParseScript(appEnvUserScriptString, tempDir, tempDir);
                var errors = UserScript.CompileUserScript(parsedScript);

                Assert.AreEqual(0, errors.Count(), "DefaultUserScript must compile before coverage enumeration.");

                var cache = UserScript.GenerateUserScriptAssemblyCache(userScriptFile + ".cache", userScriptString, parsedScript);
                var ctx = new UserScriptExecutionContext(globalConfig, null);
                UserScript.EvaluateUserScriptAssembly(ctx, cache);

                Assert.IsTrue(ctx.Profiles.Any(), "DefaultUserScript should declare at least one profile.");
                return ctx.Profiles[0].RootElement;
            }
            finally
            {
                TestHelpers.TestDirectoryMutex.ReleaseMutex();
            }
        }
    }
}
