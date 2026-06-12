using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using BlazorAppTest2.Tests.Fakes;

namespace BlazorAppTest2.Tests.Tests;

/// <summary>
/// Core authentication tests covering password hashing, user creation,
/// sign-in outcomes, account lockout, 2FA, and input validation.
/// </summary>
public class AuthTests
{
    private readonly UserManager<FakeApplicationUser> _userManager;
    private readonly Mock<UserManager<FakeApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<FakeApplicationUser>> _signInMock;

    public AuthTests()
    {
        (_userManager, _userManagerMock) = MockUserManagerFactory.Create();
        (_, _signInMock) = MockSignInManagerFactory.Create(_userManager);
    }

    [Fact]
    public void PasswordHasher_HashAndVerify_Succeeds()
    {
        var hasher = new PasswordHasher<FakeApplicationUser>();
        var user = new FakeApplicationUser();
        const string plaintext = "MySecret123!";

        var hash = hasher.HashPassword(user, plaintext);
        var result = hasher.VerifyHashedPassword(user, hash, plaintext);

        result.Should().Be(PasswordVerificationResult.Success);
    }

    [Fact]
    public void PasswordHasher_WrongPassword_FailsVerification()
    {
        var hasher = new PasswordHasher<FakeApplicationUser>();
        var user = new FakeApplicationUser();

        var hash = hasher.HashPassword(user, "CorrectPassword1!");
        var result = hasher.VerifyHashedPassword(user, hash, "WrongPassword1!");

        result.Should().Be(PasswordVerificationResult.Failed);
    }

    [Fact]
    public async Task CreateUser_WithValidData_Succeeds()
    {
        var user = new FakeApplicationUser { Email = "new@example.com", UserName = "new@example.com" };
        _userManagerMock
            .Setup(m => m.CreateAsync(user, "ValidPass1!"))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _userManager.CreateAsync(user, "ValidPass1!");

        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsIdentityError()
    {
    
        var user = new FakeApplicationUser { Email = "taken@example.com", UserName = "taken@example.com" };
        _userManagerMock
            .Setup(m => m.CreateAsync(user, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Code = "DuplicateUserName", Description = "Email is already taken." }));

        var result = await _userManager.CreateAsync(user, "ValidPass1!");

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Code == "DuplicateUserName");
    }


    [Fact]
    public async Task PasswordSignIn_ValidCredentials_ReturnsSuccess()
    {
        _signInMock
            .Setup(m => m.PasswordSignInAsync("user@example.com", "pass", false, false))
            .ReturnsAsync(SignInResult.Success);

        var result = await _signInMock.Object.PasswordSignInAsync("user@example.com", "pass", false, false);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
	[Trait("Setup", "SETUP-10")]
	public async Task PasswordSignIn_InvalidCredentials_ReturnsFailure()
    {
        _signInMock
            .Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), "wrong", It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _signInMock.Object.PasswordSignInAsync("user@example.com", "wrong", false, false);

        result.Succeeded.Should().BeFalse();
        result.IsLockedOut.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
    }

    [Fact]
    public async Task PasswordSignIn_LockedOutAccount_ReturnsLockedOut()
    {
        _signInMock
            .Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.LockedOut);

        var result = await _signInMock.Object.PasswordSignInAsync("user@example.com", "pass", false, false);

        result.IsLockedOut.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task PasswordSignIn_2faRequired_ReturnsRequiresTwoFactor()
    {
        _signInMock
            .Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.TwoFactorRequired);

        var result = await _signInMock.Object.PasswordSignInAsync("user@example.com", "pass", false, false);

        result.RequiresTwoFactor.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task TwoFactorSignIn_ValidCode_Succeeds()
    {
        var sanitised = "123 456".Replace(" ", "").Replace("-", "");

        _signInMock
            .Setup(m => m.TwoFactorAuthenticatorSignInAsync(sanitised, false, false))
            .ReturnsAsync(SignInResult.Success);

        var result = await _signInMock.Object.TwoFactorAuthenticatorSignInAsync(sanitised, false, false);

        result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    [InlineData("missing@")]
    public void Login_InvalidEmail_FailsDataAnnotationValidation(string email)
    {
        var model = new LoginInputStub { Email = email, Password = "Valid1!" };
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);

        results.Should().NotBeEmpty("'{0}' is not a valid email", email);
    }

    private class LoginInputStub
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
