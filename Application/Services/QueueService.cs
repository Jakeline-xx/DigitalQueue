using DigitalQueue.Application.Singleton;
using DigitalQueue.Domain.Entities;
using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Interfaces;
using System.Collections.Concurrent;

namespace DigitalQueue.Application.Services;
public class QueueService : IQueueService
{
    private static int _priorityCounter = 0;
    private static int _commonCounter = 0;

    private readonly Dictionary<QueueType, ConcurrentQueue<PatientCode>> _queues = new()
    {
        [QueueType.Reception] = new ConcurrentQueue<PatientCode>(),
        [QueueType.Screening] = new ConcurrentQueue<PatientCode>(),
        [QueueType.MedicalCare] = new ConcurrentQueue<PatientCode>(),
        [QueueType.Exam] = new ConcurrentQueue<PatientCode>(),
        [QueueType.Medication] = new ConcurrentQueue<PatientCode>(),
    };

    public PatientCode AddToQueue(bool isPriority)
    {
        var code = isPriority ? $"P{(++_priorityCounter):D4}" : $"C{(++_commonCounter):D4}";
        var patientCode = new PatientCode(code);

        var queue = _queues[QueueType.Reception];
        if (isPriority)
        {
            var items = queue.ToList();
            _queues[QueueType.Reception] = new ConcurrentQueue<PatientCode>(new[] { patientCode }.Concat(items));
        }
        else
        {
            queue.Enqueue(patientCode);
        }

        QueueDictionary.Instance.Add(code, QueueType.Reception);

        return patientCode;
    }

    public (string queueName, int peopleAhead, string currentCode) GetQueueStatus(string code)
    {
        var queueType = QueueDictionary.Instance.GetQueue(code);
        if (queueType == null) throw new Exception("Code not found in any queue.");

        var queue = _queues[queueType.Value].ToList();
        var position = queue.FindIndex(c => c.Code == code);
 
        var queueName = queueType.ToString() ?? string.Empty;
        var currentCode = queue.FirstOrDefault()?.Code ?? string.Empty;

        return (queueName, position, currentCode);
    }

    public void MoveToNextQueue(string code, QueueType newQueue)
    {
        var currentQueueType = QueueDictionary.Instance.GetQueue(code);
        if (currentQueueType == null) throw new Exception("Code not found.");

        var currentQueue = _queues[currentQueueType.Value];
        var newList = currentQueue.Where(c => c.Code != code).ToList();
        _queues[currentQueueType.Value] = new ConcurrentQueue<PatientCode>(newList);

        var patientCode = new PatientCode(code);
        var targetQueue = _queues[newQueue];

        if (code.StartsWith("P"))
        {
            var items = targetQueue.ToList();
            _queues[newQueue] = new ConcurrentQueue<PatientCode>(new[] { patientCode }.Concat(items));
        }
        else
        {
            targetQueue.Enqueue(patientCode);
        }

        QueueDictionary.Instance.Update(code, newQueue);
    }

    public void RemoveFromQueue(string code)
    {
        var queueType = QueueDictionary.Instance.GetQueue(code);
        if (queueType == null) return;

        var queue = _queues[queueType.Value];
        var newList = queue.Where(c => c.Code != code).ToList();
        _queues[queueType.Value] = new ConcurrentQueue<PatientCode>(newList);

        QueueDictionary.Instance.Remove(code);
    }
}