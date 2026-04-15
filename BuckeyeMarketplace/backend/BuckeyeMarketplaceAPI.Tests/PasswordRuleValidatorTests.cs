using BuckeyeMarketplaceAPI.Models;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Pure unit tests for PasswordRuleValidator.
/// No database. No HTTP. Just new PasswordRuleValidator() and Assert.
/// Default rules mirror Program.cs Identity settings:
///   min length 8, digit, uppercase, lowercase, no special char required.
/// </summary>
public class PasswordRuleValidatorTests
{
    private readonly PasswordRuleValidator _validator = new();

    // ───────────────────────────────────────────────────────────────
    // Valid passwords
    // ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Abcdefg1")]        // exactly 8 chars, meets all rules
    [InlineData("Password1")]       // classic valid password
    [InlineData("LongerPassword99")] // well above minimum length
    [InlineData("Test1234")]        // uppercase + lowercase + digits
    [InlineData("aB3!@#xyz")]       // special chars allowed (just not required)
    public void ValidPasswords_ReturnNoErrors(string password)
    {
        var errors = _validator.Validate(password);

        Assert.Empty(errors);
        Assert.True(_validator.IsValid(password));
    }

    // ───────────────────────────────────────────────────────────────
    // Null / empty
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void NullPassword_ReturnsRequiredError()
    {
        var errors = _validator.Validate(null);

        Assert.Single(errors);
        Assert.Contains("required", errors[0], StringComparison.OrdinalIgnoreCase);
        Assert.False(_validator.IsValid(null));
    }

    [Fact]
    public void EmptyPassword_ReturnsRequiredError()
    {
        var errors = _validator.Validate("");

        Assert.Single(errors);
        Assert.Contains("required", errors[0], StringComparison.OrdinalIgnoreCase);
        Assert.False(_validator.IsValid(""));
    }

    // ───────────────────────────────────────────────────────────────
    // Too short
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void TooShort_ReturnsLengthError()
    {
        var errors = _validator.Validate("Ab1");

        Assert.Contains(errors, e => e.Contains("at least 8 characters"));
        Assert.False(_validator.IsValid("Ab1"));
    }

    [Fact]
    public void ExactlyMinLength_IsAccepted()
    {
        // 8 chars: upper + lower + digit
        Assert.True(_validator.IsValid("Abcdefg1"));
    }

    [Fact]
    public void OneCharBelowMinLength_IsRejected()
    {
        // 7 chars: upper + lower + digit
        Assert.False(_validator.IsValid("Abcdef1"));
    }

    // ───────────────────────────────────────────────────────────────
    // Missing digit
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void NoDigit_ReturnsDigitError()
    {
        var errors = _validator.Validate("Abcdefgh");

        Assert.Contains(errors, e => e.Contains("digit"));
        Assert.False(_validator.IsValid("Abcdefgh"));
    }

    // ───────────────────────────────────────────────────────────────
    // Missing uppercase
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void NoUppercase_ReturnsUppercaseError()
    {
        var errors = _validator.Validate("abcdefg1");

        Assert.Contains(errors, e => e.Contains("uppercase"));
        Assert.False(_validator.IsValid("abcdefg1"));
    }

    // ───────────────────────────────────────────────────────────────
    // Missing lowercase
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void NoLowercase_ReturnsLowercaseError()
    {
        var errors = _validator.Validate("ABCDEFG1");

        Assert.Contains(errors, e => e.Contains("lowercase"));
        Assert.False(_validator.IsValid("ABCDEFG1"));
    }

    // ───────────────────────────────────────────────────────────────
    // Multiple violations at once
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void MultipleViolations_ReturnsAllErrors()
    {
        // "abc" → too short, no digit, no uppercase  (3 errors)
        var errors = _validator.Validate("abc");

        Assert.Equal(3, errors.Count);
        Assert.Contains(errors, e => e.Contains("at least 8 characters"));
        Assert.Contains(errors, e => e.Contains("digit"));
        Assert.Contains(errors, e => e.Contains("uppercase"));
    }

    [Fact]
    public void AllLowerNDigitMissing_ReturnsTwoErrors()
    {
        // "abcdefgh" → no digit, meets length & lowercase, but no uppercase
        // actually missing digit + uppercase = 2 errors
        var errors = _validator.Validate("abcdefgh");

        Assert.Equal(2, errors.Count);
        Assert.Contains(errors, e => e.Contains("digit"));
        Assert.Contains(errors, e => e.Contains("uppercase"));
    }

    // ───────────────────────────────────────────────────────────────
    // Non-alphanumeric not required by default
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void SpecialCharsNotRequired_ByDefault()
    {
        // Valid password with no special chars — should pass
        Assert.True(_validator.IsValid("Password1"));
    }

    // ───────────────────────────────────────────────────────────────
    // Custom constructor: require non-alphanumeric
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void RequireNonAlphanumeric_WhenEnabled_RejectsWithoutSpecialChar()
    {
        var strict = new PasswordRuleValidator(requireNonAlphanumeric: true);

        Assert.False(strict.IsValid("Password1"));
        var errors = strict.Validate("Password1");
        Assert.Contains(errors, e => e.Contains("non-alphanumeric"));
    }

    [Fact]
    public void RequireNonAlphanumeric_WhenEnabled_AcceptsWithSpecialChar()
    {
        var strict = new PasswordRuleValidator(requireNonAlphanumeric: true);

        Assert.True(strict.IsValid("Password1!"));
    }

    // ───────────────────────────────────────────────────────────────
    // Custom constructor: shorter min length
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void CustomMinLength_RespectsOverride()
    {
        var relaxed = new PasswordRuleValidator(requiredLength: 4);

        // "Ab1x" → 4 chars, has upper, lower, digit
        Assert.True(relaxed.IsValid("Ab1x"));
        // "Ab1" → only 3 chars
        Assert.False(relaxed.IsValid("Ab1"));
    }
}
