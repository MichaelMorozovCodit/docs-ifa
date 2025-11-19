# POC Setup Guide - Invictus Documentation MCP Server

This guide will help you set up and test the Invictus Documentation MCP Server as a Proof of Concept.

## ? Prerequisites Checklist

- [ ] .NET 10.0 SDK installed ([Download](https://dotnet.microsoft.com/download))
- [ ] MCP-compatible client installed (Claude Desktop, Cline, etc.)
- [ ] Git (optional, for cloning)
- [ ] Text editor (VS Code, Visual Studio, etc.)

## ?? POC Objectives

By the end of this setup, you will:
1. Have a working MCP server running
2. Be able to query documentation from an AI assistant
3. Understand how to add your own documentation

## ?? Step 1: Project Setup

### Option A: Using existing project

```bash
cd C:\Users\YourName\source\repos\docs-ifa-mcp-server\docs-ifa\src-mcp\docs-ifa-mcp
```

### Option B: Clone fresh copy

```bash
git clone <repo-url>
cd docs-ifa-mcp
```

## ?? Step 2: Build the Project

### Windows

```cmd
start.bat
```

Or manually:
```cmd
dotnet build
dotnet run
```

### Linux/macOS

```bash
chmod +x start.sh
./start.sh
```

Or manually:
```bash
dotnet build
dotnet run
```

### Expected Output

You should see:
```
info: docs_ifa_mcp.Services.DocumentationIndexService[0]
      Starting documentation indexing...
info: docs_ifa_mcp.Services.DocumentationIndexService[0]
      Found 5 markdown files to index
info: docs_ifa_mcp.Services.DocumentationIndexService[0]
      ? Successfully indexed 5 documentation pages
```

**If successful, press Ctrl+C to stop** and continue to the next step.

## ?? Step 3: Configure MCP Client

### For Claude Desktop

1. **Locate the config file:**
   - Windows: `%APPDATA%\Claude\claude_desktop_config.json`
   - macOS: `~/Library/Application Support/Claude/claude_desktop_config.json`

2. **Edit the file** (create if it doesn't exist):

```json
{
  "mcpServers": {
    "invictus-docs": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\YourName\\source\\repos\\docs-ifa-mcp-server\\docs-ifa\\src-mcp\\docs-ifa-mcp\\docs-ifa-mcp.csproj"
      ]
    }
  }
}
```

?? **Important:** Use the **absolute path** to your `docs-ifa-mcp.csproj` file.

3. **Save the file**

4. **Completely restart Claude Desktop** (close and reopen)

### For Cline (VS Code Extension)

1. Open your workspace in VS Code
2. Create/edit `.vscode/settings.json`:

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

3. Reload VS Code window

## ? Step 4: Test the Integration

### Verify Server Connection

In Claude Desktop:
1. Look for a ?? icon or "Tools" panel
2. You should see tools like:
   - `search_documentation`
   - `get_installation_guide`
   - `get_component_info`
   - `list_topics`

### Test Queries

Try these queries with Claude:

1. **List available topics:**
   ```
   Can you list all available documentation topics?
   ```

2. **Search for installation:**
   ```
   How do I install Invictus Framework?
   ```

3. **Get component info:**
   ```
   Tell me about the Transco component
   ```

4. **Search general docs:**
   ```
   Search the documentation for PubSub configuration
   ```

### Expected Behavior

Claude should:
- Use the MCP tools automatically
- Return information from your documentation
- Format the response nicely

## ?? Step 5: Verify Everything Works

### Checklist

- [ ] Server builds without errors
- [ ] Server starts and indexes documentation
- [ ] MCP client shows Invictus tools
- [ ] Claude can use the tools to query docs
- [ ] Search returns relevant results

### Sample Session

**You:** "List all documentation topics"

**Claude:** *Uses `list_topics` tool*
```
Here are the available documentation topics:

## Installation
- Getting Started with Invictus
- Installing Invictus Framework
- Installing Invictus Dashboard

## Components
- Transco Component
- PubSub Component
...
```

## ?? Step 6: Customize for Your Needs

### Add Your Own Documentation

1. Place your `.md` or `.mdx` files in `sample-docs/`
2. Restart the MCP server
3. The new documents will be automatically indexed

### Configure Documentation Path

Edit `appsettings.json`:

```json
{
  "Documentation": {
    "BasePath": "path/to/your/docs",
    "FallbackPath": "/backup/path"
  }
}
```

### Adjust Logging

For more detailed logs:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "ModelContextProtocol": "Debug"
    }
  }
}
```

## ?? Troubleshooting

### Server doesn't start

**Problem:** `error: Unable to find project`
**Solution:** Check that the path in MCP config is absolute and correct

**Problem:** `error: No documentation directory found`
**Solution:** Verify `sample-docs` folder exists with `.md` files

### Tools don't appear in Claude

**Problem:** No tools showing
**Solution:** 
1. Completely close and reopen Claude Desktop
2. Check Claude logs in `%APPDATA%\Claude\logs`
3. Verify MCP config file is valid JSON

### Search returns no results

**Problem:** "No relevant documentation found"
**Solution:**
1. Check that documentation was indexed (see server logs)
2. Try broader search terms
3. Verify your `.md` files are in the `sample-docs` folder

### Server crashes on startup

**Problem:** Server exits immediately
**Solution:**
1. Run manually with `dotnet run` to see error messages
2. Check that all dependencies are installed
3. Verify .NET 10.0 SDK is installed

## ?? Next Steps

### Enhance the POC

1. **Add more documentation:** 
   - Copy your Invictus docs to `sample-docs/`
   
2. **Improve search:**
   - Add more keywords to documents
   - Use clearer titles and descriptions

3. **Add custom tools:**
   - Edit `Services/InvictusDocsMcpServer.cs`
   - Add new `[McpServerTool]` methods

4. **Deploy to production:**
   - Build Docker image
   - Deploy to Azure Container Apps
   - Configure with real documentation

### Share with Team

1. Document your setup in team wiki
2. Create video walkthrough
3. Share successful queries and examples
4. Gather feedback on usefulness

## ?? Getting Help

### Common Resources

- **MCP Documentation:** https://modelcontextprotocol.io/
- **Claude Desktop:** https://claude.ai/desktop
- **Project README:** See `README.md` in this folder

### Debug Mode

Run with verbose logging:

```bash
dotnet run --configuration Debug
```

Check logs for:
- Documentation indexing status
- Search query processing
- Tool invocation details

## ? POC Success Criteria

Your POC is successful if:

1. ? Server runs without errors
2. ? Documentation is indexed (5+ documents)
3. ? MCP client connects successfully
4. ? Tools appear in client UI
5. ? Search returns relevant results
6. ? AI assistant can answer questions from docs

## ?? POC Demo Script

When demonstrating to stakeholders:

1. **Show the setup** (1 minute)
   - Open terminal, run `dotnet run`
   - Show documentation being indexed

2. **Show the integration** (2 minutes)
   - Open Claude Desktop
   - Show tools panel with Invictus tools

3. **Demonstrate queries** (5 minutes)
   - List topics
   - Search for installation guide
   - Get component info
   - Show AI assistant using tools

4. **Discuss benefits** (2 minutes)
   - Instant documentation access
   - Natural language queries
   - Always up-to-date information
   - Integration with AI workflows

---

**Ready to go?** Start with Step 1 and work through each section. Good luck! ??
