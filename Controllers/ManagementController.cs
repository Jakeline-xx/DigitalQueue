using DigitalQueue.Domain.Enums;
using DigitalQueue.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitalQueue.Controllers;
[ApiController]
[Route("api/v1")]
public class ManagementController : ControllerBase
{
    private readonly IQueueService _queueService;

    public ManagementController(IQueueService queueService)
    {
        _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
    }

    [HttpPost("patient")]
    public IActionResult CreatePatient([FromQuery] bool priority)
    {
        var result = _queueService.AddToQueue(priority);
        return Ok(new { code = result.Code });
    }

    [HttpGet("{code}")]
    public IActionResult GetPatientStatus([Required] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { error = "Code cannot be null or empty." });
        }

        try
        {
            var (queueName, peopleAhead, currentCode) = _queueService.GetQueueStatus(code);
            return Ok(new { currentCode, peopleAhead, queueName });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An unexpected error occurred.", detail = ex.Message });
        }
    }

    [HttpGet("queues")]
    public IActionResult GetAllQueues()
    {
        var queues = _queueService.GetAllQueues();
        return Ok(queues);
    }

    [HttpPut("{code}/{queueName}")]
    public IActionResult MovePatient([Required] string code, QueueType queueName)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { error = "Code cannot be null or empty." });
        }

        _queueService.MoveToNextQueue(code, queueName);
        return NoContent();
    }

    [HttpDelete("{code}")]
    public IActionResult DeletePatient([Required] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { error = "Code cannot be null or empty." });
        }

        _queueService.RemoveFromQueue(code);
        return NoContent();
    }
}