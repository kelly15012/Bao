using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bao.Data;
using Bao.Areas.Identity.Data;

namespace Bao
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("BaoContextConnection") ?? throw new InvalidOperationException("Connection string 'BaoContextConnection' not found.");

            builder.Services.AddDbContext<BaoContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<BaoUser>(options => options.SignIn.RequireConfirmedAccount = false) // Remember to change this to true in production
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BaoContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Initialize roles and seed manager user
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await InitializeRolesAsync(roleManager);

                    var userManager = services.GetRequiredService<UserManager<BaoUser>>();
                    await SeedManagerUserAsync(userManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        private static async Task InitializeRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Baozi", "Admin", "Manager" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedManagerUserAsync(UserManager<BaoUser> userManager)
        {
            string email = "manager@bao.com";
            string password = "Manager@1234";

            // Ensure the user is created only after roles have been seeded
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new BaoUser
                {
                    UserName = email,
                    Email = email
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Manager");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
        }
    }
}
