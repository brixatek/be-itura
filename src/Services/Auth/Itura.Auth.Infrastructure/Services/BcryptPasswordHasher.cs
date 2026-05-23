using Itura.Auth.Application.Common.Interfaces;

namespace Itura.Auth.Infrastructure.Services;

internal sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
