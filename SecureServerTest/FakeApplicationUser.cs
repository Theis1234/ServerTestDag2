using Microsoft.AspNetCore.Identity;

namespace BlazorAppTest2.Tests.Fakes;

/// <summary>
/// Stand-in for BlazorAppTest2.Data.ApplicationUser used in tests.
/// Once you add a project reference to your main app, replace this class
/// with a using alias:
///   using FakeApplicationUser = BlazorAppTest2.Data.ApplicationUser;
/// and delete this file.
/// </summary>
public class FakeApplicationUser : IdentityUser
{
    public bool TwoFactorEnabled { get; set; }
}
