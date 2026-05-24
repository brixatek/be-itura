using FluentAssertions;
using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.Features.Auth.Register;
using Itura.Auth.Domain.Repositories;
using Moq;

namespace Itura.Auth.Unit.Tests;

public sealed class RegisterCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<IAuthUnitOfWork> _uow = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IEmailService> _email = new();

    private RegisterCommandHandler CreateHandler() =>
        new(_accountRepo.Object, _uow.Object, _hasher.Object, _email.Object);

    [Fact]
    public async Task Handle_WhenEmailNotTaken_ReturnsSuccessWithNewAccountId()
    {
        _accountRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");

        var command = new RegisterCommand("test@example.com", "Password1!", "Test User", "Africa/Lagos");
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _accountRepo.Verify(r => r.AddAsync(It.IsAny<Itura.Auth.Domain.Entities.Account>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _email.Verify(e => e.SendEmailVerificationAsync(
            "test@example.com", "Test User", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyTaken_ReturnsConflictFailure()
    {
        _accountRepo.Setup(r => r.ExistsByEmailAsync("taken@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterCommand("taken@example.com", "Password1!", "Test User", "UTC");
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.EmailTaken");
        _accountRepo.Verify(r => r.AddAsync(It.IsAny<Itura.Auth.Domain.Entities.Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailNotTaken_HashesPassword()
    {
        _accountRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _hasher.Setup(h => h.Hash("Password1!")).Returns("bcrypt_hash");

        var command = new RegisterCommand("new@example.com", "Password1!", "Jane Doe", "UTC");
        await CreateHandler().Handle(command, CancellationToken.None);

        _hasher.Verify(h => h.Hash("Password1!"), Times.Once);
    }
}
