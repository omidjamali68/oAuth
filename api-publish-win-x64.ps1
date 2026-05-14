[CmdletBinding()]
param(
  [string]$Configuration = "Release",
  [string]$Runtime = "win-x64",
  [string]$OutputRoot = (Join-Path $PSScriptRoot "artifacts/publish"),
  [switch]$SelfContained,
  [switch]$Clean,
  [switch]$NoRestore
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$projectPath = Join-Path $PSScriptRoot "Auth.API/Auth.API.csproj"
$outputPath = Join-Path $OutputRoot (Join-Path "api" $Runtime)

if (-not (Test-Path $projectPath)) {
  throw "Project not found: $projectPath"
}

if ($Clean -and (Test-Path $outputPath)) {
  Remove-Item -LiteralPath $outputPath -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

$selfContainedValue = if ($SelfContained) { "true" } else { "false" }
$publishArgs = @(
  "publish",
  $projectPath,
  "-c", $Configuration,
  "-r", $Runtime,
  "-o", $outputPath,
  "--self-contained", $selfContainedValue
)

if ($NoRestore) {
  $publishArgs += "--no-restore"
}

Push-Location $PSScriptRoot
try {
  dotnet @publishArgs
} finally {
  Pop-Location
}

Write-Host "Published API to: $outputPath"
