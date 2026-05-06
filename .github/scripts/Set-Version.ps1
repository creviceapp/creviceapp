param(
  [Parameter(Mandatory = $true)]
  [string] $Version
)

$ErrorActionPreference = 'Stop'

$normalized = $Version.Trim()
if ($normalized.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
  $normalized = $normalized.Substring(1)
}

if ($normalized -match '^\d+\.\d+\.\d+$') {
  $normalized = "$normalized.0"
}

if ($normalized -notmatch '^\d+\.\d+\.\d+\.\d+$') {
  throw "Version must be numeric and have three or four parts. Got '$Version'."
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$utf8NoBom = [System.Text.UTF8Encoding]::new($false)
$utf8Bom = [System.Text.UTF8Encoding]::new($true)

$assemblyInfo = Join-Path $repoRoot 'CreviceApp\Properties\AssemblyInfo.cs'
$assemblyText = [System.IO.File]::ReadAllText($assemblyInfo)
$assemblyText = [regex]::Replace($assemblyText, '(?m)^\[assembly:\s*AssemblyVersion\("[^"]+"\)\]', "[assembly: AssemblyVersion(`"$normalized`")]")
$assemblyText = [regex]::Replace($assemblyText, '(?m)^\[assembly:\s*AssemblyFileVersion\("[^"]+"\)\]', "[assembly: AssemblyFileVersion(`"$normalized`")]")
[System.IO.File]::WriteAllText($assemblyInfo, $assemblyText, $utf8Bom)

$manifest = Join-Path $repoRoot 'CreviceAppPackage\Package.appxmanifest'
if (Test-Path -LiteralPath $manifest) {
  $manifestText = [System.IO.File]::ReadAllText($manifest)
  $manifestText = [regex]::Replace(
    $manifestText,
    '(<Identity\b[^>]*\bVersion=")[^"]+(")',
    { $args[0].Groups[1].Value + $normalized + $args[0].Groups[2].Value }
  )
  [System.IO.File]::WriteAllText($manifest, $manifestText, $utf8NoBom)
}

if ($env:GITHUB_ENV) {
  "CREVICE_VERSION=$normalized" | Out-File -FilePath $env:GITHUB_ENV -Append -Encoding utf8
}

Write-Host "Crevice version set to $normalized"
