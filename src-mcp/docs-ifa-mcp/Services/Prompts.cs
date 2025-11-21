using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;
using System.ComponentModel;

namespace docs_ifa_mcp.Services;

/// <summary>
/// MCP Prompts for Invictus documentation
/// Provides prompt templates for common Invictus documentation queries
/// </summary>
[McpServerPromptType]
public class Prompts
{
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
