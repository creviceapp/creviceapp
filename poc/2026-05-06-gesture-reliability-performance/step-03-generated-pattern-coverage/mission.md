# Mission: Step 03 - Generated Pattern Coverage

## Purpose

Extend the deterministic replay suite so default gestures are validated across generated motion patterns, not only one hand-picked path per gesture. This step should improve confidence that the core gesture recognizer accepts realistic variations while still remaining stable enough for CI.

## Method

1. Read Step 01 and Step 02 reports before editing.
2. Inspect `CreviceLib` stroke recognition behavior enough to choose safe generated variants.
3. Expand the replay harness or factor helpers if needed for readability.
4. Generate deterministic stroke scenarios for every active default stroke gesture: `U`, `D`, `L`, `R`, `UD`, `DR`.
5. Vary these dimensions in a controlled way:
   - start point: normal screen center, wide-monitor right side, negative-monitor coordinate;
   - segment distance: at least three distances above the stroke threshold;
   - point density: at least three subdivision counts;
   - light orthogonal jitter that should not change the intended stroke;
   - multi-stroke transitions for `UD` and `DR`.
6. Add generated negative scenarios that should not execute handlers:
   - below-threshold generated movement;
   - unknown generated stroke such as `RL` or `LR`;
   - ambiguous/noisy movement that should not match any default gesture if the recognizer classifies it differently.
7. Keep this phase correctness-only. Do not record elapsed time or add timing assertions.
8. Preserve deterministic reproducibility. If pseudo-random generation is used, use fixed seeds and save them in `output/`.
9. Build and run the affected generated-pattern tests if feasible using the sanitized Visual Studio MSBuild/VSTest approach.
10. Append English notes to `log.md`, write a detailed English `report.md`, and save structured scenario/test results under `output/`.

## Expected Results

- Every active default stroke gesture has multiple generated successful replay scenarios.
- Generated scenarios cover multiple coordinate regions, distances, densities, and small jitter variants.
- Negative generated scenarios prove basic false-positive resistance.
- The output directory contains a structured inventory of generated scenarios and results.
- The test suite remains deterministic and suitable for normal CI if runtime is acceptable; otherwise the report must identify a smaller CI-safe subset and a heavier manual subset.

## Constraints

- Do not commit or push.
- Do not touch `docs/`.
- Do not benchmark or tune performance in this step.
- Do not use hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.
- Do not run measurement phases concurrently with this step.
- Respect existing uncommitted changes.

## Supplemental Notes

Step 02 already created a replay root and recorder. Step 03 may refactor helpers out of the large test file if that makes the generated-pattern coverage maintainable. If a generated idea reveals a strong insight or surprising failure mode, add a focused sub-phase in `log.md`, save the case under `output/`, and either add a stable regression test or document why it should remain exploratory.
