# Mission: Step 04 - Performance Baseline Measurement

## Purpose

Create a reproducible, noise-aware performance measurement baseline for synthetic gesture replay. This phase must measure the current behavior without tuning it, and must keep measurement separate from correctness and CI pass/fail reliability tests.

## Method

1. Read Step 01, Step 02, and Step 03 reports and outputs.
2. Inspect the current replay helpers in `CreviceAppTests/GestureCoverageHarnessTests.cs` and decide whether a small helper refactor is needed to share replay code with a measurement harness.
3. Add a measurement-only harness that can be run manually or by a dedicated workflow later. Preferred shape:
   - a separate MSTest class/category such as `GesturePerformanceHarnessTests` with `[TestCategory("Performance")]`; or
   - a PoC-local runner only if a test-project integration would be too invasive.
4. The harness must replay deterministic gesture scenarios and collect repeatable metrics:
   - warmup iterations;
   - measured iterations;
   - scenario count;
   - event count;
   - per-scenario elapsed ticks/milliseconds;
   - aggregate min, median, p95, max, mean;
   - separate summaries for wheel, single-stroke, multi-stroke, generated-positive, and generated-negative scenarios when feasible.
5. Treat performance as noisy:
   - run multiple iterations;
   - do not fail the test on timing thresholds;
   - record environment metadata such as machine name, OS version, process architecture, CLR version, build configuration, and UTC/JST timestamp;
   - include an explicit note that results are not stable enough for a hard gate yet.
6. Save detailed measurement results to Step 04 `output/` as JSON and write a concise Markdown summary if useful.
7. Build and run only the measurement harness with exactly one sub-agent executing measurements. Do not run other measurement agents concurrently.
8. Append detailed English notes to `log.md` and write detailed English `report.md` with commands/results and Step 05 analysis recommendations.

## Expected Results

- A measurement harness exists and is clearly separated from correctness tests.
- At least one baseline measurement run succeeds locally.
- Step 04 `output/` contains structured measurement results and a readable summary.
- No performance tuning is implemented in this phase.
- The report identifies the most promising areas for Step 05 analysis without making premature optimization claims.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not add timing thresholds that can fail normal CI.
- Do not change production code unless an extremely small testability seam is unavoidable; stop and report if broader production changes seem necessary.
- Do not use hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.
- This is a measurement phase: only one sub-agent may execute it.

## Supplemental Notes

The current generated correctness matrix passed with 324 positive scenarios and 4 generated negative scenarios. Step 04 should reuse this deterministic coverage shape for measurement but must avoid turning timing into correctness. Any surprising performance shape should be documented as a hypothesis for Step 05, not fixed immediately.
