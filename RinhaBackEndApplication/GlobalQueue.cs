using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RinhaBackend.Application;

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
        int count = 0;
        while (count < 500 && TryDequeue(out Person result))
        {
            yield return result;
            count++;
        }
    }

    public int Count
    {
        get { return queue.Count; }
    }
}
