using System.Collections.Concurrent;

namespace RinhaBackend.Application;

public sealed class GlobalQueue
{
    private readonly ConcurrentQueue<Person> queue = new ConcurrentQueue<Person>();

    public void Enqueue(Person item)
    {
        queue.Enqueue(item);
    }

    public bool TryDequeue(out Person result)
    {
        return queue.TryDequeue(out result);
    }

    public int Count
    {
        get { return queue.Count; }
    }
}
