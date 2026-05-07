# Report: Step 04 - Performance Baseline Measurement

## Scope

This step added and executed a measurement-only synthetic gesture replay harness in `C:\Users\seigl\projects\creviceapp` on branch `split-store-msix-release-zip`. Work stayed inside `CreviceAppTests/` and this Step 04 PoC directory. No production code was changed, no commit or push was made, and the untracked `docs/` directory was not touched.

This was a measurement phase only. The harness validates that each measured replay remains semantically valid, but it does not apply hard timing thresholds and should not be treated as a CI performance gate yet.

## Harness

Added `CreviceAppTests/GesturePerformanceHarnessTests.cs` as a separate MSTest class with `[TestCategory("Performance")]`.

The harness uses deterministic synthetic `GestureMachine.Input(...)` replay only:

- No hooks.
- No `SendInput`.
- No real cursor movement.
- No foreground window or display-state dependency.
- No overlay UI.
- No production tuning.

Measured groups:

- `wheel`: `RButton + WheelUp`, `RButton + WheelDown`.
- `single-stroke`: baseline `U`, `D`, `L`, `R`.
- `multi-stroke`: baseline `UD`, `DR`.
- `generated-positive`: the 324 positive generated stroke scenarios from Step 03's matrix shape.
- `generated-negative`: below-threshold, unknown `RL`, unknown `LR`, and noisy `URDL` non-match.

Configuration:

- Warmup iterations: 2.
- Measured iterations: 5.
- Scenario count per iteration: 336.
- Event count per measured iteration: 2,620.
- Total measured events: 13,100.

## Output

Structured measurement JSON:

```text
poc/2026-05-06-gesture-reliability-performance/step-04-performance-baseline-measurement/output/gesture-performance-baseline.json
```

Concise Markdown summary:

```text
poc/2026-05-06-gesture-reliability-performance/step-04-performance-baseline-measurement/output/gesture-performance-baseline-summary.md
```

Focused VSTest TRX:

```text
poc/2026-05-06-gesture-reliability-performance/step-04-performance-baseline-measurement/output/test-results/GesturePerformanceHarnessTests.Step04.trx
```

The JSON includes scenario inventory, environment metadata, every measured sample, group summaries, and per-scenario summaries with min/median/p95/max/mean timings.

## Baseline Results

Environment:

- Machine: `9TH`
- OS: `Microsoft Windows NT 10.0.26200.0`
- CLR: `4.0.30319.42000`
- Process architecture: `x64`
- Build configuration: `Release`
- Stopwatch high resolution: recorded in JSON.

Summary:

| Scope | Group | Scenarios | Samples | Events | Min ms | Median ms | P95 ms | Max ms | Mean ms |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| overall | all | 336 | 1680 | 13100 | 0.008 | 0.024 | 0.059 | 14.777 | 0.202 |
| group | generated-negative | 4 | 20 | 250 | 0.015 | 0.028 | 0.126 | 13.048 | 0.688 |
| group | generated-positive | 324 | 1620 | 12600 | 0.008 | 0.023 | 0.059 | 14.777 | 0.200 |
| group | multi-stroke | 2 | 10 | 100 | 0.016 | 0.026 | 0.042 | 0.042 | 0.027 |
| group | single-stroke | 4 | 20 | 120 | 0.012 | 0.026 | 0.048 | 0.066 | 0.028 |
| group | wheel | 2 | 10 | 30 | 0.020 | 0.024 | 0.055 | 0.055 | 0.032 |

The high maximum samples are treated as noise until Step 05 inspects the distribution and replay synchronization behavior. Most medians and p95 values were below 0.13 ms in this local run.

## Commands And Results

Initial build attempt:

```text
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: failed with the known local duplicate `Path`/`PATH` dictionary-key exception from Roslyn/MSBuild.

Adjusted build:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed.

Focused measurement run:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe' CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"TestCategory=Performance" /Logger:"trx;LogFileName=GesturePerformanceHarnessTests.Step04.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-04-performance-baseline-measurement\output\test-results"
```

Result: passed.

```text
Total tests: 1
Passed: 1
Reported test duration: 707 ms
Total VSTest time: 1.2791 s
```

JSON validation:

```text
Get-Content output\gesture-performance-baseline.json -Raw | ConvertFrom-Json
```

Result: passed; the structured JSON parsed successfully and contained the expected 336 scenarios and 1,680 measured samples.

## Observations

The measurement path is fast enough for local manual use, but the distribution contains visible outliers. Since the replay path waits for the asynchronous stroke watcher to observe expected stroke state before release, some measured samples include synchronization scheduling noise in addition to input-processing cost.

The generated-positive group dominates sample count and event count, so the overall mean is mostly a generated-positive summary. The small wheel/single/multi groups are useful sanity buckets but have too few samples for strong conclusions in this run.

The below-threshold and unknown/noisy negative cases remained semantically correct during measurement and did not execute handlers.

## Step 05 Recommendations

1. Inspect per-scenario summaries in the JSON before optimizing. Sort by `max_ms`, `p95_ms`, and `mean_ms` to separate repeated slow patterns from isolated scheduling outliers.
2. Split timing attribution in a follow-up harness if needed: one stopwatch around raw `Input(...)` calls and another around stroke-watcher synchronization. This will help tell processing cost apart from async observation latency.
3. Keep performance results advisory until multiple runs on an idle machine show stable medians and p95 values.
4. Consider increasing measured iterations for Step 05 analysis runs, but keep the Step 04 harness threshold-free.
5. Do not tune production code from this single baseline. Use this run to pick candidate hot spots and design narrower instrumentation.
