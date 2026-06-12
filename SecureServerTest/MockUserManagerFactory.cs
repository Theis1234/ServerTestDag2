using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using BlazorAppTest2.Tests.Fakes;

namespace BlazorAppTest2.Tests.Helpers;

/// <summary>
/// Factory for a Moq-based UserManager backed by FakeUserStore.
/// Usage:
///   var (manager, mock) = MockUserManagerFactory.Create();
///   mock.Setup(m => m.CreateAsync(It.IsAny<FakeApplicationUser>(), It.IsAny<string>()))
///       .ReturnsAsync(IdentityResult.Success);
/// </summary>
public static class MockUserManagerFactory
{
    public static (UserManager<FakeApplicationUser> Manager, Mock<UserManager<FakeApplicationUser>> Mock)
        Create(IUserStore<FakeApplicationUser>? store = null)
    {
        store ??= new FakeUserStore();

        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());

        var passwordHasher = new Mock<IPasswordHasher<FakeApplicationUser>>();
        var userValidators = new List<IUserValidator<FakeApplicationUser>>();
        var pwdValidators = new List<IPasswordValidator<FakeApplicationUser>>();
        var keyNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<FakeApplicationUser>>>();

        var mock = new Mock<UserManager<FakeApplicationUser>>(
            store,
            options.Object,
            passwordHasher.Object,
            userValidators,
            pwdValidators,
            keyNormalizer,
            errors,
            services.Object,
            logger.Object)
        {
            CallBase = false
        };

        return (mock.Object, mock);
    }
}
