#!/bin/bash
# Build Docker image for Invictus Documentation MCP Server
# Run this from the REPO ROOT (docs-ifa/), not from src-mcp/docs-ifa-mcp/

echo "?? Building Invictus Documentation MCP Server Docker Image"
echo ""

# Check if we're in the right directory
if [ ! -d "versioned_docs" ]; then
    echo "? Error: versioned_docs directory not found"
    echo "Please run this script from the repository root (docs-ifa/)"
    echo "Current directory: $(pwd)"
    exit 1
fi

if [ ! -f "src-mcp/docs-ifa-mcp/docs-ifa-mcp.csproj" ]; then
    echo "? Error: MCP project not found"
    echo "Please ensure src-mcp/docs-ifa-mcp/ exists"
    exit 1
fi

# Count documentation files
DOC_COUNT=$(find versioned_docs/version-v6.0.0 -name "*.md" -o -name "*.mdx" 2>/dev/null | wc -l)
echo "? Found $DOC_COUNT documentation files to include"
echo ""

# Build the Docker image
echo "?? Building Docker image..."
docker build -t invictus-docs-mcp:latest -f src-mcp/docs-ifa-mcp/Dockerfile .

if [ $? -eq 0 ]; then
    echo ""
    echo "? Docker image built successfully!"
    echo ""
    echo "?? Next steps:"
    echo ""
    echo "1. Test locally:"
    echo "   docker run -i --rm invictus-docs-mcp:latest"
    echo ""
    echo "2. Tag for Azure Container Registry:"
    echo "   docker tag invictus-docs-mcp:latest <your-acr>.azurecr.io/invictus-docs-mcp:latest"
    echo ""
    echo "3. Push to ACR:"
    echo "   az acr login --name <your-acr>"
    echo "   docker push <your-acr>.azurecr.io/invictus-docs-mcp:latest"
    echo ""
else
    echo ""
    echo "? Docker build failed"
    exit 1
fi
