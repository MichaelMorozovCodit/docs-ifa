namespace docs_ifa_mcp.Constants
{
    /// <summary>
    /// Configuration keys and default values for documentation indexing
    /// </summary>
    public static class DocumentationConstants
    {
        /// <summary>
        /// Maximum length for description text before truncation
        /// </summary>
        public const int MaxDescriptionLength = 200;

        /// <summary>
        /// Ellipsis to append to truncated descriptions
        /// </summary>
        public const string DescriptionEllipsis = "...";

        /// <summary>
        /// Default title when none can be extracted
        /// </summary>
        public const string DefaultTitle = "Untitled";

        /// <summary>
        /// Default directory name for root-level documents
        /// </summary>
        public const string RootDirectoryName = "root";

        /// <summary>
        /// File search pattern for Markdown files
        /// </summary>
        public const string MarkdownFilePattern = "*.md*";

        /// <summary>
        /// Configuration key for documentation base path
        /// </summary>
        public const string ConfigKeyBasePath = "Documentation:BasePath";

        /// <summary>
        /// Configuration key for documentation fallback path
        /// </summary>
        public const string ConfigKeyFallbackPath = "Documentation:FallbackPath";

        /// <summary>
        /// Relative path segments to documentation version
        /// </summary>
        public static readonly string[] DocumentationPathSegments = 
        [
            "versioned_docs",
            "version-v6.0.0"
        ];
    }
}
