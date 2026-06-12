using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using BlazorAppTest2.Tests.Fakes;

namespace BlazorAppTest2.Tests.Helpers;

/// <summary>
/// Factory for a Moq-based SignInManager that can be configured per test.
/// Usage:
///   var (manager, mock) = MockSignInManagerFactory.Create();
///   mock.Setup(m => m.PasswordSignInAsync(...)).ReturnsAsync(SignInResult.Success);
/// </summary>
public static class MockSignInManagerFactory
{
    public static (SignInManager<FakeApplicationUser> Manager, Mock<SignInManager<FakeApplicationUser>> Mock)
        Create(UserManager<FakeApplicationUser>? userManager = null)
    {
        userManager ??= MockUserManagerFactory.Create().Manager;

        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<FakeApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());

        var logger = new Mock<ILogger<SignInManager<FakeApplicationUser>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<FakeApplicationUser>>();

        var mock = new Mock<SignInManager<FakeApplicationUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object,
            confirmation.Object)
        {
            CallBase = false
        };

        return (mock.Object, mock);
    }
}
