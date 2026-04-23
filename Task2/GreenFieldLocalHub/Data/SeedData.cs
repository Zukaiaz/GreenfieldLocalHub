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

            // Farmer user 1
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

            // Farmer user 2
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

            // Farmer user 3
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

            // Farmer user 4
            var farmerUser4 = await userManager.FindByEmailAsync("farmer4@example.com"); // Finds the fourth farmer user
            if (farmerUser4 == null) // If missing
            { // Start if
                farmerUser4 = new IdentityUser { UserName = "farmer4@example.com", Email = "farmer4@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser4, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser4, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser4, "Farmer"); // Assigns role
            } // End if

            // Farmer user 5
            var farmerUser5 = await userManager.FindByEmailAsync("farmer5@example.com"); // Finds the fifth farmer user
            if (farmerUser5 == null) // If missing
            { // Start if
                farmerUser5 = new IdentityUser { UserName = "farmer5@example.com", Email = "farmer5@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser5, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser5, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser5, "Farmer"); // Assigns role
            } // End if

            // Farmer user 6
            var farmerUser6 = await userManager.FindByEmailAsync("farmer6@example.com"); // Finds the sixth farmer user
            if (farmerUser6 == null) // If missing
            { // Start if
                farmerUser6 = new IdentityUser { UserName = "farmer6@example.com", Email = "farmer6@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser6, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser6, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser6, "Farmer"); // Assigns role
            } // End if

            // Farmer user 7
            var farmerUser7 = await userManager.FindByEmailAsync("farmer7@example.com"); // Finds the seventh farmer user
            if (farmerUser7 == null) // If missing
            { // Start if
                farmerUser7 = new IdentityUser { UserName = "farmer7@example.com", Email = "farmer7@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser7, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser7, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser7, "Farmer"); // Assigns role
            } // End if

            // Farmer user 8
            var farmerUser8 = await userManager.FindByEmailAsync("farmer8@example.com"); // Finds the eighth farmer user
            if (farmerUser8 == null) // If missing
            { // Start if
                farmerUser8 = new IdentityUser { UserName = "farmer8@example.com", Email = "farmer8@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser8, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser8, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser8, "Farmer"); // Assigns role
            } // End if

            // Farmer user 9
            var farmerUser9 = await userManager.FindByEmailAsync("farmer9@example.com"); // Finds the ninth farmer user
            if (farmerUser9 == null) // If missing
            { // Start if
                farmerUser9 = new IdentityUser { UserName = "farmer9@example.com", Email = "farmer9@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser9, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser9, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser9, "Farmer"); // Assigns role
            } // End if

            // Farmer user 10
            var farmerUser10 = await userManager.FindByEmailAsync("farmer10@example.com"); // Finds the tenth farmer user
            if (farmerUser10 == null) // If missing
            { // Start if
                farmerUser10 = new IdentityUser { UserName = "farmer10@example.com", Email = "farmer10@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser10, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser10, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser10, "Farmer"); // Assigns role
            } // End if

            // Farmer user 11
            var farmerUser11 = await userManager.FindByEmailAsync("farmer11@example.com"); // Finds the eleventh farmer user
            if (farmerUser11 == null) // If missing
            { // Start if
                farmerUser11 = new IdentityUser { UserName = "farmer11@example.com", Email = "farmer11@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser11, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser11, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser11, "Farmer"); // Assigns role
            } // End if

            // Farmer user 12
            var farmerUser12 = await userManager.FindByEmailAsync("farmer12@example.com"); // Finds the twelfth farmer user
            if (farmerUser12 == null) // If missing
            { // Start if
                farmerUser12 = new IdentityUser { UserName = "farmer12@example.com", Email = "farmer12@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser12, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser12, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser12, "Farmer"); // Assigns role
            } // End if

            // Farmer user 13
            var farmerUser13 = await userManager.FindByEmailAsync("farmer13@example.com"); // Finds the thirteenth farmer user
            if (farmerUser13 == null) // If missing
            { // Start if
                farmerUser13 = new IdentityUser { UserName = "farmer13@example.com", Email = "farmer13@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser13, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser13, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser13, "Farmer"); // Assigns role
            } // End if

            // Farmer user 14
            var farmerUser14 = await userManager.FindByEmailAsync("farmer14@example.com"); // Finds the fourteenth farmer user
            if (farmerUser14 == null) // If missing
            { // Start if
                farmerUser14 = new IdentityUser { UserName = "farmer14@example.com", Email = "farmer14@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser14, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser14, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser14, "Farmer"); // Assigns role
            } // End if

            // Farmer user 15
            var farmerUser15 = await userManager.FindByEmailAsync("farmer15@example.com"); // Finds the fifteenth farmer user
            if (farmerUser15 == null) // If missing
            { // Start if
                farmerUser15 = new IdentityUser { UserName = "farmer15@example.com", Email = "farmer15@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser15, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser15, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser15, "Farmer"); // Assigns role
            } // End if

            // Farmer user 16
            var farmerUser16 = await userManager.FindByEmailAsync("farmer16@example.com"); // Finds the sixteenth farmer user
            if (farmerUser16 == null) // If missing
            { // Start if
                farmerUser16 = new IdentityUser { UserName = "farmer16@example.com", Email = "farmer16@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser16, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser16, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser16, "Farmer"); // Assigns role
            } // End if

            // Farmer user 17
            var farmerUser17 = await userManager.FindByEmailAsync("farmer17@example.com"); // Finds the seventeenth farmer user
            if (farmerUser17 == null) // If missing
            { // Start if
                farmerUser17 = new IdentityUser { UserName = "farmer17@example.com", Email = "farmer17@example.com", EmailConfirmed = true }; // Setup details
                await userManager.CreateAsync(farmerUser17, "Password123!"); // Create user
            } // End if
            if (!await userManager.IsInRoleAsync(farmerUser17, "Farmer")) // Checks role
            { // Start if
                await userManager.AddToRoleAsync(farmerUser17, "Farmer"); // Assigns role
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

            var farmerUser1 = await userManager.FindByEmailAsync("farmer@example.com");
            var farmerUser2 = await userManager.FindByEmailAsync("farmer2@example.com");
            var farmerUser3 = await userManager.FindByEmailAsync("farmer3@example.com");
            var farmerUser4 = await userManager.FindByEmailAsync("farmer4@example.com");
            var farmerUser5 = await userManager.FindByEmailAsync("farmer5@example.com");
            var farmerUser6 = await userManager.FindByEmailAsync("farmer6@example.com");
            var farmerUser7 = await userManager.FindByEmailAsync("farmer7@example.com");
            var farmerUser8 = await userManager.FindByEmailAsync("farmer8@example.com");
            var farmerUser9 = await userManager.FindByEmailAsync("farmer9@example.com");
            var farmerUser10 = await userManager.FindByEmailAsync("farmer10@example.com");
            var farmerUser11 = await userManager.FindByEmailAsync("farmer11@example.com");
            var farmerUser12 = await userManager.FindByEmailAsync("farmer12@example.com");
            var farmerUser13 = await userManager.FindByEmailAsync("farmer13@example.com");
            var farmerUser14 = await userManager.FindByEmailAsync("farmer14@example.com");
            var farmerUser15 = await userManager.FindByEmailAsync("farmer15@example.com");
            var farmerUser16 = await userManager.FindByEmailAsync("farmer16@example.com");
            var farmerUser17 = await userManager.FindByEmailAsync("farmer17@example.com");

            if (farmerUser1 == null || farmerUser2 == null || farmerUser3 == null ||
                farmerUser4 == null || farmerUser5 == null || farmerUser6 == null ||
                farmerUser7 == null || farmerUser8 == null || farmerUser9 == null ||
                farmerUser10 == null || farmerUser11 == null || farmerUser12 == null ||
                farmerUser13 == null || farmerUser14 == null || farmerUser15 == null ||
                farmerUser16 == null || farmerUser17 == null) // If any user account is missing
            { // Start if
                throw new Exception("One or more farmer users not found. Ensure SeedUsersAndRoles ran first."); // Stop the app and report error
            } // End if

            if (context.Farmers.Any()) // Check if there are already farmers in the database
                return; // Exit if the table is already populated

            var farmers = new List<Farmers> // Create a list of Farmer profile objects
            { // Start list
 
                // --- Original 3 farmers ---
 
                new Farmers // Farmer 1
                { // Start object
                    FarmerName    = "Vienne's Local Grub", // Sets business name
                    FarmerEmail   = "contact@VienneLG.co.uk", // Sets business email
                    FarmerInfo    = "Born and raised in GreenField, wanting to promote eco-friendly produce for everyone to eat!", // Bio
                    FarmingMethod = "CropField", // Category
                    ImagePath     = "/images/land-o-lakes-inc-JUivXruBs2U-unsplash.jpg", // Local image file path
                    UserId        = farmerUser1.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 2
                { // Start object
                    FarmerName    = "Henderson's Harvest", // Business name
                    FarmerEmail   = "contact@HendersonsHarvest.co.uk", // Email
                    FarmerInfo    = "Third generation farmer bringing fresh seasonal produce straight from our fields to your table!", // Bio
                    FarmingMethod = "Free Range", // Category
                    ImagePath     = "/images/randy-fath-dDc0vuVH_LU-unsplash.jpg", // Local image file path
                    UserId        = farmerUser2.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 3
                { // Start object
                    FarmerName    = "Green Acres Farm", // Business name
                    FarmerEmail   = "contact@GreenAcresFarm.co.uk", // Email
                    FarmerInfo    = "Passionate about sustainable farming and delivering the finest organic produce in GreenField!", // Bio
                    FarmingMethod = "Organic", // Category
                    ImagePath     = "/images/gregory-hayes-QFmNQXLPbZc-unsplash.jpg", // Local image file path
                    UserId        = farmerUser3.Id // Links to identity user
                }, // End object
 
                // --- 14 new farmers ---
 
                new Farmers // Farmer 4
                { // Start object
                    FarmerName    = "Thornberry Meadows", // Business name
                    FarmerEmail   = "contact@ThornberryMeadows.co.uk", // Email
                    FarmerInfo    = "A family-run smallholding nestled on the edge of GreenField, specialising in heritage vegetable varieties and wild flower honey.", // Bio
                    FarmingMethod = "Organic", // Category
                    ImagePath     = "/images/brooke-cagle-EenUxvVltMs-unsplash.jpg", // Local image file path
                    UserId        = farmerUser4.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 5
                { // Start object
                    FarmerName    = "Riverbend Dairy", // Business name
                    FarmerEmail   = "contact@RiverbendDairy.co.uk", // Email
                    FarmerInfo    = "Award-winning artisan dairy producing rich whole milk, creamy butter, and handcrafted cheeses from our grass-fed herd.", // Bio
                    FarmingMethod = "Free Range", // Category
                    ImagePath     = "/images/gregory-hayes-QFmNQXLPbZc-unsplash.jpg", // Local image file path
                    UserId        = farmerUser5.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 6
                { // Start object
                    FarmerName    = "Sunfield Polytunnels", // Business name
                    FarmerEmail   = "contact@SunfieldPolytunnels.co.uk", // Email
                    FarmerInfo    = "Growing sun-ripened tomatoes, sweet peppers, and fresh herbs year-round using low-energy polytunnel technology.", // Bio
                    FarmingMethod = "CropField", // Category
                    ImagePath     = "/images/jake-gard-CetB-bTDBtY-unsplash.jpg", // Local image file path
                    UserId        = farmerUser6.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 7
                { // Start object
                    FarmerName    = "Willowbrook Orchard", // Business name
                    FarmerEmail   = "contact@WillowbrookOrchard.co.uk", // Email
                    FarmerInfo    = "Over 200 apple and pear varieties grown across our ancient orchard. We press our own single-variety juices and ciders on-site.", // Bio
                    FarmingMethod = "Organic", // Category
                    ImagePath     = "/images/jed-owen-1JgUGDdcWnM-unsplash.jpg", // Local image file path
                    UserId        = farmerUser7.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 8
                { // Start object
                    FarmerName    = "Foxglove Free Range Eggs", // Business name
                    FarmerEmail   = "contact@FoxgloveFreeRange.co.uk", // Email
                    FarmerInfo    = "Our hens roam the open pastures of Foxglove Hill every day. Expect deep golden yolks and eggs packed with natural goodness.", // Bio
                    FarmingMethod = "Free Range", // Category
                    ImagePath     = "/images/jed-owen-ajZibDGpPew-unsplash.jpg", // Local image file path
                    UserId        = farmerUser8.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 9
                { // Start object
                    FarmerName    = "Coppergate Microgreens", // Business name
                    FarmerEmail   = "contact@CoppergateGreens.co.uk", // Email
                    FarmerInfo    = "Hyper-local urban grower bringing nutrient-dense microgreens, sprouts, and edible flowers to GreenField restaurants and homes.", // Bio
                    FarmingMethod = "CropField", // Category
                    ImagePath     = "/images/land-o-lakes-inc-0VIqR7HmDZw-unsplash.jpg", // Local image file path
                    UserId        = farmerUser9.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 10
                { // Start object
                    FarmerName    = "Hawthorn Hill Soft Fruits", // Business name
                    FarmerEmail   = "contact@HawthornHillFruits.co.uk", // Email
                    FarmerInfo    = "Pick-your-own and pre-packed raspberries, blackcurrants, and gooseberries grown on the sunny south-facing slopes of Hawthorn Hill.", // Bio
                    FarmingMethod = "Organic", // Category
                    ImagePath     = "/images/land-o-lakes-inc-BlXa_riHlp4-unsplash.jpg", // Local image file path
                    UserId        = farmerUser10.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 11
                { // Start object
                    FarmerName    = "Mossy Stone Mushrooms", // Business name
                    FarmerEmail   = "contact@MossyStoneMushrooms.co.uk", // Email
                    FarmerInfo    = "Cultivating a wide range of gourmet and medicinal mushrooms — from oyster and shiitake to lion's mane — in our converted stone barn.", // Bio
                    FarmingMethod = "CropField", // Category
                    ImagePath     = "/images/land-o-lakes-inc-iFx1WMvjvpw-unsplash.jpg", // Local image file path
                    UserId        = farmerUser11.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 12
                { // Start object
                    FarmerName    = "Bramblewood Herb Garden", // Business name
                    FarmerEmail   = "contact@BramblewoodHerbs.co.uk", // Email
                    FarmerInfo    = "Freshly cut culinary and medicinal herbs grown without pesticides. We also offer dried herb bundles, herb salts, and seasonal herb boxes.", // Bio
                    FarmingMethod = "Organic", // Category
                    ImagePath     = "/images/land-o-lakes-inc-JUivXruBs2U-unsplash.jpg", // Local image file path
                    UserId        = farmerUser12.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 13
                { // Start object
                    FarmerName    = "Elmfield Rare Breeds", // Business name
                    FarmerEmail   = "contact@ElmfieldRareBreeds.co.uk", // Email
                    FarmerInfo    = "Dedicated to preserving rare and native livestock breeds. Our Dexter beef and Saddleback pork are reared slowly on traditional pasture.", // Bio
                    FarmingMethod = "Free Range", // Category
                    ImagePath     = "/images/randy-fath-dDc0vuVH_LU-unsplash.jpg", // Local image file path
                    UserId        = farmerUser13.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 14
                { // Start object
                    FarmerName    = "Lakeside Watercress Co.", // Business name
                    FarmerEmail   = "contact@LakesideWatercress.co.uk", // Email
                    FarmerInfo    = "Growing crisp, peppery watercress in the clear spring-fed streams running through our GreenField plot. Harvested fresh every morning.", // Bio
                    FarmingMethod = "CropField", // Category
                    ImagePath     = "/images/saikiran-kesari-zSn8VuwV7Kg-unsplash.jpg", // Local image file path
                    UserId        = farmerUser14.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 15
                { // Start object
                    FarmerName    = "Cloverfield Honey Farm", // Business name
                    FarmerEmail   = "contact@CloverfieldHoney.co.uk", // Email
                    FarmerInfo    = "Keepers of over 40 hives across wildflower meadows and hedgerows. Our raw, unfiltered honeys change with the seasons and the flowers.", // Bio
                    FarmingMethod = "Organic", // Category
                    ImagePath     = "/images/amanda-wolbert-NqyJZprqP9c-unsplash.jpg", // Local image file path
                    UserId        = farmerUser15.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 16
                { // Start object
                    FarmerName    = "Ironwood Market Garden", // Business name
                    FarmerEmail   = "contact@IronwoodMarketGarden.co.uk", // Email
                    FarmerInfo    = "A no-dig market garden producing mixed salad leaves, root vegetables, and brassicas for weekly veg box deliveries across GreenField.", // Bio
                    FarmingMethod = "CropField", // Category
                    ImagePath     = "/images/tim-mossholder-xDwEa2kaeJA-unsplash.jpg", // Local image file path
                    UserId        = farmerUser16.Id // Links to identity user
                }, // End object
 
                new Farmers // Farmer 17
                { // Start object
                    FarmerName    = "Pebble Creek Aquaponics", // Business name
                    FarmerEmail   = "contact@PebbleCreekAquaponics.co.uk", // Email
                    FarmerInfo    = "Combining fish cultivation and hydroponics to grow pesticide-free salad greens and herbs alongside sustainably farmed trout.", // Bio
                    FarmingMethod = "CropField", // Category
                    ImagePath     = "/images/tony-pham-TV7m_tpmqhw-unsplash.jpg", // Local image file path
                    UserId        = farmerUser17.Id // Links to identity user
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
            var ThornberryMeadows = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Thornberry Meadows");
            var RiverbendDairy = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Riverbend Dairy");
            var SunfieldPolytunnels = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Sunfield Polytunnels");
            var WillowbrookOrchard = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Willowbrook Orchard");
            var FoxgloveFreeRangeEggs = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Foxglove Free Range Eggs");
            var CoppergateGreens = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Coppergate Microgreens");
            var HawthornHillFruits = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Hawthorn Hill Soft Fruits");
            var MossyStoneMushrooms = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Mossy Stone Mushrooms");
            var BramblewoodHerbs = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Bramblewood Herb Garden");
            var ElmfieldRareBreeds = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Elmfield Rare Breeds");
            var LakesideWatercress = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Lakeside Watercress Co.");
            var CloverfieldHoney = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Cloverfield Honey Farm");
            var IronwoodMarketGarden = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Ironwood Market Garden");
            var PebbleCreekAquaponics = await context.Farmers.FirstOrDefaultAsync(x => x.FarmerName == "Pebble Creek Aquaponics");

            if (ViennesLocalGrub == null || HendersonsHarvest == null || GreenAcresFarm == null || ThornberryMeadows == null || RiverbendDairy == null || RiverbendDairy == null || SunfieldPolytunnels == null || WillowbrookOrchard == null || // Error check
                FoxgloveFreeRangeEggs == null || CoppergateGreens == null || HawthornHillFruits == null || MossyStoneMushrooms == null || BramblewoodHerbs == null || ElmfieldRareBreeds == null || LakesideWatercress == null ||
                CloverfieldHoney == null || IronwoodMarketGarden == null || PebbleCreekAquaponics == null)
            { // Start if
                throw new Exception("Farmer not Found"); // Stop if farmers aren't in DB yet
            } // End if

            if (!context.Products.Any()) // Only run if no products exist
            { // Start if
                var products = new List<Products> // Create a list of product objects
                { // Start list
                    new Products() // First product
                        { // Start object
                            ProductName        = "Green Onions", // Name
                            ProductDescription = "Fresh green onions, organically grown by our family!", // Description
                            StockQuantity      = 50, // Initial stock
                            IsAvailable        = true, // Availability flag
                            ProductPrice       = 0.60m, // Price per unit
                            FarmersId          = ViennesLocalGrub.FarmersId, // Linked to Vienne
                            ImagePath          = "/images/armbrustanna-green-onions-699943.jpg" // Local image file path
                        }, // End object
                        new Products() // Second product
                        { // Start object
                            ProductName        = "Apples", // Name
                            ProductDescription = "Red juicy apples, hand picked from our acre of apple trees!", // Description
                            StockQuantity      = 250, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 0.80m, // Price
                            FarmersId          = GreenAcresFarm.FarmersId, // Linked to Green Acres
                            ImagePath          = "/images/bajarita-berner-rose-75320.jpg" // Image path
                        }, // End object
                        new Products() // Third product
                        { // Start object
                            ProductName        = "Carrots", // Name
                            ProductDescription = "Our carrots are the perfect balance of refreshing and sweet, making them perfect for any meal!", // Description
                            StockQuantity      = 200, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 0.45m, // Price
                            FarmersId          = HendersonsHarvest.FarmersId, // Linked to Henderson
                            ImagePath          = "/images/jackmac34-carrots-673184_1920.jpg" // Image path
                        }, // End object
                        new Products() // Fourth product
                        { // Start object
                            ProductName        = "Strawberries", // Name
                            ProductDescription = "Our strawberries are fresh, juicy, and naturally sweet—perfect for desserts, snacks, or adding a burst of flavour to any meal!", // Description
                            StockQuantity      = 150, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.20m, // Price
                            FarmersId          = ViennesLocalGrub.FarmersId, // Linked to Vienne
                            ImagePath          = "/images/jackmac34-basket-strawberries-2208356.jpg" // Image path
                        }, // End object

                        // --- 20 new products ---

                        new Products() // Product 5
                        { // Start object
                            ProductName        = "Heritage Beetroot", // Name
                            ProductDescription = "A colourful mix of golden, candy-striped, and deep red heritage beetroot varieties, grown the old-fashioned way at Thornberry Meadows.", // Description
                            StockQuantity      = 120, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 0.75m, // Price
                            FarmersId          = ThornberryMeadows.FarmersId, // Linked to Thornberry Meadows
                            ImagePath          = "/images/nick-collins-udo5pIvRfrA-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 6
                        { // Start object
                            ProductName        = "Wildflower Honey", // Name
                            ProductDescription = "Raw, unfiltered honey collected from Thornberry Meadows' own hives set among the wildflower borders. Rich, complex, and utterly delicious.", // Description
                            StockQuantity      = 60, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 4.50m, // Price
                            FarmersId          = ThornberryMeadows.FarmersId, // Linked to Thornberry Meadows
                            ImagePath          = "/images/roberta-sorge-kp9UVn-PUac-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 7
                        { // Start object
                            ProductName        = "Whole Milk (1L)", // Name
                            ProductDescription = "Creamy, full-fat whole milk from Riverbend Dairy's grass-fed herd. Non-homogenised with a thick layer of cream on top.", // Description
                            StockQuantity      = 180, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.10m, // Price
                            FarmersId          = RiverbendDairy.FarmersId, // Linked to Riverbend Dairy
                            ImagePath          = "/images/gabi-miranda-dxb_HSjoQ40-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 8
                        { // Start object
                            ProductName        = "Artisan Cheddar (200g)", // Name
                            ProductDescription = "A mature, hand-pressed cheddar made exclusively from Riverbend's own milk. Crumbly, sharp, and full of character.", // Description
                            StockQuantity      = 80, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 3.80m, // Price
                            FarmersId          = RiverbendDairy.FarmersId, // Linked to Riverbend Dairy
                            ImagePath          = "/images/david-foodphototasty-JJcT6VJWDlg-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 9
                        { // Start object
                            ProductName        = "Vine Tomatoes (500g)", // Name
                            ProductDescription = "Sun-ripened vine tomatoes bursting with flavour, grown year-round in Sunfield's low-energy polytunnels. Perfect for salads and sauces.", // Description
                            StockQuantity      = 200, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.50m, // Price
                            FarmersId          = SunfieldPolytunnels.FarmersId, // Linked to Sunfield Polytunnels
                            ImagePath          = "/images/valentina-ivanova-6IorVhJSylY-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 10
                        { // Start object
                            ProductName        = "Mixed Sweet Peppers", // Name
                            ProductDescription = "A vibrant bundle of red, yellow, and orange sweet peppers freshly picked from Sunfield's polytunnels. Crunchy, sweet, and versatile.", // Description
                            StockQuantity      = 140, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.80m, // Price
                            FarmersId          = SunfieldPolytunnels.FarmersId, // Linked to Sunfield Polytunnels
                            ImagePath          = "/images/vino-li-v7H-fV9Ydkk-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 11
                        { // Start object
                            ProductName        = "Single-Variety Apple Juice (750ml)", // Name
                            ProductDescription = "Cold-pressed juice made from a single variety of heritage apple, pressed on-site at Willowbrook Orchard. No additives, no concentrates.", // Description
                            StockQuantity      = 90, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 2.95m, // Price
                            FarmersId          = WillowbrookOrchard.FarmersId, // Linked to Willowbrook Orchard
                            ImagePath          = "/images/jocelyn-morales-hFVjgO8JGSU-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 12
                        { // Start object
                            ProductName        = "Still Pear Cider (500ml)", // Name
                            ProductDescription = "A gentle, still cider pressed from Willowbrook's finest pear varieties. Light, fragrant, and refreshingly dry.", // Description
                            StockQuantity      = 70, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 2.50m, // Price
                            FarmersId          = WillowbrookOrchard.FarmersId, // Linked to Willowbrook Orchard
                            ImagePath          = "/images/jennifer-schmidt-XkUJ_hgBZX4-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 13
                        { // Start object
                            ProductName        = "Free Range Eggs (6 pack)", // Name
                            ProductDescription = "Six large eggs from hens that roam the open pastures of Foxglove Hill daily. Expect deep golden yolks and exceptional flavour.", // Description
                            StockQuantity      = 220, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.90m, // Price
                            FarmersId          = FoxgloveFreeRangeEggs.FarmersId, // Linked to Foxglove Free Range Eggs
                            ImagePath          = "/images/becca-paul-Q-XawiS4KUc-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 14
                        { // Start object
                            ProductName        = "Microgreens Mix (100g)", // Name
                            ProductDescription = "A freshly harvested blend of pea shoots, radish, and sunflower microgreens from Coppergate. Packed with nutrients and ready to eat.", // Description
                            StockQuantity      = 100, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 2.20m, // Price
                            FarmersId          = CoppergateGreens.FarmersId, // Linked to Coppergate Microgreens
                            ImagePath          = "/images/kate-cullen-cjEoFVjaMfQ-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 15
                        { // Start object
                            ProductName        = "Fresh Raspberries (250g)", // Name
                            ProductDescription = "Plump, intensely flavoured raspberries hand-picked from the sunny slopes of Hawthorn Hill. Available fresh during the season.", // Description
                            StockQuantity      = 130, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 2.00m, // Price
                            FarmersId          = HawthornHillFruits.FarmersId, // Linked to Hawthorn Hill Soft Fruits
                            ImagePath          = "/images/geertje-caliguire-JNoODfMAyLA-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 16
                        { // Start object
                            ProductName        = "Blackcurrants (250g)", // Name
                            ProductDescription = "Deep, tangy blackcurrants bursting with antioxidants. Excellent for jams, cordials, or simply eaten with a little cream.", // Description
                            StockQuantity      = 95, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.75m, // Price
                            FarmersId          = HawthornHillFruits.FarmersId, // Linked to Hawthorn Hill Soft Fruits
                            ImagePath          = "/images/joanna-stolowicz-KcDwPR4cL5k-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 17
                        { // Start object
                            ProductName        = "Mushrooms (200g)", // Name
                            ProductDescription = "Freshly cultivated mushrooms from Mossy Stone's converted barn. Delicate, silky, and wonderful in stir-fries or on toast.", // Description
                            StockQuantity      = 85, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 2.40m, // Price
                            FarmersId          = MossyStoneMushrooms.FarmersId, // Linked to Mossy Stone Mushrooms
                            ImagePath          = "/images/andrew-ridley-6KCS---7jbc-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 18
                        { // Start object
                            ProductName        = "Mushroom (150g)", // Name
                            ProductDescription = "A prized gourmet mushroom with a meaty texture and subtle seafood-like flavour. Grown carefully at Mossy Stone and harvested to order.", // Description
                            StockQuantity      = 40, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 3.50m, // Price
                            FarmersId          = MossyStoneMushrooms.FarmersId, // Linked to Mossy Stone Mushrooms
                            ImagePath          = "/images/christine-siracusa-XJY1C5LVNn8-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 19
                        { // Start object
                            ProductName        = "Fresh Herb Bundle (Rosemary, Thyme, Sage)", // Name
                            ProductDescription = "A hand-tied bundle of freshly cut rosemary, thyme, and sage from Bramblewood's pesticide-free herb garden. Perfect for roasts and slow-cooked dishes.", // Description
                            StockQuantity      = 110, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.30m, // Price
                            FarmersId          = BramblewoodHerbs.FarmersId, // Linked to Bramblewood Herb Garden
                            ImagePath          = "/images/anne-nygard-eUjzGp1pGws-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 20
                        { // Start object
                            ProductName        = "Dexter Beef Mince (500g)", // Name
                            ProductDescription = "Rich, flavourful mince from slowly reared Dexter cattle at Elmfield. A heritage breed prized for its marbling and depth of taste.", // Description
                            StockQuantity      = 65, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 5.20m, // Price
                            FarmersId          = ElmfieldRareBreeds.FarmersId, // Linked to Elmfield Rare Breeds
                            ImagePath          = "/images/luciano-liu-I8Qxi5Hmxp8-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 21
                        { // Start object
                            ProductName        = "Saddleback Pork Sausages (6 pack)", // Name
                            ProductDescription = "Traditional pork sausages made from Elmfield's Saddleback pigs, reared slowly on open pasture. Succulent, well-seasoned, and utterly satisfying.", // Description
                            StockQuantity      = 75, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 4.80m, // Price
                            FarmersId          = ElmfieldRareBreeds.FarmersId, // Linked to Elmfield Rare Breeds
                            ImagePath          = "/images/marko-anastasijevic-ipcIeLJsoNg-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 22
                        { // Start object
                            ProductName        = "Fresh Watercress (80g)", // Name
                            ProductDescription = "Crisp, peppery watercress grown in the clear spring-fed streams at Lakeside. Harvested each morning and delivered the same day.", // Description
                            StockQuantity      = 160, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 1.00m, // Price
                            FarmersId          = LakesideWatercress.FarmersId, // Linked to Lakeside Watercress Co.
                            ImagePath          = "/images/karolina-kolodziejczak-CNhsp0UcrDc-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 23
                        { // Start object
                            ProductName        = "Raw Wildflower Honey (340g)", // Name
                            ProductDescription = "Unfiltered, unpasteurised honey drawn from Cloverfield's hives across seasonal wildflower meadows. Each jar is unique to the time of year it was harvested.", // Description
                            StockQuantity      = 55, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 5.75m, // Price
                            FarmersId          = CloverfieldHoney.FarmersId, // Linked to Cloverfield Honey Farm
                            ImagePath          = "/images/art-rachen-Asj5DFw8UAw-unsplash.jpg" // Image path
                        }, // End object
                        new Products() // Product 24
                        { // Start object
                            ProductName        = "Weekly Veg Box (Mixed)", // Name
                            ProductDescription = "A seasonal selection of freshly harvested root vegetables, salad leaves, and brassicas from Ironwood's no-dig market garden. Contents vary by week.", // Description
                            StockQuantity      = 45, // Stock
                            IsAvailable        = true, // Availability
                            ProductPrice       = 9.50m, // Price
                            FarmersId          = IronwoodMarketGarden.FarmersId, // Linked to Ironwood Market Garden
                            ImagePath          = "/images/randy-fath-5aJVJvJ9rG8-unsplash.jpg" // Image path
                        } // End object

                    }; // End list

                await context.Products.AddRangeAsync(products); // Adds products to tracker asynchronously
                await context.SaveChangesAsync(); // Commits products to database
            } // End if

        }

        public static async Task SeedLoyaltyAccounts(IServiceProvider serviceProvider) // Method to create starting loyalty accounts
        { // Start of SeedLoyaltyAccounts
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); // Gets user manager service
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Gets database context

            if (context.LoyaltyAccount.Any()) // Check if loyalty accounts exist
                return; // Exit if already seeded

            var bronzeUser = await userManager.FindByEmailAsync("bronze@example.com"); // Get the bronze identity user
            var silverUser = await userManager.FindByEmailAsync("silver@example.com"); // Get the silver identity user
            var goldUser = await userManager.FindByEmailAsync("gold@example.com");   // Get the gold identity user

            if (bronzeUser == null || silverUser == null || goldUser == null) // Check if users exist
                throw new Exception("Loyalty seed users not found. Ensure SeedUsersAndRoles ran first."); // Error check

            var accounts = new List<LoyaltyAccount> // Create list of loyalty profiles
            { // Start list
                new LoyaltyAccount // Bronze account
                { // Start object
                    UserId    = bronzeUser.Id,                       // Linked ID
                    Points    = 350,                                  // Point balance
                    Tier      = "Bronze",                             // Current tier
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)        // Set creation date to 3 months ago
                }, // End object
                new LoyaltyAccount // Silver account
                { // Start object
                    UserId    = silverUser.Id,                       // Linked ID
                    Points    = 650,                                  // Point balance
                    Tier      = "Silver",                             // Current tier
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)        // Set creation date to 6 months ago
                }, // End object
                new LoyaltyAccount // Gold account
                { // Start object
                    UserId    = goldUser.Id,                         // Linked ID
                    Points    = 1200,                                 // Point balance
                    Tier      = "Gold",                               // Current tier
                    CreatedAt = DateTime.UtcNow.AddMonths(-12)       // Set creation date to 1 year ago
                } // End object
            }; // End list

            context.LoyaltyAccount.AddRange(accounts); // Add accounts to tracker
            await context.SaveChangesAsync(); // Commit accounts to database
        } // End of SeedLoyaltyAccounts
    } // End of class
} // End of namespace