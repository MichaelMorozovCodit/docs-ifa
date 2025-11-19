using docs_ifa_mcp.Models;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace docs_ifa_mcp.Services
{
    /// <summary>
    /// Service for indexing and managing Invictus documentation
    /// </summary>
    public class DocumentationIndexService
    {
        private readonly ConcurrentDictionary<string, DocumentationPage> _documents = new();
        private readonly ILogger<DocumentationIndexService> _logger;
        private readonly IConfiguration _configuration;
        private bool _isInitialized;

        public DocumentationIndexService(
            ILogger<DocumentationIndexService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            _logger.LogInformation("Starting documentation indexing...");
            _logger.LogInformation("Current directory: {CurrentDir}", Directory.GetCurrentDirectory());

            // Try configured paths
            var basePath = _configuration["Documentation:BasePath"];
            var fallbackPath = _configuration["Documentation:FallbackPath"];

            _logger.LogInformation("Configured BasePath: {BasePath}", basePath ?? "(not set)");
            _logger.LogInformation("Configured FallbackPath: {FallbackPath}", fallbackPath ?? "(not set)");

            string? docsPath = null;

            // Try base path first
            if (!string.IsNullOrEmpty(basePath))
            {
                var fullBasePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), basePath));
                _logger.LogInformation("Checking base path: {Path}", fullBasePath);
                
                if (Directory.Exists(fullBasePath))
                {
                    docsPath = fullBasePath;
                    _logger.LogInformation("✓ Using configured base path: {Path}", docsPath);
                }
                else
                {
                    _logger.LogWarning("❌ Configured base path not found: {Path}", fullBasePath);
                }
            }

            // Try fallback path (absolute)
            if (docsPath == null && !string.IsNullOrEmpty(fallbackPath))
            {
                _logger.LogInformation("Checking fallback path: {Path}", fallbackPath);
                
                if (Directory.Exists(fallbackPath))
                {
                    docsPath = fallbackPath;
                    _logger.LogInformation("✓ Using fallback path: {Path}", docsPath);
                }
                else
                {
                    _logger.LogWarning("❌ Fallback path not found: {Path}", fallbackPath);
                }
            }

            // Try multiple relative paths from current directory
            if (docsPath == null)
            {
                var possiblePaths = new[]
                {
                    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "versioned_docs", "version-v6.0.0")),
                    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "versioned_docs", "version-v6.0.0")),
                    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "versioned_docs", "version-v6.0.0")),
                    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "versioned_docs", "version-v6.0.0"))
                };

                _logger.LogInformation("Trying alternative paths...");
                
                foreach (var path in possiblePaths)
                {
                    _logger.LogInformation("Checking: {Path}", path);
                    
                    if (Directory.Exists(path))
                    {
                        docsPath = path;
                        _logger.LogInformation("✓ Found documentation at: {Path}", docsPath);
                        break;
                    }
                }
            }

            if (docsPath != null && Directory.Exists(docsPath))
            {
                await IndexDirectoryAsync(docsPath);
                _logger.LogInformation("✓ Successfully indexed {Count} documentation pages from: {Path}", _documents.Count, docsPath);
                _isInitialized = true;
            }
            else
            {
                _logger.LogError("❌ No documentation directory found!");
                _logger.LogError("Current working directory: {CurrentDir}", Directory.GetCurrentDirectory());
                _logger.LogError("Expected one of these paths to exist:");
                _logger.LogError("  1. {Path}", Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "versioned_docs", "version-v6.0.0")));
                _logger.LogError("  2. {Path}", Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "versioned_docs", "version-v6.0.0")));
                _logger.LogError("Please ensure the documentation files are in the correct location.");
                
                throw new DirectoryNotFoundException($"Documentation directory not found. Looked in multiple locations starting from: {Directory.GetCurrentDirectory()}");
            }
        }

        private async Task IndexDirectoryAsync(string path)
        {
            var files = Directory.GetFiles(path, "*.md*", SearchOption.AllDirectories);

            _logger.LogInformation("Found {Count} markdown files to index", files.Length);

            foreach (var file in files)
            {
                try
                {
                    await IndexFileAsync(file);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error indexing file: {File}", file);
                }
            }
        }

        private async Task IndexFileAsync(string filePath)
        {
            var content = await File.ReadAllTextAsync(filePath);
            var relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

            var page = new DocumentationPage
            {
                Id = GenerateDocId(filePath),
                Title = ExtractTitle(content),
                Description = ExtractDescription(content),
                Content = CleanMarkdown(content),
                Path = relativePath,
                Category = DetermineCategory(filePath),
                Keywords = ExtractKeywords(content)
            };

            _documents[page.Id] = page;
            _logger.LogDebug("Indexed: {Title} ({Id})", page.Title, page.Id);
        }

        private string GenerateDocId(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var dirName = Path.GetFileName(Path.GetDirectoryName(filePath)) ?? "root";
            return $"{fileName}_{dirName}".Replace(" ", "_");
        }

        private string ExtractTitle(string markdown)
        {
            // Extract from frontmatter or first H1
            var titleMatch = Regex.Match(markdown, @"^title:\s*[""']?([^""'\n]+)[""']?", RegexOptions.Multiline);
            if (titleMatch.Success)
                return titleMatch.Groups[1].Value.Trim();

            var h1Match = Regex.Match(markdown, @"^#\s+(.+)$", RegexOptions.Multiline);
            return h1Match.Success ? h1Match.Groups[1].Value.Trim() : "Untitled";
        }

        private string ExtractDescription(string markdown)
        {
            // Get first paragraph after title
            var cleaned = Regex.Replace(markdown, @"^---.*?---\s*", "", RegexOptions.Singleline);
            cleaned = Regex.Replace(cleaned, @"^#+\s+.*?$", "", RegexOptions.Multiline);

            var firstPara = Regex.Match(cleaned.Trim(), @"^[^\n]+", RegexOptions.Multiline);
            if (firstPara.Success && firstPara.Value.Length > 0)
            {
                var desc = firstPara.Value.Trim();
                return desc.Length > 200 ? desc.Substring(0, 200) + "..." : desc;
            }

            return "";
        }

        private string CleanMarkdown(string markdown)
        {
            // Remove frontmatter
            var content = Regex.Replace(markdown, @"^---.*?---\s*", "", RegexOptions.Singleline);

            // Remove JSX/React components
            content = Regex.Replace(content, @"<[A-Z][^>]*>.*?</[A-Z][^>]*>", "", RegexOptions.Singleline);
            content = Regex.Replace(content, @"import\s+.*?;", "", RegexOptions.Multiline);

            return content.Trim();
        }

        private string DetermineCategory(string filePath)
        {
            var path = filePath.ToLower();
            if (path.Contains("dashboard")) return "Dashboard";
            if (path.Contains("framework")) return "Framework";
            if (path.Contains("installation") || path.Contains("install")) return "Installation";
            if (path.Contains("component")) return "Components";
            if (path.Contains("tutorial")) return "Tutorials";
            if (path.Contains("guide")) return "Guides";
            return "General";
        }

        private List<string> ExtractKeywords(string content)
        {
            var keywords = new List<string>();

            // Extract common technical terms
            var patterns = new[]
            {
                @"\b(Invictus|Dashboard|Framework|Transco|PubSub|Logic App[s]?|Azure|Bicep|Container App[s]?)\b",
                @"\b(deployment|installation|configuration|authentication|migration)\b"
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                keywords.AddRange(matches.Select(m => m.Value.ToLower()).Distinct());
            }

            return keywords.Distinct().ToList();
        }

        public IEnumerable<DocumentationPage> GetAllDocuments() => _documents.Values;

        public DocumentationPage? GetDocument(string id) =>
            _documents.TryGetValue(id, out var doc) ? doc : null;

        public IEnumerable<DocumentationPage> SearchByKeyword(string keyword)
        {
            return _documents.Values
                .Where(d => d.Keywords.Contains(keyword.ToLower()) ||
                           d.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
