﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Buffers;
using System.Runtime.InteropServices;

namespace RinhaBackend.Application;

public sealed class BulkInsertWorker(GlobalQueue queue, IConfiguration configuration) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(2);
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
                    await FlushQueueAsync(personToInsert, stoppingToken);
                    personToInsert.Clear();
                }
            }

            if (personToInsert.Count != 0)
            {
                await FlushQueueAsync(personToInsert, stoppingToken);
                personToInsert.Clear();  // Clear after flush, just for clarity
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private static async Task FlushQueueAsync(List<Person> items, CancellationToken stoppingToken)
    {
        var size = items.Count;
        var itemsToCreate = ArrayPool<Person>.Shared.Rent(size);
        try
        {
            items.CopyTo(itemsToCreate, 0);
            await PersonRepository.BulkInsertAsync(itemsToCreate, stoppingToken);
        }
        finally
        {
            ArrayPool<Person>.Shared.Return(itemsToCreate);
        }
    }

}