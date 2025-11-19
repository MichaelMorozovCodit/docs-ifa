# Invictus Documentation MCP Server

A Model Context Protocol (MCP) server that provides AI assistants with access to Invictus for Azure documentation through semantic search and structured queries.

## ?? What is this?

This MCP server allows AI assistants (like Claude, ChatGPT with MCP support, or GitHub Copilot) to:
- **Search** Invictus documentation using natural language
- **Retrieve** installation guides and setup instructions
- **Get** detailed component information (Transco, PubSub, etc.)
- **Access** migration guides and troubleshooting help

## ?? Prerequisites

- .NET 10.0 SDK or later
- An MCP-compatible client (Claude Desktop, Cline, etc.)
- Documentation files (Markdown/MDX format)

## ?? Quick Start

### 1. Clone and Build

```bash
git clone <your-repo-url>
cd docs-ifa-mcp
dotnet build
```

### 2. Add Sample Documentation

Sample documentation is provided in the `sample-docs/` folder. To use your own:

1. Place your `.md` or `.mdx` files in `sample-docs/`
2. Or update `appsettings.json` to point to your documentation directory

### 3. Configure MCP Client

Add this to your MCP client configuration (e.g., Claude Desktop's `claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "invictus-docs": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/docs-ifa-mcp/docs-ifa-mcp.csproj"
      ]
    }
  }
}
```

**Windows path example:**
```json
"args": [
  "run",
  "--project",
  "C:\\Users\\YourName\\source\\repos\\docs-ifa-mcp\\docs-ifa-mcp.csproj"
]
```

### 4. Restart Your MCP Client

Restart Claude Desktop (or your MCP client) to load the server.

## ?? Configuration

### Documentation Path

Edit `appsettings.json` to configure where your documentation is located:

```json
{
  "Documentation": {
    "BasePath": "../../versioned_docs/version-v6.0.0",
    "FallbackPath": "/app/docs"
  }
}
```

The server will search for documentation in this order:
1. `Documentation:BasePath` (relative to app directory)
2. `Documentation:FallbackPath` (absolute path)
3. `./sample-docs/` (default sample location)
4. `./docs/`
5. `../../docs/`

### Logging

Configure logging levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "ModelContextProtocol": "Information"
    }
  }
}
```

## ??? Available Tools

The server exposes these tools to AI assistants:

### 1. `search_documentation`
Search all documentation using semantic search.

**Parameters:**
- `query` (string, required): Search query
- `maxResults` (integer, optional): Max results (1-10, default: 5)

**Example:**
```
Use the search_documentation tool to find information about installing Invictus Framework
```

### 2. `get_installation_guide`
Get step-by-step installation instructions.

**Parameters:**
- `component` (string): `dashboard`, `framework`, or `both`
- `step` (string, optional): `prerequisites`, `build`, `release`, or `all`

**Example:**
```
Use get_installation_guide for the dashboard component
```

### 3. `get_component_info`
Get detailed component information with examples.

**Parameters:**
- `component` (string, required): Component name (e.g., "Transco", "PubSub")
- `includeExamples` (boolean, optional): Include code examples (default: true)

**Example:**
```
Use get_component_info for the Transco component
```

### 4. `get_migration_guide`
Get migration/upgrade instructions.

**Parameters:**
- `fromComponent` (string, required): Source component/version
- `toComponent` (string, optional): Target component/version

**Example:**
```
Use get_migration_guide to migrate from Matrix v1 to v2
```

### 5. `list_topics`
List all available documentation topics by category.

**Example:**
```
Use list_topics to see what documentation is available
```

## ?? Available Prompts

Pre-configured prompts for common scenarios:

### 1. `installation_help`
Get help with Invictus installation.

**Parameters:**
- `component` (string): `dashboard` or `framework`

### 2. `troubleshooting_help`
Get troubleshooting assistance.

**Parameters:**
- `component` (string): Component having issues
- `problem` (string): Problem description

### 3. `component_config_help`
Get component configuration help.

**Parameters:**
- `component` (string): Component name

## ?? Docker Deployment

### Build Image

```bash
docker build -t invictus-docs-mcp .
```

### Run Container

```bash
docker run -d \
  -v /path/to/your/docs:/app/docs \
  --name invictus-mcp \
  invictus-docs-mcp
```

### Use with MCP Client

Update your MCP configuration:

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
        "/path/to/docs:/app/docs",
        "invictus-docs-mcp"
      ]
    }
  }
}
```

## ?? Adding Your Documentation

### Supported Formats

- Markdown (`.md`)
- MDX (`.mdx`)

### Documentation Structure

The server extracts:
- **Title**: From YAML frontmatter or first H1
- **Description**: First paragraph after title
- **Category**: Based on directory structure
- **Keywords**: Automatically extracted technical terms

### Example Document

```markdown
---
title: My Component Guide
---

# My Component

This component provides amazing functionality.

## Configuration

Configure the component like this...

## Examples

Here are some examples...
```

### Best Practices

1. Use clear, descriptive titles
2. Include descriptions at the start of documents
3. Use consistent heading structure
4. Add code examples where relevant
5. Organize files by topic/category

## ?? Testing

### Test the Server

```bash
dotnet run
```

The server should start and log:
```
? Successfully indexed X documentation pages
```

### Test with MCP Client

1. Open your MCP client (e.g., Claude Desktop)
2. Look for the Invictus Documentation tools in the tools panel
3. Try a query like: "Search for Transco component documentation"

### Test Search Functionality

Ask your AI assistant:
- "How do I install Invictus Framework?"
- "Show me how to configure the Transco component"
- "What components are available in Invictus?"

## ?? Troubleshooting

### Server Doesn't Start

**Check:** .NET version
```bash
dotnet --version  # Should be 10.0 or later
```

**Check:** Build errors
```bash
dotnet build
```

### No Documentation Indexed

**Check logs** for documentation path errors:
```
? No documentation directory found. Server will start with empty index.
```

**Solution:** Verify your documentation path in `appsettings.json`

### Tools Not Appearing in MCP Client

1. Check MCP client configuration file path
2. Verify the project path is absolute
3. Restart the MCP client
4. Check client logs for connection errors

### Search Returns No Results

1. Verify documentation is being indexed (check logs)
2. Try broader search terms
3. Check that your files are `.md` or `.mdx` format

## ?? Project Structure

```
docs-ifa-mcp/
??? Controllers/               (removed - using stdio MCP)
??? Models/
?   ??? DocumentationModels.cs   # Data models
??? Services/
?   ??? DocumentationService.cs  # Indexing service
?   ??? QueryService.cs          # Search service
?   ??? InvictusDocsMcpServer.cs # MCP tools & prompts
?   ??? DocumentationInitializationService.cs
??? sample-docs/                 # Sample documentation
?   ??? getting-started.md
?   ??? installation.md
?   ??? transco-component.md
?   ??? ...
??? Program.cs                   # App entry point
??? appsettings.json            # Configuration
??? Dockerfile                   # Container build
??? README.md                    # This file
```

## ?? Contributing

To add new tools or improve search:

1. Edit `Services/InvictusDocsMcpServer.cs`
2. Add `[McpServerTool]` attribute to methods
3. Update this README
4. Rebuild and test

## ?? License

[Your License Here]

## ?? Support

For issues or questions:
- Check the [Troubleshooting](#-troubleshooting) section
- Review MCP client documentation
- Check application logs

## ?? Resources

- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [Invictus for Azure Documentation](https://docs.invictus-integration.com/)
- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/)

---

**Made with ?? for the Invictus community**
