using DigitalQueue.Application.Singleton;
using DigitalQueue.Domain.Entities;
using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Interfaces;

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
        try
        {
            var code = isPriority ? $"P{(++_priorityCounter):D4}" : $"C{(++_commonCounter):D4}";
            var patientCode = new PatientCode(code);

            InsertIntoQueue(_queues[QueueType.Reception], patientCode);

            QueueDictionary.Instance.Add(new(code), QueueType.Reception);

            return patientCode;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to add patient to the queue.", ex);
        }
    }

    public (string queueName, int peopleAhead, string currentCode) GetQueueStatus(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty.", nameof(code));

        try
        {
            var queueType = QueueDictionary.Instance.GetQueue(new(code));
            if (queueType == null) throw new KeyNotFoundException("Code not found in any queue.");

            var queue = _queues[queueType.Value];
            var position = queue.FindIndex(c => c.Code == code);

            var currentCode = queue.FirstOrDefault()?.Code ?? string.Empty;

            return (queueType.ToString() ?? string.Empty, position, currentCode);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve queue status.", ex);
        }
    }

    public void MoveToNextQueue(string code, QueueType newQueue)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty.", nameof(code));

        if (!_queues.ContainsKey(newQueue))
            throw new ArgumentException("Invalid queue type.", nameof(newQueue));

        try
        {
            var currentQueueType = QueueDictionary.Instance.GetQueue(new(code));
            if (currentQueueType == null) throw new KeyNotFoundException("Code not found.");

            var currentQueue = _queues[currentQueueType.Value];
            var patient = currentQueue.FirstOrDefault(c => c.Code == code);
            if (patient == null) throw new KeyNotFoundException("Patient not found in the current queue.");

            currentQueue.Remove(patient);

            InsertIntoQueue(_queues[newQueue], patient);

            QueueDictionary.Instance.Update(new(code), newQueue);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to move patient to the next queue.", ex);
        }
    }

    public void RemoveFromQueue(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty.", nameof(code));

        try
        {
            var queueType = QueueDictionary.Instance.GetQueue(new(code));
            if (queueType == null) throw new KeyNotFoundException("Code not found.");

            var queue = _queues[queueType.Value];
            queue.RemoveAll(c => c.Code == code);

            QueueDictionary.Instance.Remove(new(code));
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to remove patient from the queue.", ex);
        }
    }

    private void InsertIntoQueue(List<PatientCode> queue, PatientCode patientCode)
    {
        if (queue == null)
            throw new ArgumentNullException(nameof(queue), "Queue cannot be null.");

        if (patientCode == null)
            throw new ArgumentNullException(nameof(patientCode), "Patient code cannot be null.");

        try
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
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to insert patient into the queue.", ex);
        }
    }

    public Dictionary<string, List<string>> GetAllQueues()
    {
        try
        {
            return _queues.ToDictionary(
                q => q.Key.ToString(),
                q => q.Value.Select(c => c.Code).ToList()
            );
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve all queues.", ex);
        }
    }
}
