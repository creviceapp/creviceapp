using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crevice4Tests
{
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Events;
    using Crevice.Core.Stroke;
    using Crevice.Threading;
    using Crevice.UserScript.Keys;

    [TestClass]
    public class GesturePerformanceHarnessTests
    {
        private const int WarmupIterations = 2;
        private const int MeasuredIterations = 5;
        private const int RepeatabilityRuns = 3;
        private const int SchedulerComparisonRuns = 3;
        private const int Step08SchedulerComparisonRuns = 1;
        private const int Step09IntervalComparisonRuns = 1;
        private const int Step09WarmupIterations = 0;
        private const int Step09MeasuredIterations = 1;
        private const string PerformanceHarnessEnabledEnvironmentVariable = "CREVICE_RUN_PERFORMANCE_HARNESS";
        private const string PerformanceHarnessOutputDirectoryEnvironmentVariable = "CREVICE_PERF_OUTPUT_DIR";
        private static readonly TimeSpan StrokeProcessingTimeout = TimeSpan.FromSeconds(5);
        private static readonly int ProductionStrokeWatchIntervalMilliseconds = new Crevice.Core.FSM.GestureMachineConfig().StrokeWatchInterval;

        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory("Performance")]
        [TestCategory("DiagnosticPerformance")]
        public void SyntheticGestureReplayPerformanceBaseline()
        {
            RequirePerformanceHarnessEnabled();
            var result = GesturePerformanceMeasurer.Measure(WarmupIterations, MeasuredIterations);
            var outputDirectory = GetStep04OutputDirectory();

            Directory.CreateDirectory(outputDirectory);

            var jsonPath = Path.Combine(outputDirectory, "gesture-performance-baseline.json");
            var markdownPath = Path.Combine(outputDirectory, "gesture-performance-baseline-summary.md");

            File.WriteAllText(jsonPath, PerformanceResultWriter.ToJson(result), Encoding.UTF8);
            File.WriteAllText(markdownPath, PerformanceResultWriter.ToMarkdown(result), Encoding.UTF8);

            TestContext.WriteLine("Performance baseline JSON: " + jsonPath);
            TestContext.WriteLine("Performance baseline summary: " + markdownPath);

            Assert.IsTrue(result.Samples.Count > 0, "Measurement harness should record at least one sample.");
            Assert.AreEqual(MeasuredIterations, result.MeasuredIterations, "Measured iteration count should match the harness configuration.");
        }

        [TestMethod]
        [TestCategory("PerformanceAttribution")]
        [TestCategory("DiagnosticPerformance")]
        public void SyntheticGestureReplayPerformanceAttribution()
        {
            RequirePerformanceHarnessEnabled();
            var result = GestureAttributionMeasurer.Measure(WarmupIterations, MeasuredIterations);
            var outputDirectory = GetStep05OutputDirectory();

            Directory.CreateDirectory(outputDirectory);

            var jsonPath = Path.Combine(outputDirectory, "gesture-performance-attribution.json");
            var markdownPath = Path.Combine(outputDirectory, "gesture-performance-attribution-summary.md");

            File.WriteAllText(jsonPath, PerformanceResultWriter.ToJson(result), Encoding.UTF8);
            File.WriteAllText(markdownPath, PerformanceResultWriter.ToMarkdown(result), Encoding.UTF8);

            TestContext.WriteLine("Performance attribution JSON: " + jsonPath);
            TestContext.WriteLine("Performance attribution summary: " + markdownPath);

            Assert.IsTrue(result.Samples.Count > 0, "Attribution harness should record at least one sample.");
            Assert.AreEqual(MeasuredIterations, result.MeasuredIterations, "Measured iteration count should match the harness configuration.");
        }

        [TestMethod]
        [TestCategory("PerformanceRepeatability")]
        [TestCategory("DiagnosticPerformance")]
        public void SyntheticGestureReplayPerformanceRepeatability()
        {
            RequirePerformanceHarnessEnabled();
            var result = GestureRepeatabilityMeasurer.Measure(WarmupIterations, MeasuredIterations, RepeatabilityRuns);
            var outputDirectory = GetStep06OutputDirectory();

            Directory.CreateDirectory(outputDirectory);

            var jsonPath = Path.Combine(outputDirectory, "gesture-performance-repeatability.json");
            var markdownPath = Path.Combine(outputDirectory, "gesture-performance-repeatability-summary.md");

            File.WriteAllText(jsonPath, PerformanceResultWriter.ToJson(result), Encoding.UTF8);
            File.WriteAllText(markdownPath, PerformanceResultWriter.ToMarkdown(result), Encoding.UTF8);

            TestContext.WriteLine("Performance repeatability JSON: " + jsonPath);
            TestContext.WriteLine("Performance repeatability summary: " + markdownPath);

            Assert.AreEqual(RepeatabilityRuns, result.RunSummaries.Count, "Repeatability harness should record every requested run.");
            Assert.IsTrue(result.RunSummaries.All(r => r.SampleCount > 0), "Each repeatability run should record samples.");
            Assert.IsTrue(result.ComponentSummaries.Count > 0, "Repeatability harness should produce component variance summaries.");
        }

        [TestMethod]
        [TestCategory("PerformanceSchedulerComparison")]
        [TestCategory("DiagnosticPerformance")]
        public void SyntheticGestureReplaySchedulerComparison()
        {
            RequirePerformanceHarnessEnabled();
            var result = GestureSchedulerComparisonMeasurer.Measure(WarmupIterations, MeasuredIterations, SchedulerComparisonRuns);
            var outputDirectory = GetStep07OutputDirectory();

            Directory.CreateDirectory(outputDirectory);

            var jsonPath = Path.Combine(outputDirectory, "gesture-performance-scheduler-comparison.json");
            var markdownPath = Path.Combine(outputDirectory, "gesture-performance-scheduler-comparison-summary.md");

            File.WriteAllText(jsonPath, PerformanceResultWriter.ToJson(result), Encoding.UTF8);
            File.WriteAllText(markdownPath, PerformanceResultWriter.ToMarkdown(result), Encoding.UTF8);

            TestContext.WriteLine("Performance scheduler comparison JSON: " + jsonPath);
            TestContext.WriteLine("Performance scheduler comparison summary: " + markdownPath);

            Assert.AreEqual(2, result.ModeSummaries.Count, "Scheduler comparison should record the two requested replay scheduler modes.");
            Assert.IsTrue(result.ModeSummaries.All(m => m.SampleCount > 0), "Each scheduler mode should record samples.");
            Assert.IsTrue(result.ModeSummaries.All(m => m.RunSummaries.Count == SchedulerComparisonRuns), "Each scheduler mode should record every requested run.");
        }

        [TestMethod]
        [TestCategory("PerformanceStep08PointProcessorZeroInterval")]
        [TestCategory("DiagnosticPerformance")]
        public void SyntheticGestureReplaySchedulerComparisonAfterPointProcessorZeroIntervalGuard()
        {
            RequirePerformanceHarnessEnabled();
            var result = GestureSchedulerComparisonMeasurer.Measure(WarmupIterations, MeasuredIterations, Step08SchedulerComparisonRuns);
            result.Step = "08-pointprocessor-zero-interval-experiment";
            result.TimingNote = "Step 08 focused scheduler comparison after the PointProcessor zero-interval guard; no hard pass/fail timing threshold is applied.";
            var outputDirectory = GetStep08OutputDirectory();

            Directory.CreateDirectory(outputDirectory);

            var jsonPath = Path.Combine(outputDirectory, "gesture-performance-scheduler-comparison-after-zero-interval-guard.json");
            var markdownPath = Path.Combine(outputDirectory, "gesture-performance-scheduler-comparison-after-zero-interval-guard-summary.md");

            File.WriteAllText(jsonPath, PerformanceResultWriter.ToJson(result), Encoding.UTF8);
            File.WriteAllText(markdownPath, PerformanceResultWriter.ToMarkdown(result), Encoding.UTF8);

            TestContext.WriteLine("Step 08 scheduler comparison JSON: " + jsonPath);
            TestContext.WriteLine("Step 08 scheduler comparison summary: " + markdownPath);

            Assert.AreEqual(2, result.ModeSummaries.Count, "Step 08 scheduler comparison should record the two requested replay scheduler modes.");
            Assert.IsTrue(result.ModeSummaries.All(m => m.SampleCount > 0), "Each scheduler mode should record samples.");
            Assert.IsTrue(result.ModeSummaries.All(m => m.RunSummaries.Count == Step08SchedulerComparisonRuns), "Each scheduler mode should record every requested run.");
        }

        [TestMethod]
        [TestCategory("PerformanceStep09ProductionIntervalComparison")]
        [TestCategory("DiagnosticPerformance")]
        public void SyntheticGestureReplayProductionIntervalComparison()
        {
            RequirePerformanceHarnessEnabled();
            var result = GestureIntervalComparisonMeasurer.Measure(Step09WarmupIterations, Step09MeasuredIterations, Step09IntervalComparisonRuns);
            var outputDirectory = GetStep09OutputDirectory();

            Directory.CreateDirectory(outputDirectory);

            var jsonPath = Path.Combine(outputDirectory, "gesture-performance-production-interval-comparison.json");
            var markdownPath = Path.Combine(outputDirectory, "gesture-performance-production-interval-comparison-summary.md");

            File.WriteAllText(jsonPath, PerformanceResultWriter.ToJson(result), Encoding.UTF8);
            File.WriteAllText(markdownPath, PerformanceResultWriter.ToMarkdown(result), Encoding.UTF8);

            TestContext.WriteLine("Step 09 interval comparison JSON: " + jsonPath);
            TestContext.WriteLine("Step 09 interval comparison summary: " + markdownPath);

            Assert.AreEqual(5, result.ModeSummaries.Count, "Step 09 interval comparison should record the four primary modes plus the exploratory pool-size mode.");
            Assert.IsTrue(result.ModeSummaries.All(m => m.SampleCount > 0), "Each interval/scheduler mode should record samples.");
            Assert.IsTrue(result.ModeSummaries.All(m => m.RunSummaries.Count == Step09IntervalComparisonRuns), "Each interval/scheduler mode should record every requested run.");
            Assert.IsTrue(result.ModeSummaries.Any(m => m.StrokeWatchIntervalMilliseconds == ProductionStrokeWatchIntervalMilliseconds), "Step 09 should include the production default StrokeWatchInterval.");
        }


        private static void RequirePerformanceHarnessEnabled()
        {
            var enabled = Environment.GetEnvironmentVariable(PerformanceHarnessEnabledEnvironmentVariable);
            if (!string.Equals(enabled, "1", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Inconclusive(
                    "Performance diagnostics are opt-in. Set " +
                    PerformanceHarnessEnabledEnvironmentVariable +
                    "=1 and " +
                    PerformanceHarnessOutputDirectoryEnvironmentVariable +
                    " before running this harness.");
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(PerformanceHarnessOutputDirectoryEnvironmentVariable)))
            {
                Assert.Inconclusive(
                    "Performance diagnostics need an explicit output directory. Set " +
                    PerformanceHarnessOutputDirectoryEnvironmentVariable +
                    " before running this harness.");
            }
        }

        private static string GetPerformanceOutputDirectory()
        {
            var outputDirectory = Environment.GetEnvironmentVariable(PerformanceHarnessOutputDirectoryEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                Assert.Inconclusive(
                    "Performance diagnostics need an explicit output directory. Set " +
                    PerformanceHarnessOutputDirectoryEnvironmentVariable +
                    " before running this harness.");
            }

            return outputDirectory;
        }

        private static string GetStep04OutputDirectory()
        {
            return GetPerformanceOutputDirectory();
        }

        private static string GetStep05OutputDirectory()
        {
            return GetPerformanceOutputDirectory();
        }

        private static string GetStep06OutputDirectory()
        {
            return GetPerformanceOutputDirectory();
        }

        private static string GetStep07OutputDirectory()
        {
            return GetPerformanceOutputDirectory();
        }

        private static string GetStep08OutputDirectory()
        {
            return GetPerformanceOutputDirectory();
        }

        private static string GetStep09OutputDirectory()
        {
            return GetPerformanceOutputDirectory();
        }

        private static class GesturePerformanceMeasurer
        {
            public static PerformanceRunResult Measure(int warmupIterations, int measuredIterations)
            {
                var scenarios = PerformanceScenarioFactory.CreateScenarios();
                var startedUtc = DateTimeOffset.UtcNow;
                var result = new PerformanceRunResult
                {
                    Step = "04-performance-baseline-measurement",
                    Repository = GetRepositoryRoot(),
                    Branch = GetBranchName(),
                    StartedUtc = startedUtc,
                    StartedJst = ToJst(startedUtc),
                    WarmupIterations = warmupIterations,
                    MeasuredIterations = measuredIterations,
                    ScenarioCount = scenarios.Count,
                    EventCountPerMeasuredIteration = scenarios.Sum(s => s.EventCount),
                    TimingNote = "No hard pass/fail timing threshold is applied; results are noisy and intended only as a local baseline.",
                    Environment = EnvironmentSnapshot.Capture(),
                    ScenarioInventory = scenarios.Select(ScenarioInventoryEntry.FromScenario).ToList(),
                };

                for (var iteration = 0; iteration < warmupIterations; iteration++)
                {
                    RunIteration(scenarios, iteration, false, result);
                }

                for (var iteration = 0; iteration < measuredIterations; iteration++)
                {
                    RunIteration(scenarios, iteration, true, result);
                }

                result.FinishedUtc = DateTimeOffset.UtcNow;
                result.FinishedJst = ToJst(result.FinishedUtc);
                result.TotalMeasuredEventCount = result.EventCountPerMeasuredIteration * measuredIterations;
                result.Summaries = SummaryCalculator.Calculate(result.Samples);
                result.PerScenarioSummaries = SummaryCalculator.CalculatePerScenario(result.Samples);
                return result;
            }

            private static void RunIteration(
                IReadOnlyList<PerformanceScenario> scenarios,
                int iteration,
                bool measured,
                PerformanceRunResult result)
            {
                var recorder = new ExecutionRecorder();
                using (var gestureMachine = CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.All))
                {
                    foreach (var scenario in scenarios)
                    {
                        var beforeLabel = scenario.ExpectedHandler == null ? 0 : recorder.Count(scenario.ExpectedHandler);
                        var beforeTotal = recorder.TotalCount;
                        var stopwatch = Stopwatch.StartNew();
                        var replayResult = scenario.Replay(gestureMachine);
                        stopwatch.Stop();

                        ValidateScenario(scenario, replayResult, recorder, beforeLabel, beforeTotal);

                        if (measured)
                        {
                            result.Samples.Add(new PerformanceSample
                            {
                                Iteration = iteration,
                                Group = scenario.Group,
                                ScenarioId = scenario.Id,
                                Category = scenario.Category,
                                ExpectedStroke = scenario.ExpectedStroke,
                                ExpectedHandler = scenario.ExpectedHandler,
                                EventCount = scenario.EventCount,
                                PointCount = scenario.PointCount,
                                ElapsedTicks = stopwatch.ElapsedTicks,
                                ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
                                PressConsumed = replayResult.PressConsumed,
                                TriggerConsumed = replayResult.TriggerConsumed,
                                ReleaseConsumed = replayResult.ReleaseConsumed,
                                StrokeObservedBeforeRelease = replayResult.StrokeObservedBeforeRelease,
                                ObservedStroke = replayResult.ObservedStroke ?? "",
                            });
                        }
                    }
                }
            }

            private static void ValidateScenario(
                PerformanceScenario scenario,
                GestureReplayResult replayResult,
                ExecutionRecorder recorder,
                int beforeLabel,
                int beforeTotal)
            {
                Assert.AreEqual(scenario.ExpectedPressConsumed, replayResult.PressConsumed, scenario.Id + " press consumption changed.");
                Assert.AreEqual(scenario.ExpectedReleaseConsumed, replayResult.ReleaseConsumed, scenario.Id + " release consumption changed.");

                if (scenario.ExpectsWheelTrigger)
                {
                    Assert.IsTrue(replayResult.TriggerConsumed, scenario.Id + " wheel trigger should be consumed.");
                }

                if (scenario.ExpectedStroke.Length > 0)
                {
                    Assert.IsTrue(replayResult.StrokeObservedBeforeRelease, scenario.Id + " should observe stroke before release.");
                    Assert.AreEqual(scenario.ExpectedStroke, replayResult.ObservedStroke, scenario.Id + " observed stroke changed.");
                }

                if (scenario.ExpectedHandler == null)
                {
                    Assert.AreEqual(beforeTotal, recorder.TotalCount, scenario.Id + " should not execute any handler.");
                }
                else
                {
                    Assert.AreEqual(beforeLabel + 1, recorder.Count(scenario.ExpectedHandler), scenario.Id + " should execute the expected handler once.");
                    Assert.AreEqual(beforeTotal + 1, recorder.TotalCount, scenario.Id + " should execute no extra handlers.");
                }
            }

            private static string GetRepositoryRoot()
            {
                var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                return assemblyDirectory.Parent.Parent.Parent.FullName;
            }

            private static string GetBranchName()
            {
                var repositoryRoot = GetRepositoryRoot();
                var headPath = Path.Combine(repositoryRoot, ".git", "HEAD");
                if (!File.Exists(headPath))
                {
                    return "";
                }

                var head = File.ReadAllText(headPath).Trim();
                const string prefix = "ref: refs/heads/";
                return head.StartsWith(prefix, StringComparison.Ordinal) ? head.Substring(prefix.Length) : head;
            }

            private static DateTimeOffset ToJst(DateTimeOffset utc)
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    return TimeZoneInfo.ConvertTime(utc, timeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
                catch (InvalidTimeZoneException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
            }
        }

        private static class GestureAttributionMeasurer
        {
            public static AttributionRunResult Measure(int warmupIterations, int measuredIterations)
                => Measure(warmupIterations, measuredIterations, ReplaySchedulerMode.TaskFactoryDefault, 0);

            public static AttributionRunResult Measure(
                int warmupIterations,
                int measuredIterations,
                ReplaySchedulerMode schedulerMode)
                => Measure(warmupIterations, measuredIterations, schedulerMode, 0);

            public static AttributionRunResult Measure(
                int warmupIterations,
                int measuredIterations,
                ReplaySchedulerMode schedulerMode,
                int strokeWatchIntervalMilliseconds)
                => Measure(
                    warmupIterations,
                    measuredIterations,
                    schedulerMode,
                    strokeWatchIntervalMilliseconds,
                    "05-performance-attribution-analysis",
                    "Attribution-only measurement; no hard pass/fail timing threshold is applied.");

            public static AttributionRunResult Measure(
                int warmupIterations,
                int measuredIterations,
                ReplaySchedulerMode schedulerMode,
                int strokeWatchIntervalMilliseconds,
                string step,
                string timingNote,
                IReadOnlyList<PerformanceScenario> scenarios = null)
            {
                scenarios = scenarios ?? PerformanceScenarioFactory.CreateScenarios();
                var startedUtc = DateTimeOffset.UtcNow;
                var result = new AttributionRunResult
                {
                    Step = step,
                    Repository = GetRepositoryRoot(),
                    Branch = GetBranchName(),
                    StartedUtc = startedUtc,
                    StartedJst = ToJst(startedUtc),
                    WarmupIterations = warmupIterations,
                    MeasuredIterations = measuredIterations,
                    ScenarioCount = scenarios.Count,
                    EventCountPerMeasuredIteration = scenarios.Sum(s => s.EventCount),
                    SchedulerMode = schedulerMode.Name,
                    SchedulerDescription = schedulerMode.Description,
                    StrokeWatchIntervalMilliseconds = strokeWatchIntervalMilliseconds,
                    TimingNote = timingNote,
                    Environment = EnvironmentSnapshot.Capture(),
                    ScenarioInventory = scenarios.Select(ScenarioInventoryEntry.FromScenario).ToList(),
                };

                for (var iteration = 0; iteration < warmupIterations; iteration++)
                {
                    RunIteration(scenarios, iteration, false, result, schedulerMode, strokeWatchIntervalMilliseconds);
                }

                for (var iteration = 0; iteration < measuredIterations; iteration++)
                {
                    RunIteration(scenarios, iteration, true, result, schedulerMode, strokeWatchIntervalMilliseconds);
                }

                result.FinishedUtc = DateTimeOffset.UtcNow;
                result.FinishedJst = ToJst(result.FinishedUtc);
                result.TotalMeasuredEventCount = result.EventCountPerMeasuredIteration * measuredIterations;
                result.Summaries = AttributionSummaryCalculator.Calculate(result.Samples);
                result.PerScenarioSummaries = AttributionSummaryCalculator.CalculatePerScenario(result.Samples);
                return result;
            }

            private static void RunIteration(
                IReadOnlyList<PerformanceScenario> scenarios,
                int iteration,
                bool measured,
                AttributionRunResult result,
                ReplaySchedulerMode schedulerMode,
                int strokeWatchIntervalMilliseconds)
            {
                var recorder = new ExecutionRecorder();
                using (var gestureMachine = CreateReplayGestureMachine(recorder, ExpectedDefaultGestures.All, schedulerMode, strokeWatchIntervalMilliseconds))
                {
                    foreach (var scenario in scenarios)
                    {
                        var beforeLabel = scenario.ExpectedHandler == null ? 0 : recorder.Count(scenario.ExpectedHandler);
                        var beforeTotal = recorder.TotalCount;
                        var replayResult = AttributionReplayHarness.ReplayScenario(gestureMachine, scenario, strokeWatchIntervalMilliseconds);

                        var validationStart = Stopwatch.GetTimestamp();
                        ValidateScenario(scenario, replayResult, recorder, beforeLabel, beforeTotal);
                        var validationTicks = Stopwatch.GetTimestamp() - validationStart;

                        if (measured)
                        {
                            result.Samples.Add(new AttributionSample
                            {
                                Iteration = iteration,
                                Group = scenario.Group,
                                ScenarioId = scenario.Id,
                                Category = scenario.Category,
                                ExpectedStroke = scenario.ExpectedStroke,
                                ExpectedHandler = scenario.ExpectedHandler,
                                EventCount = scenario.EventCount,
                                PointCount = scenario.PointCount,
                                TotalReplayTicks = replayResult.TotalReplayTicks,
                                TotalReplayMilliseconds = TicksToMilliseconds(replayResult.TotalReplayTicks),
                                TotalInputTicks = replayResult.TotalInputTicks,
                                TotalInputMilliseconds = TicksToMilliseconds(replayResult.TotalInputTicks),
                                PressInputTicks = replayResult.PressInputTicks,
                                PressInputMilliseconds = TicksToMilliseconds(replayResult.PressInputTicks),
                                MoveInputTicks = replayResult.MoveInputTicks,
                                MoveInputMilliseconds = TicksToMilliseconds(replayResult.MoveInputTicks),
                                TriggerInputTicks = replayResult.TriggerInputTicks,
                                TriggerInputMilliseconds = TicksToMilliseconds(replayResult.TriggerInputTicks),
                                ReleaseInputTicks = replayResult.ReleaseInputTicks,
                                ReleaseInputMilliseconds = TicksToMilliseconds(replayResult.ReleaseInputTicks),
                                ObservationWaitTicks = replayResult.ObservationWaitTicks,
                                ObservationWaitMilliseconds = TicksToMilliseconds(replayResult.ObservationWaitTicks),
                                ObservationReadTicks = replayResult.ObservationReadTicks,
                                ObservationReadMilliseconds = TicksToMilliseconds(replayResult.ObservationReadTicks),
                                ValidationTicks = validationTicks,
                                ValidationMilliseconds = TicksToMilliseconds(validationTicks),
                                PressConsumed = replayResult.PressConsumed,
                                TriggerConsumed = replayResult.TriggerConsumed,
                                ReleaseConsumed = replayResult.ReleaseConsumed,
                                StrokeObservedBeforeRelease = replayResult.StrokeObservedBeforeRelease,
                                ObservedStroke = replayResult.ObservedStroke ?? "",
                            });
                        }
                    }
                }
            }

            private static void ValidateScenario(
                PerformanceScenario scenario,
                GestureReplayResult replayResult,
                ExecutionRecorder recorder,
                int beforeLabel,
                int beforeTotal)
            {
                Assert.AreEqual(scenario.ExpectedPressConsumed, replayResult.PressConsumed, scenario.Id + " press consumption changed.");
                Assert.AreEqual(scenario.ExpectedReleaseConsumed, replayResult.ReleaseConsumed, scenario.Id + " release consumption changed.");

                if (scenario.ExpectsWheelTrigger)
                {
                    Assert.IsTrue(replayResult.TriggerConsumed, scenario.Id + " wheel trigger should be consumed.");
                }

                if (scenario.ExpectedStroke.Length > 0)
                {
                    Assert.IsTrue(replayResult.StrokeObservedBeforeRelease, scenario.Id + " should observe stroke before release.");
                    Assert.AreEqual(scenario.ExpectedStroke, replayResult.ObservedStroke, scenario.Id + " observed stroke changed.");
                }

                if (scenario.ExpectedHandler == null)
                {
                    Assert.AreEqual(beforeTotal, recorder.TotalCount, scenario.Id + " should not execute any handler.");
                }
                else
                {
                    Assert.AreEqual(beforeLabel + 1, recorder.Count(scenario.ExpectedHandler), scenario.Id + " should execute the expected handler once.");
                    Assert.AreEqual(beforeTotal + 1, recorder.TotalCount, scenario.Id + " should execute no extra handlers.");
                }
            }

            private static string GetRepositoryRoot()
            {
                var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                return assemblyDirectory.Parent.Parent.Parent.FullName;
            }

            private static string GetBranchName()
            {
                var repositoryRoot = GetRepositoryRoot();
                var headPath = Path.Combine(repositoryRoot, ".git", "HEAD");
                if (!File.Exists(headPath))
                {
                    return "";
                }

                var head = File.ReadAllText(headPath).Trim();
                const string prefix = "ref: refs/heads/";
                return head.StartsWith(prefix, StringComparison.Ordinal) ? head.Substring(prefix.Length) : head;
            }

            private static DateTimeOffset ToJst(DateTimeOffset utc)
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    return TimeZoneInfo.ConvertTime(utc, timeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
                catch (InvalidTimeZoneException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
            }
        }

        private static class GestureRepeatabilityMeasurer
        {
            public static RepeatabilityRunResult Measure(int warmupIterations, int measuredIterations, int repeatRuns)
            {
                var startedUtc = DateTimeOffset.UtcNow;
                var result = new RepeatabilityRunResult
                {
                    Step = "06-measurement-determinism-tuning",
                    Repository = GetRepositoryRoot(),
                    Branch = GetBranchName(),
                    StartedUtc = startedUtc,
                    StartedJst = ToJst(startedUtc),
                    WarmupIterations = warmupIterations,
                    MeasuredIterations = measuredIterations,
                    RepeatRuns = repeatRuns,
                    TimingNote = "Repeatability-only attribution measurement; no hard pass/fail timing threshold is applied.",
                    Environment = EnvironmentSnapshot.Capture(),
                };

                var slowSamples = new List<RepeatabilitySlowSample>();
                for (var runIndex = 0; runIndex < repeatRuns; runIndex++)
                {
                    var run = GestureAttributionMeasurer.Measure(warmupIterations, measuredIterations);
                    result.RunSummaries.Add(RepeatabilityRunSummary.FromRun(runIndex, run));
                    slowSamples.AddRange(CreateSlowSamples(runIndex, run));

                    if (result.ScenarioCount == 0)
                    {
                        result.ScenarioCount = run.ScenarioCount;
                        result.EventCountPerMeasuredIteration = run.EventCountPerMeasuredIteration;
                    }
                }

                result.FinishedUtc = DateTimeOffset.UtcNow;
                result.FinishedJst = ToJst(result.FinishedUtc);
                result.TotalMeasuredEventCount = result.EventCountPerMeasuredIteration * measuredIterations * repeatRuns;
                result.ComponentSummaries = RepeatabilitySummaryCalculator.Calculate(result.RunSummaries);
                result.SlowestSamples = slowSamples
                    .GroupBy(s => s.Component)
                    .SelectMany(g => g.OrderByDescending(s => s.ComponentMilliseconds).Take(10))
                    .OrderBy(s => s.Component)
                    .ThenByDescending(s => s.ComponentMilliseconds)
                    .ToList();
                return result;
            }

            private static IEnumerable<RepeatabilitySlowSample> CreateSlowSamples(int runIndex, AttributionRunResult run)
            {
                foreach (var sample in run.Samples)
                {
                    yield return RepeatabilitySlowSample.FromSample(runIndex, "raw_input", sample.TotalInputMilliseconds, sample);
                    yield return RepeatabilitySlowSample.FromSample(runIndex, "observation_wait", sample.ObservationWaitMilliseconds, sample);
                }
            }

            private static string GetRepositoryRoot()
            {
                var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                return assemblyDirectory.Parent.Parent.Parent.FullName;
            }

            private static string GetBranchName()
            {
                var repositoryRoot = GetRepositoryRoot();
                var headPath = Path.Combine(repositoryRoot, ".git", "HEAD");
                if (!File.Exists(headPath))
                {
                    return "";
                }

                var head = File.ReadAllText(headPath).Trim();
                const string prefix = "ref: refs/heads/";
                return head.StartsWith(prefix, StringComparison.Ordinal) ? head.Substring(prefix.Length) : head;
            }

            private static DateTimeOffset ToJst(DateTimeOffset utc)
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    return TimeZoneInfo.ConvertTime(utc, timeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
                catch (InvalidTimeZoneException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
            }
        }

        private static class GestureSchedulerComparisonMeasurer
        {
            public static SchedulerComparisonRunResult Measure(int warmupIterations, int measuredIterations, int repeatRunsPerScheduler)
            {
                var startedUtc = DateTimeOffset.UtcNow;
                var schedulerModes = new[]
                {
                    ReplaySchedulerMode.TaskFactoryDefault,
                    ReplaySchedulerMode.ProductionLikeLowLatency,
                };

                var result = new SchedulerComparisonRunResult
                {
                    Step = "07-strokewatcher-scheduler-tuning",
                    Repository = GetRepositoryRoot(),
                    Branch = GetBranchName(),
                    StartedUtc = startedUtc,
                    StartedJst = ToJst(startedUtc),
                    WarmupIterations = warmupIterations,
                    MeasuredIterations = measuredIterations,
                    RepeatRunsPerScheduler = repeatRunsPerScheduler,
                    TimingNote = "Scheduler comparison measurement; no hard pass/fail timing threshold is applied.",
                    Environment = EnvironmentSnapshot.Capture(),
                    ScenarioInventory = PerformanceScenarioFactory.CreateScenarios()
                        .Select(ScenarioInventoryEntry.FromScenario)
                        .ToList(),
                };

                var slowSamples = new List<SchedulerSlowSample>();
                foreach (var schedulerMode in schedulerModes)
                {
                    var runs = new List<AttributionRunResult>();
                    for (var runIndex = 0; runIndex < repeatRunsPerScheduler; runIndex++)
                    {
                        var run = GestureAttributionMeasurer.Measure(warmupIterations, measuredIterations, schedulerMode);
                        runs.Add(run);
                        slowSamples.AddRange(CreateSlowSamples(schedulerMode.Name, runIndex, run));

                        if (result.ScenarioCount == 0)
                        {
                            result.ScenarioCount = run.ScenarioCount;
                            result.EventCountPerMeasuredIteration = run.EventCountPerMeasuredIteration;
                        }
                    }

                    result.ModeSummaries.Add(SchedulerModeSummary.FromRuns(schedulerMode, runs));
                }

                result.FinishedUtc = DateTimeOffset.UtcNow;
                result.FinishedJst = ToJst(result.FinishedUtc);
                result.TotalMeasuredEventCount = result.EventCountPerMeasuredIteration * measuredIterations * repeatRunsPerScheduler * schedulerModes.Length;
                result.ComponentComparisons = SchedulerComparisonSummaryCalculator.Calculate(result.ModeSummaries);
                result.SlowestSamples = slowSamples
                    .GroupBy(s => new { s.SchedulerMode, s.Component })
                    .SelectMany(g => g.OrderByDescending(s => s.ComponentMilliseconds).Take(5))
                    .OrderBy(s => s.SchedulerMode)
                    .ThenBy(s => s.Component)
                    .ThenByDescending(s => s.ComponentMilliseconds)
                    .ToList();
                return result;
            }

            private static IEnumerable<SchedulerSlowSample> CreateSlowSamples(
                string schedulerMode,
                int runIndex,
                AttributionRunResult run)
            {
                foreach (var sample in run.Samples)
                {
                    yield return SchedulerSlowSample.FromSample(schedulerMode, runIndex, "raw_input", sample.TotalInputMilliseconds, sample);
                    yield return SchedulerSlowSample.FromSample(schedulerMode, runIndex, "observation_wait", sample.ObservationWaitMilliseconds, sample);
                }
            }

            private static string GetRepositoryRoot()
            {
                var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                return assemblyDirectory.Parent.Parent.Parent.FullName;
            }

            private static string GetBranchName()
            {
                var repositoryRoot = GetRepositoryRoot();
                var headPath = Path.Combine(repositoryRoot, ".git", "HEAD");
                if (!File.Exists(headPath))
                {
                    return "";
                }

                var head = File.ReadAllText(headPath).Trim();
                const string prefix = "ref: refs/heads/";
                return head.StartsWith(prefix, StringComparison.Ordinal) ? head.Substring(prefix.Length) : head;
            }

            private static DateTimeOffset ToJst(DateTimeOffset utc)
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    return TimeZoneInfo.ConvertTime(utc, timeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
                catch (InvalidTimeZoneException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
            }
        }

        private static class GestureIntervalComparisonMeasurer
        {
            public static SchedulerComparisonRunResult Measure(int warmupIterations, int measuredIterations, int repeatRunsPerMode)
            {
                var startedUtc = DateTimeOffset.UtcNow;
                var modes = ReplayIntervalComparisonMode.CreateStep09Modes(ProductionStrokeWatchIntervalMilliseconds);
                var scenarios = PerformanceScenarioFactory.CreateProductionIntervalScenarios();
                var result = new SchedulerComparisonRunResult
                {
                    Step = "09-production-interval-measurement",
                    Repository = GetRepositoryRoot(),
                    Branch = GetBranchName(),
                    StartedUtc = startedUtc,
                    StartedJst = ToJst(startedUtc),
                    WarmupIterations = warmupIterations,
                    MeasuredIterations = measuredIterations,
                    RepeatRunsPerScheduler = repeatRunsPerMode,
                    TimingNote = "Step 09 interval comparison measurement; compares StrokeWatchInterval 0 and production default without hard pass/fail timing thresholds. Interval > 0 replay uses production-paced point delivery to preserve semantic validation.",
                    Environment = EnvironmentSnapshot.Capture(),
                    ScenarioInventory = scenarios
                        .Select(ScenarioInventoryEntry.FromScenario)
                        .ToList(),
                };

                var slowSamples = new List<SchedulerSlowSample>();
                foreach (var mode in modes)
                {
                    var runs = new List<AttributionRunResult>();
                    for (var runIndex = 0; runIndex < repeatRunsPerMode; runIndex++)
                    {
                        var run = GestureAttributionMeasurer.Measure(
                            warmupIterations,
                            measuredIterations,
                            mode.SchedulerMode,
                            mode.StrokeWatchIntervalMilliseconds,
                            "09-production-interval-measurement",
                            "Step 09 attribution run for interval/scheduler comparison; no hard pass/fail timing threshold is applied.",
                            scenarios);
                        runs.Add(run);
                        slowSamples.AddRange(CreateSlowSamples(mode.Name, runIndex, run));

                        if (result.ScenarioCount == 0)
                        {
                            result.ScenarioCount = run.ScenarioCount;
                            result.EventCountPerMeasuredIteration = run.EventCountPerMeasuredIteration;
                        }
                    }

                    result.ModeSummaries.Add(SchedulerModeSummary.FromRuns(mode, runs));
                }

                result.FinishedUtc = DateTimeOffset.UtcNow;
                result.FinishedJst = ToJst(result.FinishedUtc);
                result.TotalMeasuredEventCount = result.EventCountPerMeasuredIteration * measuredIterations * repeatRunsPerMode * modes.Count;
                result.ComponentComparisons = SchedulerComparisonSummaryCalculator.Calculate(result.ModeSummaries);
                result.SlowestSamples = slowSamples
                    .GroupBy(s => new { s.SchedulerMode, s.Component })
                    .SelectMany(g => g.OrderByDescending(s => s.ComponentMilliseconds).Take(5))
                    .OrderBy(s => s.SchedulerMode)
                    .ThenBy(s => s.Component)
                    .ThenByDescending(s => s.ComponentMilliseconds)
                    .ToList();
                return result;
            }

            private static IEnumerable<SchedulerSlowSample> CreateSlowSamples(
                string modeName,
                int runIndex,
                AttributionRunResult run)
            {
                foreach (var sample in run.Samples)
                {
                    yield return SchedulerSlowSample.FromSample(modeName, runIndex, "raw_input", sample.TotalInputMilliseconds, sample);
                    yield return SchedulerSlowSample.FromSample(modeName, runIndex, "observation_wait", sample.ObservationWaitMilliseconds, sample);
                }
            }

            private static string GetRepositoryRoot()
            {
                var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                return assemblyDirectory.Parent.Parent.Parent.FullName;
            }

            private static string GetBranchName()
            {
                var repositoryRoot = GetRepositoryRoot();
                var headPath = Path.Combine(repositoryRoot, ".git", "HEAD");
                if (!File.Exists(headPath))
                {
                    return "";
                }

                var head = File.ReadAllText(headPath).Trim();
                const string prefix = "ref: refs/heads/";
                return head.StartsWith(prefix, StringComparison.Ordinal) ? head.Substring(prefix.Length) : head;
            }

            private static DateTimeOffset ToJst(DateTimeOffset utc)
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    return TimeZoneInfo.ConvertTime(utc, timeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
                catch (InvalidTimeZoneException)
                {
                    return utc.ToOffset(TimeSpan.FromHours(9));
                }
            }
        }

        private sealed class ReplayIntervalComparisonMode
        {
            public string Name { get; private set; }
            public string Description { get; private set; }
            public string MeasurementRole { get; private set; }
            public int StrokeWatchIntervalMilliseconds { get; private set; }
            public ReplaySchedulerMode SchedulerMode { get; private set; }

            public static IReadOnlyList<ReplayIntervalComparisonMode> CreateStep09Modes(int productionStrokeWatchIntervalMilliseconds)
            {
                return new[]
                {
                    Create("primary", 0, ReplaySchedulerMode.TaskFactoryDefault),
                    Create("primary", 0, ReplaySchedulerMode.ProductionLikeLowLatency),
                    Create("primary", productionStrokeWatchIntervalMilliseconds, ReplaySchedulerMode.TaskFactoryDefault),
                    Create("primary", productionStrokeWatchIntervalMilliseconds, ReplaySchedulerMode.ProductionLikeLowLatency),
                    Create("exploratory", productionStrokeWatchIntervalMilliseconds, ReplaySchedulerMode.ProductionLikeLowLatencyPool4),
                };
            }

            private static ReplayIntervalComparisonMode Create(
                string measurementRole,
                int strokeWatchIntervalMilliseconds,
                ReplaySchedulerMode schedulerMode)
            {
                return new ReplayIntervalComparisonMode
                {
                    Name = "interval-" + strokeWatchIntervalMilliseconds + "-" + schedulerMode.Name,
                    Description = measurementRole + " interval comparison: StrokeWatchInterval = " + strokeWatchIntervalMilliseconds + " ms with " + schedulerMode.Description,
                    MeasurementRole = measurementRole,
                    StrokeWatchIntervalMilliseconds = strokeWatchIntervalMilliseconds,
                    SchedulerMode = schedulerMode,
                };
            }
        }

        private sealed class ReplaySchedulerMode
        {
            private readonly Func<ReplaySchedulerLease> leaseFactory;

            public string Name { get; private set; }
            public string Description { get; private set; }

            public static readonly ReplaySchedulerMode TaskFactoryDefault = new ReplaySchedulerMode(
                "task-factory-default",
                "Current test replay scheduler: Task.Factory.",
                () => new ReplaySchedulerLease(Task.Factory, null));

            public static readonly ReplaySchedulerMode ProductionLikeLowLatency = CreateProductionLikeLowLatency(
                "production-like-low-latency",
                "Step07StrokeWatcherTaskScheduler",
                2);

            public static readonly ReplaySchedulerMode ProductionLikeLowLatencyPool4 = CreateProductionLikeLowLatency(
                "production-like-low-latency-pool4",
                "Step09StrokeWatcherTaskSchedulerPool4",
                4);

            private static ReplaySchedulerMode CreateProductionLikeLowLatency(string name, string schedulerName, int poolSize)
            {
                return new ReplaySchedulerMode(
                    name,
                    "Production-like replay scheduler: LowLatencyScheduler(\"" + schedulerName + "\", ThreadPriority.Highest, " + poolSize + ").",
                    () =>
                    {
                        var scheduler = new LowLatencyScheduler(schedulerName, ThreadPriority.Highest, poolSize);
                        return new ReplaySchedulerLease(new TaskFactory(scheduler), scheduler);
                    });
            }

            private ReplaySchedulerMode(string name, string description, Func<ReplaySchedulerLease> leaseFactory)
            {
                Name = name;
                Description = description;
                this.leaseFactory = leaseFactory;
            }

            public ReplaySchedulerLease CreateLease()
                => leaseFactory();
        }

        private sealed class ReplaySchedulerLease : IDisposable
        {
            private readonly IDisposable disposable;

            public TaskFactory TaskFactory { get; private set; }

            public ReplaySchedulerLease(TaskFactory taskFactory, IDisposable disposable)
            {
                TaskFactory = taskFactory;
                this.disposable = disposable;
            }

            public void Dispose()
            {
                disposable?.Dispose();
            }
        }

        private static class PerformanceScenarioFactory
        {
            public static List<PerformanceScenario> CreateScenarios()
            {
                var scenarios = new List<PerformanceScenario>();

                scenarios.AddRange(ExpectedDefaultGestures.Wheel.Select(CreateWheelScenario));
                scenarios.AddRange(ExpectedDefaultGestures.Stroke
                    .Where(g => g.Label.Length == 1)
                    .Select(g => CreateStrokeScenario("single-stroke", MotionPatternGenerator.Generate(g), g.Label)));
                scenarios.AddRange(ExpectedDefaultGestures.Stroke
                    .Where(g => g.Label.Length > 1)
                    .Select(g => CreateStrokeScenario("multi-stroke", MotionPatternGenerator.Generate(g), g.Label)));
                scenarios.AddRange(MotionPatternGenerator.GeneratePositiveStrokeScenarios()
                    .Select(s => CreateStrokeScenario("generated-positive", s, s.ExpectedStroke)));
                scenarios.AddRange(MotionPatternGenerator.GenerateNegativeStrokeScenarios()
                    .Select(s => CreateStrokeScenario("generated-negative", s, null)));

                return scenarios;
            }

            public static List<PerformanceScenario> CreateProductionIntervalScenarios()
            {
                var scenarios = new List<PerformanceScenario>();

                scenarios.AddRange(ExpectedDefaultGestures.Wheel.Select(CreateWheelScenario));
                scenarios.AddRange(ExpectedDefaultGestures.Stroke
                    .Select(g => CreateStrokeScenario("production-interval-baseline", MotionPatternGenerator.Generate(g), g.Label)));

                return scenarios;
            }

            private static PerformanceScenario CreateWheelScenario(DefaultGesture gesture)
            {
                return new PerformanceScenario
                {
                    Group = "wheel",
                    Id = "wheel-" + gesture.Label,
                    Category = "positive-wheel",
                    ExpectedStroke = "",
                    ExpectedHandler = gesture.Label,
                    EventCount = 3,
                    PointCount = 1,
                    ExpectedPressConsumed = true,
                    ExpectedReleaseConsumed = true,
                    ExpectsWheelTrigger = true,
                    Gesture = gesture,
                    Replay = machine => GestureReplayHarness.ReplayWheelGesture(machine, gesture),
                };
            }

            private static PerformanceScenario CreateStrokeScenario(string group, MotionScenario scenario, string expectedHandler)
            {
                return new PerformanceScenario
                {
                    Group = group,
                    Id = scenario.Id,
                    Category = scenario.Category,
                    ExpectedStroke = scenario.ExpectedStroke,
                    ExpectedHandler = expectedHandler,
                    EventCount = scenario.Points.Count + 1,
                    PointCount = scenario.Points.Count,
                    ExpectedPressConsumed = true,
                    ExpectedReleaseConsumed = true,
                    ExpectsWheelTrigger = false,
                    MotionScenario = scenario,
                    WaitForStrokeBeforeRelease = scenario.ExpectedStroke.Length > 0,
                    Replay = machine => GestureReplayHarness.ReplayStrokeScenario(
                        machine,
                        scenario,
                        waitForStrokeBeforeRelease: scenario.ExpectedStroke.Length > 0),
                };
            }
        }

        private static ReplayGestureMachine CreateReplayGestureMachine(
            ExecutionRecorder recorder,
            IEnumerable<DefaultGesture> gestures)
            => CreateReplayGestureMachine(recorder, gestures, ReplaySchedulerMode.TaskFactoryDefault);

        private static ReplayGestureMachine CreateReplayGestureMachine(
            ExecutionRecorder recorder,
            IEnumerable<DefaultGesture> gestures,
            ReplaySchedulerMode schedulerMode)
            => CreateReplayGestureMachine(recorder, gestures, schedulerMode, 0);

        private static ReplayGestureMachine CreateReplayGestureMachine(
            ExecutionRecorder recorder,
            IEnumerable<DefaultGesture> gestures,
            ReplaySchedulerMode schedulerMode,
            int strokeWatchIntervalMilliseconds)
        {
            var root = new RootElement<ReplayEvaluationContext, ReplayExecutionContext>();
            var rightButton = root.When(ctx => true).On(SupportedKeys.Keys.RButton);

            foreach (var gesture in gestures)
            {
                var captured = gesture;
                if (captured.Kind == DefaultGestureKind.Wheel)
                {
                    rightButton.On(captured.WheelKey).Do(ctx => recorder.Record(captured.Label), captured.Label);
                }
                else
                {
                    rightButton.On(ParseStrokeSequence(captured.Label).ToArray()).Do(ctx => recorder.Record(captured.Label), captured.Label);
                }
            }

            var config = new ReplayGestureMachineConfig
            {
                GestureTimeout = 0,
                StrokeWatchInterval = strokeWatchIntervalMilliseconds,
            };
            var callbackManager = new ReplayCallbackManager();
            var gestureMachine = new ReplayGestureMachine(config, callbackManager, schedulerMode.CreateLease());
            gestureMachine.Run(root);
            return gestureMachine;
        }

        private static StrokeSequence ParseStrokeSequence(string value)
        {
            return new StrokeSequence(value.Select(ParseStrokeDirection));
        }

        private static StrokeDirection ParseStrokeDirection(char value)
        {
            switch (value)
            {
                case 'U': return StrokeDirection.Up;
                case 'D': return StrokeDirection.Down;
                case 'L': return StrokeDirection.Left;
                case 'R': return StrokeDirection.Right;
                default: throw new ArgumentException("Unknown stroke direction: " + value);
            }
        }

        private static double TicksToMilliseconds(long ticks)
            => ticks * 1000.0 / Stopwatch.Frequency;

        private static bool WaitUntil(Func<bool> condition, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < timeout)
            {
                if (condition())
                {
                    return true;
                }

                Thread.Sleep(1);
            }

            return condition();
        }

        private static class GestureReplayHarness
        {
            public static GestureReplayResult ReplayWheelGesture(
                ReplayGestureMachine gestureMachine,
                DefaultGesture gesture)
            {
                var point = new Point(400, 400);
                return new GestureReplayResult
                {
                    PressConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent, point),
                    TriggerConsumed = gestureMachine.Input(gesture.WheelKey.FireEvent, point),
                    ReleaseConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent, point),
                    ObservedStroke = "",
                };
            }

            public static GestureReplayResult ReplayStrokeScenario(
                ReplayGestureMachine gestureMachine,
                MotionScenario scenario,
                bool waitForStrokeBeforeRelease)
            {
                var result = new GestureReplayResult();
                var nullEvent = new NullEvent();

                result.PressConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent, scenario.Points.First());

                foreach (var point in scenario.Points.Skip(1))
                {
                    gestureMachine.Input(nullEvent, point);
                }

                if (waitForStrokeBeforeRelease)
                {
                    var expected = ParseStrokeSequence(scenario.ExpectedStroke);
                    result.StrokeObservedBeforeRelease = WaitUntil(
                        () => gestureMachine.StrokeWatcher.GetStrokeSequence().Equals(expected),
                        StrokeProcessingTimeout);
                }
                else
                {
                    WaitUntil(
                        () => gestureMachine.StrokeWatcher.GetBufferedPoints().Count >= Math.Max(0, scenario.Points.Count - 1),
                        StrokeProcessingTimeout);
                    result.StrokeObservedBeforeRelease = false;
                }

                result.ObservedStroke = gestureMachine.StrokeWatcher.GetStrokeSequence().ToString();
                result.ReleaseConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent, scenario.Points.Last());
                return result;
            }
        }

        private static class AttributionReplayHarness
        {
            public static AttributionReplayResult ReplayScenario(
                ReplayGestureMachine gestureMachine,
                PerformanceScenario scenario,
                int strokeWatchIntervalMilliseconds)
            {
                return scenario.ExpectsWheelTrigger
                    ? ReplayWheelGesture(gestureMachine, scenario.Gesture)
                    : ReplayStrokeScenario(gestureMachine, scenario.MotionScenario, scenario.WaitForStrokeBeforeRelease, strokeWatchIntervalMilliseconds);
            }

            private static AttributionReplayResult ReplayWheelGesture(
                ReplayGestureMachine gestureMachine,
                DefaultGesture gesture)
            {
                var point = new Point(400, 400);
                var result = new AttributionReplayResult();
                var totalStart = Stopwatch.GetTimestamp();
                long elapsedTicks;

                result.PressConsumed = MeasureInput(
                    () => gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent, point),
                    out elapsedTicks);
                result.PressInputTicks += elapsedTicks;

                result.TriggerConsumed = MeasureInput(
                    () => gestureMachine.Input(gesture.WheelKey.FireEvent, point),
                    out elapsedTicks);
                result.TriggerInputTicks += elapsedTicks;

                result.ReleaseConsumed = MeasureInput(
                    () => gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent, point),
                    out elapsedTicks);
                result.ReleaseInputTicks += elapsedTicks;

                result.ObservedStroke = "";
                result.TotalReplayTicks = Stopwatch.GetTimestamp() - totalStart;
                return result;
            }

            private static AttributionReplayResult ReplayStrokeScenario(
                ReplayGestureMachine gestureMachine,
                MotionScenario scenario,
                bool waitForStrokeBeforeRelease,
                int strokeWatchIntervalMilliseconds)
            {
                var result = new AttributionReplayResult();
                var nullEvent = new NullEvent();
                var totalStart = Stopwatch.GetTimestamp();
                long elapsedTicks;

                result.PressConsumed = MeasureInput(
                    () => gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent, scenario.Points.First()),
                    out elapsedTicks);
                result.PressInputTicks += elapsedTicks;

                foreach (var indexedPoint in scenario.Points.Skip(1).Select((point, index) => new { Point = point, SegmentIndex = index + 1 }))
                {
                    MeasureInput(
                        () => gestureMachine.Input(nullEvent, indexedPoint.Point),
                        out elapsedTicks);
                    result.MoveInputTicks += elapsedTicks;

                    if (strokeWatchIntervalMilliseconds > 0)
                    {
                        Thread.Sleep(strokeWatchIntervalMilliseconds + 1);
                    }
                }

                var waitStart = Stopwatch.GetTimestamp();
                if (waitForStrokeBeforeRelease)
                {
                    var expected = ParseStrokeSequence(scenario.ExpectedStroke);
                    result.StrokeObservedBeforeRelease = WaitUntil(
                        () => gestureMachine.StrokeWatcher.GetStrokeSequence().Equals(expected),
                        StrokeProcessingTimeout);
                }
                else
                {
                    if (strokeWatchIntervalMilliseconds <= 0)
                    {
                        WaitUntil(
                            () => gestureMachine.StrokeWatcher.GetBufferedPoints().Count >= Math.Max(0, scenario.Points.Count - 1),
                            StrokeProcessingTimeout);
                    }
                    else
                    {
                        WaitUntil(
                            () => gestureMachine.StrokeWatcher.StrokeIsEstablished || gestureMachine.StrokeWatcher.GetBufferedPoints().Count > 0,
                            StrokeProcessingTimeout);
                    }

                    result.StrokeObservedBeforeRelease = false;
                }
                result.ObservationWaitTicks = Stopwatch.GetTimestamp() - waitStart;

                var readStart = Stopwatch.GetTimestamp();
                result.ObservedStroke = gestureMachine.StrokeWatcher.GetStrokeSequence().ToString();
                result.ObservationReadTicks = Stopwatch.GetTimestamp() - readStart;

                result.ReleaseConsumed = MeasureInput(
                    () => gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent, scenario.Points.Last()),
                    out elapsedTicks);
                result.ReleaseInputTicks += elapsedTicks;

                result.TotalReplayTicks = Stopwatch.GetTimestamp() - totalStart;
                return result;
            }

            private static bool MeasureInput(Func<bool> input, out long elapsedTicks)
            {
                var start = Stopwatch.GetTimestamp();
                var result = input();
                elapsedTicks = Stopwatch.GetTimestamp() - start;
                return result;
            }
        }

        private enum DefaultGestureKind
        {
            Wheel,
            Stroke,
        }

        private sealed class DefaultGesture
        {
            public DefaultGestureKind Kind { get; private set; }
            public string Label { get; private set; }
            public Crevice.Core.Keys.PhysicalSingleThrowKey WheelKey { get; private set; }

            public static DefaultGesture Wheel(string label, Crevice.Core.Keys.PhysicalSingleThrowKey wheelKey)
                => new DefaultGesture
                {
                    Kind = DefaultGestureKind.Wheel,
                    Label = label,
                    WheelKey = wheelKey,
                };

            public static DefaultGesture Stroke(string label)
                => new DefaultGesture
                {
                    Kind = DefaultGestureKind.Stroke,
                    Label = label,
                };
        }

        private static class ExpectedDefaultGestures
        {
            public static readonly IReadOnlyList<DefaultGesture> Wheel = new[]
            {
                DefaultGesture.Wheel("WheelUp", SupportedKeys.PhysicalKeys.WheelUp),
                DefaultGesture.Wheel("WheelDown", SupportedKeys.PhysicalKeys.WheelDown),
            };

            public static readonly IReadOnlyList<DefaultGesture> Stroke = new[]
            {
                DefaultGesture.Stroke("U"),
                DefaultGesture.Stroke("D"),
                DefaultGesture.Stroke("L"),
                DefaultGesture.Stroke("R"),
                DefaultGesture.Stroke("UD"),
                DefaultGesture.Stroke("DR"),
            };

            public static readonly IReadOnlyList<DefaultGesture> All = Wheel.Concat(Stroke).ToList();
        }

        private sealed class ExecutionRecorder
        {
            private readonly List<string> labels = new List<string>();

            public int TotalCount => labels.Count;

            public void Record(string label)
            {
                labels.Add(label);
            }

            public int Count(string label)
            {
                return labels.Count(x => x == label);
            }
        }

        private sealed class MotionScenario
        {
            public string Id { get; set; }
            public string Category { get; set; }
            public string ExpectedStroke { get; set; }
            public Point Start { get; set; }
            public int Distance { get; set; }
            public int Subdivisions { get; set; }
            public int JitterAmplitude { get; set; }
            public List<Point> Points { get; set; }
        }

        private static class MotionPatternGenerator
        {
            public static readonly IReadOnlyList<Point> StartCoordinates = new[]
            {
                new Point(400, 400),
                new Point(2600, 540),
                new Point(-800, 360),
            };

            public static readonly IReadOnlyList<int> AboveThresholdDistances = new[]
            {
                32,
                48,
                72,
            };

            public static readonly IReadOnlyList<int> PointDensities = new[]
            {
                2,
                4,
                7,
            };

            public static readonly IReadOnlyList<int> JitterAmplitudes = new[]
            {
                0,
                3,
            };

            public static MotionScenario Generate(DefaultGesture gesture)
            {
                return new MotionScenario
                {
                    Id = "baseline-" + gesture.Label,
                    Category = "positive-baseline",
                    ExpectedStroke = gesture.Label,
                    Start = new Point(400, 400),
                    Distance = 64,
                    Subdivisions = 4,
                    JitterAmplitude = 0,
                    Points = GeneratePoints(new Point(400, 400), ParseStrokeSequence(gesture.Label), 64, 4, 0),
                };
            }

            public static MotionScenario GenerateBelowThresholdMovement()
            {
                return new MotionScenario
                {
                    Id = "baseline-below-threshold",
                    Category = "negative-below-threshold",
                    ExpectedStroke = "",
                    Start = new Point(400, 400),
                    Distance = 10,
                    Subdivisions = 3,
                    JitterAmplitude = 0,
                    Points = new List<Point>
                    {
                        new Point(400, 400),
                        new Point(405, 400),
                        new Point(405, 405),
                    },
                };
            }

            public static IEnumerable<MotionScenario> GeneratePositiveStrokeScenarios()
            {
                foreach (var gesture in ExpectedDefaultGestures.Stroke)
                {
                    foreach (var start in StartCoordinates)
                    {
                        foreach (var distance in AboveThresholdDistances)
                        {
                            foreach (var subdivisions in PointDensities)
                            {
                                foreach (var jitterAmplitude in JitterAmplitudes)
                                {
                                    yield return CreateScenario(
                                        "positive",
                                        "positive-" + gesture.Label + "-start-" + start.X + "-" + start.Y + "-distance-" + distance + "-density-" + subdivisions + "-jitter-" + jitterAmplitude,
                                        gesture.Label,
                                        start,
                                        distance,
                                        subdivisions,
                                        jitterAmplitude);
                                }
                            }
                        }
                    }
                }
            }

            public static IEnumerable<MotionScenario> GenerateNegativeStrokeScenarios()
            {
                yield return GenerateBelowThresholdMovement();
                yield return CreateScenario("negative-unknown", "negative-unknown-RL", "RL", new Point(400, 400), 64, 4, 0);
                yield return CreateScenario("negative-unknown", "negative-unknown-LR", "LR", new Point(2600, 540), 64, 4, 3);
                yield return CreateScenario("negative-noisy", "negative-noisy-URDL", "URDL", new Point(-800, 360), 48, 6, 4);
            }

            private static MotionScenario CreateScenario(
                string category,
                string id,
                string expectedStroke,
                Point start,
                int distance,
                int subdivisions,
                int jitterAmplitude)
            {
                return new MotionScenario
                {
                    Id = id,
                    Category = category,
                    ExpectedStroke = expectedStroke,
                    Start = start,
                    Distance = distance,
                    Subdivisions = subdivisions,
                    JitterAmplitude = jitterAmplitude,
                    Points = GeneratePoints(start, ParseStrokeSequence(expectedStroke), distance, subdivisions, jitterAmplitude),
                };
            }

            private static List<Point> GeneratePoints(
                Point start,
                StrokeSequence strokes,
                int distance,
                int subdivisions,
                int jitterAmplitude)
            {
                var points = new List<Point> { start };
                var current = start;
                foreach (var stroke in strokes)
                {
                    var segmentStart = current;
                    for (var step = 1; step <= subdivisions; step++)
                    {
                        var along = (int)Math.Round(distance * step / (double)subdivisions);
                        var jitter = step == subdivisions ? 0 : ((step % 2 == 0) ? -jitterAmplitude : jitterAmplitude);
                        points.Add(Offset(segmentStart, stroke, along, jitter));
                    }

                    current = points.Last();
                }

                return points;
            }

            private static Point Offset(Point point, StrokeDirection direction, int distance, int orthogonalJitter)
            {
                switch (direction)
                {
                    case StrokeDirection.Up: return new Point(point.X + orthogonalJitter, point.Y - distance);
                    case StrokeDirection.Down: return new Point(point.X + orthogonalJitter, point.Y + distance);
                    case StrokeDirection.Left: return new Point(point.X - distance, point.Y + orthogonalJitter);
                    case StrokeDirection.Right: return new Point(point.X + distance, point.Y + orthogonalJitter);
                    default: throw new ArgumentException("Unknown stroke direction: " + direction);
                }
            }
        }

        private sealed class PerformanceScenario
        {
            public string Group { get; set; }
            public string Id { get; set; }
            public string Category { get; set; }
            public string ExpectedStroke { get; set; }
            public string ExpectedHandler { get; set; }
            public int EventCount { get; set; }
            public int PointCount { get; set; }
            public bool ExpectedPressConsumed { get; set; }
            public bool ExpectedReleaseConsumed { get; set; }
            public bool ExpectsWheelTrigger { get; set; }
            public DefaultGesture Gesture { get; set; }
            public MotionScenario MotionScenario { get; set; }
            public bool WaitForStrokeBeforeRelease { get; set; }
            public Func<ReplayGestureMachine, GestureReplayResult> Replay { get; set; }
        }

        private class GestureReplayResult
        {
            public bool PressConsumed { get; set; }
            public bool TriggerConsumed { get; set; }
            public bool ReleaseConsumed { get; set; }
            public bool StrokeObservedBeforeRelease { get; set; }
            public string ObservedStroke { get; set; }
        }

        private sealed class AttributionReplayResult : GestureReplayResult
        {
            public long TotalReplayTicks { get; set; }
            public long TotalInputTicks => PressInputTicks + MoveInputTicks + TriggerInputTicks + ReleaseInputTicks;
            public long PressInputTicks { get; set; }
            public long MoveInputTicks { get; set; }
            public long TriggerInputTicks { get; set; }
            public long ReleaseInputTicks { get; set; }
            public long ObservationWaitTicks { get; set; }
            public long ObservationReadTicks { get; set; }
        }

        private sealed class PerformanceRunResult
        {
            public string Step { get; set; }
            public string Repository { get; set; }
            public string Branch { get; set; }
            public DateTimeOffset StartedUtc { get; set; }
            public DateTimeOffset StartedJst { get; set; }
            public DateTimeOffset FinishedUtc { get; set; }
            public DateTimeOffset FinishedJst { get; set; }
            public int WarmupIterations { get; set; }
            public int MeasuredIterations { get; set; }
            public int ScenarioCount { get; set; }
            public int EventCountPerMeasuredIteration { get; set; }
            public int TotalMeasuredEventCount { get; set; }
            public string TimingNote { get; set; }
            public EnvironmentSnapshot Environment { get; set; }
            public List<ScenarioInventoryEntry> ScenarioInventory { get; set; }
            public List<PerformanceSample> Samples { get; private set; }
            public List<PerformanceSummary> Summaries { get; set; }
            public List<PerformanceSummary> PerScenarioSummaries { get; set; }

            public PerformanceRunResult()
            {
                Samples = new List<PerformanceSample>();
                ScenarioInventory = new List<ScenarioInventoryEntry>();
                Summaries = new List<PerformanceSummary>();
                PerScenarioSummaries = new List<PerformanceSummary>();
            }
        }

        private sealed class AttributionRunResult
        {
            public string Step { get; set; }
            public string Repository { get; set; }
            public string Branch { get; set; }
            public DateTimeOffset StartedUtc { get; set; }
            public DateTimeOffset StartedJst { get; set; }
            public DateTimeOffset FinishedUtc { get; set; }
            public DateTimeOffset FinishedJst { get; set; }
            public int WarmupIterations { get; set; }
            public int MeasuredIterations { get; set; }
            public int ScenarioCount { get; set; }
            public int EventCountPerMeasuredIteration { get; set; }
            public int TotalMeasuredEventCount { get; set; }
            public string SchedulerMode { get; set; }
            public string SchedulerDescription { get; set; }
            public int StrokeWatchIntervalMilliseconds { get; set; }
            public string TimingNote { get; set; }
            public EnvironmentSnapshot Environment { get; set; }
            public List<ScenarioInventoryEntry> ScenarioInventory { get; set; }
            public List<AttributionSample> Samples { get; private set; }
            public List<AttributionSummary> Summaries { get; set; }
            public List<AttributionSummary> PerScenarioSummaries { get; set; }

            public AttributionRunResult()
            {
                Samples = new List<AttributionSample>();
                ScenarioInventory = new List<ScenarioInventoryEntry>();
                Summaries = new List<AttributionSummary>();
                PerScenarioSummaries = new List<AttributionSummary>();
            }
        }

        private sealed class RepeatabilityRunResult
        {
            public string Step { get; set; }
            public string Repository { get; set; }
            public string Branch { get; set; }
            public DateTimeOffset StartedUtc { get; set; }
            public DateTimeOffset StartedJst { get; set; }
            public DateTimeOffset FinishedUtc { get; set; }
            public DateTimeOffset FinishedJst { get; set; }
            public int WarmupIterations { get; set; }
            public int MeasuredIterations { get; set; }
            public int RepeatRuns { get; set; }
            public int ScenarioCount { get; set; }
            public int EventCountPerMeasuredIteration { get; set; }
            public int TotalMeasuredEventCount { get; set; }
            public string TimingNote { get; set; }
            public EnvironmentSnapshot Environment { get; set; }
            public List<RepeatabilityRunSummary> RunSummaries { get; private set; }
            public List<RepeatabilityComponentSummary> ComponentSummaries { get; set; }
            public List<RepeatabilitySlowSample> SlowestSamples { get; set; }

            public RepeatabilityRunResult()
            {
                RunSummaries = new List<RepeatabilityRunSummary>();
                ComponentSummaries = new List<RepeatabilityComponentSummary>();
                SlowestSamples = new List<RepeatabilitySlowSample>();
            }
        }

        private sealed class SchedulerComparisonRunResult
        {
            public string Step { get; set; }
            public string Repository { get; set; }
            public string Branch { get; set; }
            public DateTimeOffset StartedUtc { get; set; }
            public DateTimeOffset StartedJst { get; set; }
            public DateTimeOffset FinishedUtc { get; set; }
            public DateTimeOffset FinishedJst { get; set; }
            public int WarmupIterations { get; set; }
            public int MeasuredIterations { get; set; }
            public int RepeatRunsPerScheduler { get; set; }
            public int ScenarioCount { get; set; }
            public int EventCountPerMeasuredIteration { get; set; }
            public int TotalMeasuredEventCount { get; set; }
            public string TimingNote { get; set; }
            public EnvironmentSnapshot Environment { get; set; }
            public List<ScenarioInventoryEntry> ScenarioInventory { get; set; }
            public List<SchedulerModeSummary> ModeSummaries { get; private set; }
            public List<SchedulerComponentComparison> ComponentComparisons { get; set; }
            public List<SchedulerSlowSample> SlowestSamples { get; set; }

            public SchedulerComparisonRunResult()
            {
                ScenarioInventory = new List<ScenarioInventoryEntry>();
                ModeSummaries = new List<SchedulerModeSummary>();
                ComponentComparisons = new List<SchedulerComponentComparison>();
                SlowestSamples = new List<SchedulerSlowSample>();
            }
        }

        private sealed class SchedulerModeSummary
        {
            public string SchedulerMode { get; set; }
            public string SchedulerDescription { get; set; }
            public string MeasurementRole { get; set; }
            public int StrokeWatchIntervalMilliseconds { get; set; }
            public int RunCount { get; set; }
            public int SampleCount { get; set; }
            public int EventCount { get; set; }
            public double TotalMedianMilliseconds { get; set; }
            public double TotalP95Milliseconds { get; set; }
            public double TotalMaxMilliseconds { get; set; }
            public double TotalMeanMilliseconds { get; set; }
            public double InputMedianMilliseconds { get; set; }
            public double InputP95Milliseconds { get; set; }
            public double InputMaxMilliseconds { get; set; }
            public double InputMeanMilliseconds { get; set; }
            public double WaitMedianMilliseconds { get; set; }
            public double WaitP95Milliseconds { get; set; }
            public double WaitMaxMilliseconds { get; set; }
            public double WaitMeanMilliseconds { get; set; }
            public int TotalAtOrAboveOneMillisecondCount { get; set; }
            public int InputAtOrAboveOneMillisecondCount { get; set; }
            public int WaitAtOrAboveOneMillisecondCount { get; set; }
            public List<RepeatabilityRunSummary> RunSummaries { get; private set; }

            public SchedulerModeSummary()
            {
                RunSummaries = new List<RepeatabilityRunSummary>();
            }

            public static SchedulerModeSummary FromRuns(ReplaySchedulerMode schedulerMode, IReadOnlyList<AttributionRunResult> runs)
                => FromRuns(schedulerMode.Name, schedulerMode.Description, "", 0, runs);

            public static SchedulerModeSummary FromRuns(ReplayIntervalComparisonMode mode, IReadOnlyList<AttributionRunResult> runs)
                => FromRuns(mode.Name, mode.Description, mode.MeasurementRole, mode.StrokeWatchIntervalMilliseconds, runs);

            private static SchedulerModeSummary FromRuns(
                string schedulerMode,
                string schedulerDescription,
                string measurementRole,
                int strokeWatchIntervalMilliseconds,
                IReadOnlyList<AttributionRunResult> runs)
            {
                var samples = runs.SelectMany(r => r.Samples).ToList();
                var total = samples.Select(s => s.TotalReplayMilliseconds).OrderBy(x => x).ToList();
                var input = samples.Select(s => s.TotalInputMilliseconds).OrderBy(x => x).ToList();
                var wait = samples.Select(s => s.ObservationWaitMilliseconds).OrderBy(x => x).ToList();

                return new SchedulerModeSummary
                {
                    SchedulerMode = schedulerMode,
                    SchedulerDescription = schedulerDescription,
                    MeasurementRole = measurementRole,
                    StrokeWatchIntervalMilliseconds = strokeWatchIntervalMilliseconds,
                    RunCount = runs.Count,
                    SampleCount = samples.Count,
                    EventCount = runs.Sum(r => r.TotalMeasuredEventCount),
                    TotalMedianMilliseconds = Percentile(total, 0.50),
                    TotalP95Milliseconds = Percentile(total, 0.95),
                    TotalMaxMilliseconds = total.Last(),
                    TotalMeanMilliseconds = total.Average(),
                    InputMedianMilliseconds = Percentile(input, 0.50),
                    InputP95Milliseconds = Percentile(input, 0.95),
                    InputMaxMilliseconds = input.Last(),
                    InputMeanMilliseconds = input.Average(),
                    WaitMedianMilliseconds = Percentile(wait, 0.50),
                    WaitP95Milliseconds = Percentile(wait, 0.95),
                    WaitMaxMilliseconds = wait.Last(),
                    WaitMeanMilliseconds = wait.Average(),
                    TotalAtOrAboveOneMillisecondCount = samples.Count(s => s.TotalReplayMilliseconds >= 1.0),
                    InputAtOrAboveOneMillisecondCount = samples.Count(s => s.TotalInputMilliseconds >= 1.0),
                    WaitAtOrAboveOneMillisecondCount = samples.Count(s => s.ObservationWaitMilliseconds >= 1.0),
                    RunSummaries = runs
                        .Select((run, index) => RepeatabilityRunSummary.FromRun(index, run))
                        .ToList(),
                };
            }

            private static double Percentile(IReadOnlyList<double> ordered, double percentile)
            {
                if (ordered.Count == 0)
                {
                    return 0;
                }

                var rank = (int)Math.Ceiling(percentile * ordered.Count) - 1;
                rank = Math.Max(0, Math.Min(rank, ordered.Count - 1));
                return ordered[rank];
            }
        }

        private sealed class SchedulerComponentComparison
        {
            public string Component { get; set; }
            public string SchedulerMode { get; set; }
            public double MedianMilliseconds { get; set; }
            public double P95Milliseconds { get; set; }
            public double MaxMilliseconds { get; set; }
            public double MeanMilliseconds { get; set; }
            public int AtOrAboveOneMillisecondCount { get; set; }
        }

        private sealed class SchedulerSlowSample
        {
            public string SchedulerMode { get; set; }
            public int RunIndex { get; set; }
            public string Component { get; set; }
            public double ComponentMilliseconds { get; set; }
            public int Iteration { get; set; }
            public string Group { get; set; }
            public string ScenarioId { get; set; }
            public string Category { get; set; }
            public double TotalReplayMilliseconds { get; set; }
            public double TotalInputMilliseconds { get; set; }
            public double ObservationWaitMilliseconds { get; set; }
            public double PressInputMilliseconds { get; set; }
            public double MoveInputMilliseconds { get; set; }
            public double ReleaseInputMilliseconds { get; set; }
            public string ObservedStroke { get; set; }

            public static SchedulerSlowSample FromSample(
                string schedulerMode,
                int runIndex,
                string component,
                double componentMilliseconds,
                AttributionSample sample)
            {
                return new SchedulerSlowSample
                {
                    SchedulerMode = schedulerMode,
                    RunIndex = runIndex,
                    Component = component,
                    ComponentMilliseconds = componentMilliseconds,
                    Iteration = sample.Iteration,
                    Group = sample.Group,
                    ScenarioId = sample.ScenarioId,
                    Category = sample.Category,
                    TotalReplayMilliseconds = sample.TotalReplayMilliseconds,
                    TotalInputMilliseconds = sample.TotalInputMilliseconds,
                    ObservationWaitMilliseconds = sample.ObservationWaitMilliseconds,
                    PressInputMilliseconds = sample.PressInputMilliseconds,
                    MoveInputMilliseconds = sample.MoveInputMilliseconds,
                    ReleaseInputMilliseconds = sample.ReleaseInputMilliseconds,
                    ObservedStroke = sample.ObservedStroke,
                };
            }
        }

        private sealed class RepeatabilityRunSummary
        {
            public int RunIndex { get; set; }
            public DateTimeOffset StartedUtc { get; set; }
            public DateTimeOffset StartedJst { get; set; }
            public DateTimeOffset FinishedUtc { get; set; }
            public DateTimeOffset FinishedJst { get; set; }
            public int SampleCount { get; set; }
            public int EventCount { get; set; }
            public double TotalMedianMilliseconds { get; set; }
            public double TotalP95Milliseconds { get; set; }
            public double TotalMaxMilliseconds { get; set; }
            public double TotalMeanMilliseconds { get; set; }
            public double InputMedianMilliseconds { get; set; }
            public double InputP95Milliseconds { get; set; }
            public double InputMaxMilliseconds { get; set; }
            public double InputMeanMilliseconds { get; set; }
            public double WaitMedianMilliseconds { get; set; }
            public double WaitP95Milliseconds { get; set; }
            public double WaitMaxMilliseconds { get; set; }
            public double WaitMeanMilliseconds { get; set; }
            public double ValidationMaxMilliseconds { get; set; }
            public int TotalAtOrAboveOneMillisecondCount { get; set; }
            public int InputAtOrAboveOneMillisecondCount { get; set; }
            public int WaitAtOrAboveOneMillisecondCount { get; set; }

            public static RepeatabilityRunSummary FromRun(int runIndex, AttributionRunResult run)
            {
                var overall = run.Summaries.First(s => s.Scope == "overall");
                return new RepeatabilityRunSummary
                {
                    RunIndex = runIndex,
                    StartedUtc = run.StartedUtc,
                    StartedJst = run.StartedJst,
                    FinishedUtc = run.FinishedUtc,
                    FinishedJst = run.FinishedJst,
                    SampleCount = run.Samples.Count,
                    EventCount = run.TotalMeasuredEventCount,
                    TotalMedianMilliseconds = overall.TotalMedianMilliseconds,
                    TotalP95Milliseconds = overall.TotalP95Milliseconds,
                    TotalMaxMilliseconds = overall.TotalMaxMilliseconds,
                    TotalMeanMilliseconds = overall.TotalMeanMilliseconds,
                    InputMedianMilliseconds = overall.InputMedianMilliseconds,
                    InputP95Milliseconds = overall.InputP95Milliseconds,
                    InputMaxMilliseconds = overall.InputMaxMilliseconds,
                    InputMeanMilliseconds = overall.InputMeanMilliseconds,
                    WaitMedianMilliseconds = overall.WaitMedianMilliseconds,
                    WaitP95Milliseconds = overall.WaitP95Milliseconds,
                    WaitMaxMilliseconds = overall.WaitMaxMilliseconds,
                    WaitMeanMilliseconds = overall.WaitMeanMilliseconds,
                    ValidationMaxMilliseconds = overall.ValidationMaxMilliseconds,
                    TotalAtOrAboveOneMillisecondCount = run.Samples.Count(s => s.TotalReplayMilliseconds >= 1.0),
                    InputAtOrAboveOneMillisecondCount = run.Samples.Count(s => s.TotalInputMilliseconds >= 1.0),
                    WaitAtOrAboveOneMillisecondCount = run.Samples.Count(s => s.ObservationWaitMilliseconds >= 1.0),
                };
            }
        }

        private sealed class RepeatabilityComponentSummary
        {
            public string Component { get; set; }
            public int RunCount { get; set; }
            public double RunMedianMinMilliseconds { get; set; }
            public double RunMedianMaxMilliseconds { get; set; }
            public double RunMedianRangeMilliseconds { get; set; }
            public double RunP95MinMilliseconds { get; set; }
            public double RunP95MaxMilliseconds { get; set; }
            public double RunP95RangeMilliseconds { get; set; }
            public double RunMaxMinMilliseconds { get; set; }
            public double RunMaxMaxMilliseconds { get; set; }
            public double RunMaxRangeMilliseconds { get; set; }
            public int AtOrAboveOneMillisecondCountTotal { get; set; }
        }

        private sealed class RepeatabilitySlowSample
        {
            public int RunIndex { get; set; }
            public string Component { get; set; }
            public double ComponentMilliseconds { get; set; }
            public int Iteration { get; set; }
            public string Group { get; set; }
            public string ScenarioId { get; set; }
            public string Category { get; set; }
            public double TotalReplayMilliseconds { get; set; }
            public double TotalInputMilliseconds { get; set; }
            public double ObservationWaitMilliseconds { get; set; }
            public double PressInputMilliseconds { get; set; }
            public double MoveInputMilliseconds { get; set; }
            public double ReleaseInputMilliseconds { get; set; }
            public string ObservedStroke { get; set; }

            public static RepeatabilitySlowSample FromSample(
                int runIndex,
                string component,
                double componentMilliseconds,
                AttributionSample sample)
            {
                return new RepeatabilitySlowSample
                {
                    RunIndex = runIndex,
                    Component = component,
                    ComponentMilliseconds = componentMilliseconds,
                    Iteration = sample.Iteration,
                    Group = sample.Group,
                    ScenarioId = sample.ScenarioId,
                    Category = sample.Category,
                    TotalReplayMilliseconds = sample.TotalReplayMilliseconds,
                    TotalInputMilliseconds = sample.TotalInputMilliseconds,
                    ObservationWaitMilliseconds = sample.ObservationWaitMilliseconds,
                    PressInputMilliseconds = sample.PressInputMilliseconds,
                    MoveInputMilliseconds = sample.MoveInputMilliseconds,
                    ReleaseInputMilliseconds = sample.ReleaseInputMilliseconds,
                    ObservedStroke = sample.ObservedStroke,
                };
            }
        }

        private sealed class EnvironmentSnapshot
        {
            public string MachineName { get; set; }
            public string UserName { get; set; }
            public string OSVersion { get; set; }
            public string CLRVersion { get; set; }
            public string ProcessArchitecture { get; set; }
            public bool Is64BitOperatingSystem { get; set; }
            public bool Is64BitProcess { get; set; }
            public string BuildConfiguration { get; set; }
            public string TestAssembly { get; set; }
            public long StopwatchFrequency { get; set; }
            public bool StopwatchIsHighResolution { get; set; }

            public static EnvironmentSnapshot Capture()
            {
                return new EnvironmentSnapshot
                {
                    MachineName = Environment.MachineName,
                    UserName = Environment.UserName,
                    OSVersion = Environment.OSVersion.ToString(),
                    CLRVersion = Environment.Version.ToString(),
                    ProcessArchitecture = IntPtr.Size == 8 ? "x64" : "x86",
                    Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                    Is64BitProcess = Environment.Is64BitProcess,
#if DEBUG
                    BuildConfiguration = "Debug",
#else
                    BuildConfiguration = "Release",
#endif
                    TestAssembly = Assembly.GetExecutingAssembly().Location,
                    StopwatchFrequency = Stopwatch.Frequency,
                    StopwatchIsHighResolution = Stopwatch.IsHighResolution,
                };
            }
        }

        private sealed class ScenarioInventoryEntry
        {
            public string Group { get; set; }
            public string Id { get; set; }
            public string Category { get; set; }
            public string ExpectedStroke { get; set; }
            public string ExpectedHandler { get; set; }
            public int EventCount { get; set; }
            public int PointCount { get; set; }

            public static ScenarioInventoryEntry FromScenario(PerformanceScenario scenario)
            {
                return new ScenarioInventoryEntry
                {
                    Group = scenario.Group,
                    Id = scenario.Id,
                    Category = scenario.Category,
                    ExpectedStroke = scenario.ExpectedStroke,
                    ExpectedHandler = scenario.ExpectedHandler,
                    EventCount = scenario.EventCount,
                    PointCount = scenario.PointCount,
                };
            }
        }

        private sealed class PerformanceSample
        {
            public int Iteration { get; set; }
            public string Group { get; set; }
            public string ScenarioId { get; set; }
            public string Category { get; set; }
            public string ExpectedStroke { get; set; }
            public string ExpectedHandler { get; set; }
            public int EventCount { get; set; }
            public int PointCount { get; set; }
            public long ElapsedTicks { get; set; }
            public double ElapsedMilliseconds { get; set; }
            public bool PressConsumed { get; set; }
            public bool TriggerConsumed { get; set; }
            public bool ReleaseConsumed { get; set; }
            public bool StrokeObservedBeforeRelease { get; set; }
            public string ObservedStroke { get; set; }
        }

        private sealed class AttributionSample
        {
            public int Iteration { get; set; }
            public string Group { get; set; }
            public string ScenarioId { get; set; }
            public string Category { get; set; }
            public string ExpectedStroke { get; set; }
            public string ExpectedHandler { get; set; }
            public int EventCount { get; set; }
            public int PointCount { get; set; }
            public long TotalReplayTicks { get; set; }
            public double TotalReplayMilliseconds { get; set; }
            public long TotalInputTicks { get; set; }
            public double TotalInputMilliseconds { get; set; }
            public long PressInputTicks { get; set; }
            public double PressInputMilliseconds { get; set; }
            public long MoveInputTicks { get; set; }
            public double MoveInputMilliseconds { get; set; }
            public long TriggerInputTicks { get; set; }
            public double TriggerInputMilliseconds { get; set; }
            public long ReleaseInputTicks { get; set; }
            public double ReleaseInputMilliseconds { get; set; }
            public long ObservationWaitTicks { get; set; }
            public double ObservationWaitMilliseconds { get; set; }
            public long ObservationReadTicks { get; set; }
            public double ObservationReadMilliseconds { get; set; }
            public long ValidationTicks { get; set; }
            public double ValidationMilliseconds { get; set; }
            public bool PressConsumed { get; set; }
            public bool TriggerConsumed { get; set; }
            public bool ReleaseConsumed { get; set; }
            public bool StrokeObservedBeforeRelease { get; set; }
            public string ObservedStroke { get; set; }
        }

        private sealed class PerformanceSummary
        {
            public string Scope { get; set; }
            public string Group { get; set; }
            public string ScenarioId { get; set; }
            public int ScenarioCount { get; set; }
            public int SampleCount { get; set; }
            public int EventCount { get; set; }
            public double MinMilliseconds { get; set; }
            public double MedianMilliseconds { get; set; }
            public double P95Milliseconds { get; set; }
            public double MaxMilliseconds { get; set; }
            public double MeanMilliseconds { get; set; }
        }

        private sealed class AttributionSummary
        {
            public string Scope { get; set; }
            public string Group { get; set; }
            public string ScenarioId { get; set; }
            public int ScenarioCount { get; set; }
            public int SampleCount { get; set; }
            public int EventCount { get; set; }
            public double TotalMedianMilliseconds { get; set; }
            public double TotalP95Milliseconds { get; set; }
            public double TotalMaxMilliseconds { get; set; }
            public double TotalMeanMilliseconds { get; set; }
            public double InputMedianMilliseconds { get; set; }
            public double InputP95Milliseconds { get; set; }
            public double InputMaxMilliseconds { get; set; }
            public double InputMeanMilliseconds { get; set; }
            public double WaitMedianMilliseconds { get; set; }
            public double WaitP95Milliseconds { get; set; }
            public double WaitMaxMilliseconds { get; set; }
            public double WaitMeanMilliseconds { get; set; }
            public double ValidationMedianMilliseconds { get; set; }
            public double ValidationP95Milliseconds { get; set; }
            public double ValidationMaxMilliseconds { get; set; }
            public double ValidationMeanMilliseconds { get; set; }
        }

        private static class SummaryCalculator
        {
            public static List<PerformanceSummary> Calculate(IReadOnlyList<PerformanceSample> samples)
            {
                var summaries = new List<PerformanceSummary>();
                summaries.Add(CreateSummary("overall", "all", "", samples));

                foreach (var group in samples.Select(s => s.Group).Distinct().OrderBy(x => x))
                {
                    summaries.Add(CreateSummary("group", group, "", samples.Where(s => s.Group == group).ToList()));
                }

                return summaries;
            }

            public static List<PerformanceSummary> CalculatePerScenario(IReadOnlyList<PerformanceSample> samples)
            {
                return samples
                    .GroupBy(s => new { s.Group, s.ScenarioId })
                    .OrderBy(g => g.Key.Group)
                    .ThenBy(g => g.Key.ScenarioId)
                    .Select(g => CreateSummary("scenario", g.Key.Group, g.Key.ScenarioId, g.ToList()))
                    .ToList();
            }

            private static PerformanceSummary CreateSummary(
                string scope,
                string group,
                string scenarioId,
                IReadOnlyList<PerformanceSample> samples)
            {
                var ordered = samples.Select(s => s.ElapsedMilliseconds).OrderBy(x => x).ToList();
                return new PerformanceSummary
                {
                    Scope = scope,
                    Group = group,
                    ScenarioId = scenarioId,
                    ScenarioCount = samples.Select(s => s.ScenarioId).Distinct().Count(),
                    SampleCount = samples.Count,
                    EventCount = samples.Sum(s => s.EventCount),
                    MinMilliseconds = ordered.First(),
                    MedianMilliseconds = Percentile(ordered, 0.50),
                    P95Milliseconds = Percentile(ordered, 0.95),
                    MaxMilliseconds = ordered.Last(),
                    MeanMilliseconds = ordered.Average(),
                };
            }

            private static double Percentile(IReadOnlyList<double> ordered, double percentile)
            {
                if (ordered.Count == 0)
                {
                    return 0;
                }

                var rank = (int)Math.Ceiling(percentile * ordered.Count) - 1;
                rank = Math.Max(0, Math.Min(rank, ordered.Count - 1));
                return ordered[rank];
            }
        }

        private static class AttributionSummaryCalculator
        {
            public static List<AttributionSummary> Calculate(IReadOnlyList<AttributionSample> samples)
            {
                var summaries = new List<AttributionSummary>();
                summaries.Add(CreateSummary("overall", "all", "", samples));

                foreach (var group in samples.Select(s => s.Group).Distinct().OrderBy(x => x))
                {
                    summaries.Add(CreateSummary("group", group, "", samples.Where(s => s.Group == group).ToList()));
                }

                return summaries;
            }

            public static List<AttributionSummary> CalculatePerScenario(IReadOnlyList<AttributionSample> samples)
            {
                return samples
                    .GroupBy(s => new { s.Group, s.ScenarioId })
                    .OrderBy(g => g.Key.Group)
                    .ThenBy(g => g.Key.ScenarioId)
                    .Select(g => CreateSummary("scenario", g.Key.Group, g.Key.ScenarioId, g.ToList()))
                    .ToList();
            }

            private static AttributionSummary CreateSummary(
                string scope,
                string group,
                string scenarioId,
                IReadOnlyList<AttributionSample> samples)
            {
                var total = samples.Select(s => s.TotalReplayMilliseconds).OrderBy(x => x).ToList();
                var input = samples.Select(s => s.TotalInputMilliseconds).OrderBy(x => x).ToList();
                var wait = samples.Select(s => s.ObservationWaitMilliseconds).OrderBy(x => x).ToList();
                var validation = samples.Select(s => s.ValidationMilliseconds).OrderBy(x => x).ToList();

                return new AttributionSummary
                {
                    Scope = scope,
                    Group = group,
                    ScenarioId = scenarioId,
                    ScenarioCount = samples.Select(s => s.ScenarioId).Distinct().Count(),
                    SampleCount = samples.Count,
                    EventCount = samples.Sum(s => s.EventCount),
                    TotalMedianMilliseconds = Percentile(total, 0.50),
                    TotalP95Milliseconds = Percentile(total, 0.95),
                    TotalMaxMilliseconds = total.Last(),
                    TotalMeanMilliseconds = total.Average(),
                    InputMedianMilliseconds = Percentile(input, 0.50),
                    InputP95Milliseconds = Percentile(input, 0.95),
                    InputMaxMilliseconds = input.Last(),
                    InputMeanMilliseconds = input.Average(),
                    WaitMedianMilliseconds = Percentile(wait, 0.50),
                    WaitP95Milliseconds = Percentile(wait, 0.95),
                    WaitMaxMilliseconds = wait.Last(),
                    WaitMeanMilliseconds = wait.Average(),
                    ValidationMedianMilliseconds = Percentile(validation, 0.50),
                    ValidationP95Milliseconds = Percentile(validation, 0.95),
                    ValidationMaxMilliseconds = validation.Last(),
                    ValidationMeanMilliseconds = validation.Average(),
                };
            }

            private static double Percentile(IReadOnlyList<double> ordered, double percentile)
            {
                if (ordered.Count == 0)
                {
                    return 0;
                }

                var rank = (int)Math.Ceiling(percentile * ordered.Count) - 1;
                rank = Math.Max(0, Math.Min(rank, ordered.Count - 1));
                return ordered[rank];
            }
        }

        private static class RepeatabilitySummaryCalculator
        {
            public static List<RepeatabilityComponentSummary> Calculate(IReadOnlyList<RepeatabilityRunSummary> runs)
            {
                return new List<RepeatabilityComponentSummary>
                {
                    CreateSummary(
                        "total_replay",
                        runs,
                        r => r.TotalMedianMilliseconds,
                        r => r.TotalP95Milliseconds,
                        r => r.TotalMaxMilliseconds,
                        r => r.TotalAtOrAboveOneMillisecondCount),
                    CreateSummary(
                        "raw_input",
                        runs,
                        r => r.InputMedianMilliseconds,
                        r => r.InputP95Milliseconds,
                        r => r.InputMaxMilliseconds,
                        r => r.InputAtOrAboveOneMillisecondCount),
                    CreateSummary(
                        "observation_wait",
                        runs,
                        r => r.WaitMedianMilliseconds,
                        r => r.WaitP95Milliseconds,
                        r => r.WaitMaxMilliseconds,
                        r => r.WaitAtOrAboveOneMillisecondCount),
                    CreateSummary(
                        "validation",
                        runs,
                        r => 0,
                        r => 0,
                        r => r.ValidationMaxMilliseconds,
                        r => 0),
                };
            }

            private static RepeatabilityComponentSummary CreateSummary(
                string component,
                IReadOnlyList<RepeatabilityRunSummary> runs,
                Func<RepeatabilityRunSummary, double> medianSelector,
                Func<RepeatabilityRunSummary, double> p95Selector,
                Func<RepeatabilityRunSummary, double> maxSelector,
                Func<RepeatabilityRunSummary, int> countSelector)
            {
                var medians = runs.Select(medianSelector).ToList();
                var p95s = runs.Select(p95Selector).ToList();
                var maxes = runs.Select(maxSelector).ToList();
                return new RepeatabilityComponentSummary
                {
                    Component = component,
                    RunCount = runs.Count,
                    RunMedianMinMilliseconds = medians.Min(),
                    RunMedianMaxMilliseconds = medians.Max(),
                    RunMedianRangeMilliseconds = medians.Max() - medians.Min(),
                    RunP95MinMilliseconds = p95s.Min(),
                    RunP95MaxMilliseconds = p95s.Max(),
                    RunP95RangeMilliseconds = p95s.Max() - p95s.Min(),
                    RunMaxMinMilliseconds = maxes.Min(),
                    RunMaxMaxMilliseconds = maxes.Max(),
                    RunMaxRangeMilliseconds = maxes.Max() - maxes.Min(),
                    AtOrAboveOneMillisecondCountTotal = runs.Sum(countSelector),
                };
            }
        }

        private static class SchedulerComparisonSummaryCalculator
        {
            public static List<SchedulerComponentComparison> Calculate(IReadOnlyList<SchedulerModeSummary> modes)
            {
                var comparisons = new List<SchedulerComponentComparison>();
                foreach (var mode in modes)
                {
                    comparisons.Add(CreateComparison(
                        "total_replay",
                        mode,
                        mode.TotalMedianMilliseconds,
                        mode.TotalP95Milliseconds,
                        mode.TotalMaxMilliseconds,
                        mode.TotalMeanMilliseconds,
                        mode.TotalAtOrAboveOneMillisecondCount));
                    comparisons.Add(CreateComparison(
                        "raw_input",
                        mode,
                        mode.InputMedianMilliseconds,
                        mode.InputP95Milliseconds,
                        mode.InputMaxMilliseconds,
                        mode.InputMeanMilliseconds,
                        mode.InputAtOrAboveOneMillisecondCount));
                    comparisons.Add(CreateComparison(
                        "observation_wait",
                        mode,
                        mode.WaitMedianMilliseconds,
                        mode.WaitP95Milliseconds,
                        mode.WaitMaxMilliseconds,
                        mode.WaitMeanMilliseconds,
                        mode.WaitAtOrAboveOneMillisecondCount));
                }

                return comparisons
                    .OrderBy(c => c.Component)
                    .ThenBy(c => c.SchedulerMode)
                    .ToList();
            }

            private static SchedulerComponentComparison CreateComparison(
                string component,
                SchedulerModeSummary mode,
                double medianMilliseconds,
                double p95Milliseconds,
                double maxMilliseconds,
                double meanMilliseconds,
                int outlierCount)
            {
                return new SchedulerComponentComparison
                {
                    Component = component,
                    SchedulerMode = mode.SchedulerMode,
                    MedianMilliseconds = medianMilliseconds,
                    P95Milliseconds = p95Milliseconds,
                    MaxMilliseconds = maxMilliseconds,
                    MeanMilliseconds = meanMilliseconds,
                    AtOrAboveOneMillisecondCount = outlierCount,
                };
            }
        }

        private static class PerformanceResultWriter
        {
            public static string ToJson(PerformanceRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("{");
                WriteProperty(builder, 1, "step", result.Step, true);
                WriteProperty(builder, 1, "repository", result.Repository, true);
                WriteProperty(builder, 1, "branch", result.Branch, true);
                WriteProperty(builder, 1, "started_utc", result.StartedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "started_jst", result.StartedJst.ToString("o"), true);
                WriteProperty(builder, 1, "finished_utc", result.FinishedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "finished_jst", result.FinishedJst.ToString("o"), true);
                WriteProperty(builder, 1, "warmup_iterations", result.WarmupIterations, true);
                WriteProperty(builder, 1, "measured_iterations", result.MeasuredIterations, true);
                WriteProperty(builder, 1, "scenario_count", result.ScenarioCount, true);
                WriteProperty(builder, 1, "event_count_per_measured_iteration", result.EventCountPerMeasuredIteration, true);
                WriteProperty(builder, 1, "total_measured_event_count", result.TotalMeasuredEventCount, true);

                WriteProperty(builder, 1, "timing_note", result.TimingNote, true);

                Indent(builder, 1).AppendLine("\"environment\": {");
                WriteProperty(builder, 2, "machine_name", result.Environment.MachineName, true);
                WriteProperty(builder, 2, "user_name", result.Environment.UserName, true);
                WriteProperty(builder, 2, "os_version", result.Environment.OSVersion, true);
                WriteProperty(builder, 2, "clr_version", result.Environment.CLRVersion, true);
                WriteProperty(builder, 2, "process_architecture", result.Environment.ProcessArchitecture, true);
                WriteProperty(builder, 2, "is_64_bit_operating_system", result.Environment.Is64BitOperatingSystem, true);
                WriteProperty(builder, 2, "is_64_bit_process", result.Environment.Is64BitProcess, true);
                WriteProperty(builder, 2, "build_configuration", result.Environment.BuildConfiguration, true);
                WriteProperty(builder, 2, "test_assembly", result.Environment.TestAssembly, true);
                WriteProperty(builder, 2, "stopwatch_frequency", result.Environment.StopwatchFrequency, true);
                WriteProperty(builder, 2, "stopwatch_is_high_resolution", result.Environment.StopwatchIsHighResolution, false);
                Indent(builder, 1).AppendLine("},");

                WriteScenarioInventory(builder, result.ScenarioInventory);
                builder.AppendLine(",");
                WriteSummaries(builder, "summaries", result.Summaries);
                builder.AppendLine(",");
                WriteSummaries(builder, "per_scenario_summaries", result.PerScenarioSummaries);
                builder.AppendLine(",");
                WriteSamples(builder, result.Samples);
                builder.AppendLine();
                builder.AppendLine("}");
                return builder.ToString();
            }

            public static string ToJson(AttributionRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("{");
                WriteProperty(builder, 1, "step", result.Step, true);
                WriteProperty(builder, 1, "repository", result.Repository, true);
                WriteProperty(builder, 1, "branch", result.Branch, true);
                WriteProperty(builder, 1, "started_utc", result.StartedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "started_jst", result.StartedJst.ToString("o"), true);
                WriteProperty(builder, 1, "finished_utc", result.FinishedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "finished_jst", result.FinishedJst.ToString("o"), true);
                WriteProperty(builder, 1, "warmup_iterations", result.WarmupIterations, true);
                WriteProperty(builder, 1, "measured_iterations", result.MeasuredIterations, true);
                WriteProperty(builder, 1, "scenario_count", result.ScenarioCount, true);
                WriteProperty(builder, 1, "event_count_per_measured_iteration", result.EventCountPerMeasuredIteration, true);
                WriteProperty(builder, 1, "total_measured_event_count", result.TotalMeasuredEventCount, true);

                WriteProperty(builder, 1, "scheduler_mode", result.SchedulerMode, true);
                WriteProperty(builder, 1, "scheduler_description", result.SchedulerDescription, true);
                WriteProperty(builder, 1, "stroke_watch_interval_ms", result.StrokeWatchIntervalMilliseconds, true);
                WriteProperty(builder, 1, "timing_note", result.TimingNote, true);

                Indent(builder, 1).AppendLine("\"environment\": {");
                WriteProperty(builder, 2, "machine_name", result.Environment.MachineName, true);
                WriteProperty(builder, 2, "user_name", result.Environment.UserName, true);
                WriteProperty(builder, 2, "os_version", result.Environment.OSVersion, true);
                WriteProperty(builder, 2, "clr_version", result.Environment.CLRVersion, true);
                WriteProperty(builder, 2, "process_architecture", result.Environment.ProcessArchitecture, true);
                WriteProperty(builder, 2, "is_64_bit_operating_system", result.Environment.Is64BitOperatingSystem, true);
                WriteProperty(builder, 2, "is_64_bit_process", result.Environment.Is64BitProcess, true);
                WriteProperty(builder, 2, "build_configuration", result.Environment.BuildConfiguration, true);
                WriteProperty(builder, 2, "test_assembly", result.Environment.TestAssembly, true);
                WriteProperty(builder, 2, "stopwatch_frequency", result.Environment.StopwatchFrequency, true);
                WriteProperty(builder, 2, "stopwatch_is_high_resolution", result.Environment.StopwatchIsHighResolution, false);
                Indent(builder, 1).AppendLine("},");

                WriteScenarioInventory(builder, result.ScenarioInventory);
                builder.AppendLine(",");
                WriteAttributionSummaries(builder, "summaries", result.Summaries);
                builder.AppendLine(",");
                WriteAttributionSummaries(builder, "per_scenario_summaries", result.PerScenarioSummaries);
                builder.AppendLine(",");
                WriteAttributionSamples(builder, result.Samples);
                builder.AppendLine();
                builder.AppendLine("}");
                return builder.ToString();
            }

            public static string ToJson(RepeatabilityRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("{");
                WriteProperty(builder, 1, "step", result.Step, true);
                WriteProperty(builder, 1, "repository", result.Repository, true);
                WriteProperty(builder, 1, "branch", result.Branch, true);
                WriteProperty(builder, 1, "started_utc", result.StartedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "started_jst", result.StartedJst.ToString("o"), true);
                WriteProperty(builder, 1, "finished_utc", result.FinishedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "finished_jst", result.FinishedJst.ToString("o"), true);
                WriteProperty(builder, 1, "warmup_iterations", result.WarmupIterations, true);
                WriteProperty(builder, 1, "measured_iterations", result.MeasuredIterations, true);
                WriteProperty(builder, 1, "repeat_runs", result.RepeatRuns, true);
                WriteProperty(builder, 1, "scenario_count", result.ScenarioCount, true);
                WriteProperty(builder, 1, "event_count_per_measured_iteration", result.EventCountPerMeasuredIteration, true);
                WriteProperty(builder, 1, "total_measured_event_count", result.TotalMeasuredEventCount, true);

                WriteProperty(builder, 1, "timing_note", result.TimingNote, true);

                Indent(builder, 1).AppendLine("\"environment\": {");
                WriteProperty(builder, 2, "machine_name", result.Environment.MachineName, true);
                WriteProperty(builder, 2, "user_name", result.Environment.UserName, true);
                WriteProperty(builder, 2, "os_version", result.Environment.OSVersion, true);
                WriteProperty(builder, 2, "clr_version", result.Environment.CLRVersion, true);
                WriteProperty(builder, 2, "process_architecture", result.Environment.ProcessArchitecture, true);
                WriteProperty(builder, 2, "is_64_bit_operating_system", result.Environment.Is64BitOperatingSystem, true);
                WriteProperty(builder, 2, "is_64_bit_process", result.Environment.Is64BitProcess, true);
                WriteProperty(builder, 2, "build_configuration", result.Environment.BuildConfiguration, true);
                WriteProperty(builder, 2, "test_assembly", result.Environment.TestAssembly, true);
                WriteProperty(builder, 2, "stopwatch_frequency", result.Environment.StopwatchFrequency, true);
                WriteProperty(builder, 2, "stopwatch_is_high_resolution", result.Environment.StopwatchIsHighResolution, false);
                Indent(builder, 1).AppendLine("},");

                WriteRepeatabilityRunSummaries(builder, result.RunSummaries);
                builder.AppendLine(",");
                WriteRepeatabilityComponentSummaries(builder, result.ComponentSummaries);
                builder.AppendLine(",");
                WriteRepeatabilitySlowSamples(builder, result.SlowestSamples);
                builder.AppendLine();
                builder.AppendLine("}");
                return builder.ToString();
            }

            public static string ToJson(SchedulerComparisonRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("{");
                WriteProperty(builder, 1, "step", result.Step, true);
                WriteProperty(builder, 1, "repository", result.Repository, true);
                WriteProperty(builder, 1, "branch", result.Branch, true);
                WriteProperty(builder, 1, "started_utc", result.StartedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "started_jst", result.StartedJst.ToString("o"), true);
                WriteProperty(builder, 1, "finished_utc", result.FinishedUtc.ToString("o"), true);
                WriteProperty(builder, 1, "finished_jst", result.FinishedJst.ToString("o"), true);
                WriteProperty(builder, 1, "warmup_iterations", result.WarmupIterations, true);
                WriteProperty(builder, 1, "measured_iterations", result.MeasuredIterations, true);
                WriteProperty(builder, 1, "repeat_runs_per_scheduler", result.RepeatRunsPerScheduler, true);
                WriteProperty(builder, 1, "scenario_count", result.ScenarioCount, true);
                WriteProperty(builder, 1, "event_count_per_measured_iteration", result.EventCountPerMeasuredIteration, true);
                WriteProperty(builder, 1, "total_measured_event_count", result.TotalMeasuredEventCount, true);

                WriteProperty(builder, 1, "timing_note", result.TimingNote, true);

                Indent(builder, 1).AppendLine("\"environment\": {");
                WriteProperty(builder, 2, "machine_name", result.Environment.MachineName, true);
                WriteProperty(builder, 2, "user_name", result.Environment.UserName, true);
                WriteProperty(builder, 2, "os_version", result.Environment.OSVersion, true);
                WriteProperty(builder, 2, "clr_version", result.Environment.CLRVersion, true);
                WriteProperty(builder, 2, "process_architecture", result.Environment.ProcessArchitecture, true);
                WriteProperty(builder, 2, "is_64_bit_operating_system", result.Environment.Is64BitOperatingSystem, true);
                WriteProperty(builder, 2, "is_64_bit_process", result.Environment.Is64BitProcess, true);
                WriteProperty(builder, 2, "build_configuration", result.Environment.BuildConfiguration, true);
                WriteProperty(builder, 2, "test_assembly", result.Environment.TestAssembly, true);
                WriteProperty(builder, 2, "stopwatch_frequency", result.Environment.StopwatchFrequency, true);
                WriteProperty(builder, 2, "stopwatch_is_high_resolution", result.Environment.StopwatchIsHighResolution, false);
                Indent(builder, 1).AppendLine("},");

                WriteScenarioInventory(builder, result.ScenarioInventory);
                builder.AppendLine(",");
                WriteSchedulerModeSummaries(builder, result.ModeSummaries);
                builder.AppendLine(",");
                WriteSchedulerComponentComparisons(builder, result.ComponentComparisons);
                builder.AppendLine(",");
                WriteSchedulerSlowSamples(builder, result.SlowestSamples);
                builder.AppendLine();
                builder.AppendLine("}");
                return builder.ToString();
            }

            public static string ToMarkdown(PerformanceRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("# Gesture Performance Baseline Summary");
                builder.AppendLine();
                builder.AppendLine("No hard pass/fail timing threshold is applied. These numbers are noisy local measurements for Step 05 analysis.");
                builder.AppendLine();
                builder.AppendLine("- Started UTC: " + result.StartedUtc.ToString("o"));
                builder.AppendLine("- Started JST: " + result.StartedJst.ToString("o"));
                builder.AppendLine("- Warmup iterations: " + result.WarmupIterations);
                builder.AppendLine("- Measured iterations: " + result.MeasuredIterations);
                builder.AppendLine("- Scenario count: " + result.ScenarioCount);
                builder.AppendLine("- Events per measured iteration: " + result.EventCountPerMeasuredIteration);
                builder.AppendLine("- Total measured events: " + result.TotalMeasuredEventCount);
                builder.AppendLine("- Machine: " + result.Environment.MachineName);
                builder.AppendLine("- OS: " + result.Environment.OSVersion);
                builder.AppendLine("- CLR: " + result.Environment.CLRVersion);
                builder.AppendLine("- Process architecture: " + result.Environment.ProcessArchitecture);
                builder.AppendLine("- Build configuration: " + result.Environment.BuildConfiguration);
                builder.AppendLine();
                builder.AppendLine("| Scope | Group | Scenarios | Samples | Events | Min ms | Median ms | P95 ms | Max ms | Mean ms |");
                builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var summary in result.Summaries)
                {
                    builder.Append("| ")
                        .Append(summary.Scope).Append(" | ")
                        .Append(summary.Group).Append(" | ")
                        .Append(summary.ScenarioCount).Append(" | ")
                        .Append(summary.SampleCount).Append(" | ")
                        .Append(summary.EventCount).Append(" | ")
                        .Append(FormatMilliseconds(summary.MinMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.MedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.P95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.MaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.MeanMilliseconds)).AppendLine(" |");
                }

                return builder.ToString();
            }

            public static string ToMarkdown(AttributionRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("# Gesture Performance Attribution Summary");
                builder.AppendLine();
                builder.AppendLine("No hard pass/fail timing threshold is applied. This run separates replay input calls, stroke-watcher observation waits, observed-stroke reads, and validation.");
                builder.AppendLine();
                builder.AppendLine("- Started UTC: " + result.StartedUtc.ToString("o"));
                builder.AppendLine("- Started JST: " + result.StartedJst.ToString("o"));
                builder.AppendLine("- Warmup iterations: " + result.WarmupIterations);
                builder.AppendLine("- Measured iterations: " + result.MeasuredIterations);
                builder.AppendLine("- Scenario count: " + result.ScenarioCount);
                builder.AppendLine("- Events per measured iteration: " + result.EventCountPerMeasuredIteration);
                builder.AppendLine("- Total measured events: " + result.TotalMeasuredEventCount);
                builder.AppendLine("- Machine: " + result.Environment.MachineName);
                builder.AppendLine("- OS: " + result.Environment.OSVersion);
                builder.AppendLine("- CLR: " + result.Environment.CLRVersion);
                builder.AppendLine("- Process architecture: " + result.Environment.ProcessArchitecture);
                builder.AppendLine("- Build configuration: " + result.Environment.BuildConfiguration);
                builder.AppendLine();
                builder.AppendLine("| Scope | Group | Scenarios | Samples | Events | Total median ms | Total p95 ms | Total max ms | Total mean ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Validation median ms | Validation p95 ms | Validation max ms |");
                builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var summary in result.Summaries)
                {
                    builder.Append("| ")
                        .Append(summary.Scope).Append(" | ")
                        .Append(summary.Group).Append(" | ")
                        .Append(summary.ScenarioCount).Append(" | ")
                        .Append(summary.SampleCount).Append(" | ")
                        .Append(summary.EventCount).Append(" | ")
                        .Append(FormatMilliseconds(summary.TotalMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.TotalP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.TotalMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.TotalMeanMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.InputMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.InputP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.InputMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.WaitMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.WaitP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.WaitMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.ValidationMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.ValidationP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.ValidationMaxMilliseconds)).AppendLine(" |");
                }

                return builder.ToString();
            }

            public static string ToMarkdown(RepeatabilityRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("# Gesture Performance Repeatability Summary");
                builder.AppendLine();
                builder.AppendLine("No hard pass/fail timing threshold is applied. This Step 06 run repeats the attribution measurement to separate raw input timing, watcher observation wait, and run-to-run variance.");
                builder.AppendLine();
                builder.AppendLine("- Started UTC: " + result.StartedUtc.ToString("o"));
                builder.AppendLine("- Started JST: " + result.StartedJst.ToString("o"));
                builder.AppendLine("- Warmup iterations per run: " + result.WarmupIterations);
                builder.AppendLine("- Measured iterations per run: " + result.MeasuredIterations);
                builder.AppendLine("- Repeat runs: " + result.RepeatRuns);
                builder.AppendLine("- Scenario count: " + result.ScenarioCount);
                builder.AppendLine("- Events per measured iteration: " + result.EventCountPerMeasuredIteration);
                builder.AppendLine("- Total measured events: " + result.TotalMeasuredEventCount);
                builder.AppendLine("- Machine: " + result.Environment.MachineName);
                builder.AppendLine("- OS: " + result.Environment.OSVersion);
                builder.AppendLine("- CLR: " + result.Environment.CLRVersion);
                builder.AppendLine("- Process architecture: " + result.Environment.ProcessArchitecture);
                builder.AppendLine("- Build configuration: " + result.Environment.BuildConfiguration);
                builder.AppendLine();
                builder.AppendLine("## Per-Run Attribution");
                builder.AppendLine();
                builder.AppendLine("| Run | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |");
                builder.AppendLine("| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var run in result.RunSummaries)
                {
                    builder.Append("| ")
                        .Append(run.RunIndex).Append(" | ")
                        .Append(run.SampleCount).Append(" | ")
                        .Append(run.EventCount).Append(" | ")
                        .Append(FormatMilliseconds(run.TotalMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.TotalP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.TotalMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.InputMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.InputP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.InputMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.WaitMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.WaitP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(run.WaitMaxMilliseconds)).Append(" | ")
                        .Append(run.TotalAtOrAboveOneMillisecondCount).Append(" | ")
                        .Append(run.InputAtOrAboveOneMillisecondCount).Append(" | ")
                        .Append(run.WaitAtOrAboveOneMillisecondCount).AppendLine(" |");
                }

                builder.AppendLine();
                builder.AppendLine("## Run-to-Run Range");
                builder.AppendLine();
                builder.AppendLine("| Component | Runs | Run median min ms | Run median max ms | Run median range ms | Run p95 min ms | Run p95 max ms | Run p95 range ms | Run max min ms | Run max max ms | Run max range ms | >=1 ms total |");
                builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var summary in result.ComponentSummaries)
                {
                    builder.Append("| ")
                        .Append(summary.Component).Append(" | ")
                        .Append(summary.RunCount).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunMedianMinMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunMedianMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunMedianRangeMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunP95MinMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunP95MaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunP95RangeMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunMaxMinMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunMaxMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(summary.RunMaxRangeMilliseconds)).Append(" | ")
                        .Append(summary.AtOrAboveOneMillisecondCountTotal).AppendLine(" |");
                }

                builder.AppendLine();
                builder.AppendLine("## Slowest Raw Input and Watcher Wait Samples");
                builder.AppendLine();
                builder.AppendLine("| Component | Run | Iteration | Group | Scenario | Component ms | Total ms | Input ms | Wait ms | Press input ms | Move input ms | Release input ms |");
                builder.AppendLine("| --- | ---: | ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var sample in result.SlowestSamples)
                {
                    builder.Append("| ")
                        .Append(sample.Component).Append(" | ")
                        .Append(sample.RunIndex).Append(" | ")
                        .Append(sample.Iteration).Append(" | ")
                        .Append(sample.Group).Append(" | ")
                        .Append(sample.ScenarioId).Append(" | ")
                        .Append(FormatMilliseconds(sample.ComponentMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.TotalReplayMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.TotalInputMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.ObservationWaitMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.PressInputMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.MoveInputMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.ReleaseInputMilliseconds)).AppendLine(" |");
                }

                return builder.ToString();
            }

            public static string ToMarkdown(SchedulerComparisonRunResult result)
            {
                var builder = new StringBuilder();
                builder.AppendLine("# Gesture Performance Scheduler Comparison Summary");
                builder.AppendLine();
                builder.AppendLine(result.TimingNote);
                builder.AppendLine();
                builder.AppendLine("Step: " + result.Step + ". This run compares deterministic attribution scenarios across replay scheduler and StrokeWatchInterval modes.");
                builder.AppendLine();
                builder.AppendLine("- Started UTC: " + result.StartedUtc.ToString("o"));
                builder.AppendLine("- Started JST: " + result.StartedJst.ToString("o"));
                builder.AppendLine("- Warmup iterations per run: " + result.WarmupIterations);
                builder.AppendLine("- Measured iterations per run: " + result.MeasuredIterations);
                builder.AppendLine("- Repeat runs per scheduler: " + result.RepeatRunsPerScheduler);
                builder.AppendLine("- Scenario count: " + result.ScenarioCount);
                builder.AppendLine("- Events per measured iteration: " + result.EventCountPerMeasuredIteration);
                builder.AppendLine("- Total measured events: " + result.TotalMeasuredEventCount);
                builder.AppendLine("- Machine: " + result.Environment.MachineName);
                builder.AppendLine("- OS: " + result.Environment.OSVersion);
                builder.AppendLine("- CLR: " + result.Environment.CLRVersion);
                builder.AppendLine("- Process architecture: " + result.Environment.ProcessArchitecture);
                builder.AppendLine("- Build configuration: " + result.Environment.BuildConfiguration);
                builder.AppendLine();
                builder.AppendLine("## Scheduler Mode Summary");
                builder.AppendLine();
                builder.AppendLine("| Role | Stroke interval ms | Scheduler mode | Runs | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |");
                builder.AppendLine("| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var mode in result.ModeSummaries)
                {
                    builder.Append("| ")
                        .Append(mode.MeasurementRole).Append(" | ")
                        .Append(mode.StrokeWatchIntervalMilliseconds).Append(" | ")
                        .Append(mode.SchedulerMode).Append(" | ")
                        .Append(mode.RunCount).Append(" | ")
                        .Append(mode.SampleCount).Append(" | ")
                        .Append(mode.EventCount).Append(" | ")
                        .Append(FormatMilliseconds(mode.TotalMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.TotalP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.TotalMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.InputMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.InputP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.InputMaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.WaitMedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.WaitP95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(mode.WaitMaxMilliseconds)).Append(" | ")
                        .Append(mode.TotalAtOrAboveOneMillisecondCount).Append(" | ")
                        .Append(mode.InputAtOrAboveOneMillisecondCount).Append(" | ")
                        .Append(mode.WaitAtOrAboveOneMillisecondCount).AppendLine(" |");
                }

                builder.AppendLine();
                builder.AppendLine("## Per-Run Attribution");
                builder.AppendLine();
                builder.AppendLine("| Role | Stroke interval ms | Scheduler mode | Run | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |");
                builder.AppendLine("| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var mode in result.ModeSummaries)
                {
                    foreach (var run in mode.RunSummaries)
                    {
                        builder.Append("| ")
                            .Append(mode.MeasurementRole).Append(" | ")
                            .Append(mode.StrokeWatchIntervalMilliseconds).Append(" | ")
                            .Append(mode.SchedulerMode).Append(" | ")
                            .Append(run.RunIndex).Append(" | ")
                            .Append(run.SampleCount).Append(" | ")
                            .Append(run.EventCount).Append(" | ")
                            .Append(FormatMilliseconds(run.TotalMedianMilliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.TotalP95Milliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.TotalMaxMilliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.InputMedianMilliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.InputP95Milliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.InputMaxMilliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.WaitMedianMilliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.WaitP95Milliseconds)).Append(" | ")
                            .Append(FormatMilliseconds(run.WaitMaxMilliseconds)).Append(" | ")
                            .Append(run.TotalAtOrAboveOneMillisecondCount).Append(" | ")
                            .Append(run.InputAtOrAboveOneMillisecondCount).Append(" | ")
                            .Append(run.WaitAtOrAboveOneMillisecondCount).AppendLine(" |");
                    }
                }

                builder.AppendLine();
                builder.AppendLine("## Component Comparison");
                builder.AppendLine();
                builder.AppendLine("| Component | Scheduler mode | Median ms | P95 ms | Max ms | Mean ms | >=1 ms count |");
                builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: |");

                foreach (var comparison in result.ComponentComparisons)
                {
                    builder.Append("| ")
                        .Append(comparison.Component).Append(" | ")
                        .Append(comparison.SchedulerMode).Append(" | ")
                        .Append(FormatMilliseconds(comparison.MedianMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(comparison.P95Milliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(comparison.MaxMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(comparison.MeanMilliseconds)).Append(" | ")
                        .Append(comparison.AtOrAboveOneMillisecondCount).AppendLine(" |");
                }

                builder.AppendLine();
                builder.AppendLine("## Slowest Raw Input and Watcher Wait Samples");
                builder.AppendLine();
                builder.AppendLine("| Scheduler mode | Component | Run | Iteration | Group | Scenario | Component ms | Total ms | Input ms | Wait ms | Press input ms | Move input ms | Release input ms |");
                builder.AppendLine("| --- | --- | ---: | ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

                foreach (var sample in result.SlowestSamples)
                {
                    builder.Append("| ")
                        .Append(sample.SchedulerMode).Append(" | ")
                        .Append(sample.Component).Append(" | ")
                        .Append(sample.RunIndex).Append(" | ")
                        .Append(sample.Iteration).Append(" | ")
                        .Append(sample.Group).Append(" | ")
                        .Append(sample.ScenarioId).Append(" | ")
                        .Append(FormatMilliseconds(sample.ComponentMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.TotalReplayMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.TotalInputMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.ObservationWaitMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.PressInputMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.MoveInputMilliseconds)).Append(" | ")
                        .Append(FormatMilliseconds(sample.ReleaseInputMilliseconds)).AppendLine(" |");
                }

                return builder.ToString();
            }

            private static void WriteRepeatabilityRunSummaries(StringBuilder builder, IReadOnlyList<RepeatabilityRunSummary> runs)
            {
                Indent(builder, 1).AppendLine("\"run_summaries\": [");
                for (var i = 0; i < runs.Count; i++)
                {
                    var run = runs[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "run_index", run.RunIndex, true);
                    WriteProperty(builder, 3, "started_utc", run.StartedUtc.ToString("o"), true);
                    WriteProperty(builder, 3, "started_jst", run.StartedJst.ToString("o"), true);
                    WriteProperty(builder, 3, "finished_utc", run.FinishedUtc.ToString("o"), true);
                    WriteProperty(builder, 3, "finished_jst", run.FinishedJst.ToString("o"), true);
                    WriteProperty(builder, 3, "sample_count", run.SampleCount, true);
                    WriteProperty(builder, 3, "event_count", run.EventCount, true);
                    WriteProperty(builder, 3, "total_median_ms", run.TotalMedianMilliseconds, true);
                    WriteProperty(builder, 3, "total_p95_ms", run.TotalP95Milliseconds, true);
                    WriteProperty(builder, 3, "total_max_ms", run.TotalMaxMilliseconds, true);
                    WriteProperty(builder, 3, "total_mean_ms", run.TotalMeanMilliseconds, true);
                    WriteProperty(builder, 3, "input_median_ms", run.InputMedianMilliseconds, true);
                    WriteProperty(builder, 3, "input_p95_ms", run.InputP95Milliseconds, true);
                    WriteProperty(builder, 3, "input_max_ms", run.InputMaxMilliseconds, true);
                    WriteProperty(builder, 3, "input_mean_ms", run.InputMeanMilliseconds, true);
                    WriteProperty(builder, 3, "wait_median_ms", run.WaitMedianMilliseconds, true);
                    WriteProperty(builder, 3, "wait_p95_ms", run.WaitP95Milliseconds, true);
                    WriteProperty(builder, 3, "wait_max_ms", run.WaitMaxMilliseconds, true);
                    WriteProperty(builder, 3, "wait_mean_ms", run.WaitMeanMilliseconds, true);
                    WriteProperty(builder, 3, "validation_max_ms", run.ValidationMaxMilliseconds, true);
                    WriteProperty(builder, 3, "total_at_or_above_1ms_count", run.TotalAtOrAboveOneMillisecondCount, true);
                    WriteProperty(builder, 3, "input_at_or_above_1ms_count", run.InputAtOrAboveOneMillisecondCount, true);
                    WriteProperty(builder, 3, "wait_at_or_above_1ms_count", run.WaitAtOrAboveOneMillisecondCount, false);
                    Indent(builder, 2).Append(i == runs.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteRepeatabilityComponentSummaries(StringBuilder builder, IReadOnlyList<RepeatabilityComponentSummary> summaries)
            {
                Indent(builder, 1).AppendLine("\"component_summaries\": [");
                for (var i = 0; i < summaries.Count; i++)
                {
                    var summary = summaries[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "component", summary.Component, true);
                    WriteProperty(builder, 3, "run_count", summary.RunCount, true);
                    WriteProperty(builder, 3, "run_median_min_ms", summary.RunMedianMinMilliseconds, true);
                    WriteProperty(builder, 3, "run_median_max_ms", summary.RunMedianMaxMilliseconds, true);
                    WriteProperty(builder, 3, "run_median_range_ms", summary.RunMedianRangeMilliseconds, true);
                    WriteProperty(builder, 3, "run_p95_min_ms", summary.RunP95MinMilliseconds, true);
                    WriteProperty(builder, 3, "run_p95_max_ms", summary.RunP95MaxMilliseconds, true);
                    WriteProperty(builder, 3, "run_p95_range_ms", summary.RunP95RangeMilliseconds, true);
                    WriteProperty(builder, 3, "run_max_min_ms", summary.RunMaxMinMilliseconds, true);
                    WriteProperty(builder, 3, "run_max_max_ms", summary.RunMaxMaxMilliseconds, true);
                    WriteProperty(builder, 3, "run_max_range_ms", summary.RunMaxRangeMilliseconds, true);
                    WriteProperty(builder, 3, "at_or_above_1ms_count_total", summary.AtOrAboveOneMillisecondCountTotal, false);
                    Indent(builder, 2).Append(i == summaries.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteRepeatabilitySlowSamples(StringBuilder builder, IReadOnlyList<RepeatabilitySlowSample> samples)
            {
                Indent(builder, 1).AppendLine("\"slowest_samples\": [");
                for (var i = 0; i < samples.Count; i++)
                {
                    var sample = samples[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "run_index", sample.RunIndex, true);
                    WriteProperty(builder, 3, "component", sample.Component, true);
                    WriteProperty(builder, 3, "component_ms", sample.ComponentMilliseconds, true);
                    WriteProperty(builder, 3, "iteration", sample.Iteration, true);
                    WriteProperty(builder, 3, "group", sample.Group, true);
                    WriteProperty(builder, 3, "scenario_id", sample.ScenarioId, true);
                    WriteProperty(builder, 3, "category", sample.Category, true);
                    WriteProperty(builder, 3, "total_replay_ms", sample.TotalReplayMilliseconds, true);
                    WriteProperty(builder, 3, "total_input_ms", sample.TotalInputMilliseconds, true);
                    WriteProperty(builder, 3, "observation_wait_ms", sample.ObservationWaitMilliseconds, true);
                    WriteProperty(builder, 3, "press_input_ms", sample.PressInputMilliseconds, true);
                    WriteProperty(builder, 3, "move_input_ms", sample.MoveInputMilliseconds, true);
                    WriteProperty(builder, 3, "release_input_ms", sample.ReleaseInputMilliseconds, true);
                    WriteProperty(builder, 3, "observed_stroke", sample.ObservedStroke, false);
                    Indent(builder, 2).Append(i == samples.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteSchedulerModeSummaries(StringBuilder builder, IReadOnlyList<SchedulerModeSummary> modes)
            {
                Indent(builder, 1).AppendLine("\"mode_summaries\": [");
                for (var i = 0; i < modes.Count; i++)
                {
                    var mode = modes[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "scheduler_mode", mode.SchedulerMode, true);
                    WriteProperty(builder, 3, "scheduler_description", mode.SchedulerDescription, true);
                    WriteProperty(builder, 3, "measurement_role", mode.MeasurementRole, true);
                    WriteProperty(builder, 3, "stroke_watch_interval_ms", mode.StrokeWatchIntervalMilliseconds, true);
                    WriteProperty(builder, 3, "run_count", mode.RunCount, true);
                    WriteProperty(builder, 3, "sample_count", mode.SampleCount, true);
                    WriteProperty(builder, 3, "event_count", mode.EventCount, true);
                    WriteProperty(builder, 3, "total_median_ms", mode.TotalMedianMilliseconds, true);
                    WriteProperty(builder, 3, "total_p95_ms", mode.TotalP95Milliseconds, true);
                    WriteProperty(builder, 3, "total_max_ms", mode.TotalMaxMilliseconds, true);
                    WriteProperty(builder, 3, "total_mean_ms", mode.TotalMeanMilliseconds, true);
                    WriteProperty(builder, 3, "input_median_ms", mode.InputMedianMilliseconds, true);
                    WriteProperty(builder, 3, "input_p95_ms", mode.InputP95Milliseconds, true);
                    WriteProperty(builder, 3, "input_max_ms", mode.InputMaxMilliseconds, true);
                    WriteProperty(builder, 3, "input_mean_ms", mode.InputMeanMilliseconds, true);
                    WriteProperty(builder, 3, "wait_median_ms", mode.WaitMedianMilliseconds, true);
                    WriteProperty(builder, 3, "wait_p95_ms", mode.WaitP95Milliseconds, true);
                    WriteProperty(builder, 3, "wait_max_ms", mode.WaitMaxMilliseconds, true);
                    WriteProperty(builder, 3, "wait_mean_ms", mode.WaitMeanMilliseconds, true);
                    WriteProperty(builder, 3, "total_at_or_above_1ms_count", mode.TotalAtOrAboveOneMillisecondCount, true);
                    WriteProperty(builder, 3, "input_at_or_above_1ms_count", mode.InputAtOrAboveOneMillisecondCount, true);
                    WriteProperty(builder, 3, "wait_at_or_above_1ms_count", mode.WaitAtOrAboveOneMillisecondCount, true);
                    WriteNestedRepeatabilityRunSummaries(builder, mode.RunSummaries, 3);
                    builder.AppendLine();
                    Indent(builder, 2).Append(i == modes.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteNestedRepeatabilityRunSummaries(
                StringBuilder builder,
                IReadOnlyList<RepeatabilityRunSummary> runs,
                int indent)
            {
                Indent(builder, indent).AppendLine("\"run_summaries\": [");
                for (var i = 0; i < runs.Count; i++)
                {
                    var run = runs[i];
                    Indent(builder, indent + 1).AppendLine("{");
                    WriteProperty(builder, indent + 2, "run_index", run.RunIndex, true);
                    WriteProperty(builder, indent + 2, "started_utc", run.StartedUtc.ToString("o"), true);
                    WriteProperty(builder, indent + 2, "started_jst", run.StartedJst.ToString("o"), true);
                    WriteProperty(builder, indent + 2, "finished_utc", run.FinishedUtc.ToString("o"), true);
                    WriteProperty(builder, indent + 2, "finished_jst", run.FinishedJst.ToString("o"), true);
                    WriteProperty(builder, indent + 2, "sample_count", run.SampleCount, true);
                    WriteProperty(builder, indent + 2, "event_count", run.EventCount, true);
                    WriteProperty(builder, indent + 2, "total_median_ms", run.TotalMedianMilliseconds, true);
                    WriteProperty(builder, indent + 2, "total_p95_ms", run.TotalP95Milliseconds, true);
                    WriteProperty(builder, indent + 2, "total_max_ms", run.TotalMaxMilliseconds, true);
                    WriteProperty(builder, indent + 2, "total_mean_ms", run.TotalMeanMilliseconds, true);
                    WriteProperty(builder, indent + 2, "input_median_ms", run.InputMedianMilliseconds, true);
                    WriteProperty(builder, indent + 2, "input_p95_ms", run.InputP95Milliseconds, true);
                    WriteProperty(builder, indent + 2, "input_max_ms", run.InputMaxMilliseconds, true);
                    WriteProperty(builder, indent + 2, "input_mean_ms", run.InputMeanMilliseconds, true);
                    WriteProperty(builder, indent + 2, "wait_median_ms", run.WaitMedianMilliseconds, true);
                    WriteProperty(builder, indent + 2, "wait_p95_ms", run.WaitP95Milliseconds, true);
                    WriteProperty(builder, indent + 2, "wait_max_ms", run.WaitMaxMilliseconds, true);
                    WriteProperty(builder, indent + 2, "wait_mean_ms", run.WaitMeanMilliseconds, true);
                    WriteProperty(builder, indent + 2, "validation_max_ms", run.ValidationMaxMilliseconds, true);
                    WriteProperty(builder, indent + 2, "total_at_or_above_1ms_count", run.TotalAtOrAboveOneMillisecondCount, true);
                    WriteProperty(builder, indent + 2, "input_at_or_above_1ms_count", run.InputAtOrAboveOneMillisecondCount, true);
                    WriteProperty(builder, indent + 2, "wait_at_or_above_1ms_count", run.WaitAtOrAboveOneMillisecondCount, false);
                    Indent(builder, indent + 1).Append(i == runs.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, indent).Append("]");
            }

            private static void WriteSchedulerComponentComparisons(StringBuilder builder, IReadOnlyList<SchedulerComponentComparison> comparisons)
            {
                Indent(builder, 1).AppendLine("\"component_comparisons\": [");
                for (var i = 0; i < comparisons.Count; i++)
                {
                    var comparison = comparisons[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "component", comparison.Component, true);
                    WriteProperty(builder, 3, "scheduler_mode", comparison.SchedulerMode, true);
                    WriteProperty(builder, 3, "median_ms", comparison.MedianMilliseconds, true);
                    WriteProperty(builder, 3, "p95_ms", comparison.P95Milliseconds, true);
                    WriteProperty(builder, 3, "max_ms", comparison.MaxMilliseconds, true);
                    WriteProperty(builder, 3, "mean_ms", comparison.MeanMilliseconds, true);
                    WriteProperty(builder, 3, "at_or_above_1ms_count", comparison.AtOrAboveOneMillisecondCount, false);
                    Indent(builder, 2).Append(i == comparisons.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteSchedulerSlowSamples(StringBuilder builder, IReadOnlyList<SchedulerSlowSample> samples)
            {
                Indent(builder, 1).AppendLine("\"slowest_samples\": [");
                for (var i = 0; i < samples.Count; i++)
                {
                    var sample = samples[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "scheduler_mode", sample.SchedulerMode, true);
                    WriteProperty(builder, 3, "run_index", sample.RunIndex, true);
                    WriteProperty(builder, 3, "component", sample.Component, true);
                    WriteProperty(builder, 3, "component_ms", sample.ComponentMilliseconds, true);
                    WriteProperty(builder, 3, "iteration", sample.Iteration, true);
                    WriteProperty(builder, 3, "group", sample.Group, true);
                    WriteProperty(builder, 3, "scenario_id", sample.ScenarioId, true);
                    WriteProperty(builder, 3, "category", sample.Category, true);
                    WriteProperty(builder, 3, "total_replay_ms", sample.TotalReplayMilliseconds, true);
                    WriteProperty(builder, 3, "total_input_ms", sample.TotalInputMilliseconds, true);
                    WriteProperty(builder, 3, "observation_wait_ms", sample.ObservationWaitMilliseconds, true);
                    WriteProperty(builder, 3, "press_input_ms", sample.PressInputMilliseconds, true);
                    WriteProperty(builder, 3, "move_input_ms", sample.MoveInputMilliseconds, true);
                    WriteProperty(builder, 3, "release_input_ms", sample.ReleaseInputMilliseconds, true);
                    WriteProperty(builder, 3, "observed_stroke", sample.ObservedStroke, false);
                    Indent(builder, 2).Append(i == samples.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteScenarioInventory(StringBuilder builder, IReadOnlyList<ScenarioInventoryEntry> scenarios)
            {
                Indent(builder, 1).AppendLine("\"scenario_inventory\": [");
                for (var i = 0; i < scenarios.Count; i++)
                {
                    var scenario = scenarios[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "group", scenario.Group, true);
                    WriteProperty(builder, 3, "id", scenario.Id, true);
                    WriteProperty(builder, 3, "category", scenario.Category, true);
                    WriteProperty(builder, 3, "expected_stroke", scenario.ExpectedStroke, true);
                    WriteProperty(builder, 3, "expected_handler", scenario.ExpectedHandler, true);
                    WriteProperty(builder, 3, "event_count", scenario.EventCount, true);
                    WriteProperty(builder, 3, "point_count", scenario.PointCount, false);
                    Indent(builder, 2).Append(i == scenarios.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteSummaries(StringBuilder builder, string propertyName, IReadOnlyList<PerformanceSummary> summaries)
            {
                Indent(builder, 1).Append("\"").Append(propertyName).AppendLine("\": [");
                for (var i = 0; i < summaries.Count; i++)
                {
                    var summary = summaries[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "scope", summary.Scope, true);
                    WriteProperty(builder, 3, "group", summary.Group, true);
                    WriteProperty(builder, 3, "scenario_id", summary.ScenarioId, true);
                    WriteProperty(builder, 3, "scenario_count", summary.ScenarioCount, true);
                    WriteProperty(builder, 3, "sample_count", summary.SampleCount, true);
                    WriteProperty(builder, 3, "event_count", summary.EventCount, true);
                    WriteProperty(builder, 3, "min_ms", summary.MinMilliseconds, true);
                    WriteProperty(builder, 3, "median_ms", summary.MedianMilliseconds, true);
                    WriteProperty(builder, 3, "p95_ms", summary.P95Milliseconds, true);
                    WriteProperty(builder, 3, "max_ms", summary.MaxMilliseconds, true);
                    WriteProperty(builder, 3, "mean_ms", summary.MeanMilliseconds, false);
                    Indent(builder, 2).Append(i == summaries.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteAttributionSummaries(StringBuilder builder, string propertyName, IReadOnlyList<AttributionSummary> summaries)
            {
                Indent(builder, 1).Append("\"").Append(propertyName).AppendLine("\": [");
                for (var i = 0; i < summaries.Count; i++)
                {
                    var summary = summaries[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "scope", summary.Scope, true);
                    WriteProperty(builder, 3, "group", summary.Group, true);
                    WriteProperty(builder, 3, "scenario_id", summary.ScenarioId, true);
                    WriteProperty(builder, 3, "scenario_count", summary.ScenarioCount, true);
                    WriteProperty(builder, 3, "sample_count", summary.SampleCount, true);
                    WriteProperty(builder, 3, "event_count", summary.EventCount, true);
                    WriteProperty(builder, 3, "total_median_ms", summary.TotalMedianMilliseconds, true);
                    WriteProperty(builder, 3, "total_p95_ms", summary.TotalP95Milliseconds, true);
                    WriteProperty(builder, 3, "total_max_ms", summary.TotalMaxMilliseconds, true);
                    WriteProperty(builder, 3, "total_mean_ms", summary.TotalMeanMilliseconds, true);
                    WriteProperty(builder, 3, "input_median_ms", summary.InputMedianMilliseconds, true);
                    WriteProperty(builder, 3, "input_p95_ms", summary.InputP95Milliseconds, true);
                    WriteProperty(builder, 3, "input_max_ms", summary.InputMaxMilliseconds, true);
                    WriteProperty(builder, 3, "input_mean_ms", summary.InputMeanMilliseconds, true);
                    WriteProperty(builder, 3, "wait_median_ms", summary.WaitMedianMilliseconds, true);
                    WriteProperty(builder, 3, "wait_p95_ms", summary.WaitP95Milliseconds, true);
                    WriteProperty(builder, 3, "wait_max_ms", summary.WaitMaxMilliseconds, true);
                    WriteProperty(builder, 3, "wait_mean_ms", summary.WaitMeanMilliseconds, true);
                    WriteProperty(builder, 3, "validation_median_ms", summary.ValidationMedianMilliseconds, true);
                    WriteProperty(builder, 3, "validation_p95_ms", summary.ValidationP95Milliseconds, true);
                    WriteProperty(builder, 3, "validation_max_ms", summary.ValidationMaxMilliseconds, true);
                    WriteProperty(builder, 3, "validation_mean_ms", summary.ValidationMeanMilliseconds, false);
                    Indent(builder, 2).Append(i == summaries.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteSamples(StringBuilder builder, IReadOnlyList<PerformanceSample> samples)
            {
                Indent(builder, 1).AppendLine("\"samples\": [");
                for (var i = 0; i < samples.Count; i++)
                {
                    var sample = samples[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "iteration", sample.Iteration, true);
                    WriteProperty(builder, 3, "group", sample.Group, true);
                    WriteProperty(builder, 3, "scenario_id", sample.ScenarioId, true);
                    WriteProperty(builder, 3, "category", sample.Category, true);
                    WriteProperty(builder, 3, "expected_stroke", sample.ExpectedStroke, true);
                    WriteProperty(builder, 3, "expected_handler", sample.ExpectedHandler, true);
                    WriteProperty(builder, 3, "event_count", sample.EventCount, true);
                    WriteProperty(builder, 3, "point_count", sample.PointCount, true);
                    WriteProperty(builder, 3, "elapsed_ticks", sample.ElapsedTicks, true);
                    WriteProperty(builder, 3, "elapsed_ms", sample.ElapsedMilliseconds, true);
                    WriteProperty(builder, 3, "press_consumed", sample.PressConsumed, true);
                    WriteProperty(builder, 3, "trigger_consumed", sample.TriggerConsumed, true);
                    WriteProperty(builder, 3, "release_consumed", sample.ReleaseConsumed, true);
                    WriteProperty(builder, 3, "stroke_observed_before_release", sample.StrokeObservedBeforeRelease, true);
                    WriteProperty(builder, 3, "observed_stroke", sample.ObservedStroke, false);
                    Indent(builder, 2).Append(i == samples.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteAttributionSamples(StringBuilder builder, IReadOnlyList<AttributionSample> samples)
            {
                Indent(builder, 1).AppendLine("\"samples\": [");
                for (var i = 0; i < samples.Count; i++)
                {
                    var sample = samples[i];
                    Indent(builder, 2).AppendLine("{");
                    WriteProperty(builder, 3, "iteration", sample.Iteration, true);
                    WriteProperty(builder, 3, "group", sample.Group, true);
                    WriteProperty(builder, 3, "scenario_id", sample.ScenarioId, true);
                    WriteProperty(builder, 3, "category", sample.Category, true);
                    WriteProperty(builder, 3, "expected_stroke", sample.ExpectedStroke, true);
                    WriteProperty(builder, 3, "expected_handler", sample.ExpectedHandler, true);
                    WriteProperty(builder, 3, "event_count", sample.EventCount, true);
                    WriteProperty(builder, 3, "point_count", sample.PointCount, true);
                    WriteProperty(builder, 3, "total_replay_ticks", sample.TotalReplayTicks, true);
                    WriteProperty(builder, 3, "total_replay_ms", sample.TotalReplayMilliseconds, true);
                    WriteProperty(builder, 3, "total_input_ticks", sample.TotalInputTicks, true);
                    WriteProperty(builder, 3, "total_input_ms", sample.TotalInputMilliseconds, true);
                    WriteProperty(builder, 3, "press_input_ticks", sample.PressInputTicks, true);
                    WriteProperty(builder, 3, "press_input_ms", sample.PressInputMilliseconds, true);
                    WriteProperty(builder, 3, "move_input_ticks", sample.MoveInputTicks, true);
                    WriteProperty(builder, 3, "move_input_ms", sample.MoveInputMilliseconds, true);
                    WriteProperty(builder, 3, "trigger_input_ticks", sample.TriggerInputTicks, true);
                    WriteProperty(builder, 3, "trigger_input_ms", sample.TriggerInputMilliseconds, true);
                    WriteProperty(builder, 3, "release_input_ticks", sample.ReleaseInputTicks, true);
                    WriteProperty(builder, 3, "release_input_ms", sample.ReleaseInputMilliseconds, true);
                    WriteProperty(builder, 3, "observation_wait_ticks", sample.ObservationWaitTicks, true);
                    WriteProperty(builder, 3, "observation_wait_ms", sample.ObservationWaitMilliseconds, true);
                    WriteProperty(builder, 3, "observation_read_ticks", sample.ObservationReadTicks, true);
                    WriteProperty(builder, 3, "observation_read_ms", sample.ObservationReadMilliseconds, true);
                    WriteProperty(builder, 3, "validation_ticks", sample.ValidationTicks, true);
                    WriteProperty(builder, 3, "validation_ms", sample.ValidationMilliseconds, true);
                    WriteProperty(builder, 3, "press_consumed", sample.PressConsumed, true);
                    WriteProperty(builder, 3, "trigger_consumed", sample.TriggerConsumed, true);
                    WriteProperty(builder, 3, "release_consumed", sample.ReleaseConsumed, true);
                    WriteProperty(builder, 3, "stroke_observed_before_release", sample.StrokeObservedBeforeRelease, true);
                    WriteProperty(builder, 3, "observed_stroke", sample.ObservedStroke, false);
                    Indent(builder, 2).Append(i == samples.Count - 1 ? "}" : "},").AppendLine();
                }

                Indent(builder, 1).Append("]");
            }

            private static void WriteProperty(StringBuilder builder, int indent, string name, string value, bool comma)
            {
                Indent(builder, indent).Append("\"").Append(name).Append("\": ");
                if (value == null)
                {
                    builder.Append("null");
                }
                else
                {
                    AppendJsonString(builder, value);
                }

                if (comma)
                {
                    builder.Append(",");
                }

                builder.AppendLine();
            }

            private static void WriteProperty(StringBuilder builder, int indent, string name, int value, bool comma)
            {
                Indent(builder, indent).Append("\"").Append(name).Append("\": ").Append(value);
                if (comma)
                {
                    builder.Append(",");
                }

                builder.AppendLine();
            }

            private static void WriteProperty(StringBuilder builder, int indent, string name, long value, bool comma)
            {
                Indent(builder, indent).Append("\"").Append(name).Append("\": ").Append(value);
                if (comma)
                {
                    builder.Append(",");
                }

                builder.AppendLine();
            }

            private static void WriteProperty(StringBuilder builder, int indent, string name, double value, bool comma)
            {
                Indent(builder, indent)
                    .Append("\"")
                    .Append(name)
                    .Append("\": ")
                    .Append(value.ToString("0.########", CultureInfo.InvariantCulture));
                if (comma)
                {
                    builder.Append(",");
                }

                builder.AppendLine();
            }

            private static void WriteProperty(StringBuilder builder, int indent, string name, bool value, bool comma)
            {
                Indent(builder, indent).Append("\"").Append(name).Append("\": ").Append(value ? "true" : "false");
                if (comma)
                {
                    builder.Append(",");
                }

                builder.AppendLine();
            }

            private static StringBuilder Indent(StringBuilder builder, int level)
            {
                return builder.Append(new string(' ', level * 2));
            }

            private static void AppendJsonString(StringBuilder builder, string value)
            {
                builder.Append("\"");
                foreach (var c in value)
                {
                    switch (c)
                    {
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            if (char.IsControl(c))
                            {
                                builder.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                builder.Append(c);
                            }

                            break;
                    }
                }

                builder.Append("\"");
            }

            private static string FormatMilliseconds(double value)
            {
                return value.ToString("0.###", CultureInfo.InvariantCulture);
            }
        }

        private sealed class ReplayEvaluationContext : EvaluationContext
        { }

        private sealed class ReplayExecutionContext : ExecutionContext
        { }

        private sealed class ReplayContextManager : ContextManager<ReplayEvaluationContext, ReplayExecutionContext>
        {
            public override ReplayEvaluationContext CreateEvaluateContext()
                => new ReplayEvaluationContext();

            public override ReplayExecutionContext CreateExecutionContext(ReplayEvaluationContext evaluationContext)
                => new ReplayExecutionContext();
        }

        private sealed class ReplayGestureMachineConfig : Crevice.Core.FSM.GestureMachineConfig
        { }

        private sealed class ReplayCallbackManager : Crevice.Core.Callback.CallbackManager<ReplayGestureMachineConfig, ReplayContextManager, ReplayEvaluationContext, ReplayExecutionContext>
        { }

        private sealed class ReplayGestureMachine : Crevice.Core.FSM.GestureMachine<ReplayGestureMachineConfig, ReplayContextManager, ReplayEvaluationContext, ReplayExecutionContext>
        {
            private readonly ReplaySchedulerLease schedulerLease;

            public ReplayGestureMachine(
                ReplayGestureMachineConfig config,
                ReplayCallbackManager callbackManager,
                ReplaySchedulerLease schedulerLease)
                : base(config, callbackManager, new ReplayContextManager())
            {
                this.schedulerLease = schedulerLease;
            }

            protected internal override TaskFactory StrokeWatcherTaskFactory => schedulerLease.TaskFactory;

            protected override void Dispose(bool disposing)
            {
                try
                {
                    base.Dispose(disposing);
                }
                finally
                {
                    if (disposing)
                    {
                        schedulerLease.Dispose();
                    }
                }
            }
        }
    }
}



