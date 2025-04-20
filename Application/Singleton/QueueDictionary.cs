using DigitalQueue.Domain.Enums;
using System.Collections.Concurrent;

namespace DigitalQueue.Application.Singleton;
public class QueueDictionary
{
    private static readonly Lazy<QueueDictionary> _instance = new(() => new QueueDictionary());
    private readonly ConcurrentDictionary<string, QueueType> _dictionary = new();

    public static QueueDictionary Instance => _instance.Value;

    public void Add(string code, QueueType queueType) => _dictionary[code] = queueType;

    public void Remove(string code) => _dictionary.TryRemove(code, out _);

    public QueueType? GetQueue(string code) => _dictionary.TryGetValue(code, out var queue) ? queue : null;

    public void Update(string code, QueueType newQueue) => _dictionary[code] = newQueue;
}