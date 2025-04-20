using DigitalQueue.Application.Singleton;
using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Models;
using Xunit;

namespace DigitalQueue.Tests;

public class QueueDictionaryTests
{
    private readonly QueueDictionary _queueDictionary;

    public QueueDictionaryTests()
    {
        _queueDictionary = QueueDictionary.Instance; 
    }

    [Fact]
    public void Add_ShouldAddPatientToDictionary()
    {
        // Arrange
        var patientKey = new PatientKey("C0001");
        var queueType = QueueType.Reception;

        // Act
        _queueDictionary.Add(patientKey, queueType);

        // Assert
        var result = _queueDictionary.GetQueue(patientKey);
        Assert.NotNull(result);
        Assert.Equal(queueType, result);
    }

    [Fact]
    public void Remove_ShouldRemovePatientFromDictionary()
    {
        // Arrange
        var patientKey = new PatientKey("C0002");
        var queueType = QueueType.Screening;
        _queueDictionary.Add(patientKey, queueType);

        // Act
        _queueDictionary.Remove(patientKey);

        // Assert
        var result = _queueDictionary.GetQueue(patientKey);
        Assert.Null(result);
    }

    [Fact]
    public void GetQueue_ShouldReturnCorrectQueueType()
    {
        // Arrange
        var patientKey = new PatientKey("C0003");
        var queueType = QueueType.MedicalCare;
        _queueDictionary.Add(patientKey, queueType);

        // Act
        var result = _queueDictionary.GetQueue(patientKey);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(queueType, result);
    }

    [Fact]
    public void GetQueue_ShouldReturnNullForNonExistentPatient()
    {
        // Arrange
        var patientKey = new PatientKey("C9999");

        // Act
        var result = _queueDictionary.GetQueue(patientKey);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Update_ShouldUpdateQueueTypeForPatient()
    {
        // Arrange
        var patientKey = new PatientKey("C0004");
        var initialQueueType = QueueType.Exam;
        var updatedQueueType = QueueType.Medication;
        _queueDictionary.Add(patientKey, initialQueueType);

        // Act
        _queueDictionary.Update(patientKey, updatedQueueType);

        // Assert
        var result = _queueDictionary.GetQueue(patientKey);
        Assert.NotNull(result);
        Assert.Equal(updatedQueueType, result);
    }
}
