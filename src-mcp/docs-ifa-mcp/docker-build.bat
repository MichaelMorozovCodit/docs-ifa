@echo off
REM Build Docker image for Invictus Documentation MCP Server
REM Run this from the REPO ROOT (docs-ifa\), not from src-mcp\docs-ifa-mcp\

echo ================================
echo Building Invictus Documentation MCP Server
echo ================================
echo.

REM Check if we're in the right directory
if not exist "versioned_docs" (
    echo [ERROR] versioned_docs directory not found
    echo Please run this script from the repository root ^(docs-ifa\^)
    echo Current directory: %CD%
    pause
    exit /b 1
)

if not exist "src-mcp\docs-ifa-mcp\docs-ifa-mcp.csproj" (
    echo [ERROR] MCP project not found
    echo Please ensure src-mcp\docs-ifa-mcp\ exists
    pause
    exit /b 1
)

REM Count documentation files
set DOC_COUNT=0
for /r versioned_docs\version-v6.0.0 %%f in (*.md *.mdx) do set /a DOC_COUNT+=1
echo [OK] Found %DOC_COUNT% documentation files to include
echo.

REM Build the Docker image
echo Building Docker image...
docker build -t invictus-docs-mcp:latest -f src-mcp\docs-ifa-mcp\Dockerfile .

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ================================
    echo Docker image built successfully!
    echo ================================
    echo.
    echo Next steps:
    echo.
    echo 1. Test locally:
    echo    docker run -i --rm invictus-docs-mcp:latest
    echo.
    echo 2. Tag for Azure Container Registry:
    echo    docker tag invictus-docs-mcp:latest ^<your-acr^>.azurecr.io/invictus-docs-mcp:latest
    echo.
    echo 3. Push to ACR:
    echo    az acr login --name ^<your-acr^>
    echo    docker push ^<your-acr^>.azurecr.io/invictus-docs-mcp:latest
    echo.
) else (
    echo.
    echo [ERROR] Docker build failed
    pause
    exit /b 1
)

pause
