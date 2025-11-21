namespace docs_ifa_mcp.Models
{
    public class DocumentationPage
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Content { get; set; } = "";
        public string Path { get; set; } = "";
        public string Category { get; set; } = "";
        public List<string> Keywords { get; set; } = [];
    }

    public class SearchResult
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Content { get; set; } = "";
        public string Category { get; set; } = "";
        public double Score { get; set; }
    }
}
