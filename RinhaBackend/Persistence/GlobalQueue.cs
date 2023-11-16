using RinhaBackend.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RinhaBackend.Persistence;

public sealed class GlobalQueue
{
    private readonly ConcurrentQueue<Person> queue = new();

    public void Enqueue(Person item)
    {
        queue.Enqueue(item);
    }

    public bool TryDequeue(out Person result)
    {
        return queue.TryDequeue(out result);
    }

    public IEnumerable<Person> GetAll()
    {
        while (TryDequeue(out Person result))
        {
            yield return result;
        }
    }

    public int Count
    {
        get { return queue.Count; }
    }
}
