namespace docs_ifa_mcp.Services
{
    public class DocumentationInitializationService(
        DocumentationIndexService indexService,
        ILogger<DocumentationInitializationService> logger) : IHostedService
    {
        private readonly DocumentationIndexService _indexService = indexService;
        private readonly ILogger<DocumentationInitializationService> _logger = logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initializing documentation index...");
            await _indexService.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping documentation service");
            return Task.CompletedTask;
        }
    }
}
