param(
  [string] $InstallerUri = 'https://go.microsoft.com/fwlink/p/?LinkId=838916'
)

$ErrorActionPreference = 'Stop'

$contractPath = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\References\Windows.ApplicationModel.StartupTaskContract\1.0.0.0\Windows.ApplicationModel.StartupTaskContract.winmd'
if (Test-Path -LiteralPath $contractPath) {
  Write-Host "Legacy Windows SDK contract already exists at $contractPath"
  exit 0
}

$installerPath = Join-Path $env:RUNNER_TEMP 'winsdksetup-14393.exe'
if ([string]::IsNullOrWhiteSpace($env:RUNNER_TEMP)) {
  $installerPath = Join-Path $env:TEMP 'winsdksetup-14393.exe'
}

Write-Host "Downloading legacy Windows SDK installer from $InstallerUri"
Invoke-WebRequest -Uri $InstallerUri -OutFile $installerPath

Write-Host 'Installing legacy Windows SDK contracts'
$process = Start-Process -FilePath $installerPath -ArgumentList '/quiet', '/norestart' -Wait -PassThru
if ($process.ExitCode -ne 0 -and $process.ExitCode -ne 3010) {
  throw "Windows SDK installer failed with exit code $($process.ExitCode)."
}

if (-not (Test-Path -LiteralPath $contractPath)) {
  throw "Windows.ApplicationModel.StartupTaskContract was not found after Windows SDK installation at $contractPath."
}

Write-Host "Legacy Windows SDK contract installed at $contractPath"
