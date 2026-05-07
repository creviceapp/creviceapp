# Mission: Step 05 - Performance Attribution Analysis

## Purpose

Analyze the Step 04 baseline before making any tuning changes. The main question is whether the observed outliers and averages come from gesture input processing itself, asynchronous stroke-watcher synchronization, scenario composition, or measurement harness structure. This phase may perform additional attribution measurement, but it must not tune production code yet.

## Method

1. Read Step 04 `report.md`, `gesture-performance-baseline-summary.md`, and `gesture-performance-baseline.json`.
2. Parse the baseline JSON and identify:
   - slowest samples by max elapsed time;
   - per-scenario summaries sorted by max, p95, and mean;
   - whether slow samples repeat on the same scenarios or look like isolated scheduling noise;
   - differences between wheel, single-stroke, multi-stroke, generated-positive, and generated-negative groups.
3. Inspect the measurement harness and replay path to determine what is included in each elapsed sample.
4. If feasible, add an attribution-only measurement path that separates at least:
   - press/move/wheel/release `GestureMachine.Input(...)` elapsed time;
   - stroke watcher observation wait elapsed time;
   - validation/recording overhead if practical.
5. If additional attribution measurement is implemented, run only that measurement with this single sub-agent and save structured results under this Step 05 `output/` directory. Do not run other measurement agents concurrently.
6. Analyze tuning candidates without implementing them yet. Rank by expected benefit, behavioral risk, and code complexity.
7. Consider maintainability of the test harness itself. If the large measurement class is becoming hard to evolve, recommend a refactor plan for shared test helpers, but do not perform broad refactoring unless it is necessary for attribution.
8. Append English notes to `log.md` as the analysis proceeds.
9. Write a detailed English `report.md` with findings, evidence, hypotheses, and concrete Step 06 tuning recommendations.

## Expected Results

- A clear explanation of the Step 04 timing distribution.
- Evidence about whether outliers are input-processing cost or asynchronous stroke-watcher wait/scheduling noise.
- Ranked tuning candidates for Step 06.
- A recommendation on whether to tune production code now, improve measurement first, or focus on harness maintainability.
- Structured analysis output in JSON/Markdown under `output/`.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not tune production code in this phase.
- Do not add hard timing thresholds to tests.
- Do not use hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.
- This phase may include measurement; if it does, only this one sub-agent may execute it.
- Respect existing uncommitted changes.

## Supplemental Notes

Step 04 baseline: 336 scenarios, 1,680 measured samples, 13,100 measured events, overall median 0.024 ms, p95 0.059 ms, max 14.777 ms. The report suspects synchronization noise because the timed replay includes waiting for the asynchronous stroke watcher to observe the expected stroke before release.
