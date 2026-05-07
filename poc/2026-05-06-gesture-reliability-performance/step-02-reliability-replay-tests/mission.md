# Mission: Step 02 - Reliability Replay Tests

## Purpose

Expand CI-safe reliability coverage for Crevice's default gesture behavior. This step must convert the current exploratory gesture replay work into deterministic correctness tests for basic operation, while separating timing and benchmarking concerns into later phases.

## Method

1. Read Step 01's `report.md` and `output/baseline-inventory.json`.
2. Inspect the current uncommitted `CreviceAppTests/GestureCoverageHarnessTests.cs` and `CreviceAppTests/CreviceAppTests.csproj` changes.
3. Refactor or extend the gesture replay tests so that reliability assertions are independent from timing thresholds.
4. Cover all active default browser gestures from `DefaultUserScript.csx`:
   - `RButton + WheelUp`
   - `RButton + WheelDown`
   - `RButton + U`
   - `RButton + D`
   - `RButton + L`
   - `RButton + R`
   - `RButton + UD`
   - `RButton + DR`
5. Add negative/basic failure coverage:
   - an unregistered stroke must not execute a handler;
   - a `When` condition returning false must prevent execution;
   - a short movement below stroke threshold must not execute a stroke handler;
   - wheel gestures and stroke gestures must not cross-trigger each other.
6. Use synthetic `GestureMachine.Input(...)` sequences only. Do not use global hooks, SendInput, cursor movement, foreground window state, display state, or overlay UI.
7. If new helper classes are needed, keep them scoped to the test project unless there is a strong reason to alter production code.
8. Build and run the affected test class if feasible. If local app-test build requires sanitized MSBuild, use the known sanitized Visual Studio MSBuild approach.
9. Append detailed notes to `log.md` as work proceeds.
10. Write `report.md` in English with changed files, test cases added, commands/results, and Step 03 recommendations.

## Expected Results

- Deterministic tests prove the active default gesture set is complete.
- Each default gesture has at least one successful synthetic replay case.
- Basic false-positive scenarios are covered.
- Timing/performance assertions are removed from, ignored by, or clearly separated from the reliability test path.
- The affected app test project builds and the new reliability tests pass locally, or any blocker is documented precisely.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not benchmark or tune performance in this step.
- Do not add machine-dependent test behavior to normal reliability tests.
- Respect existing uncommitted user/worktree changes; do not revert them.
- Measurement phases must remain serialized later; this step is correctness only.

## Supplemental Notes

The current test harness already contains useful enumeration and synthetic replay ideas. Step 02 should make those correctness tests more complete and maintainable, and prepare a clean boundary for Step 03 generated-pattern coverage.
