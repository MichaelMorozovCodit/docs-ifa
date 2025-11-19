using docs_ifa_mcp.Services;

namespace docs_ifa_mcp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Configure logging to stderr (as per MCP spec)
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            // Configure all logs to go to stderr
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        // Register documentation services
        builder.Services.AddSingleton<DocumentationIndexService>();
        builder.Services.AddSingleton<QueryService>();
        builder.Services.AddHostedService<DocumentationInitializationService>();

        // Register MCP server with stdio transport and discover tools/prompts/resources from assembly
        builder.Services
            .AddMcpServer(options =>
            {
                options.ServerInfo = new ModelContextProtocol.Protocol.Implementation
                {
                    Name = "invictus-docs-mcp",
                    Version = "1.0.0"
                };
            })
            .WithStdioServerTransport()
            .WithToolsFromAssembly()
            .WithPromptsFromAssembly()
            .WithResourcesFromAssembly();

        var app = builder.Build();
        await app.RunAsync();
    }
}
