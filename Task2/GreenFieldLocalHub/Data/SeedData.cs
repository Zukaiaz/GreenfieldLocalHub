using GreenFieldLocalHub.Models; // Imports the data models like Farmers, Products, etc.
using Microsoft.AspNetCore.Identity; // Imports Identity tools for managing users and roles
using Microsoft.EntityFrameworkCore; // Imports Entity Framework for database queries

namespace GreenFieldLocalHub.Data // Defines the namespace for data-related classes
{ // Start of namespace
    public class SeedData // Defines the SeedData class used to populate the database
    { // Start of class
        public static async Task SeedUsersAndRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) // Method to create roles and default users
        { // Start of SeedUsersAndRoles
            string[] roleNames = { "Admin", "Farmer", "Standard", "Developer" }; // Defines the list of security roles for the app
            foreach (string roleName in roleNames) // Loops through each role name in the array
            { // Start foreach
                var roleExists = await roleManager.RoleExistsAsync(roleName); // Checks if the role is already in the database
                if (!roleExists) // If the role does not exist yet
                { // Start if
                    var role = new IdentityRole(roleName); // Creates a new IdentityRole object
                    await roleManager.CreateAsync(role); // Saves the new role to the database
                } // End if
            } // End foreach

            // Admin user
            var adminUser = await userManager.FindByEmailAsync("admin@example.com"); // Tries to find the admin user by email
            if (adminUser == null) // If the admin user doesn't exist
            { // Start if
                adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true }; // Sets up the admin account details
                await userManager.CreateAsync(adminUser, "Password123!"); // Creates the user with a default password
            } // End if
            if (!await userManager.IsInRoleAsync(adminUser, "Admin")) // Checks if the admin user is missing the Admin role
            { // Start if
                await userManager.AddToRoleAsync(adminUser, "Admin"); // Assigns the "Admin" role to this user
            } // End if

