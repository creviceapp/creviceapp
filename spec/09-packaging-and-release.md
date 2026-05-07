## 9. Packaging and Release

### 9.1 Target Runtime

CreviceApp and test projects target `.NET Framework 4.6.2`. `CreviceLib` builds as the shared core library used by the desktop app and tests.

GitHub-hosted builds verify the `.NET Framework 4.6.2` targeting pack before compiling projects that require it.

### 9.2 Store Upload Package

`CreviceAppPackage` builds the Microsoft Store upload artifact. The Store packaging workflow runs on `windows-2022`, uses VS2022 MSBuild, verifies Windows Application Packaging Project targets, and builds with `UapAppxPackageBuildMode=StoreUpload`.

Store upload package signing is disabled in the repository workflow with `AppxPackageSigningEnabled=false`. Microsoft Store signing occurs after upload through the Store pipeline.

Public CI and release workflows do not require `MSIX_CERTIFICATE_BASE64` or `MSIX_CERTIFICATE_PASSWORD`.

### 9.3 GitHub Release Zip

The GitHub release zip workflow builds the desktop app and compresses `CreviceApp/bin/Release/*`.

The zip artifact filename is `Crevice-<version>-win32-unsigned.zip`. The workflow writes `SHA256SUMS.txt` beside the zip.

The zip artifact is unsigned. Direct-download users receive the desktop application files as a zip archive.

### 9.4 NuGet Package Assets

CI packs `Crevice.Core` package assets from `CreviceLib/CreviceLib.csproj.nuspec` into `artifacts/nuget`.

### 9.5 Artifact Separation

Store upload artifacts and GitHub release zip artifacts remain separate. Store package generation does not publish a direct-download zip. Release zip generation does not produce a Store upload package.
