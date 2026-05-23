using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.Paystack;

public sealed record HandlePaystackWebhookCommand(
    string Payload,
    string Signature) : IRequest<Result>;
