using Itura.SharedKernel.Entities;

namespace Itura.Payment.Domain.Entities;

public sealed class CoachBankAccount : AuditableEntity
{
    public Guid CoachUserId { get; private set; }
    public string BankCodeEncrypted { get; private set; } = string.Empty;
    public string AccountNumberEncrypted { get; private set; } = string.Empty;
    public string AccountNameEncrypted { get; private set; } = string.Empty;
    public string BankName { get; private set; } = string.Empty;
    public bool IsVerified { get; private set; }

    private CoachBankAccount() { }

    public static CoachBankAccount Create(
        Guid coachUserId,
        string bankCodeEncrypted,
        string accountNumberEncrypted,
        string accountNameEncrypted,
        string bankName)
    {
        return new CoachBankAccount
        {
            Id = Guid.NewGuid(),
            CoachUserId = coachUserId,
            BankCodeEncrypted = bankCodeEncrypted,
            AccountNumberEncrypted = accountNumberEncrypted,
            AccountNameEncrypted = accountNameEncrypted,
            BankName = bankName,
            IsVerified = false
        };
    }

    public void MarkVerified() { IsVerified = true; MarkUpdated(); }

    public void UpdateDetails(
        string bankCodeEncrypted, string accountNumberEncrypted,
        string accountNameEncrypted, string bankName)
    {
        BankCodeEncrypted = bankCodeEncrypted;
        AccountNumberEncrypted = accountNumberEncrypted;
        AccountNameEncrypted = accountNameEncrypted;
        BankName = bankName;
        IsVerified = false;
        MarkUpdated();
    }
}
