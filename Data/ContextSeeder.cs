using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Enums;

namespace OnlineBookStoreMVC.Data
{
    public class ContextSeeder
    {
        public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Role.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.User.ToString()));
        }
        public static async Task SeedSuperAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Abiola",
                FullName = "Ogunride Abd.Muheez",
                Email = "abiola@gmail.com",
                PhoneNumber = "09062380583",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Gender = "Male",
            };


            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin@123");
                await userManager.AddToRoleAsync(defaultUser, Role.User.ToString());
                await userManager.AddToRoleAsync(defaultUser, Role.Admin.ToString());
                await userManager.AddToRoleAsync(defaultUser, Role.SuperAdmin.ToString());
            }
        }
    }
}
