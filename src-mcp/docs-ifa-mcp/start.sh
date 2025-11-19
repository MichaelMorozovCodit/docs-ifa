#!/bin/bash

# Quick start script for Invictus Documentation MCP Server

echo "?? Starting Invictus Documentation MCP Server..."
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "? Error: .NET SDK not found"
    echo "Please install .NET 10.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "? Found .NET SDK version: $DOTNET_VERSION"

# Check if sample docs exist
if [ ! -d "sample-docs" ]; then
    echo "??  Warning: sample-docs directory not found"
    echo "Creating sample-docs directory..."
    mkdir -p sample-docs
fi

DOC_COUNT=$(find sample-docs -name "*.md" -o -name "*.mdx" 2>/dev/null | wc -l)
echo "? Found $DOC_COUNT documentation files"

# Build the project
echo ""
echo "?? Building project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "? Build failed"
    exit 1
fi

echo ""
echo "? Build successful!"
echo ""
echo "?? Starting MCP server..."
echo "   Logs will be displayed below."
echo "   Press Ctrl+C to stop the server."
echo ""
echo "??????????????????????????????????????????????????????????????????"
echo ""

# Run the server
dotnet run
