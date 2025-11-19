using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using docs_ifa_mcp.Models;
using docs_ifa_mcp.Services;

namespace InvictusMcpServer.Controllers;

[ApiController]
[Route("mcp")]
public class McpController : ControllerBase
{
    private readonly QueryService _queryService;
    private readonly DocumentationIndexService _indexService;
    private readonly ILogger<McpController> _logger;

    public McpController(
        QueryService queryService,
        DocumentationIndexService indexService,
        ILogger<McpController> logger)
    {
        _queryService = queryService;
        _indexService = indexService;
        _logger = logger;
    }

    /// <summary>
    /// List available resources (documentation pages)
    /// </summary>
    [HttpPost("resources/list")]
    public IActionResult ListResources([FromBody] McpRequest request)
    {
        _logger.LogInformation("Listing available documentation resources");

        var resources = _indexService.GetAllDocuments()
            .Select(doc => new McpResource
            {
                Uri = $"invictus://docs/{doc.Id}",
                Name = doc.Title,
                Description = doc.Description,
                MimeType = "text/markdown"
            })
            .ToList();

        return Ok(new McpResourceListResponse
        {
            Resources = resources
        });
    }

    /// <summary>
    /// Read a specific documentation resource
    /// </summary>
    [HttpPost("resources/read")]
    public IActionResult ReadResource([FromBody] McpReadRequest request)
    {
        _logger.LogInformation("Reading resource: {Uri}", request.Params.Uri);

        var docId = request.Params.Uri.Replace("invictus://docs/", "");
        var document = _indexService.GetDocument(docId);

        if (document == null)
        {
            return NotFound(new { error = "Resource not found" });
        }

        return Ok(new McpResourceResponse
        {
            Contents = new[]
            {
                new McpContent
                {
                    Uri = request.Params.Uri,
                    MimeType = "text/markdown",
                    Text = document.Content
                }
            }
        });
    }

    /// <summary>
    /// List available tools for querying documentation
    /// </summary>
    [HttpPost("tools/list")]
    public IActionResult ListTools()
    {
        _logger.LogInformation("Listing available tools");

        var tools = new[]
        {
            new McpTool
            {
                Name = "search_documentation",
                Description = "Search Invictus documentation using semantic search. Returns relevant documentation sections.",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        query = new
                        {
                            type = "string",
                            description = "The search query for Invictus documentation"
                        },
                        maxResults = new
                        {
                            type = "integer",
                            description = "Maximum number of results to return (default: 5)",
                            @default = 5
                        }
                    },
                    required = new[] { "query" }
                }
            },
            new McpTool
            {
                Name = "get_installation_guide",
                Description = "Get step-by-step installation instructions for Invictus Dashboard or Framework",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        component = new
                        {
                            type = "string",
                            @enum = new[] { "dashboard", "framework" },
                            description = "Which Invictus component to get installation guide for"
                        }
                    },
                    required = new[] { "component" }
                }
            },
            new McpTool
            {
                Name = "get_component_info",
                Description = "Get detailed information about a specific Invictus Framework component (e.g., Transco, PubSub, XML/JSON Converter)",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        component = new
                        {
                            type = "string",
                            description = "Name of the Framework component"
                        }
                    },
                    required = new[] { "component" }
                }
            }
        };

        return Ok(new McpToolListResponse
        {
            Tools = tools
        });
    }

    /// <summary>
    /// Execute a tool call
    /// </summary>
    [HttpPost("tools/call")]
    public async Task<IActionResult> CallTool([FromBody] McpToolCallRequest request)
    {
        _logger.LogInformation("Calling tool: {ToolName}", request.Params.Name);

        try
        {
            return request.Params.Name switch
            {
                "search_documentation" => await SearchDocumentation(request.Params.Arguments),
                "get_installation_guide" => GetInstallationGuide(request.Params.Arguments),
                "get_component_info" => GetComponentInfo(request.Params.Arguments),
                _ => BadRequest(new { error = $"Unknown tool: {request.Params.Name}" })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool: {ToolName}", request.Params.Name);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private async Task<IActionResult> SearchDocumentation(JsonElement arguments)
    {
        var query = arguments.GetProperty("query").GetString() ?? "";
        var maxResults = arguments.TryGetProperty("maxResults", out var maxProp)
            ? maxProp.GetInt32()
            : 5;

        var results = await _queryService.SearchAsync(query, maxResults);

        var content = results.Any()
            ? string.Join("\n\n---\n\n", results.Select(r =>
                $"**{r.Title}** (Relevance: {r.Score:P0})\n{r.Description}\n\n{r.Content}"))
            : "No relevant documentation found for your query.";

        return Ok(new McpToolResponse
        {
            Content = new[]
            {
                new McpContent
                {
                    Type = "text",
                    Text = content
                }
            }
        });
    }

    private IActionResult GetInstallationGuide(JsonElement arguments)
    {
        var component = arguments.GetProperty("component").GetString();

        var searchQuery = component?.ToLower() == "dashboard"
            ? "Installing Invictus Dashboard build pipeline release pipeline"
            : "Installing Invictus Framework build pipeline release pipeline";

        var results = _queryService.SearchAsync(searchQuery, 3).Result;

        var content = results.Any()
            ? string.Join("\n\n", results.Select(r => $"# {r.Title}\n\n{r.Content}"))
            : $"Installation guide for {component} not found.";

        return Ok(new McpToolResponse
        {
            Content = new[]
            {
                new McpContent
                {
                    Type = "text",
                    Text = content
                }
            }
        });
    }

    private IActionResult GetComponentInfo(JsonElement arguments)
    {
        var component = arguments.GetProperty("component").GetString() ?? "";

        var results = _queryService.SearchAsync($"{component} component endpoint parameters", 2).Result;

        var content = results.Any()
            ? string.Join("\n\n", results.Select(r => $"# {r.Title}\n\n{r.Content}"))
            : $"No information found for component: {component}";

        return Ok(new McpToolResponse
        {
            Content = new[]
            {
                new McpContent
                {
                    Type = "text",
                    Text = content
                }
            }
        });
    }
}