using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Buffers;
using System.Runtime.InteropServices;

namespace RinhaBackend.Application;

public sealed class BulkInsertWorker(GlobalQueue queue) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            await PersonRepository.BulkInsertAsync(queue.GetAll(), stoppingToken);
        }
    }
}