using DigitalQueue.Domain.Entities;
using DigitalQueue.Domain.Enums;

namespace DigitalQueue.Domain.Interfaces;
public interface IQueueService
{
    PatientCode AddToQueue(bool isPriority);
    (string queueName, int peopleAhead, string currentCode) GetQueueStatus(string code);
    void MoveToNextQueue(string code, QueueType newQueue);
    void RemoveFromQueue(string code);
    Dictionary<string, List<string>> GetAllQueues();
}