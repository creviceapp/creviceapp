# PointProcessor Zero-Interval Before/After Summary

No hard pass/fail timing threshold is applied.

## Hypothesis

When `watchInterval <= 0`, `PointProcessor.MustBeProcessed(...)` always returns true, so `Process(Point)` immediately calls `OnProcess(point)`. The skipped-point polling task has no useful flush work in this mode and can occupy a scheduler thread unnecessarily.

## Production Change

`CreviceLib/Core.Stroke.cs` now starts the `PointProcessor` background skipped-point task only when `watchInterval > 0`. If no task was started, `Dispose` directly disposes the token source.

## Before vs After

| Evidence | Scheduler mode | Runs | Samples | Total median ms | Total p95 ms | Wait median ms | Wait p95 ms | Wait >=1 ms |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Step 07 before | task-factory-default | 3 | 5040 | 0.024 | 0.059 | 0.008 | 0.029 | 68 |
| Step 07 before | production-like-low-latency | 3 | 5040 | 30.964 | 32.063 | 30.813 | 31.918 | 5005 |
| Step 08 after | task-factory-default | 1 | 1680 | 0.012 | 0.026 | 0.002 | 0.013 | 2 |
| Step 08 after | production-like-low-latency | 1 | 1680 | 0.032 | 0.060 | 0.023 | 0.050 | 22 |

## Correctness

- `GestureCoverageHarnessTests`: passed 7/7.
- Focused core stroke tests: passed 2/2.
- Step 08 focused scheduler comparison: passed 1/1.

## Recommendation

Keep the zero-interval guard. Investigate default production `StrokeWatchInterval = 10` separately if needed.
