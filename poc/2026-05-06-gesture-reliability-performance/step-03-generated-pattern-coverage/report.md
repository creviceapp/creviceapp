# Report: Step 03 - Generated Pattern Coverage

## Scope

This step expanded deterministic correctness coverage for generated default browser stroke gestures in `C:\Users\seigl\projects\creviceapp` on branch `split-store-msix-release-zip`. Work stayed inside the supervised write scope: `CreviceAppTests/GestureCoverageHarnessTests.cs` and Step 03 PoC artifacts. No production code was changed, no commit or push was made, and the untracked `docs/` directory was not touched.

## Harness Changes

`CreviceAppTests/GestureCoverageHarnessTests.cs` now includes a generated positive stroke matrix in `GeneratedDefaultBrowserStrokePatternsReplayThroughSyntheticInput`.

The matrix covers every active default stroke gesture:

- `U`
- `D`
- `L`
- `R`
- `UD`
- `DR`

Each gesture is replayed across:

- Start coordinates: `(400,400)`, `(2600,540)`, `(-800,360)`.
- Distances above threshold: `32`, `48`, `72` px.
- Point densities: `2`, `4`, `7` subdivisions per segment.
- Orthogonal jitter amplitudes: `0`, `3` px.

That produces 324 positive generated scenarios, 54 per default stroke gesture. Each scenario asserts RButton press/release consumption, observes the expected stroke sequence before release, verifies the expected handler runs exactly once, and verifies no extra handler runs. The wait remains a synchronization wait for the asynchronous stroke watcher, not a performance assertion.

The new `GeneratedNegativeStrokePatternsDoNotExecuteHandlers` test keeps negative correctness separate from positive replay. It covers:

- Generated below-threshold movement with no established stroke.
- Unknown generated `RL` stroke.
- Unknown generated `LR` stroke.
- Stable noisy `URDL` non-match.

The replay suite remains correctness-only. It does not use `Stopwatch`, timing thresholds, BenchmarkDotNet, elapsed-time logs for pass/fail, hooks, SendInput, real cursor movement, foreground windows, display state, or overlay UI.

## Structured Output

Saved structured inventory and results to:

```text
poc/2026-05-06-gesture-reliability-performance/step-03-generated-pattern-coverage/output/generated-pattern-scenarios-results.json
```

The JSON contains:

- Scenario dimensions and default stroke gestures.
- 328 generated scenarios total.
- Per-scenario ids, category, expected stroke, expected handler result, start metadata, distance, density, jitter, point count, generated points, and pass status.
- Focused VSTest command/result metadata and TRX path.

## Commands And Results

Initial sanitized Visual Studio MSBuild attempt:

```text
$msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
Remove-Item Env:PATH -ErrorAction SilentlyContinue
& $msbuild CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: failed before compilation. Removing `PATH` entirely caused the .NET SDK resolver to fail resolving `Microsoft.NET.Sdk` for `CreviceLib` with a `System.NullReferenceException` in `Microsoft.DotNet.NativeWrapper.EnvironmentProvider.get_SearchPaths()`.

Adjusted sanitized Visual Studio MSBuild:

```text
$msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& $msbuild CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed.

```text
CreviceLib -> CreviceLib\bin\Release\net462\Crevice.Core.dll
CreviceApp -> CreviceApp\bin\Release\crevice4.exe
CreviceAppTests -> CreviceAppTests\bin\Release\Crevice4Tests.dll
```

Focused VSTest run:

```text
$vstest = 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe'
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& $vstest CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"FullyQualifiedName~GestureCoverageHarnessTests" /Logger:"trx;LogFileName=GestureCoverageHarnessTests.Step03.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-03-generated-pattern-coverage\output\test-results"
```

Result: passed.

```text
Total tests: 7
Passed: 7
TRX: poc/2026-05-06-gesture-reliability-performance/step-03-generated-pattern-coverage/output/test-results/GestureCoverageHarnessTests.Step03.trx
```

## Observations

No gesture-recognition failure mode appeared during this step. The only surprising behavior was environmental: fully removing `PATH` avoids duplicate `Path`/`PATH` issues but breaks the Visual Studio .NET SDK resolver on this machine. A minimal single `PATH` is the stable local build/test recipe.

## Step 04 Recommendations

1. Keep the generated matrix as correctness coverage and resist adding timing assertions to this class.
2. If Step 04 introduces performance measurement, put it in a separate measurement artifact or benchmark path with explicit manual/diagnostic intent.
3. Consider promoting the minimal single-`PATH` Visual Studio MSBuild/VSTest recipe into the PoC instructions for this machine.
4. If generated coverage grows further, split scenario metadata and replay helpers into a dedicated helper file under `CreviceAppTests/` to keep the test class readable.
5. Add CI guidance for whether the full 324-case matrix should run on every PR; the current local focused run passed quickly, so it appears CI-safe unless hosted-agent variability proves otherwise.
