using System.Diagnostics; // Imports diagnostic tools to track process IDs and activity
using GreenFieldLocalHub.Models; // Imports your data models, specifically for the ErrorViewModel
using Microsoft.AspNetCore.Mvc; // Imports the core MVC framework for Controllers and Views

namespace GreenFieldLocalHub.Controllers // Defines the namespace where this controller is organized
{ // Start of namespace
    public class HomeController : Controller // Defines the HomeController class, inheriting from the base Controller
    { // Start of class
        private readonly ILogger<HomeController> _logger; // Declares a private variable for logging events and errors

        public HomeController(ILogger<HomeController> logger) // Constructor that "injects" the logging service into the class
        { // Start of constructor
            _logger = logger; // Stores the logger service in the private variable for use in methods
        } // End of constructor

        [HttpGet] // GET: Standard attribute for retrieving the home page
        public IActionResult Index() // Method that handles requests for the website's homepage
        { // Start of Index
            return View(); // Returns the "Index.cshtml" view to the user's browser
        } // End of Index

        [HttpGet] // GET: Standard attribute for retrieving the privacy policy page
        public IActionResult Privacy() // Method that handles requests for the Privacy page
        { // Start of Privacy
            return View(); // Returns the "Privacy.cshtml" view to the user's browser
        } // End of Privacy

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Configures the browser NOT to cache this page (since errors change)
        [HttpGet] // GET: Standard attribute for retrieving the error page
        public IActionResult Error() // Method that handles application errors and shows the Error page
        { // Start of Error
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Creates an error model with a unique Request ID and sends it to the View
        } // End of Error
    } // End of class
} // End of namespace