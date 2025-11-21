using docs_ifa_mcp.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace docs_ifa_mcp.Services;

/// <summary>
/// MCP Tools for Invictus documentation
/// Provides tools for querying Invictus for Azure documentation
/// </summary>
[McpServerToolType]
public class Tools(
    DocumentationIndexService indexService,
    QueryService queryService,
    ILogger<Tools> logger)
{
    private readonly DocumentationIndexService _indexService = indexService;
    private readonly QueryService _queryService = queryService;
    private readonly ILogger<Tools> _logger = logger;

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
        string searchQuery = component.ToLower() switch
        {
            "dashboard" => $"Installing Invictus Dashboard {step ?? "all"}",
            "framework" => $"Installing Invictus Framework {step ?? "all"}",
            "both" => "Installing Invictus Dashboard Framework",
            _ => "Installing Invictus"
        };

        List<SearchResult> results = await _queryService.SearchAsync(searchQuery, 5);
        
        if (results.Count == 0)
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
}
