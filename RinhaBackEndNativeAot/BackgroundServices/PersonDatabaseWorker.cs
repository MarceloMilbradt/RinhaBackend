using RinhaBackEndNativeAot.Models;
using RinhaBackEndNativeAot.Persistence;

namespace RinhaBackEndNativeAot.BackgroundServices;

internal sealed class PersonDatabaseWorker(PersonInsertQueue queue) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(2);
    private const int SizeThreshold = 500;

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
                    await FlushQueueAsync(personToInsert, stoppingToken);
                    personToInsert.Clear();
                }
            }

            if (personToInsert.Any())
            {
                await FlushQueueAsync(personToInsert, stoppingToken);
                personToInsert.Clear();  // Clear after flush, just for clarity
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private static async Task FlushQueueAsync(List<Person> items, CancellationToken stoppingToken)
    {
        await PersonRepository.BulkInsertAsync(items, stoppingToken);
    }
}
