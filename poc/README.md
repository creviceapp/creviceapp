# PoC Harness

This directory contains the common operating harness for PoC work.

The harness is intentionally small. It defines how to run a PoC, how to structure each step, how to delegate work, and how to produce reports that can be read without prior conversation context.

## Directory Layout

Use this layout for each PoC:

```text
poc/
├── README.md
├── protocol.md
├── prompts.md
├── template/
└── <yyyy-mm-dd>-<project-name>/
    ├── queue.md
    └── step-*/
        ├── mission.md
        ├── log.md
        ├── report.md
        └── output/
```

## How To Use

1. Read `protocol.md`.
2. Create a project directory under `poc/<yyyy-mm-dd>-<project-name>/`.
3. Copy `template/queue.md` into the project directory.
4. For each step, create a `step-*` directory and copy the step templates.
5. Write `mission.md` before starting work.
6. Keep `log.md` append-only while working.
7. Write `report.md` so it can be handed to a key stakeholder who has not followed the conversation.
8. Store machine-readable or reproducible artifacts under `output/`.

## Scope Of This Harness

Keep this harness generic. Project-specific commands, paths, failures, measurements, and local environment notes belong in the relevant PoC project's `report.md` or `output/` files, not in these common files.
