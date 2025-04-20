using DigitalQueue.Controllers;
using DigitalQueue.Domain.Entities;
using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DigitalQueue.Tests;

public class ManagementControllerTests
{
    private readonly Mock<IQueueService> _queueServiceMock;
    private readonly ManagementController _controller;

    public ManagementControllerTests()
    {
        _queueServiceMock = new Mock<IQueueService>();
        _controller = new ManagementController(_queueServiceMock.Object);
    }

    [Fact]
    public void CreatePatient_ShouldReturnOkWithCode()
    {
        // Arrange
        var patientCode = new PatientCode("P0001");
        _queueServiceMock.Setup(s => s.AddToQueue(true)).Returns(patientCode);

        // Act
        var result = _controller.CreatePatient(true) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(new { code = "P0001" }, result.Value);
    }

    [Fact]
    public void GetPatientStatus_ShouldReturnOkWithStatus()
    {
        // Arrange
        var code = "P0001";
        _queueServiceMock.Setup(s => s.GetQueueStatus(code))
            .Returns(("Reception", 2, "P0001"));

        // Act
        var result = _controller.GetPatientStatus(code) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(new { currentCode = "P0001", peopleAhead = 2, queueName = "Reception" }, result.Value);
    }

    [Fact]
    public void GetPatientStatus_ShouldReturnNotFound_WhenCodeNotFound()
    {
        // Arrange
        var code = "P0001";
        _queueServiceMock.Setup(s => s.GetQueueStatus(code))
            .Throws(new KeyNotFoundException("Code not found in any queue."));

        // Act
        var result = _controller.GetPatientStatus(code) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal(new { error = "Code not found in any queue." }, result.Value);
    }

    [Fact]
    public void GetAllQueues_ShouldReturnOkWithQueues()
    {
        // Arrange
        var queues = new Dictionary<string, List<string>>
           {
               { "Reception", new List<string> { "P0001", "C0001" } }
           };
        _queueServiceMock.Setup(s => s.GetAllQueues()).Returns(queues);

        // Act
        var result = _controller.GetAllQueues() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(queues, result.Value);
    }

    [Fact]
    public void MovePatient_ShouldReturnNoContent()
    {
        // Arrange
        var code = "P0001";
        var queueName = QueueType.Screening;

        // Act
        var result = _controller.MovePatient(code, queueName) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
        _queueServiceMock.Verify(s => s.MoveToNextQueue(code, queueName), Times.Once);
    }

    [Fact]
    public void DeletePatient_ShouldReturnNoContent()
    {
        // Arrange
        var code = "P0001";

        // Act
        var result = _controller.DeletePatient(code) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
        _queueServiceMock.Verify(s => s.RemoveFromQueue(code), Times.Once);
    }
}
