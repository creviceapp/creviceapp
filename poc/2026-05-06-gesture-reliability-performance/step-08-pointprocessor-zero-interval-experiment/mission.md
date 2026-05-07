# Mission: Step 08 - PointProcessor Zero-Interval Experiment

## Purpose

Verify whether `PointProcessor` should avoid starting its background skipped-point task when `watchInterval <= 0`. Step 07 showed that the production-like two-thread `LowLatencyScheduler` performs extremely poorly in the test replay harness when `StrokeWatchInterval = 0`. Code inspection suggests `PointProcessor` still starts a long-lived background task even though zero interval means every point is processed immediately and no skipped-point flushing is needed.

## Method

1. Read Step 07 report and scheduler comparison summary.
2. Inspect `CreviceLib/Core.Stroke.cs`, `CreviceLib/Core.Stroke.StrokeWatcher.cs`, and the performance harness scheduler comparison code.
3. Create a tiny, reversible production experiment if justified:
   - for `watchInterval <= 0`, do not start the `PointProcessor` background task;
   - ensure `Process(Point)` still immediately calls `OnProcess(point)` as before;
   - ensure disposal remains safe when no background task/token continuation was started.
4. Keep the change as small as possible. Do not broadly refactor `StrokeWatcher`, scheduler, or FSM state transitions.
5. Run focused correctness tests that cover stroke recognition and generated gesture replay.
6. Run the Step 07 scheduler comparison again, or a smaller focused scheduler comparison if the full run is too expensive, and save before/after interpretation under Step 08 output.
7. If the change helps only test-mode `StrokeWatchInterval = 0`, document that clearly. If it also suggests production default interval tuning, recommend a separate phase rather than broadening this one.
8. Keep all timings threshold-free.
9. Append English notes to `log.md`; write a detailed English `report.md` with evidence and a recommendation to keep, revert, or further investigate the production experiment.

## Expected Results

- Evidence on whether avoiding the zero-interval `PointProcessor` background task reduces low-latency scheduler observation waits.
- Correctness tests still pass.
- The production change, if made, is tiny and scoped to `watchInterval <= 0` behavior.
- A clear recommendation for Step 09: keep this change and proceed, revert it, or investigate production default `StrokeWatchInterval = 10` separately.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not add hard timing thresholds.
- Do not use hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.
- This phase includes measurement; exactly one sub-agent may execute it.
- Respect existing uncommitted changes.
- If the production experiment becomes larger than a small `PointProcessor` lifecycle guard, stop and report instead.

## Supplemental Notes

Current relevant facts:
- `GestureMachineConfig.StrokeWatchInterval` defaults to 10 ms, while the replay harness uses 0 ms for deterministic immediate processing.
- `PointProcessor.MustBeProcessed(...)` returns true for `watchInterval <= 0`, so no skipped-point background flush is needed in that mode.
- Step 07 low-latency scheduler comparison with zero interval showed wait median about 30.8 ms and wait p95 about 31.9 ms.
