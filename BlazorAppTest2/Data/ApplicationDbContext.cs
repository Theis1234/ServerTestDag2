using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static BlazorAppTest2.Models.PasskeyModels;

namespace BlazorAppTest2.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
	{
		public DbSet<PasskeyCredential> PasskeyCredentials => Set<PasskeyCredential>();
	}
}
