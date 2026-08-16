@echo off
setlocal
set "ROOT=%~dp0"
set "TEST_PROJECT=%ROOT%Tests\SfallScriptEditor.Tests.csproj"
set "TEST_EXE=%ROOT%Tests\bin\Release\SfallScriptEditor.Tests.exe"
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set "MSBUILD="
set "NET40_MSBUILD=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

if exist "%VSWHERE%" (
    for /f "usebackq delims=" %%I in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%I"
)
if not defined MSBUILD (
    for /f "delims=" %%I in ('where msbuild 2^>nul') do if not defined MSBUILD set "MSBUILD=%%I"
)
if not defined MSBUILD (
    echo ERROR: MSBuild was not found.
    exit /b 1
)

if not exist "%NET40_MSBUILD%" (
    echo ERROR: .NET Framework 4.0 MSBuild was not found:
    echo %NET40_MSBUILD%
    exit /b 1
)

"%NET40_MSBUILD%" "%ROOT%SfallScriptEditor.sln" /t:Build /p:Configuration=Release /p:Platform="Any CPU" /p:TargetFrameworkVersion=v4.0 /p:TargetFrameworkProfile= /nologo /verbosity:minimal
if errorlevel 1 exit /b 1

"%NET40_MSBUILD%" "%TEST_PROJECT%" /t:Build /p:Configuration=Release /p:Platform=x86 /p:TargetFrameworkVersion=v4.0 /p:TargetFrameworkProfile= /nologo /verbosity:minimal
if errorlevel 1 exit /b 1

"%TEST_EXE%"
exit /b %errorlevel%