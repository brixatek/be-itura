using FluentAssertions;
using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.Features.Auth.Login;
using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Moq;

namespace Itura.Auth.Unit.Tests;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<IRefreshTokenRepository> _refreshRepo = new();
    private readonly Mock<IAuthAuditLogRepository> _auditRepo = new();
    private readonly Mock<IAuthUnitOfWork> _uow = new();
    private readonly Mock<IJwtService> _jwt = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private LoginCommandHandler CreateHandler() =>
        new(_accountRepo.Object, _refreshRepo.Object, _auditRepo.Object,
            _uow.Object, _jwt.Object, _hasher.Object);

    private Account CreateActiveAccount(string email = "user@example.com") =>
        Account.Create(email, "hashed", "verify_token", "Test User", "UTC").Value;

    [Fact]
    public async Task Handle_WhenAccountNotFound_ReturnsUnauthorized()
    {
        _accountRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        var command = new LoginCommand("missing@example.com", "Password1!", null, null);
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Unauthorized");
    }

    [Fact]
    public async Task Handle_WhenPasswordWrong_ReturnsUnauthorized()
    {
        var account = CreateActiveAccount();
        _accountRepo.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _hasher.Setup(h => h.Verify("wrong", account.PasswordHash)).Returns(false);

        var command = new LoginCommand("user@example.com", "wrong", null, null);
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Unauthorized");
    }

    [Fact]
    public async Task Handle_WhenCredentialsValid_ReturnsTokens()
    {
        var account = CreateActiveAccount();
        _accountRepo.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _hasher.Setup(h => h.Verify("Password1!", account.PasswordHash)).Returns(true);
        _jwt.Setup(j => j.GenerateAccessToken(account.Id, account.Email, It.IsAny<string>(), It.IsAny<string>()))
            .Returns("access_token");
        _jwt.Setup(j => j.GenerateRefreshToken())
            .Returns(("raw_token", "hashed_token"));

        var command = new LoginCommand("user@example.com", "Password1!", "device", "127.0.0.1");
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Response.Should().NotBeNull();
        result.Value.Response!.Tokens.AccessToken.Should().Be("access_token");
        result.Value.Response!.Tokens.RefreshToken.Should().Be("raw_token");
        result.Value.MfaChallenge.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenCredentialsValid_SavesRefreshTokenAndAuditLog()
    {
        var account = CreateActiveAccount();
        _accountRepo.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _hasher.Setup(h => h.Verify("Password1!", account.PasswordHash)).Returns(true);
        _jwt.Setup(j => j.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("access");
        _jwt.Setup(j => j.GenerateRefreshToken()).Returns(("raw", "hash"));

        await CreateHandler().Handle(
            new LoginCommand("user@example.com", "Password1!", null, null), CancellationToken.None);

        _refreshRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditRepo.Verify(r => r.AddAsync(It.IsAny<AuthAuditLog>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
