using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Services;
using System.Text.Json;

namespace ShopOnline.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayPalWebhookController(IPaymentService paymentService, ILogger<PayPalWebhookController> logger) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ILogger<PayPalWebhookController> _logger = logger;

    /// <summary>
    /// Receives PayPal webhook events.
    /// This endpoint must be publicly accessible.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> HandleWebhook(
        [FromBody] JsonElement payload,
        [FromHeader(Name = "PayPal-Transmission-Event-Type")]
        string eventType)
    {
        if (string.IsNullOrEmpty(eventType))
            return BadRequest("Missing PayPal event type.");

        try
        {
            await _paymentService
                .HandlePayPalWebhookAsync(eventType, payload);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing PayPal webhook. EventType: {EventType}",
                eventType
            );

            // PayPal will retry if non-2xx is returned
            return StatusCode(500);
        }
    }
}