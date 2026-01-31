using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Services;
using ShopOnline.Common;

namespace ShopOnline.Api.Controllers;

/// <summary>
/// This controller receives asynchronous events from PayPal and ensures payment state consistency even if the user flow is interrupted.
/// </summary>

[ApiController]
[Route("api/[controller]")]
[Authorize]                             
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
    }

    /// <summary>
    /// Creates a PayPal payment and returns the approval URL.
    /// </summary>
    [HttpPost("paypal/create")]
    public async Task<IActionResult> CreatePayPalPayment([FromBody] CreatePaymentDto request)
    {
        if (request == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _paymentService.CreatePaymentAsync(request, userId);

        // Return 201 Created with a Location header pointing to the GET endpoint
        return CreatedAtAction(nameof(GetPayment), new { id = result.PaymentId }, result);
    }

    /// <summary>
    /// Captures a PayPal payment after user approval.
    /// Usually triggered by frontend after redirect.
    /// </summary>
    [HttpPost("paypal/capture")]
    public async Task<IActionResult> CapturePayPalPayment([FromQuery] string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("token is required");

        await _paymentService.CapturePaymentAsync(token);
        return NoContent();
    }

    /// <summary>
    /// PayPal webhook endpoint.
    /// Webhooks must be callable without authentication; validate signature inside service/middleware
    /// </summary>
    [AllowAnonymous]
    [HttpPost("paypal/webhook")]
    public async Task<IActionResult> PayPalWebhook([FromBody] JsonElement payload, [FromHeader(Name = "PayPal-Transmission-Sig")] string? signature = null)
    {
        if (payload.ValueKind == JsonValueKind.Undefined || payload.ValueKind == JsonValueKind.Null)
            return BadRequest();

        // Try extracting the event type from the payload. Service should re-validate and be idempotent.
        var eventType = payload.TryGetProperty("event_type", out var p) && p.ValueKind == JsonValueKind.String
            ? p.GetString()
            : null;

        if (string.IsNullOrEmpty(eventType))
            return BadRequest("missing event_type");

        await _paymentService.HandlePayPalWebhookAsync(eventType, payload);
        return Ok();
    }

    /// <summary>
    /// Retrieves payment details by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        if (payment == null)
            return NotFound();

        return Ok(payment);
    }
}
