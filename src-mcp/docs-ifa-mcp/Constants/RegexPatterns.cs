namespace docs_ifa_mcp.Constants
{
    /// <summary>
    /// Regular expression patterns for extracting information from documentation
    /// </summary>
    public static class RegexPatterns
    {
        /// <summary>
        /// Pattern to extract title from YAML frontmatter
        /// </summary>
        public const string FrontmatterTitle = @"^title:\s*[""']?([^""'\n]+)[""']?";

        /// <summary>
        /// Pattern to extract first H1 heading
        /// </summary>
        public const string FirstH1 = @"^#\s+(.+)$";

        /// <summary>
        /// Pattern to remove YAML frontmatter block
        /// </summary>
        public const string RemoveFrontmatter = @"^---.*?---\s*";

        /// <summary>
        /// Pattern to remove all heading markers
        /// </summary>
        public const string RemoveHeadings = @"^#+\s+.*?$";

        /// <summary>
        /// Pattern to extract first paragraph
        /// </summary>
        public const string FirstParagraph = @"^[^\n]+";

        /// <summary>
        /// Pattern to match JSX/React components (opening and closing tags)
        /// </summary>
        public const string JsxComponents = @"<[A-Z][^>]*>.*?</[A-Z][^>]*>";

        /// <summary>
        /// Pattern to match import statements
        /// </summary>
        public const string ImportStatements = @"import\s+.*?;";

        /// <summary>
        /// Pattern to extract Invictus-specific technical terms
        /// </summary>
        public const string TechnicalTerms = @"\b(Invictus|Dashboard|Framework|Transco|PubSub|Logic App[s]?|Azure|Bicep|Container App[s]?)\b";

        /// <summary>
        /// Pattern to extract common deployment/configuration keywords
        /// </summary>
        public const string DeploymentKeywords = @"\b(deployment|installation|configuration|authentication|migration)\b";
    }
}