            // Farmer user
            var farmerUser = await userManager.FindByEmailAsync("farmer@example.com"); // Tries to find the first farmer user
            if (farmerUser == null) // If the user isn't found
            { // Start if
                farmerUser = new IdentityUser { UserName = "farmer@example.com", Email = "farmer@example.com", EmailConfirmed = true }; // Sets up user details
                await userManager.CreateAsync(farmerUser, "Password123!"); // Creates the account
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser, "Farmer")) // Checks for Farmer role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser, "Farmer"); // Assigns Farmer role
            } // End if

            var farmerUser2 = await userManager.FindByEmailAsync("farmer2@example.com"); // Finds the second farmer user
            if (farmerUser2 == null) // If missing
            { // Start if
                farmerUser2 = new IdentityUser { UserName = "farmer2@example.com", Email = "farmer2@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser2, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser2, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser2, "Farmer"); // Assigns role
            } // End if

            var farmerUser3 = await userManager.FindByEmailAsync("farmer3@example.com"); // Finds the third farmer user
            if (farmerUser3 == null) // If missing
            { // Start if
                farmerUser3 = new IdentityUser { UserName = "farmer3@example.com", Email = "farmer3@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser3, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser3, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser3, "Farmer"); // Assigns role
            } // End if

            // Standard user
            var standardUser = await userManager.FindByEmailAsync("standard@example.com"); // Finds a regular customer user
            if (standardUser == null) // If missing
            { // Start if
                standardUser = new IdentityUser { UserName = "standard@example.com", Email = "standard@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(standardUser, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(standardUser, "Standard")) // Checks for Standard role
            { // Start if
                await userManager.AddToRoleAsync(standardUser, "Standard"); // Assigns Standard role
            } // End if

            // Developer user
            var developerUser = await userManager.FindByEmailAsync("developer@example.com"); // Finds the developer test user
            if (developerUser == null) // If missing
            { // Start if
                developerUser = new IdentityUser { UserName = "developer@example.com", Email = "developer@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(developerUser, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(developerUser, "Developer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(developerUser, "Developer"); // Assigns Developer role
            } // End if

            // Bronze loyalty demo user
            var bronzeUser = await userManager.FindByEmailAsync("bronze@example.com"); // Finds the bronze tier test user
            if (bronzeUser == null) // If missing
            { // Start if
                bronzeUser = new IdentityUser { UserName = "bronze@example.com", Email = "bronze@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(bronzeUser, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(bronzeUser, "Standard")) // Ensures they have the Standard role
            { // Start if
                await userManager.AddToRoleAsync(bronzeUser, "Standard"); // Assigns role
            } // End if

            // Silver loyalty demo user
            var silverUser = await userManager.FindByEmailAsync("silver@example.com"); // Finds silver tier test user
            if (silverUser == null) // If missing
            { // Start if
                silverUser = new IdentityUser { UserName = "silver@example.com", Email = "silver@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(silverUser, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(silverUser, "Standard")) // Ensures they have Standard role
            { // Start if
                await userManager.AddToRoleAsync(silverUser, "Standard"); // Assigns role
            } // End if

            // Gold loyalty demo user
            var goldUser = await userManager.FindByEmailAsync("gold@example.com"); // Finds gold tier test user
            if (goldUser == null) // If missing
            { // Start if
                goldUser = new IdentityUser { UserName = "gold@example.com", Email = "gold@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(goldUser, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(goldUser, "Standard")) // Ensures they have Standard role
            { // Start if
                await userManager.AddToRoleAsync(goldUser, "Standard"); // Assigns role
            } // End if
        } // End of SeedUsersAndRoles

        public static async Task SeedFarmers(IServiceProvider serviceProvider) // Method to populate Farmer profile data
        { // Start of SeedFarmers
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); // Gets user manager service
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Gets database context service

            var farmerUser1 = await userManager.FindByEmailAsync("farmer@example.com"); // Retrieves the first farmer user
            var farmerUser2 = await userManager.FindByEmailAsync("farmer2@example.com"); // Retrieves the second farmer user
            var farmerUser3 = await userManager.FindByEmailAsync("farmer3@example.com"); // Retrieves the third farmer user

            if (farmerUser1 == null || farmerUser2 == null || farmerUser3 == null) // If any user account is missing
            { // Start if
                throw new Exception("Farmer user not found."); // Stop the app and report error
            } // End if

            if (context.Farmers.Any()) // Check if there are already farmers in the database
                return; // Exit if the table is already populated

            var farmers = new List<Farmers> // Create a list of Farmer profile objects
            { // Start list
                new Farmers // First farmer profile
                { // Start object
                    FarmerName = "Vienne's Local Grub", // Sets business name
                    FarmerEmail = "contact@VienneLG.co.uk", // Sets business email
                    FarmerInfo = "Born and raised in GreenField, wanting to promote eco-friendly produce for everyone to eat!", // Bio
                    FarmingMethod = "CropField", // Category
                    UserId = farmerUser1.Id // Links to the first identity user
                }, // End object
                new Farmers // Second farmer profile
                { // Start object
                    FarmerName = "Henderson's Harvest", // Business name
                    FarmerEmail = "contact@HendersonsHarvest.co.uk", // Email
                    FarmerInfo = "Third generation farmer bringing fresh seasonal produce straight from our fields to your table!", // Bio
                    FarmingMethod = "Free Range", // Category
                    UserId = farmerUser2.Id // Links to the second identity user
                }, // End object
                new Farmers // Third farmer profile
                { // Start object
                    FarmerName = "Green Acres Farm", // Business name
                    FarmerEmail = "contact@GreenAcresFarm.co.uk", // Email
                    FarmerInfo = "Passionate about sustainable farming and delivering the finest organic produce in GreenField!", // Bio
                    FarmingMethod = "Organic", // Category
                    UserId = farmerUser3.Id // Links to the third identity user
                } // End object
            }; // End list

            context.Farmers.AddRange(farmers); // Adds the list of farmers to the DB tracker
            await context.SaveChangesAsync(); // Commits changes to the database
        } // End of SeedFarmers

        public static async Task SeedProducts(IServiceProvider serviceProvider) // Method to populate the product catalog
        { // Start of SeedProducts
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Gets database context

            var ViennesLocalGrub = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Vienne's Local Grub"); // Finds first farmer by name
            var HendersonsHarvest = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Henderson's Harvest"); // Finds second farmer by name
            var GreenAcresFarm = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Green Acres Farm"); // Finds third farmer by name

            if (ViennesLocalGrub == null || HendersonsHarvest == null || GreenAcresFarm == null) // Error check
            { // Start if
                throw new Exception("Farmer not Found"); // Stop if farmers aren't in DB yet
            } // End if

            if (!context.Products.Any()) // Only run if no products exist
            { // Start if
                var products = new List<Products> // Create a list of product objects
                { // Start list
                    new Products() // First product
                    { // Start object
                        ProductName = "Green Onions", // Name
                        ProductDescription = "Fresh green onions, organically grown by our family!", // Description
                        StockQuantity = 50, // Initial stock
                        IsAvailable = true, // Availability flag
                        ProductPrice = 0.60m, // Price per unit
                        FarmersId = ViennesLocalGrub.FarmersId, // Linked to Vienne
                        ImagePath = "/images/armbrustanna-green-onions-699943.jpg" // Local image file path
                    }, // End object
                    new Products() // Second product
                    { // Start object
                        ProductName = "Apples", // Name
                        ProductDescription = "Red juicy apples, hand picked from our acre of apple trees!", // Description
                        StockQuantity = 250, // Stock
                        IsAvailable = true, // Availability
                        ProductPrice = 0.80m, // Price
                        FarmersId = GreenAcresFarm.FarmersId, // Linked to Green Acres
                        ImagePath = "/images/bajarita-berner-rose-75320.jpg" // Image path
                    }, // End object
                    new Products() // Third product
                    { // Start object
                        ProductName = "Carrots", // Name
                        ProductDescription = "Our carrots are the perfect balance of refreshing and sweet, making them perfect for any meal!", // Description
                        StockQuantity = 200, // Stock
                        IsAvailable = true, // Availability
                        ProductPrice = 0.45m, // Price
                        FarmersId = HendersonsHarvest.FarmersId, // Linked to Henderson
                        ImagePath = "/images/jackmac34-carrots-673184_1920.jpg" // Image path
                    }, // End object
                    new Products() // Fourth product
                    { // Start object
                        ProductName = "Strawberries", // Name
                        ProductDescription = "Our strawberries are fresh, juicy, and naturally sweet—perfect for desserts, snacks, or adding a burst of flavour to any meal!", // Description
                        StockQuantity = 150, // Stock
                        IsAvailable = true, // Availability
                        ProductPrice = 1.20m, // Price
                        FarmersId = ViennesLocalGrub.FarmersId, // Linked to Vienne
                        ImagePath = "/images/jackmac34-basket-strawberries-2208356.jpg" // Image path
                    } // End object
                }; // End list

                await context.Products.AddRangeAsync(products); // Adds products to tracker asynchronously
                await context.SaveChangesAsync(); // Commits products to database
            } // End if
        } // End of SeedProducts

        public static async Task SeedLoyaltyAccounts(IServiceProvider serviceProvider) // Method to create starting loyalty accounts
        { // Start of SeedLoyaltyAccounts
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); // Gets user manager service
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Gets database context

            if (context.LoyaltyAccount.Any()) // Check if loyalty accounts exist
                return; // Exit if already seeded

            var bronzeUser = await userManager.FindByEmailAsync("bronze@example.com"); // Get the bronze identity user
            var silverUser = await userManager.FindByEmailAsync("silver@example.com"); // Get the silver identity user
            var goldUser = await userManager.FindByEmailAsync("gold@example.com"); // Get the gold identity user

            if (bronzeUser == null || silverUser == null || goldUser == null) // Check if users exist
                throw new Exception("Loyalty seed users not found. Ensure SeedUsersAndRoles ran first."); // Error check

            var accounts = new List<LoyaltyAccount> // Create list of loyalty profiles
            { // Start list
                new LoyaltyAccount // Bronze account
                { // Start object
                    UserId    = bronzeUser.Id, // Linked ID
                    Points    = 350, // Point balance
                    Tier      = "Bronze", // Current tier
                    CreatedAt = DateTime.UtcNow.AddMonths(-3) // Set creation date to 3 months ago
                }, // End object
                new LoyaltyAccount // Silver account
                { // Start object
                    UserId    = silverUser.Id, // Linked ID
                    Points    = 650, // Point balance
                    Tier      = "Silver", // Current tier
                    CreatedAt = DateTime.UtcNow.AddMonths(-6) // Set creation date to 6 months ago
                }, // End object
                new LoyaltyAccount // Gold account
                { // Start object
                    UserId    = goldUser.Id, // Linked ID
                    Points    = 1200, // Point balance
                    Tier      = "Gold", // Current tier
                    CreatedAt = DateTime.UtcNow.AddMonths(-12) // Set creation date to 1 year ago
                } // End object
            }; // End list

            context.LoyaltyAccount.AddRange(accounts); // Add accounts to tracker
            await context.SaveChangesAsync(); // Commit accounts to database
        } // End of SeedLoyaltyAccounts
    } // End of class
} // End of namespace