using docs_ifa_mcp.Models;
using docs_ifa_mcp.Constants;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Text;

namespace docs_ifa_mcp.Services
{
    /// <summary>
    /// Service for indexing and managing Invictus documentation
    /// </summary>
    public class DocumentationIndexService(
        ILogger<DocumentationIndexService> logger,
        IConfiguration configuration)
    {
        private readonly ConcurrentDictionary<string, DocumentationPage> _documents = new();
        private readonly ILogger<DocumentationIndexService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private bool _isInitialized;

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            _logger.LogInformation("Starting documentation indexing...");

            if (_logger.IsEnabled(LogLevel.Information))
            {
                string currentDir = Directory.GetCurrentDirectory();
                _logger.LogInformation("Current directory: {CurrentDir}", currentDir);

                // Try configured paths
                var basePath = _configuration[DocumentationConstants.ConfigKeyBasePath];
                var fallbackPath = _configuration[DocumentationConstants.ConfigKeyFallbackPath];

                _logger.LogInformation("Configured BasePath: {BasePath}", basePath ?? "(not set)");
                _logger.LogInformation("Configured FallbackPath: {FallbackPath}", fallbackPath ?? "(not set)");

                string? docsPath = null;

                // Try base path first
                if (!string.IsNullOrEmpty(basePath))
                {
                    var fullBasePath = Path.GetFullPath(Path.Combine(currentDir, basePath));
                    _logger.LogInformation("Checking base path: {Path}", fullBasePath);

                    if (Directory.Exists(fullBasePath))
                    {
                        docsPath = fullBasePath;
                        _logger.LogInformation("✓ Using configured base path: {Path}", docsPath);
                    }
                    else
                    {
                        if (_logger.IsEnabled(LogLevel.Warning))
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
                        if (_logger.IsEnabled(LogLevel.Warning))
                            _logger.LogWarning("❌ Fallback path not found: {Path}", fallbackPath);
                    }
                }

                // Try multiple relative paths from current directory
                if (docsPath == null)
                {
                    var possiblePaths = new[]
                    {
                        Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", DocumentationConstants.DocumentationPathSegments[0], DocumentationConstants.DocumentationPathSegments[1])),
                        Path.GetFullPath(Path.Combine(currentDir, "..", "..", DocumentationConstants.DocumentationPathSegments[0], DocumentationConstants.DocumentationPathSegments[1])),
                        Path.GetFullPath(Path.Combine(currentDir, "..", DocumentationConstants.DocumentationPathSegments[0], DocumentationConstants.DocumentationPathSegments[1])),
                        Path.GetFullPath(Path.Combine(currentDir, DocumentationConstants.DocumentationPathSegments[0], DocumentationConstants.DocumentationPathSegments[1]))
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
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("❌ No documentation directory found!");
                        _logger.LogError("Current working directory: {CurrentDir}", currentDir);
                        _logger.LogError("Expected one of these paths to exist:");
                        _logger.LogError("  1. {Path}", Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", DocumentationConstants.DocumentationPathSegments[0], DocumentationConstants.DocumentationPathSegments[1])));
                        _logger.LogError("  2. {Path}", Path.GetFullPath(Path.Combine(currentDir, "..", DocumentationConstants.DocumentationPathSegments[0], DocumentationConstants.DocumentationPathSegments[1])));
                        _logger.LogError("  3. {Path}", Path.GetFullPath(Path.Combine(currentDir, DocumentationConstants.DocumentationPathSegments[0], DocumentationConstants.DocumentationPathSegments[1])));
                    }
                }
            }
        }

        private async Task IndexDirectoryAsync(string directoryPath)
        {
            try
            {
                var markdownFiles = Directory.EnumerateFiles(directoryPath, "*.md", SearchOption.AllDirectories);

                _logger.LogInformation("Found {Count} markdown files in directory: {Path}", markdownFiles.Count(), directoryPath);

                foreach (var filePath in markdownFiles)
                {
                    await IndexFileAsync(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error indexing directory: {Path}", directoryPath);
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

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Indexed: {Title} ({Id})", page.Title, page.Id);
        }

        private string GenerateDocId(string filePath)
        {
            // Generate a unique ID for the documentation page based on its file path
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(filePath))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private string ExtractTitle(string content)
        {
            // Extract the title from the markdown content (first line as a simple heuristic)
            return content.Split('\n').FirstOrDefault()?.Trim() ?? "Untitled";
        }

        private string ExtractDescription(string content)
        {
            // Extract the description from the markdown content (first paragraph as a simple heuristic)
            var match = Regex.Match(content, @"^#\s*(.+)$", RegexOptions.Multiline);
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }

        private string CleanMarkdown(string content)
        {
            // Basic cleaning of markdown content for indexing (remove unnecessary lines, etc.)
            var lines = content.Split('\n')
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("# "));

            return string.Join("\n", lines);
        }

        private string DetermineCategory(string filePath)
        {
            // Determine the category based on the file path (e.g., folder structure)
            var segments = filePath.Split(Path.DirectorySeparatorChar);
            return segments.Length > 2 ? segments[segments.Length - 2] : "Uncategorized";
        }

        private List<string> ExtractKeywords(string content)
        {
            // Extract keywords from the content (simple implementation: use title and first paragraph words)
            var keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Extract words from content
            var words = Regex.Matches(content, @"\b[a-zA-Z]{4,}\b")
                .Select(m => m.Value.ToLower())
                .Where(w => !string.IsNullOrWhiteSpace(w));

            foreach (var word in words.Take(50)) // Limit to first 50 meaningful words
            {
                keywords.Add(word);
            }

            return keywords.ToList();
        }

        public IEnumerable<DocumentationPage> GetAllDocuments() => _documents.Values;
    }
}
