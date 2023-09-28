using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace RinhaBackend.Application;

public sealed class BulkInsertWorker(GlobalQueue queue, IConfiguration configuration) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(3);
    private readonly int SizeThreshold = Convert.ToInt32(configuration["BulkSize"] ?? "100");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var personToInsert = new List<Person>(SizeThreshold);

        while (!stoppingToken.IsCancellationRequested)
        {
            while (queue.TryDequeue(out var item))
            {
                personToInsert.Add(item);

                if (personToInsert.Count >= SizeThreshold)
                {
                    FlushQueueAsync(personToInsert, stoppingToken);
                    personToInsert.Clear();
                }
            }

            if (personToInsert.Count != 0)
            {
                FlushQueueAsync(personToInsert, stoppingToken);
                personToInsert.Clear();  // Clear after flush, just for clarity
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private static void FlushQueueAsync(List<Person> items, CancellationToken stoppingToken)
    {
        var itemsToInsert = items.ToArray();
        Task.Run(() => PersonRepository.BulkInsertAsync(itemsToInsert, stoppingToken), stoppingToken);
    }
}