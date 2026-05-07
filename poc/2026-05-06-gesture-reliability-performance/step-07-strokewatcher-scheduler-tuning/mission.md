# Mission: Step 07 - StrokeWatcher Scheduler Tuning Investigation

## Purpose

Investigate whether the Step 04-06 watcher observation tails are a measurement artifact of the test replay scheduler, a production-relevant `StrokeWatcher` scheduling issue, or a candidate for narrow production tuning. A key discovery is that the test replay machine currently uses `Task.Factory`, while the real app gesture machine uses `LowLatencyScheduler("StrokeWatcherTaskScheduler", ThreadPriority.Highest, 2)`.

## Method

1. Read Step 06 report and repeatability summary.
2. Inspect:
   - `CreviceAppTests/GesturePerformanceHarnessTests.cs`
   - `CreviceAppTests/GestureCoverageHarnessTests.cs`
   - `CreviceApp/GM.GestureMachine.cs`
   - `CreviceApp/Threading.cs`
   - `CreviceLib/Core.Stroke.StrokeWatcher.cs`
   - relevant FSM reset/input paths.
3. Add a focused measurement comparison that can run the same attribution/repeatability scenarios across scheduler modes, at minimum:
   - current default `Task.Factory` replay scheduler;
   - production-like `LowLatencyScheduler` replay scheduler with high priority and pool size 2.
4. Keep the change test-only if possible. If production code changes appear justified, first prove the need with scheduler comparison and keep any experiment tiny and reversible.
5. Measure with warmup and repeated runs. Keep all timings threshold-free.
6. Save structured scheduler comparison JSON and a Markdown summary under Step 07 `output/`.
7. Run correctness tests affected by scheduler mode plus the focused scheduler comparison measurement.
8. Analyze whether the performance harness should switch to production-like scheduler by default, whether production code needs tuning, or whether the current tail is mostly test scheduler artifact.
9. Append English notes to `log.md`; write detailed English `report.md` with evidence and recommendations.

## Expected Results

- A direct comparison of default test scheduler vs production-like low-latency scheduler.
- Evidence about whether watcher observation wait tails shrink under production-like scheduling.
- A clear recommendation for the harness default and for any production tuning.
- No CI performance gate yet.
- No commit or push.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not add hard timing thresholds.
- Do not use hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.
- This phase includes measurement; exactly one sub-agent may execute it.
- Respect existing uncommitted changes.
- Avoid broad refactoring of the large measurement harness unless strictly necessary.

## Supplemental Notes

Evidence so far:
- Step 06 repeated runs: watcher wait had 21-22 samples >= 1 ms per run; raw input had 5-6.
- Real app `Crevice.GestureMachine.GestureMachine` overrides `StrokeWatcherTaskFactory` with a `LowLatencyScheduler` at `ThreadPriority.Highest`, pool size 2.
- Current test replay machines override `StrokeWatcherTaskFactory => Task.Factory`.
