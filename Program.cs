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

            builder.Services.AddDefaultIdentity<BaoUser>(options => options.SignIn.RequireConfirmedAccount = false) //remember to change this to true
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BaoContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<BaoUser>>();

                string email = "manager@bao.com";
                string password = "Manager@1234";

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new BaoUser { UserName = email, Email = email };
                    //user.EmailConfirmed = true;
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Manager");
                }
            }

            // Seed roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedRoles(roleManager);
            }

            app.Run();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Baozi", "Admin", "Manager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
