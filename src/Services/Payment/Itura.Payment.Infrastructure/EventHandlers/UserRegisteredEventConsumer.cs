using Itura.Contracts.Auth;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.Payment.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Itura.Payment.Infrastructure.EventHandlers;

public sealed class UserRegisteredEventConsumer(
    PaymentDbContext dbContext,
    ILogger<UserRegisteredEventConsumer> logger) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var msg = context.Message;

        // Idempotency check — wallet may already exist if message is redelivered
        var exists = await dbContext.Wallets
            .AnyAsync(w => w.UserId == msg.AccountId, context.CancellationToken);

        if (exists)
        {
            logger.LogInformation("Wallet already exists for user {UserId}, skipping creation", msg.AccountId);
            return;
        }

        var wallet = Wallet.Create(msg.AccountId);
        await dbContext.Wallets.AddAsync(wallet, context.CancellationToken);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Wallet created for user {UserId}", msg.AccountId);
    }
}
