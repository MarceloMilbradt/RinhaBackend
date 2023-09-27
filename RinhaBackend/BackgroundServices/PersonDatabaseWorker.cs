using Mediator;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Events;

namespace RinhaBackend.BackgroundServices;

internal sealed class PersonDatabaseWorker(PersonInsertQueue queue, IPublisher publisher) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private const int SizeThreshold = 50;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var personToInsert = new List<Person>(SizeThreshold);
        var eventToPublish = new QueueFlushedEvent(personToInsert);
        while (!stoppingToken.IsCancellationRequested)
        {
            while (queue.TryDequeue(out var item))
            {
                personToInsert.Add(item);

                if (personToInsert.Count >= SizeThreshold)
                {
                    await publisher.Publish(eventToPublish, stoppingToken);
                    personToInsert.Clear();
                }
            }

            if (personToInsert.Count != 0)
            {
                await publisher.Publish(eventToPublish, stoppingToken);
                personToInsert.Clear();
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

}