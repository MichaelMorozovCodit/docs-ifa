@echo off
REM Quick start script for Invictus Documentation MCP Server (Windows)

echo ================================
echo Invictus Documentation MCP Server
echo ================================
echo.

REM Check if .NET is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found
    echo Please install .NET 10.0 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Check .NET version
for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [OK] Found .NET SDK version: %DOTNET_VERSION%

REM Check if sample docs exist
if not exist "sample-docs" (
    echo [WARNING] sample-docs directory not found
    echo Creating sample-docs directory...
    mkdir sample-docs
)

REM Count documentation files
set DOC_COUNT=0
for /r sample-docs %%f in (*.md *.mdx) do set /a DOC_COUNT+=1
echo [OK] Found %DOC_COUNT% documentation files

echo.
echo Building project...
dotnet build

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed
    pause
    exit /b 1
)

echo.
echo ================================
echo Build successful!
echo ================================
echo.
echo Starting MCP server...
echo Logs will be displayed below.
echo Press Ctrl+C to stop the server.
echo.
echo --------------------------------
echo.

REM Run the server
dotnet run
