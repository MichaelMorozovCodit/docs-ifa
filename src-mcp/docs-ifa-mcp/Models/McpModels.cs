using System.Text.Json;
using System.Text.Json.Serialization;
namespace docs_ifa_mcp.Models
{
    public class McpRequest
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("method")]
        public string Method { get; set; } = "";
    }

    public class McpReadRequest : McpRequest
    {
        [JsonPropertyName("params")]
        public McpReadParams Params { get; set; } = new();
    }

    public class McpReadParams
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = "";
    }

    public class McpToolCallRequest : McpRequest
    {
        [JsonPropertyName("params")]
        public McpToolCallParams Params { get; set; } = new();
    }

    public class McpToolCallParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("arguments")]
        public JsonElement Arguments { get; set; }
    }

    public class McpResource
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = "text/plain";
    }

    public class McpTool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("inputSchema")]
        public object InputSchema { get; set; } = new();
    }

    public class McpContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }
    }

    public class McpResourceListResponse
    {
        [JsonPropertyName("resources")]
        public List<McpResource> Resources { get; set; } = new();
    }

    public class McpResourceResponse
    {
        [JsonPropertyName("contents")]
        public McpContent[] Contents { get; set; } = Array.Empty<McpContent>();
    }

    public class McpToolListResponse
    {
        [JsonPropertyName("tools")]
        public McpTool[] Tools { get; set; } = Array.Empty<McpTool>();
    }

    public class McpToolResponse
    {
        [JsonPropertyName("content")]
        public McpContent[] Content { get; set; } = Array.Empty<McpContent>();

        [JsonPropertyName("isError")]
        public bool IsError { get; set; }
    }
}
