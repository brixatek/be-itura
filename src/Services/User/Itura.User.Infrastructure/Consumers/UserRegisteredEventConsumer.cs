using Itura.Contracts.Auth;
using Itura.User.Application.Features.Users.CreateProfile;
using MassTransit;
using MediatR;

namespace Itura.User.Infrastructure.Consumers;

public sealed class UserRegisteredEventConsumer(ISender sender) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var msg = context.Message;
        var command = new CreateUserProfileCommand(msg.AccountId, msg.Email, msg.FullName, msg.Timezone);
        await sender.Send(command, context.CancellationToken);
    }
}
