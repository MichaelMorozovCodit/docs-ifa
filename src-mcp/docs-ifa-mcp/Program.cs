using docs_ifa_mcp.Services;
using ModelContextProtocol.Protocol;

namespace docs_ifa_mcp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.AddConsole(consoleLogOptions =>
        {
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        builder.Services.AddSingleton<DocumentationIndexService>();
        builder.Services.AddSingleton<QueryService>();
        builder.Services.AddHostedService<DocumentationInitializationService>();

        builder.Services
            .AddMcpServer(options =>
            {
                options.ServerInfo = new Implementation
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
