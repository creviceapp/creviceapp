# Gesture Reliability and Performance Queue

This queue is the authoritative task tracker for the PoC. The supervisor updates it as phases are planned, delegated, reviewed, and completed.

## Global Rules

- Do not commit or push unless the user explicitly requests it.
- Do not include or modify the untracked `docs/` directory.
- Store all phase artifacts under this PoC directory.
- Each phase must have a `mission.md`, append-only `log.md`, `report.md`, and `output/` artifacts when applicable.
- Measurement phases must be executed by only one sub-agent at a time.
- Performance measurements must be treated as noisy; rely on repeated runs, medians, p95, and qualitative stability checks rather than single-run conclusions.

## Tasks

| ID | Status | Owner | Phase | Goal | Notes |
| --- | --- | --- | --- | --- | --- |
| Q-001 | Done | Supervisor | Setup | Create PoC workspace, queue, and first mission. | Initial structure created. |
| Q-010 | Done | Pascal | Step 01 | Inventory current tests, gesture code paths, and reliable command flow. | Report and structured inventory completed. |
| Q-020 | Done | Noether | Step 02 | Expand default gesture coverage and basic success/failure replay tests. | 5 focused app tests passed. |
| Q-030 | Done | Ampere | Step 03 | Build generated pattern coverage for all default gestures. | 324 positive and 4 negative generated cases passed. |
| Q-040 | Done | Meitner | Step 04 | Add measurement harness and baseline performance observations. | Baseline: 336 scenarios, median 0.024 ms, p95 0.059 ms, max 14.777 ms. |
| Q-050 | Done | Beauvoir | Step 05 | Analyze tuning opportunities from baseline measurements. | Attribution shows largest outliers are watcher observation wait, not raw Input. |
| Q-060 | Done | Einstein | Step 06 | Implement low-risk performance tuning candidates. | Repeatability confirms watcher observation wait dominates tails. |
| Q-070 | Done | Aquinas | Step 07 | Investigate StrokeWatcher scheduler/notification tuning before CI integration. | Scheduler comparison showed production-like low latency mode is much slower under zero interval. |
| Q-071 | Done | McClintock | Step 08 | Test PointProcessor zero-interval lifecycle tuning. | Keep zero-interval guard; low-latency wait median improved from ~30.813 ms to 0.023 ms. |
| Q-072 | Done | Herschel | Step 09 | Measure production default StrokeWatchInterval behavior. | Production interval 10 with low-latency pool 2 had wait median 0.015 ms and p95 0.034 ms with production-paced replay. |
| Q-080 | Done | Supervisor | Step 10 | Produce consolidated final report. | Final report completed. |

## Decision Log

- 2026-05-06: Issue #21 is intentionally out of scope for this PoC.
- 2026-05-06: Start with CI-stable replay tests that avoid real hooks, real mouse movement, and live window dependencies.

- 2026-05-07: Added Step 08 because Step 07 revealed StrokeWatchInterval = 0 still starts a PointProcessor background task, which may distort scheduler comparisons and waste scheduler capacity.

- 2026-05-07: Added Step 09 to measure production default StrokeWatchInterval = 10 separately from the zero-interval replay configuration.



