using GreenFieldLocalHub.Data; // Accesses the application's database context (tables and data layer)
using Microsoft.AspNetCore.Authorization; // Provides attributes to restrict access based on roles or identity
using Microsoft.AspNetCore.Identity; // Provides tools for managing users, passwords, and profile data
using Microsoft.AspNetCore.Mvc; // Provides the base Controller and action result functionality for the web app
using Microsoft.EntityFrameworkCore; // Provides the Entity Framework Core ORM methods (ToListAsync, Include, etc.)
using System.Security.Claims; // Enables the retrieval of information (claims) about the logged-in user

namespace GreenFieldLocalHub.Controllers // Groups the dashboard logic within the project's controller namespace
{ // Opens the namespace scope
    [Authorize(Roles = "Farmer,Admin,Developer")] // Restricts access to this entire class so only users with the 'Farmer' role can enter
    public class FarmerDashboardController : Controller // Defines the controller responsible for the farmer's private overview page
    { // Opens the class scope

        public readonly ApplicationDbContext _context; // Declares a private field to hold the database connection instance

        private readonly UserManager<IdentityUser> _userManager; // Declares a private field to interact with the ASP.NET Identity system

        public FarmerDashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager) // Constructor using Dependency Injection
        { // Opens the constructor scope
            _context = context; // Assigns the injected database context to the local field for query use
            _userManager = userManager; // Assigns the injected user manager to handle user-related lookups
        } // Closes the constructor scope


        [HttpGet] // Specifies that this method handles standard browser requests to view the page
        public async Task<IActionResult> Index() // Asynchronously retrieves and prepares data for the main dashboard view
        { // Opens the Index method scope
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Extracts the unique ID (Subject) of the currently authenticated user

            var farmers = await _context.Farmers // Accesses the Farmers table in the database
                .FirstOrDefaultAsync(s => s.UserId == userId); // Finds the first record where the UserId matches the logged-in user

            if (farmers == null) // Checks if the logged-in user exists in Identity but lacks a Farmer profile record
            { // Opens the error-check scope
                return NotFound(); // Returns a 404 status code if the user is not correctly registered as a farmer
            } // Closes the error-check scope

            var products = await _context.Products // Accesses the Products table to load the farmer's inventory
                .Where(x => x.FarmersId == farmers.FarmersId) // Filters for products where the seller ID matches this farmer
                .ToListAsync(); // Executes the SQL query and converts the results into a memory-resident List

            var orders = await _context.Orders // Accesses the Orders table to check for customer purchases
                .Include(o => o.OrderProducts) // Joins the many-to-many OrderProducts table to the query
                .ThenInclude(op => op.Products) // Joins the Products table to the order items so we can see who sold them
                .Where(o => o.OrderProducts // Applies a filter based on the contents of the order
                    .Any(op => op.Products.FarmersId == farmers.FarmersId)) // Only retrieves orders that contain at least one item from this farmer
                .ToListAsync(); // Executes the join-heavy query and returns the list of relevant orders

            ViewBag.TotalProducts = products.Count; // Calculates the number of items in the products list and passes it to the UI
            ViewBag.LowStockCount = products.Count(x => x.StockQuantity <= 5); // Filters and counts products with 5 or fewer units remaining
            ViewBag.RecentOrders = orders; // Passes the entire list of filtered orders to the view via the dynamic ViewBag

            var users = await _userManager.Users.ToListAsync(); // Retrieves the full list of registered accounts from the Identity database
            ViewBag.UserEmails = users.ToDictionary(u => u.Id, u => u.Email); // Creates a fast lookup table (ID to Email) for display on the page
            ViewBag.TotalStock = products.Sum(x => x.StockQuantity); // Calculates the total sum of all stock units across all listed products

            return View(products); // Renders the dashboard view, providing the list of products as the primary data model
        } // Closes the Index method scope
    } // Closes the class scope
} // Closes the namespace scope