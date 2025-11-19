using docs_ifa_mcp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace docs_ifa_mcp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register MCP services
            builder.Services.AddSingleton<DocumentationIndexService>();
            builder.Services.AddSingleton<QueryService>();
            builder.Services.AddHostedService<DocumentationInitializationService>();

            // Configure CORS for MCP clients
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMcpClients", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Configure logging
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowMcpClients");
            app.UseRouting();

            app.MapControllers();

            // Health check endpoint
            app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Invictus MCP Server" }));

            // MCP endpoint discovery
            string[] capabilities = ["resources/list", "resources/read", "tools/list", "tools/call"];
            app.MapGet("/", () => Results.Ok(new
            {
                name = "Invictus Documentation MCP Server",
                version = "1.0.0",
                description = "MCP server for querying Invictus for Azure documentation",
                capabilities
            }));

            app.Run();
        }
    }
}
