using Microsoft.AspNetCore.Identity;

namespace BlazorAppTest2.Data
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		public string? CprNumber { get; set; }
	}

}
