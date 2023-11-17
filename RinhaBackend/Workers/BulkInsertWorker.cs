using RinhaBackend.Persistence;
namespace RinhaBackend.Workers;

public sealed class BulkInsertWorker : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly GlobalQueue _insertQueue;
    private readonly ILogger<BulkInsertWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public BulkInsertWorker(GlobalQueue insertQueue, IServiceProvider provider, ILogger<BulkInsertWorker> logger)
    {
        _insertQueue = insertQueue;
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PersonContext>();
            _logger.LogInformation("Starting Bulk Insert");
            await context.BulkInsertAsync(_insertQueue.GetAll().Where(i => i != null), options =>
            {
                options.AutoMapOutputDirection = false;
                options.RetryCount = 2;
                options.RetryInterval = _interval;
                
            }, stoppingToken);
            _logger.LogInformation("Bulk Insert Completed");
        }
    }
}
