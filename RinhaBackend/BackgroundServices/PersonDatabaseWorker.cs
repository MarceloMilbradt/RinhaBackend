using MediatR;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Events;

namespace RinhaBackend.BackgroundServices;

public class PersonDatabaseWorker : BackgroundService
{
    private readonly PersonInsertQueue _queue;
    private readonly IPublisher _publisher;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public PersonDatabaseWorker(PersonInsertQueue queue, IPublisher publisher)
    {
        _queue = queue;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var personToInsert = new List<Person>();
            while (_queue.TryDequeue(out var item))
            {
                personToInsert.Add(item);
            }

            await _publisher.Publish(new QueueFlushedEvent(personToInsert));

            await Task.Delay(_interval, stoppingToken);

        }
    }


}