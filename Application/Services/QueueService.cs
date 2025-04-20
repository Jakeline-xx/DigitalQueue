using DigitalQueue.Application.Singleton;
using DigitalQueue.Domain.Entities;
using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Interfaces;
using DigitalQueue.Domain.Models;
using System.Collections.Concurrent;

namespace DigitalQueue.Application.Services;
public class QueueService : IQueueService
{
    private static int _priorityCounter = 0;
    private static int _commonCounter = 0;

    private readonly Dictionary<QueueType, List<PatientCode>> _queues = new()
    {
        [QueueType.Reception] = new List<PatientCode>(),
        [QueueType.Screening] = new List<PatientCode>(),
        [QueueType.MedicalCare] = new List<PatientCode>(),
        [QueueType.Exam] = new List<PatientCode>(),
        [QueueType.Medication] = new List<PatientCode>(),
    };

    public PatientCode AddToQueue(bool isPriority)
    {
        var code = isPriority ? $"P{(++_priorityCounter):D4}" : $"C{(++_commonCounter):D4}";
        var patientCode = new PatientCode(code);

        InsertIntoQueue(_queues[QueueType.Reception], patientCode);

        QueueDictionary.Instance.Add(new(code), QueueType.Reception);

        return patientCode;
    }

    public (string queueName, int peopleAhead, string currentCode) GetQueueStatus(string code)
    {
        var queueType = QueueDictionary.Instance.GetQueue(new(code));
        if (queueType == null) throw new Exception("Code not found in any queue.");

        var queue = _queues[queueType.Value];
        var position = queue.FindIndex(c => c.Code == code);

        return (queueType.ToString(), position, queue.FirstOrDefault()?.Code);
    }

    public void MoveToNextQueue(string code, QueueType newQueue)
    {
        var currentQueueType = QueueDictionary.Instance.GetQueue(new(code));
        if (currentQueueType == null) throw new Exception("Code not found.");

        var currentQueue = _queues[currentQueueType.Value];
        var patient = currentQueue.FirstOrDefault(c => c.Code == code);
        if (patient == null) return;
        currentQueue.Remove(patient);

        InsertIntoQueue(_queues[newQueue], patient);

        QueueDictionary.Instance.Update(new(code), newQueue);
    }

    public void RemoveFromQueue(string code)
    {
        var queueType = QueueDictionary.Instance.GetQueue(new(code));
        if (queueType == null) return;

        var queue = _queues[queueType.Value];
        queue.RemoveAll(c => c.Code == code);

        QueueDictionary.Instance.Remove(new(code));
    }

    private void InsertIntoQueue(List<PatientCode> queue, PatientCode patientCode)
    {
        if (patientCode.IsPriority)
        {
            int index = queue.FindLastIndex(c => c.IsPriority);
            queue.Insert(index + 1, patientCode);
        }
        else
        {
            queue.Add(patientCode);
        }
    }

    public Dictionary<string, List<string>> GetAllQueues()
    {
        return _queues.ToDictionary(
            q => q.Key.ToString(),
            q => q.Value.Select(c => c.Code).ToList()
        );
    }

}