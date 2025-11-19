# ?? POC Quick Reference

## One-Page Overview

### What is This?
An MCP server that lets AI assistants query Invictus documentation using natural language.

### Quick Start (3 Steps)

```bash
# 1. Build & Run
dotnet run

# 2. Configure Claude Desktop
# Edit: %APPDATA%\Claude\claude_desktop_config.json
{
  "mcpServers": {
    "invictus-docs": {
      "command": "dotnet",
      "args": ["run", "--project", "C:\\full\\path\\to\\docs-ifa-mcp.csproj"]
    }
  }
}

# 3. Test in Claude
"List all documentation topics"
```

### Available Tools

| Tool | Purpose | Example |
|------|---------|---------|
| `search_documentation` | General search | "Search for PubSub configuration" |
| `get_installation_guide` | Install help | "How to install Framework" |
| `get_component_info` | Component details | "Tell me about Transco" |
| `get_migration_guide` | Upgrade paths | "Migrate from Matrix v1" |
| `list_topics` | Browse all docs | "What docs are available?" |

### File Structure

```
docs-ifa-mcp/
??? ?? README.md                    # Main documentation
??? ?? POC_SETUP_GUIDE.md           # Step-by-step setup
??? ?? MCP_CLIENT_CONFIG.md         # Client configs
??? ?? PROJECT_SUMMARY.md           # Technical overview
??? ?? CHANGELOG.md                 # What changed
??? ?? start.bat / start.sh         # Quick start scripts
??? ?? appsettings.json             # Configuration
??? ?? Dockerfile                   # Container build
??? ?? sample-docs/                 # Example docs (5 files)
??? ?? Services/
?   ??? InvictusDocsMcpServer.cs   # MCP tools & prompts
?   ??? DocumentationService.cs    # Doc indexing
?   ??? QueryService.cs            # Search logic
??? ?? Models/
    ??? DocumentationModels.cs     # Data models
```

### Test Queries

Copy-paste these into Claude:

```
1. "Can you list all documentation topics?"
2. "How do I install Invictus Framework?"
3. "Tell me about the Transco component with examples"
4. "Search for PubSub configuration parameters"
5. "Show me installation prerequisites"
```

### Troubleshooting (Quick)

| Problem | Solution |
|---------|----------|
| Server won't start | Check .NET 10 installed: `dotnet --version` |
| No docs indexed | Verify `sample-docs/` has .md files |
| Tools not in Claude | Use absolute path in config, restart Claude |
| Search no results | Run `dotnet run` manually to see errors |

### Configuration (Quick)

**appsettings.json:**
```json
{
  "Documentation": {
    "BasePath": "../../your-docs",
    "FallbackPath": "/app/docs"
  }
}
```

### Key Features

? Semantic search across documentation  
? 5 specialized tools for different queries  
? 3 pre-built prompts for common tasks  
? Automatic doc indexing from .md/.mdx  
? Configurable paths with fallbacks  
? Sample docs included for testing  
? Works with Claude Desktop, Cline, etc.  

### Success Criteria

- [ ] `dotnet build` succeeds
- [ ] `dotnet run` indexes docs
- [ ] Tools appear in Claude
- [ ] Search returns results
- [ ] AI can answer from docs

### Next Steps

1. **Start:** Follow `POC_SETUP_GUIDE.md`
2. **Customize:** Add your docs to `sample-docs/`
3. **Deploy:** Use `Dockerfile` for production
4. **Extend:** Add tools in `InvictusDocsMcpServer.cs`

### Resources

- ?? Full Guide: `POC_SETUP_GUIDE.md`
- ?? Config Examples: `MCP_CLIENT_CONFIG.md`
- ?? Tech Details: `PROJECT_SUMMARY.md`
- ?? Changes: `CHANGELOG.md`

---

**Ready?** Run: `start.bat` (Windows) or `./start.sh` (Linux)
