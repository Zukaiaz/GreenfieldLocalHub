using GreenFieldLocalHub.Data; // Imports the database context namespace
using Microsoft.AspNetCore.Authorization; // Imports tools to restrict access to pages
using Microsoft.AspNetCore.Mvc; // Imports the standard Model-View-Controller classes
using Microsoft.EntityFrameworkCore; // Imports the database engine for queries (Include, FirstOrDefault)
using System.Security.Claims; // Imports the ability to read user login information (User ID)

namespace GreenFieldLocalHub.Controllers // Defines the container for this specific controller
{ // Start of namespace
    [Authorize(Roles = "Farmer")] // Attribute: Restricts this entire controller to users with the 'Farmer' role
    public class FarmerDashboardController : Controller // Defines the class for the Farmer Dashboard
    { // Start of class

        public readonly ApplicationDbContext _context; // Declares a private variable for the database connection

        public FarmerDashboardController(ApplicationDbContext context) // Constructor: Runs when the controller is created
        { // Start of constructor
            _context = context; // Stores the database connection into the local variable
        } // End of constructor

        [HttpGet] // GET: Identifies this as an action that only retrieves and displays data
        public async Task<IActionResult> Index() // Method to load the main Farmer Dashboard page
        { // Start of Index method
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Fetches the unique ID of the currently logged-in user

            var farmers = await _context.Farmers // Starts a search in the 'Farmers' table
                .FirstOrDefaultAsync(s => s.UserId == userId); // Finds the first farmer record linked to this login ID

            if (farmers == null) // Checks if the logged-in user doesn't have a matching Farmer profile
            { // Start if
                return NotFound(); // Returns a 404 error page if no farmer record is found
            } // End if

            var products = await _context.Products // Starts a search in the 'Products' table
                .Where(x => x.FarmersId == farmers.FarmersId) // Filters the list to show only products belonging to this farmer
                .ToListAsync(); // Executes the query and converts the results into a List

            var orders = await _context.Orders // Starts a search in the 'Orders' table
                .Include(o => o.OrderProducts) // Joins the 'OrderProducts' table to the query
                .ThenInclude(op => op.Products) // Then joins the 'Products' table to check who the seller is
                .Where(o => o.OrderProducts // Filters the orders
                    .Any(op => op.Products.FarmersId == farmers.FarmersId)) // Only includes orders containing products from this farmer
                .ToListAsync(); // Executes the complex query and converts it to a List

            ViewBag.TotalProducts = products.Count; // Stores the count of all the farmer's products in ViewBag
            ViewBag.LowStockCount = products.Count(x => x.StockQuantity <= 5); // Counts how many products have 5 or less in stock
            ViewBag.RecentOrders = orders; // Stores the list of filtered orders in ViewBag for the display

            return View(products); // Sends the list of products to the Dashboard View
        } // End of Index method
    } // End of class
} // End of namespace