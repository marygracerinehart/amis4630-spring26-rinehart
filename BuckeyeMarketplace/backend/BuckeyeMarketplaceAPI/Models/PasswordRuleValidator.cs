namespace BuckeyeMarketplaceAPI.Models;

/// <summary>
/// Pure-logic password rule validator.
/// Mirrors the Identity PasswordOptions configured in Program.cs:
///   RequiredLength  = 8
///   RequireDigit    = true
///   RequireUppercase = true
///   RequireLowercase = true
///   RequireNonAlphanumeric = false
/// No database, no Identity, no I/O — just string inspection.
/// </summary>
public class PasswordRuleValidator
{
    public int RequiredLength { get; }
    public bool RequireDigit { get; }
    public bool RequireUppercase { get; }
    public bool RequireLowercase { get; }
    public bool RequireNonAlphanumeric { get; }

    public PasswordRuleValidator(
        int requiredLength = 8,
        bool requireDigit = true,
        bool requireUppercase = true,
        bool requireLowercase = true,
        bool requireNonAlphanumeric = false)
    {
        RequiredLength = requiredLength;
        RequireDigit = requireDigit;
        RequireUppercase = requireUppercase;
        RequireLowercase = requireLowercase;
        RequireNonAlphanumeric = requireNonAlphanumeric;
    }

    /// <summary>
    /// Validates the password and returns a list of human-readable errors.
    /// An empty list means the password is valid.
    /// </summary>
    public List<string> Validate(string? password)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(password))
        {
            errors.Add("Password is required.");
            return errors;
        }

        if (password.Length < RequiredLength)
            errors.Add($"Password must be at least {RequiredLength} characters long.");

        if (RequireDigit && !password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit.");

        if (RequireUppercase && !password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter.");

        if (RequireLowercase && !password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter.");

        if (RequireNonAlphanumeric && password.All(char.IsLetterOrDigit))
            errors.Add("Password must contain at least one non-alphanumeric character.");

        return errors;
    }

    /// <summary>
    /// Convenience: returns true when the password satisfies every rule.
    /// </summary>
    public bool IsValid(string? password) => Validate(password).Count == 0;
}
