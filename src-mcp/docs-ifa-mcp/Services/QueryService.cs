using docs_ifa_mcp.Models;
using System.Text.RegularExpressions;

namespace docs_ifa_mcp.Services
{
    /// <summary>
    /// Service for querying and searching documentation
    /// </summary>
    public class QueryService
    {
        private readonly DocumentationIndexService _indexService;
        private readonly ILogger<QueryService> _logger;

        public QueryService(
            DocumentationIndexService indexService,
            ILogger<QueryService> logger)
        {
            _indexService = indexService;
            _logger = logger;
        }

        public async Task<List<SearchResult>> SearchAsync(string query, int maxResults = 5)
        {
            _logger.LogInformation("Searching for: {Query}", query);

            await Task.CompletedTask;

            IEnumerable<DocumentationPage> allDocs = _indexService.GetAllDocuments();
            string[] queryTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            List<SearchResult> results = allDocs
                .Select(doc => new SearchResult
                {
                    Id = doc.Id,
                    Title = doc.Title,
                    Description = doc.Description,
                    Content = TruncateContent(doc.Content, query),
                    Category = doc.Category,
                    Score = CalculateRelevanceScore(doc, queryTerms)
                })
                .Where(r => r.Score > 0)
                .OrderByDescending(r => r.Score)
                .Take(maxResults)
                .ToList();

            _logger.LogInformation("Found {Count} results", results.Count);
            return results;
        }

        private double CalculateRelevanceScore(DocumentationPage doc, string[] queryTerms)
        {
            double score = 0;
            string content = (doc.Title + " " + doc.Description + " " + doc.Content).ToLower();

            foreach (var term in queryTerms)
            {
                // Title matches are weighted higher
                if (doc.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
                    score += 10;

                // Description matches
                if (doc.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
                    score += 5;

                // Keyword matches
                if (doc.Keywords.Contains(term))
                    score += 3;

                // Content matches
                var contentMatches = Regex.Matches(content, $@"\b{Regex.Escape(term)}\b");
                score += contentMatches.Count * 0.5;
            }

            // Boost for installation/configuration docs
            if (doc.Category == "Installation" && queryTerms.Any(t => t.Contains("install") || t.Contains("deploy")))
                score *= 1.5;

            return score;
        }

        private string TruncateContent(string content, string query)
        {
            // Find relevant excerpt around query terms
            string[] queryTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string contentLower = content.ToLower();

            foreach (var term in queryTerms)
            {
                var index = contentLower.IndexOf(term);
                if (index >= 0)
                {
                    var start = Math.Max(0, index - 200);
                    var length = Math.Min(600, content.Length - start);
                    var excerpt = content.Substring(start, length);

                    if (start > 0) excerpt = "..." + excerpt;
                    if (start + length < content.Length) excerpt += "...";

                    return excerpt;
                }
            }

            // Fallback to first 500 characters
            return content.Length > 500
                ? content.Substring(0, 500) + "..."
                : content;
        }
    }
}