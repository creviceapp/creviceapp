# Mission: Step 06 - Measurement Determinism Tuning

## Purpose

Improve measurement determinism and repeatability before changing production performance behavior. Step 05 showed that the largest replay outliers are dominated by asynchronous `StrokeWatcher` observation wait, not raw `GestureMachine.Input(...)` time. This step should make that distinction easier to measure repeatedly and decide whether production tuning is justified.

## Method

1. Read Step 05 `report.md`, `gesture-performance-analysis-summary.md`, and the attribution JSON files.
2. Inspect `StrokeWatcher`, `GestureMachine`, and the current performance harness enough to understand feasible test-only measurement options.
3. Implement the lowest-risk test-only improvement that separates measurement modes clearly. Preferred options:
   - add a raw-input-focused measurement mode that records each `GestureMachine.Input(...)` call and does not include stroke observation wait in the raw-input aggregate;
   - add a repeat-run attribution command/test category that runs the attribution harness several times and reports run-to-run variance;
   - add an explicit watcher-wait summary that ranks observation latency independently from raw input time.
4. Keep correctness validation intact but keep all timing threshold-free.
5. If a small harness refactor is required, keep it scoped to `CreviceAppTests/` and avoid broad churn. Do not change production code unless the evidence is overwhelming and the change is tiny; stop and report before broader production tuning.
6. Run the focused Step 06 measurement with exactly one sub-agent. Use warmup and repeated measured runs. Treat values as noisy.
7. Save structured JSON and Markdown summaries under Step 06 `output/`.
8. Append English notes to `log.md` and write detailed English `report.md` with whether Step 07 should proceed to production tuning, measurement-only refinement, or CI integration.

## Expected Results

- Measurement outputs distinguish raw input processing from watcher observation latency more clearly than Step 05.
- Repeated run results show whether the watcher-wait outliers are stable enough to optimize or mostly scheduler noise.
- No hard performance gate is introduced.
- No production behavior changes are made unless a very small, well-supported change is explicitly justified in the report.
- Step 07 recommendation is evidence-based.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not add timing thresholds to tests.
- Do not use hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.
- This phase includes measurement; exactly one sub-agent may execute it.
- Respect existing uncommitted changes.

## Supplemental Notes

Current evidence:
- Step 04: median 0.024 ms, p95 0.059 ms, max 14.777 ms.
- Step 05 attribution: overall total median 0.02 ms, p95 0.06 ms, max 14.03 ms; input max 2.00 ms; watcher wait max 13.98 ms; validation max 0.05 ms.
- Largest spikes appear isolated and scheduler-shaped.
