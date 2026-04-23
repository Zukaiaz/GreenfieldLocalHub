using System.Diagnostics; // Provides classes for interacting with system processes and diagnostic tracing
using GreenFieldLocalHub.Models; // Imports the data models used by the application (e.g., Products, ErrorViewModel)
using Microsoft.AspNetCore.Mvc; // Provides the base Controller class and action result types
using GreenFieldLocalHub.Data; // Accesses the application's database context layer
using Microsoft.EntityFrameworkCore; // Provides Entity Framework extension methods like Include and ToListAsync

namespace GreenFieldLocalHub.Controllers // Groups the home-related logic within the controller namespace
{ // Opens the namespace scope
    public class HomeController : Controller // Defines the main controller for the landing pages and general site navigation
    { // Opens the class scope
        private readonly ILogger<HomeController> _logger; // Declares a private field for logging events and errors
        private readonly ApplicationDbContext _context; // Declares a private field for the database context

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context) // Constructor that injects services via Dependency Injection
        { // Opens the constructor scope
            _logger = logger; // Assigns the injected logger service to the private field
            _context = context; // Assigns the injected database context to the private field
        } // Closes the constructor scope

        [HttpGet] // Specifies that this action responds only to standard browser GET requests
        public async Task<IActionResult> Index() // Asynchronously prepares the homepage with featured content
        { // Opens the Index method scope
            var featuredProducts = await _context.Products // Accesses the Products table in the database
                .Include(p => p.Farmers) // Performs a SQL JOIN to pull in the details of the farmer who owns each product
                .OrderBy(p => Guid.NewGuid()) // Sorts the products randomly by assigning a unique temporary ID to each row
                .Take(4) // Limits the result set to only the first four products from the randomized list
                .ToListAsync(); // Executes the query and converts the results into a List for the view

            return View(featuredProducts); // Renders the homepage view and passes the 4 random products as the model
        } // Closes the Index method scope

        [HttpGet] // Specifies that this action responds to GET requests
        public IActionResult Privacy() // Loads the static privacy policy information page
        { // Opens the Privacy method scope
            return View(); // Returns the default Privacy view to the user's browser
        } // Closes the Privacy method scope

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Disables browser and server caching for this specific page
        [HttpGet] // Specifies that this is a GET request
        public IActionResult Error() // Handles and displays application errors and system crashes
        { // Opens the Error method scope
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Creates an error model with a tracking ID and sends it to the Error view
        } // Closes the Error method scope
    } // Closes the class scope
} // Closes the namespace scope