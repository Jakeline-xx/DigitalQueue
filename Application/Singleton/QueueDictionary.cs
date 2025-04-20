using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Models;
using System.Collections.Concurrent;

namespace DigitalQueue.Application.Singleton;
public class QueueDictionary
{
    private static readonly Lazy<QueueDictionary> _instance = new(() => new QueueDictionary());
    private readonly ConcurrentDictionary<PatientKey, QueueType> _dictionary = new();

    public static QueueDictionary Instance => _instance.Value;

    public void Add(PatientKey code, QueueType queueType) => _dictionary[code] = queueType;

    public void Remove(PatientKey code) => _dictionary.TryRemove(code, out _);

    public QueueType? GetQueue(PatientKey code) => _dictionary.TryGetValue(code, out var queue) ? queue : null;

    public void Update(PatientKey code, QueueType newQueue) => _dictionary[code] = newQueue;
}