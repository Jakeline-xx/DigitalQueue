using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalQueue.Controllers;
[ApiController]
[Route("api/v1")]
public class ManagementController : ControllerBase
{
    private readonly IQueueService _queueService;

    public ManagementController(IQueueService queueService)
    {
        _queueService = queueService;
    }

    [HttpPost("patient")]
    public IActionResult CreatePatient([FromQuery] bool priority)
    {
        var result = _queueService.AddToQueue(priority);
        return Ok(new { code = result.Code });
    }

    [HttpGet("{code}")]
    public IActionResult GetPatientStatus(string code)
    {
        var (queueName, peopleAhead, currentCode) = _queueService.GetQueueStatus(code);
        return Ok(new { currentCode, peopleAhead, queueName });
    }

    [HttpGet("queues")]
    public IActionResult GetAllQueues()
    {
        var queues = _queueService.GetAllQueues();
        return Ok(queues);
    }

    [HttpPut("{code}/{queueName}")]
    public IActionResult MovePatient(string code, QueueType queueName)
    {
        _queueService.MoveToNextQueue(code, queueName);
        return NoContent();
    }

    [HttpDelete("{code}")]
    public IActionResult DeletePatient(string code)
    {
        _queueService.RemoveFromQueue(code);
        return NoContent();
    }
}