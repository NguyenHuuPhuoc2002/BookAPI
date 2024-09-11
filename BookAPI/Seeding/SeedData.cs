using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Helper;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Seeding
{
    public static class SeedData
    {
        public static async Task Seed(DataContext context, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                var result = await roleManager.CreateAsync(adminRole);
            }

            // Kiểm tra nếu vai trò "Customer" chưa tồn tại
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var customerRole = new IdentityRole("Customer");
                var result = await roleManager.CreateAsync(customerRole);

            }
           
            await context.SaveChangesAsync();
        }
    }
}
