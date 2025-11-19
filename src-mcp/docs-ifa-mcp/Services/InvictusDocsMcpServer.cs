using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;
using System.Text.Json;
using System.ComponentModel;

namespace docs_ifa_mcp.Services;

/// <summary>
/// MCP Server for Invictus documentation using the official ModelContextProtocol package
/// Provides tools, prompts, and resources for querying Invictus for Azure documentation
/// </summary>
[McpServerToolType]
[McpServerPromptType]
public class InvictusDocsMcpServer(
    DocumentationIndexService indexService,
    QueryService queryService,
    ILogger<InvictusDocsMcpServer> logger)
{
    private readonly DocumentationIndexService _indexService = indexService;
    private readonly QueryService _queryService = queryService;
    private readonly ILogger<InvictusDocsMcpServer> _logger = logger;

    /// <summary>
    /// Search Invictus documentation using semantic search
    /// </summary>
    [McpServerTool(Name = "search_documentation")]
    [Description("Search Invictus documentation using semantic search. Returns relevant documentation sections based on your query.")]
    public async Task<string> SearchDocumentation(
        [Description("The search query for Invictus documentation")] string query,
        [Description("Maximum number of results to return (1-10)")] int maxResults = 5,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching documentation: {Query}, MaxResults: {MaxResults}", query, maxResults);

        // Clamp maxResults to valid range
        maxResults = Math.Clamp(maxResults, 1, 10);

        var results = await _queryService.SearchAsync(query, maxResults);

        if (!results.Any())
        {
            return "No relevant documentation found for your query. Try different keywords.";
        }

        return string.Join("\n\n---\n\n", results.Select(r =>
            $"## {r.Title}\n\n**Category:** {r.Category} | **Relevance:** {r.Score:P0}\n\n{r.Description}\n\n{r.Content}"));
    }

    /// <summary>
    /// Get installation instructions for Invictus components
    /// </summary>
    [McpServerTool(Name = "get_installation_guide")]
    [Description("Get step-by-step installation instructions for Invictus Dashboard or Framework.")]
    public async Task<string> GetInstallationGuide(
        [Description("Component: 'dashboard', 'framework', or 'both'")] string component = "framework",
        [Description("Specific step: 'prerequisites', 'build', 'release', or 'all'")] string? step = null,
        CancellationToken cancellationToken = default)
    {
        var searchQuery = component.ToLower() switch
        {
            "dashboard" => $"Installing Invictus Dashboard {step ?? "all"}",
            "framework" => $"Installing Invictus Framework {step ?? "all"}",
            "both" => "Installing Invictus Dashboard Framework",
            _ => "Installing Invictus"
        };

        var results = await _queryService.SearchAsync(searchQuery, 5);
        
        if (!results.Any())
            return $"No installation guide found for {component}.";

        return $"# Installation Guide: {component.ToUpper()}\n\n" +
               string.Join("\n\n", results.Select(r => $"## {r.Title}\n\n{r.Content}"));
    }

    /// <summary>
    /// Get component information with examples
    /// </summary>
    [McpServerTool(Name = "get_component_info")]
    [Description("Get detailed information about Invictus Framework components (Transco, PubSub, etc).")]
    public async Task<string> GetComponentInfo(
        [Description("Component name: 'Transco', 'PubSub', 'XML/JSON Converter', etc.")] string component,
        [Description("Include usage examples")] bool includeExamples = true,
        CancellationToken cancellationToken = default)
    {
        var searchTerms = includeExamples
            ? $"{component} endpoint parameters configuration example request response"
            : $"{component} endpoint parameters configuration";

        var results = await _queryService.SearchAsync(searchTerms, 3);

        if (!results.Any())
            return $"No information found for component: {component}.";

        return $"# {component} Component\n\n" +
               string.Join("\n\n", results.Select(r => $"## {r.Title}\n\n{r.Content}"));
    }

    /// <summary>
    /// Get migration instructions
    /// </summary>
    [McpServerTool(Name = "get_migration_guide")]
    [Description("Get migration instructions for upgrading Invictus components.")]
    public async Task<string> GetMigrationGuide(
        [Description("Component to migrate from (e.g., 'Matrix v1', 'Transco v1')")] string fromComponent,
        [Description("Component to migrate to")] string? toComponent = null,
        CancellationToken cancellationToken = default)
    {
        var searchQuery = $"migrating {fromComponent} {toComponent ?? "v2"} upgrade deprecated";
        var results = await _queryService.SearchAsync(searchQuery, 3);

        if (!results.Any())
            return $"No migration guide found for {fromComponent}.";

        return $"# Migration: {fromComponent} → {toComponent ?? "latest"}\n\n" +
               string.Join("\n\n", results.Select(r => $"## {r.Title}\n\n{r.Content}"));
    }

    /// <summary>
    /// List all available documentation topics
    /// </summary>
    [McpServerTool(Name = "list_topics")]
    [Description("List all available documentation topics and categories.")]
    public async Task<string> ListTopics(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        
        var documents = _indexService.GetAllDocuments();
        var byCategory = documents.GroupBy(d => d.Category);

        var result = "# Available Documentation Topics\n\n";
        
        foreach (var category in byCategory.OrderBy(g => g.Key))
        {
            result += $"## {category.Key}\n";
            foreach (var doc in category.Take(10))
            {
                result += $"- {doc.Title}\n";
            }
            result += "\n";
        }

        result += $"\n**Total documents:** {documents.Count()}";
        return result;
    }

    // ==================== PROMPTS ====================

    /// <summary>
    /// Creates a prompt for asking about Invictus installation
    /// </summary>
    [McpServerPrompt(Name = "installation_help")]
    [Description("Get help with Invictus installation")]
    public static GetPromptResult GetInstallationPrompt(
        [Description("Component: 'dashboard' or 'framework'")] string component = "framework")
    {
        return new GetPromptResult
        {
            Messages =
            [
                new PromptMessage
                {
                    Role = Role.User,
                    Content = new TextContentBlock
                    {
                        Text = $"I need help installing Invictus {component}. " +
                               "Guide me through prerequisites, build pipeline, and release pipeline with examples."
                    }
                }
            ]
        };
    }

    /// <summary>
    /// Creates a prompt for troubleshooting Invictus issues
    /// </summary>
    [McpServerPrompt(Name = "troubleshooting_help")]
    [Description("Get help troubleshooting Invictus issues")]
    public static GetPromptResult GetTroubleshootingPrompt(
        [Description("Component having issues")] string component,
        [Description("Problem description")] string problem)
    {
        return new GetPromptResult
        {
            Messages =
            [
                new PromptMessage
                {
                    Role = Role.User,
                    Content = new TextContentBlock
                    {
                        Text = $"Issue with Invictus {component}: {problem}. " +
                               "Help diagnose and resolve. Check docs for solutions and best practices."
                    }
                }
            ]
        };
    }

    /// <summary>
    /// Creates a prompt for component configuration help
    /// </summary>
    [McpServerPrompt(Name = "component_config_help")]
    [Description("Get help configuring an Invictus component")]
    public static GetPromptResult GetComponentConfigPrompt(
        [Description("Component name")] string component)
    {
        return new GetPromptResult
        {
            Messages =
            [
                new PromptMessage
                {
                    Role = Role.User,
                    Content = new TextContentBlock
                    {
                        Text = $"Show me how to configure {component} in Invictus. " +
                               "Include parameters, examples, and best practices."
                    }
                }
            ]
        };
    }
}