using RinhaBackend.Models;
using System.Collections.Concurrent;

namespace RinhaBackend.Persistence;

public sealed class PersonInsertQueue 
{
    private readonly ConcurrentQueue<Person> _queue = new();

    public void Enqueue(Person item)
    {
        _queue.Enqueue(item);
    }

    public bool TryDequeue(out Person item)
    {
        return _queue.TryDequeue(out item!);
    }
}
