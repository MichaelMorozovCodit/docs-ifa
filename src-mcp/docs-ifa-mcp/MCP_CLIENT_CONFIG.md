# MCP Client Configuration Examples

This file provides example configurations for connecting to the Invictus Documentation MCP Server from various MCP clients.

## Claude Desktop

Edit `%APPDATA%\Claude\claude_desktop_config.json` (Windows) or `~/Library/Application Support/Claude/claude_desktop_config.json` (macOS):

```json
{
  "mcpServers": {
    "invictus-docs": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\YourName\\source\\repos\\docs-ifa-mcp\\docs-ifa-mcp.csproj"
      ]
    }
  }
}
```

## Cline (VS Code Extension)

Add to `.vscode/settings.json`:

```json
{
  "cline.mcpServers": {
    "invictus-docs": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/docs-ifa-mcp.csproj"
      ]
    }
  }
}
```

## Using Docker

If you've built the Docker image:

```json
{
  "mcpServers": {
    "invictus-docs": {
      "command": "docker",
      "args": [
        "run",
        "-i",
        "--rm",
        "-v",
        "C:\\path\\to\\your\\docs:/app/docs",
        "invictus-docs-mcp"
      ]
    }
  }
}
```

## Using Published Binary

After publishing (`dotnet publish -c Release`):

```json
{
  "mcpServers": {
    "invictus-docs": {
      "command": "C:\\path\\to\\published\\docs-ifa-mcp.exe",
      "args": []
    }
  }
}
```

## Environment Variables (Optional)

You can pass environment variables to configure the server:

```json
{
  "mcpServers": {
    "invictus-docs": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\YourName\\source\\repos\\docs-ifa-mcp\\docs-ifa-mcp.csproj"
      ],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Production",
        "Documentation__BasePath": "C:\\MyDocs\\invictus"
      }
    }
  }
}
```

## Verification

After adding the configuration:

1. **Restart your MCP client** (completely close and reopen)
2. **Check for the server** in the tools/resources panel
3. **Look for these tools:**
   - search_documentation
   - get_installation_guide
   - get_component_info
   - get_migration_guide
   - list_topics

## Troubleshooting Connection Issues

### Check Server Logs

The server logs to stderr. In Claude Desktop, check:
- Windows: `%APPDATA%\Claude\logs`
- macOS: `~/Library/Logs/Claude`

### Test Manually

Run the server directly to see if it starts:

```bash
cd C:\path\to\docs-ifa-mcp
dotnet run
```

You should see:
```
info: docs_ifa_mcp.Services.DocumentationIndexService[0]
      ? Successfully indexed X documentation pages
```

### Common Issues

1. **"Command not found"**: Ensure `dotnet` is in your PATH
2. **"Project not found"**: Use absolute paths in configuration
3. **"No tools available"**: Restart the MCP client after config changes

## Testing the Connection

Once connected, try asking your AI assistant:

```
Can you list the available documentation topics?
```

or

```
Search for information about installing Invictus Framework
```

The AI should respond using the MCP tools to query your documentation.
