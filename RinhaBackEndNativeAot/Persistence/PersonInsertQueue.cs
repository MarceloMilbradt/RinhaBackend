using RinhaBackEndNativeAot.Models;
using System.Collections.Concurrent;

namespace RinhaBackEndNativeAot.Persistence;

internal sealed class PersonInsertQueue
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
