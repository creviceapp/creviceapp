# Mission: Step 09 - Production Interval Measurement

## Purpose

Measure gesture replay behavior under production-like `StrokeWatchInterval = 10` after the Step 08 zero-interval guard. Step 08 fixed deterministic zero-interval replay, but the real app default remains 10 ms. This phase must determine whether the production default interval and low-latency scheduler have acceptable behavior, need measurement-only documentation, or justify a narrow production tuning investigation.

## Method

1. Read Step 08 `report.md` and before/after summaries.
2. Inspect the current performance harness and `GestureMachineConfig.StrokeWatchInterval` usage.
3. Add a focused threshold-free measurement mode that compares at least:
   - interval `0`, `Task.Factory` scheduler;
   - interval `0`, production-like `LowLatencyScheduler` scheduler;
   - interval `10`, `Task.Factory` scheduler;
   - interval `10`, production-like `LowLatencyScheduler` scheduler.
4. If cheap and useful, add a pool-size comparison for production-like low-latency scheduler at interval 10, e.g. pool size 2 versus 4, but keep this secondary and clearly labeled exploratory.
5. Preserve semantic validation for every replay. It is acceptable if interval 10 changes observation wait shape, but it must not change gesture outcomes.
6. Keep all timings threshold-free. Assertions should only verify that measurement data is produced and semantic replay remains valid.
7. Run focused correctness tests and the Step 09 measurement with exactly one sub-agent executing measurement.
8. Save structured JSON and Markdown summary under Step 09 `output/`.
9. Append English notes to `log.md`; write detailed English `report.md` with a recommendation for Step 10: CI integration, production tuning, or final consolidation.

## Expected Results

- Direct evidence comparing deterministic zero-interval replay against production-default interval replay.
- Evidence about whether production-like scheduler at interval 10 is acceptable or shows systematic tails.
- A decision on whether further production tuning is justified before CI integration.
- No hard timing gates.
- No commit or push.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not add hard timing thresholds.
- Do not use hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.
- This phase includes measurement; exactly one sub-agent may execute it.
- Respect existing uncommitted changes.
- Avoid broad refactoring unless narrowly required for the interval comparison.

## Supplemental Notes

Current evidence:
- Step 08 after zero-interval guard: production-like low-latency interval 0 wait median 0.023 ms, p95 0.050 ms in the focused run.
- Real app default `GestureMachineConfig.StrokeWatchInterval` is 10 ms.
- The zero-interval guard should not change default production interval 10 behavior directly.
