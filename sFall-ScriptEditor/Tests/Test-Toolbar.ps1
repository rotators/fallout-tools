# Run against an isolated copy so editor settings and generated syntax files stay disposable.
$ErrorActionPreference = 'Stop'
$runtime = Join-Path $PSScriptRoot '../ScriptEditor/bin/Release'
$testDirectory = Join-Path ([IO.Path]::GetTempPath()) ('sse-toolbar-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testDirectory | Out-Null
Copy-Item -Path (Join-Path $runtime '*') -Destination $testDirectory -Recurse
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
& $compiler /nologo /target:exe /platform:x86 "/out:$testDirectory/ToolbarRegression.exe" "/reference:$testDirectory/SfallScriptEditor.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Core.dll (Join-Path $PSScriptRoot 'ToolbarRegression.cs')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Push-Location -LiteralPath $testDirectory
try {
    & ./ToolbarRegression.exe
    $testExitCode = $LASTEXITCODE
    Write-Output "Toolbar renderings: $testDirectory"
} finally {
    Pop-Location
}
exit $testExitCode