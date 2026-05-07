# PoC Protocol

This protocol defines the minimum process for running a reusable PoC.

## Core Rules

- Track work in the project `queue.md`.
- Split work into small `step-*` directories.
- Each step must contain `mission.md`, `log.md`, `report.md`, and `output/`.
- Write `mission.md` before starting the step.
- Keep `log.md` append-only.
- Write `report.md` as a standalone document for readers without prior context.
- Do not commit or push unless explicitly requested.
- Keep project-specific ignore rules in the project queue or mission. Do not accidentally include owner-designated temporary files.
- Keep project-specific commands and pitfalls in the project artifacts, not in the common harness.

## Step Quality Standard

### mission.md

`mission.md` should be detailed and explicit. A different operator should be able to execute the step from the mission alone.

It should clearly state:

- purpose;
- background;
- scope and non-scope;
- method;
- expected results;
- constraints;
- decision criteria;
- expected outputs.

### log.md

`log.md` should preserve the thought process and reproduction path.

Record enough detail that someone can later answer:

- what was tried;
- why it was tried;
- what was observed;
- what changed;
- what command or input produced the result;
- what decision followed.

Prefer concise timestamped entries over long prose. Do not rewrite history; append corrections as new entries.

### report.md

`report.md` should be suitable for a key stakeholder in another team.

It should not assume prior conversation context. It should explain the problem, key terms, work performed, evidence, results, risks, and recommendation.

When the report uses project-specific, domain-specific, or experiment-specific words, include a `Key Terms` section after `Background`.

## Delegation Rules

- The supervisor owns the queue, step design, scope, and final judgment.
- Sub-agents execute bounded tasks with explicit write scope.
- A sub-agent must write a start entry to `log.md` before doing substantive work.
- The supervisor reviews the sub-agent's changed files, artifacts, and report before opening the next step.

## Measurement Rules

- Separate correctness from measurement.
- Do not add hard timing thresholds unless the mission explicitly justifies them.
- Only one measurement actor should run at a time.
- Treat timing as noisy. Prefer repeated runs and distribution summaries over single max values.
- Measurement reports should distinguish observation, processing, validation, and environmental noise when relevant.

## Step Completion Checklist

Before marking a step Done:

- `log.md` contains meaningful progress notes.
- `report.md` contains evidence and recommendation.
- Important artifacts are under `output/`.
- The queue status is updated.
- Any ignored or out-of-scope files remained untouched.
- Commit/push status is explicitly known.
