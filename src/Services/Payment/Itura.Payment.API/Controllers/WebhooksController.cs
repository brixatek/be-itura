using Itura.Payment.Application.Features.Payments.Paystack;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itura.Payment.API.Controllers;

[ApiController]
[Route("api/v1/webhooks")]
[AllowAnonymous]
public sealed class WebhooksController(IMediator mediator) : ControllerBase
{
    [HttpPost("paystack")]
    public async Task<IActionResult> Paystack(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(ct);
        var signature = Request.Headers["X-Paystack-Signature"].FirstOrDefault() ?? string.Empty;

        var result = await mediator.Send(new HandlePaystackWebhookCommand(payload, signature), ct);
        return result.IsSuccess ? Ok() : BadRequest();
    }
}
