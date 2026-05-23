using Itura.Booking.Application.Features.Bookings.CancelBooking;
using Itura.Booking.Application.Features.Bookings.CompleteBooking;
using Itura.Booking.Application.Features.Bookings.ConfirmBooking;
using Itura.Booking.Application.Features.Bookings.CreateBooking;
using Itura.Booking.Application.Features.Bookings.GetBooking;
using Itura.Booking.Application.Features.Bookings.GetCoachBookings;
using Itura.Booking.Application.Features.Bookings.GetMyBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Booking.API.Controllers;

[ApiController]
[Route("api/v1/bookings")]
[Authorize]
public sealed class BookingsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var command = new CreateBookingCommand(
            GetUserId(), request.CoachId, request.CoachUserId,
            request.ScheduledAt, request.DurationMinutes,
            request.Price, request.Currency);

        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });

        return CreatedAtAction(nameof(GetBooking), new { id = result.Value },
            new { success = true, data = new { id = result.Value } });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBooking(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetBookingQuery(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyBookings([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetMyBookingsQuery(GetUserId(), page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("coach")]
    public async Task<IActionResult> GetCoachBookings([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetCoachBookingsQuery(GetUserId(), page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmBooking(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ConfirmBookingCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id, [FromBody] CancelBookingRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelBookingCommand(id, GetUserId(), request.Reason), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> CompleteBooking(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new CompleteBookingCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }
}

public sealed record CreateBookingRequest(
    Guid CoachId,
    Guid CoachUserId,
    DateTime ScheduledAt,
    int DurationMinutes,
    decimal Price,
    string Currency);

public sealed record CancelBookingRequest(string? Reason);
