using Itura.Corporate.Application.Features.Accounts.Commands.AddEmployee;
using Itura.Corporate.Application.Features.Accounts.Commands.CreateAccount;
using Itura.Corporate.Application.Features.Accounts.Commands.RemoveEmployee;
using Itura.Corporate.Application.Features.Accounts.Commands.UpdateAccount;
using Itura.Corporate.Application.Features.Accounts.Queries.GetAccount;
using Itura.Corporate.Application.Features.Accounts.Queries.GetEmployees;
using Itura.Corporate.Application.Features.Accounts.Queries.GetMyAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Corporate.API.Controllers;

[ApiController]
[Route("api/v1/corporate-accounts")]
[Authorize]
public sealed class CorporateAccountsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateAccountCommand(
            req.CompanyName, GetUserId(), req.SubscriptionTier, req.MaxEmployees,
            req.Industry, req.LogoUrl, req.Website), ct);
        if (result.IsFailure) return BadRequest(new { success = false, error = result.Error });
        return CreatedAtAction(nameof(GetAccount), new { id = result.Value.Id }, new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAccount(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAccountQuery(id), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyAccount(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyAccountQuery(GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateAccountCommand(
            id, GetUserId(), req.CompanyName, req.Industry, req.SubscriptionTier,
            req.MaxEmployees, req.LogoUrl, req.Website), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}/employees")]
    public async Task<IActionResult> GetEmployees(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetEmployeesQuery(id, GetUserId(), page, pageSize), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("{id:guid}/employees")]
    public async Task<IActionResult> AddEmployee(Guid id, [FromBody] AddEmployeeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddEmployeeCommand(id, GetUserId(), req.EmployeeUserId, req.Role ?? "Member"), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return StatusCode(201, new { success = true, data = result.Value });
    }

    [HttpDelete("{accountId:guid}/employees/{employeeId:guid}")]
    public async Task<IActionResult> RemoveEmployee(Guid accountId, Guid employeeId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveEmployeeCommand(employeeId, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return NoContent();
    }
}

public sealed record CreateAccountRequest(string CompanyName, string SubscriptionTier, int MaxEmployees, string? Industry, string? LogoUrl, string? Website);
public sealed record UpdateAccountRequest(string CompanyName, string? Industry, string SubscriptionTier, int MaxEmployees, string? LogoUrl, string? Website);
public sealed record AddEmployeeRequest(Guid EmployeeUserId, string? Role);
