using EFCore.BulkExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Events;
using System.Threading;

namespace RinhaBackend.BackgroundServices;

internal sealed class PersonDatabaseWorker(PersonInsertQueue queue, IPublisher publisher) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(2);
    private const int SizeThreshold = 250;

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
                    await publisher.Publish(new QueueFlushedEvent(personToInsert), stoppingToken);
                    personToInsert.Clear();
                }
            }

            if (personToInsert.Count != 0)
            {
                await publisher.Publish(new QueueFlushedEvent(personToInsert), stoppingToken);
                personToInsert.Clear();
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

}