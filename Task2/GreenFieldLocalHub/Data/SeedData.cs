using GreenFieldLocalHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GreenFieldLocalHub.Data
{
    public class SeedData
    {
        public static async Task SeedUsersAndRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Farmer", "Standard", "Developer" };
            foreach (string roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }

            // Admin user
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

            // Bronze loyalty demo user
            var bronzeUser = await userManager.FindByEmailAsync("bronze@example.com");
            if (bronzeUser == null)
            {
                bronzeUser = new IdentityUser { UserName = "bronze@example.com", Email = "bronze@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(bronzeUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(bronzeUser, "Standard"))
            {
                await userManager.AddToRoleAsync(bronzeUser, "Standard");
            }

            // Silver loyalty demo user
            var silverUser = await userManager.FindByEmailAsync("silver@example.com");
            if (silverUser == null)
            {
                silverUser = new IdentityUser { UserName = "silver@example.com", Email = "silver@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(silverUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(silverUser, "Standard"))
            {
                await userManager.AddToRoleAsync(silverUser, "Standard");
            }

            // Gold loyalty demo user
            var goldUser = await userManager.FindByEmailAsync("gold@example.com");
            if (goldUser == null)
            {
                goldUser = new IdentityUser { UserName = "gold@example.com", Email = "gold@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(goldUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(goldUser, "Standard"))
            {
                await userManager.AddToRoleAsync(goldUser, "Standard");
            }
        }

        public static async Task SeedFarmers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var farmerUser1 = await userManager.FindByEmailAsync("farmer@example.com");
            var farmerUser2 = await userManager.FindByEmailAsync("farmer2@example.com");
            var farmerUser3 = await userManager.FindByEmailAsync("farmer3@example.com");

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

        public static async Task SeedProducts(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var ViennesLocalGrub = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Vienne's Local Grub");
            var HendersonsHarvest = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Henderson's Harvest");
            var GreenAcresFarm = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Green Acres Farm");

            if (ViennesLocalGrub == null || HendersonsHarvest == null || GreenAcresFarm == null)
            {
                throw new Exception("Farmer not Found");
            }

            if (!context.Products.Any())
            {
                var products = new List<Products>
                {
                    new Products()
                    {
                        ProductName = "Green Onions",
                        ProductDescription = "Fresh green onions, organically grown by our family!",
                        StockQuantity = 50,
                        IsAvailable = true,
                        ProductPrice = 0.60m,
                        FarmersId = ViennesLocalGrub.FarmersId,
                        ImagePath = "/images/armbrustanna-green-onions-699943.jpg"
                    },
                    new Products()
                    {
                        ProductName = "Apples",
                        ProductDescription = "Red juicy apples, hand picked from our acre of apple trees!",
                        StockQuantity = 250,
                        IsAvailable = true,
                        ProductPrice = 0.80m,
                        FarmersId = GreenAcresFarm.FarmersId,
                        ImagePath = "/images/bajarita-berner-rose-75320.jpg"
                    },
                    new Products()
                    {
                        ProductName = "Carrots",
                        ProductDescription = "Our carrots are the perfect balance of refreshing and sweet, making them perfect for any meal!",
                        StockQuantity = 200,
                        IsAvailable = true,
                        ProductPrice = 0.45m,
                        FarmersId = HendersonsHarvest.FarmersId,
                        ImagePath = "/images/jackmac34-carrots-673184_1920.jpg"
                    },
                    new Products()
                    {
                        ProductName = "Strawberries",
                        ProductDescription = "Our strawberries are fresh, juicy, and naturally sweet—perfect for desserts, snacks, or adding a burst of flavour to any meal!",
                        StockQuantity = 150,
                        IsAvailable = true,
                        ProductPrice = 1.20m,
                        FarmersId = ViennesLocalGrub.FarmersId,
                        ImagePath = "/images/jackmac34-basket-strawberries-2208356.jpg"
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedLoyaltyAccounts(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.LoyaltyAccount.Any())
                return;

            var bronzeUser = await userManager.FindByEmailAsync("bronze@example.com");
            var silverUser = await userManager.FindByEmailAsync("silver@example.com");
            var goldUser = await userManager.FindByEmailAsync("gold@example.com");

            if (bronzeUser == null || silverUser == null || goldUser == null)
                throw new Exception("Loyalty seed users not found. Ensure SeedUsersAndRoles ran first.");

            var accounts = new List<LoyaltyAccount>
            {
                new LoyaltyAccount
                {
                    UserId    = bronzeUser.Id,
                    Points    = 350,
                    Tier      = "Bronze",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new LoyaltyAccount
                {
                    UserId    = silverUser.Id,
                    Points    = 650,
                    Tier      = "Silver",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new LoyaltyAccount
                {
                    UserId    = goldUser.Id,
                    Points    = 1200,
                    Tier      = "Gold",
                    CreatedAt = DateTime.UtcNow.AddMonths(-12)
                }
            };

            context.LoyaltyAccount.AddRange(accounts);
            await context.SaveChangesAsync();
        }
    }
}