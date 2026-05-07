## 11. CI, Build, and Test

### 11.1 CI Platform

GitHub Actions is the CI and packaging platform for this repository. The CI workflow runs on `windows-2022`.

### 11.2 CI Triggers

The CI workflow runs for pushes to `master` and `develop`, pull requests, and manual dispatch.

The Store package and release zip workflows run for tag pushes and manual dispatch.

### 11.3 Build Environment

Workflows use `microsoft/setup-msbuild` with x86 MSBuild architecture. Workflows install legacy Windows SDK contracts through `.github/scripts/Install-LegacyWindowsSdk.ps1`.

CI verifies the `.NET Framework 4.6.2` targeting pack before building .NET Framework projects.

Packaging verifies Windows SDK `10.0.22621.0` and Windows Application Packaging Project targets before building Store artifacts.

### 11.4 Restore and Build

CI restores NuGet packages for `CreviceApp`, `CreviceAppTests`, and `CreviceLibTests`. CI restores `CreviceLib` through `dotnet restore`.

CI builds `CreviceLib`, `CreviceLibTests`, and `CreviceAppTests` in `Release` configuration.

### 11.5 Normal Test Execution

CI runs `CreviceLibTests` with the benchmark test excluded by fully qualified name filter `FullyQualifiedName!~EventsBenchmarkTest`.

CI runs gesture coverage tests from `CreviceAppTests` with the filter `FullyQualifiedName~GestureCoverageHarnessTests`.

Default CI does not run diagnostic performance harness tests.

Default CI does not install real low-level Windows hooks or send real integration input. OS integration smoke tests use the `OSIntegration` test category and run only when `CREVICE_RUN_OS_INTEGRATION_TESTS=1` is set.

### 11.6 Test Results and Build Artifacts

CI uploads `.trx` test results from `TestResults/**/*.trx`.

CI uploads the desktop app build output from `CreviceApp/bin/Release/**`.

CI uploads `Crevice.Core` NuGet packages from `artifacts/nuget/*.nupkg`.

### 11.7 Failure Policy

A missing targeting pack, missing VS test runner, missing Windows SDK, or missing packaging target fails the relevant workflow before producing release artifacts.
