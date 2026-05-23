using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.GetPaymentByBooking;

public sealed record GetPaymentByBookingQuery(Guid BookingId, Guid UserId) : IRequest<Result<PaymentRecordDto>>;
