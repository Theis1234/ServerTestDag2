using BlazorAppTest2.Components;
using BlazorAppTest2.Components.Account;
using BlazorAppTest2.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace BlazorAppTest2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();

			builder.Services.AddCascadingAuthenticationState();
			builder.Services.AddScoped<IdentityRedirectManager>();
			builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
			
			
			builder.Services.AddAuthentication(options =>
				{
					options.DefaultScheme = IdentityConstants.ApplicationScheme;
					options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
				})
				.AddIdentityCookies();

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddIdentityCore<ApplicationUser>(options =>
				{
					options.SignIn.RequireConfirmedAccount = true;
					options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
				})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddSignInManager()
				.AddDefaultTokenProviders();

			builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
				options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
			});

			var kestrelCertPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

			kestrelCertPath = Path.Combine(kestrelCertPath, ".aspnet" ,"https", "TheisTest.pfx");
			string kestrelCertPassword = "Theis@1";

			builder.WebHost.ConfigureKestrel(options =>
			{
				options.ListenAnyIP(7067, listenOptions =>
				{
					var cert = new X509Certificate2(kestrelCertPath, kestrelCertPassword);

					listenOptions.UseHttps(httpOptions =>
					{
						httpOptions.ServerCertificate = cert;
						httpOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
					});
				});
				options.ListenAnyIP(5286);
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
			app.UseHttpsRedirection();

			app.UseAntiforgery();

			app.MapStaticAssets();
			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

			// Add additional endpoints required by the Identity /Account Razor components.
			app.MapAdditionalIdentityEndpoints();

			app.Run();
		}
	}
}
