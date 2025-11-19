namespace docs_ifa_mcp.Services
{
    public class DocumentationInitializationService : IHostedService
    {
        private readonly DocumentationIndexService _indexService;
        private readonly ILogger<DocumentationInitializationService> _logger;

        public DocumentationInitializationService(
            DocumentationIndexService indexService,
            ILogger<DocumentationInitializationService> logger)
        {
            _indexService = indexService;
            _logger = logger;
        }

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
