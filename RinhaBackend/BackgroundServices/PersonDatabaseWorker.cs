using EFCore.BulkExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Events;
using System.Threading;

namespace RinhaBackend.BackgroundServices;

internal sealed class PersonDatabaseWorker : BackgroundService
{
    private readonly PersonInsertQueue _queue;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(2);
    private const int SizeThreshold = 200;  // Size threshold for immediate processing

    public PersonDatabaseWorker(PersonInsertQueue queue, IServiceScopeFactory serviceScopeFactory)
    {
        _queue = queue;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var personToInsert = new List<Person>();
            while (_queue.TryDequeue(out var item))
            {
                personToInsert.Add(item);

                // If we reach the size threshold, flush the queue and break out of the loop
                if (personToInsert.Count >= SizeThreshold)
                {
                    await FlushQueue(personToInsert, stoppingToken);
                    personToInsert.Clear();
                }
            }

            // If there are any leftover items after breaking the loop or if the loop completes normally
            if (personToInsert.Any())
            {
                await FlushQueue(personToInsert, stoppingToken);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task FlushQueue(IEnumerable<Person> items, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<PersonContext>();
        await _context.BulkInsertAsync(items, cancellationToken: stoppingToken);
    }
}
