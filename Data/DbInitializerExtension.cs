using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.Entities;

namespace OnlineBookStoreMVC.Data
{
    public static class DbInitializerExtension
    {
        public static async Task<IApplicationBuilder> UseItToSeedSqlServer(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await ContextSeeder.SeedRolesAsync(userManager, roleManager);
                await ContextSeeder.SeedSuperAdminAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {

            }

            return app;
        }
    }
}
