using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Workers;

public sealed class WarmupWorker : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<WarmupWorker> _logger;
    public WarmupWorker(IServiceProvider provider, ILogger<WarmupWorker> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        _logger.LogInformation("Starting Warmup");

        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PersonContext>();

        var personList = new List<Person>();
        for (int x = 0; x < 10_000; x++)
        {
            var newPerson = new Person
            {
                Id = Guid.NewGuid(),
                Stack = ["C#"],
                Apelido = $"{x}",
                Nome = $"{x}"
            };
            personList.Add(newPerson);
        }

        try
        {
            await context.BulkInsertAsync(personList, options =>
            {
                options.AutoMapOutputDirection = false;
            }, stoppingToken);

            await context.Persons.ExecuteDeleteAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error on warmup");
        }
        _logger.LogCritical("Warmup finished");
    }
}
