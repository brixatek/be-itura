using Itura.Corporate.Domain.Enums;
using Itura.Corporate.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Corporate.Domain.Entities;

public sealed class CorporateAccount : AggregateRoot
{
    public string CompanyName { get; private set; } = string.Empty;
    public Guid AdminUserId { get; private set; }
    public string? Industry { get; private set; }
    public SubscriptionTier SubscriptionTier { get; private set; }
    public int MaxEmployees { get; private set; }
    public bool IsActive { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Website { get; private set; }

    private CorporateAccount() { }

    public static Result<CorporateAccount> Create(
        string companyName,
        Guid adminUserId,
        SubscriptionTier tier,
        int maxEmployees,
        string? industry = null,
        string? logoUrl = null,
        string? website = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return Result.Failure<CorporateAccount>(Error.Validation("CorporateAccount.CompanyName", "Company name is required."));
        if (adminUserId == Guid.Empty)
            return Result.Failure<CorporateAccount>(Error.Validation("CorporateAccount.AdminUserId", "Admin user is required."));
        if (maxEmployees <= 0)
            return Result.Failure<CorporateAccount>(Error.Validation("CorporateAccount.MaxEmployees", "Max employees must be greater than zero."));

        var account = new CorporateAccount
        {
            CompanyName = companyName.Trim(),
            AdminUserId = adminUserId,
            SubscriptionTier = tier,
            MaxEmployees = maxEmployees,
            Industry = industry,
            LogoUrl = logoUrl,
            Website = website,
            IsActive = true
        };

        account.RaiseDomainEvent(new CorporateAccountCreatedDomainEvent(
            account.Id, account.CompanyName, account.AdminUserId, account.SubscriptionTier));

        return account;
    }

    public Result Update(string companyName, string? industry, SubscriptionTier tier, int maxEmployees, string? logoUrl, string? website)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return Result.Failure(Error.Validation("CorporateAccount.CompanyName", "Company name is required."));

        CompanyName = companyName.Trim();
        Industry = industry;
        SubscriptionTier = tier;
        MaxEmployees = maxEmployees;
        LogoUrl = logoUrl;
        Website = website;
        return Result.Success();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
