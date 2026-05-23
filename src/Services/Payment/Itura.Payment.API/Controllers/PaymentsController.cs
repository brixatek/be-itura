using Itura.Payment.Application.Features.Payments.GetMyPayments;
using Itura.Payment.Application.Features.Payments.GetPayment;
using Itura.Payment.Application.Features.Payments.GetPaymentByBooking;
using Itura.Payment.Application.Features.Payments.InitiatePayment;
using Itura.Payment.Application.Features.Payments.ProcessPayment;
using Itura.Payment.Application.Features.Payments.RefundPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Payment.API.Controllers;

[ApiController]
[Route("api/v1/payments")]
[Authorize]
public sealed class PaymentsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest request, CancellationToken ct)
    {
        var command = new InitiatePaymentCommand(
            GetUserId(), request.PayeeUserId, request.BookingId,
            request.Amount, request.Currency, request.PaymentMethod);

        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });

        return CreatedAtAction(nameof(GetPayment), new { id = result.Value },
            new { success = true, data = new { id = result.Value } });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPaymentQuery(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("booking/{bookingId:guid}")]
    public async Task<IActionResult> GetPaymentByBooking(Guid bookingId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPaymentByBookingQuery(bookingId, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetMyPaymentsQuery(GetUserId(), page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}/process")]
    public async Task<IActionResult> ProcessPayment(Guid id, [FromBody] ProcessPaymentRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new ProcessPaymentCommand(
            id, GetUserId(), request.Succeed, request.TransactionReference, request.FailureReason), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpPut("{id:guid}/refund")]
    public async Task<IActionResult> RefundPayment(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new RefundPaymentCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }
}

public sealed record InitiatePaymentRequest(
    Guid PayeeUserId,
    Guid BookingId,
    decimal Amount,
    string Currency,
    string? PaymentMethod);

public sealed record ProcessPaymentRequest(
    bool Succeed,
    string? TransactionReference,
    string? FailureReason);
