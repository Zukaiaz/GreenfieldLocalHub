using GreenFieldLocalHub.Models;
using Microsoft.AspNetCore.Identity;

namespace GreenFieldLocalHub.Data
{
    public class SeedData
    {
        public static async Task SeedUsersAndRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Farmer", "Standard", "Loyalty", "Developer" };
            foreach (string roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }

            //Admin user
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Password123!");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Farmer user
            var farmerUser = await userManager.FindByEmailAsync("farmer@example.com");
            if (farmerUser == null)
            {
                farmerUser = new IdentityUser { UserName = "farmer@example.com", Email = "farmer@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(farmerUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(farmerUser, "Farmer"))
            {
                await userManager.AddToRoleAsync(farmerUser, "Farmer");
            }

            var farmerUser2 = await userManager.FindByEmailAsync("farmer2@example.com");
            if (farmerUser2 == null)
            {
                farmerUser2 = new IdentityUser { UserName = "farmer2@example.com", Email = "farmer2@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(farmerUser2, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(farmerUser2, "Farmer"))
            {
                await userManager.AddToRoleAsync(farmerUser2, "Farmer");
            }

            var farmerUser3 = await userManager.FindByEmailAsync("farmer3@example.com");
            if (farmerUser3 == null)
            {
                farmerUser3 = new IdentityUser { UserName = "farmer3@example.com", Email = "farmer3@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(farmerUser3, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(farmerUser3, "Farmer"))
            {
                await userManager.AddToRoleAsync(farmerUser3, "Farmer");
            }

            // Standard user
            var standardUser = await userManager.FindByEmailAsync("standard@example.com");
            if (standardUser == null)
            {
                standardUser = new IdentityUser { UserName = "standard@example.com", Email = "standard@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(standardUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(standardUser, "Standard"))
            {
                await userManager.AddToRoleAsync(standardUser, "Standard");
            }

            // Loyalty user
            var loyaltyUser = await userManager.FindByEmailAsync("loyalty@example.com");
            if (loyaltyUser == null)
            {
                loyaltyUser = new IdentityUser { UserName = "loyalty@example.com", Email = "loyalty@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(loyaltyUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(loyaltyUser, "Loyalty"))
            {
                await userManager.AddToRoleAsync(loyaltyUser, "Loyalty");
            }

            // Developer user
            var developerUser = await userManager.FindByEmailAsync("developer@example.com");
            if (developerUser == null)
            {
                developerUser = new IdentityUser { UserName = "developer@example.com", Email = "developer@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(developerUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(developerUser, "Developer"))
            {
                await userManager.AddToRoleAsync(developerUser, "Developer");
            }
        }

        public static async Task SeedFarmers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Find the farmer users by email
            var farmerUser1 = await userManager.FindByEmailAsync("farmer@example.com");
            var farmerUser2 = await userManager.FindByEmailAsync("farmer2@example.com");
            var farmerUser3 = await userManager.FindByEmailAsync("farmer3@example.com");

            // If any of the farmer users don't exist, stop and throw an error
            if (farmerUser1 == null || farmerUser2 == null || farmerUser3 == null)
            {
                throw new Exception("Farmer user not found.");
            }

            if (context.Farmers.Any())
                return;

            var farmers = new List<Farmers>
            {
                new Farmers
                {
                    FarmerName = "Vienne's Local Grub",
                    FarmerEmail = "contact@VienneLG.co.uk",
                    FarmerInfo = "Born and raised in GreenField, wanting to promote eco-friendly produce for everyone to eat!",
                    FarmingMethod = "CropField",
                    UserId = farmerUser1.Id
                },

                 new Farmers
                 {
                    FarmerName = "Henderson's Harvest",
                    FarmerEmail = "contact@HendersonsHarvest.co.uk",
                    FarmerInfo = "Third generation farmer bringing fresh seasonal produce straight from our fields to your table!",
                    FarmingMethod = "Free Range",
                    UserId = farmerUser2.Id
                 },
                new Farmers
                {
                    FarmerName = "Green Acres Farm",
                    FarmerEmail = "contact@GreenAcresFarm.co.uk",
                    FarmerInfo = "Passionate about sustainable farming and delivering the finest organic produce in GreenField!",
                    FarmingMethod = "Organic",
                    UserId = farmerUser3.Id
                }
            };

            context.Farmers.AddRange(farmers);
            await context.SaveChangesAsync();
   
        }
    }
}
