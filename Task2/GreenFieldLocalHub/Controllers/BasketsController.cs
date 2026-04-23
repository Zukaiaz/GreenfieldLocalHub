using System; // Provides fundamental classes like DateTime and base types
using System.Collections.Generic; // Enables the use of generic collections like List<T>
using System.Linq; // Provides LINQ methods like Sum(), Where(), and FirstOrDefault()
using System.Threading.Tasks; // Supports asynchronous programming with Task and await
using Microsoft.AspNetCore.Mvc; // Provides base Controller classes and IActionResult results
using Microsoft.AspNetCore.Mvc.Rendering; // Used for rendering HTML helpers like SelectLists
using Microsoft.EntityFrameworkCore; // Provides the Entity Framework Core ORM for DB queries
using GreenFieldLocalHub.Data; // Imports the ApplicationDbContext for database access
using GreenFieldLocalHub.Models; // Imports the data models (Basket, Products, etc.)
using System.Security.Claims; // Provides tools to access user identity claims
using Microsoft.AspNetCore.Authorization; // Provides the [Authorize] attribute for security

namespace GreenFieldLocalHub.Controllers // Defines the namespace for the Baskets controller
{ // Opens the namespace scope
    public class BasketsController : Controller // Defines the controller for managing shopping baskets
    { // Opens the class scope
        private readonly ApplicationDbContext _context; // Private field to hold the database context instance

        public BasketsController(ApplicationDbContext context) // Constructor that accepts the DB context via Dependency Injection
        { // Opens constructor scope
            _context = context; // Assigns the injected context to the local private variable
        } // Closes constructor scope

        // GET: Baskets
        [HttpGet] // Explicitly marks this as a GET request
        [Authorize(Roles = "Standard,Admin,Developer")] // Allows access only to logged-in users with these roles
        public async Task<IActionResult> Index() // Main action to load the "My Basket" page
        { // Opens Index method scope
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieves the unique ID of the currently logged-in user

            if (userId == null) // Checks if the user's identity cannot be found
            { // Opens null check scope
                return Unauthorized(); // Returns a 401 Unauthorized status if the user isn't identified
            } // Closes null check scope

            var basket = await _context.Basket // Accesses the Basket table in the database
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Finds the first record where Status is active (true) for this user

            if (basket == null) // Logical check if the user doesn't have an active basket yet
            { // Opens create-basket scope
                { // Opens inner scope
                    basket = new Basket // Instantiates a new Basket object
                    { // Opens object initializer
                        Status = true, // Sets the status to active so it appears as their current cart
                        UserId = userId, // Assigns the basket to the logged-in user's ID
                        BasketCreatedAt = DateTime.UtcNow // Sets the creation timestamp to the current UTC time
                    }; // Closes object initializer

                    _context.Basket.Add(basket); // Commands the context to track this new basket for insertion
                    await _context.SaveChangesAsync(); // Executes the SQL INSERT command and generates the BasketId
                } // Closes inner scope
            } // Closes create-basket scope

            var basketProducts = await _context.BasketProducts // Accesses the linking table between Baskets and Products
                .Where(x => x.BasketId == basket.BasketId) // Filters for rows belonging to the user's current basket
                .Include(x => x.Basket) // Performs a SQL JOIN to get the related Basket record
                .Include(x => x.Products) // Performs a SQL JOIN to get the related Product details (Price/Name)
                .ToListAsync(); // Executes the query and returns the results as a list

            decimal subtotal = 0m; // Declares a decimal variable to store the pre-discount total

            foreach (var basketProduct in basketProducts) // Iterates through every line item in the basket list
            { // Opens calculation loop
                var productTotal = basketProduct.Products.ProductPrice * basketProduct.ProductQuantity; // Calculates cost for this specific item (Price x Qty)
                subtotal += productTotal; // Adds the line item total to the overall running subtotal
            } // Closes calculation loop

            // Get the users loyalty account
            var loyaltyAccount = await _context.LoyaltyAccount // Accesses the LoyaltyAccount table
                .FirstOrDefaultAsync(x => x.UserId == userId); // Finds the loyalty record associated with this user

            // Work out discount based on their tier
            decimal discountPercent = 0m; // Declares a decimal to hold the percentage (e.g., 0.10 for 10%)

            if (loyaltyAccount != null) // Checks if a loyalty record exists for this user
            { // Opens loyalty check
                discountPercent = loyaltyAccount.Tier switch // Uses C# switch expression to determine the discount rate
                { // Opens switch cases
                    "Bronze" => 0.05m,  // Assigns 5% discount for Bronze members
                    "Silver" => 0.10m,  // Assigns 10% discount for Silver members
                    "Gold" => 0.15m,    // Assigns 15% discount for Gold members
                    _ => 0m      // Assigns 0% for any other value or null tier
                }; // Closes switch cases
            } // Closes loyalty check

            decimal discountAmount = subtotal * discountPercent; // Multiplies the subtotal by the percentage to get the cash savings
            decimal total = subtotal - discountAmount; // Subtracts the savings from the subtotal to get the final bill

            // Pass values to the view
            ViewBag.Subtotal = subtotal; // Transports the subtotal value to the Razor view via ViewBag
            ViewBag.DiscountAmount = discountAmount; // Transports the saved amount value to the Razor view
            ViewBag.Total = total; // Transports the final total value to the Razor view
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None"; // Transports the tier name (or "None") to display to the user

            return View(basketProducts); // Renders the Index view using the list of basket products as the model
        } // Closes Index method scope

        // GET: Baskets/Details/5
        [HttpGet] // Marks this as a GET request
        [Authorize(Roles = "Developer")] // Restricts this administrative view to Developer accounts
        public async Task<IActionResult> Details(int? id) // Loads the details for a specific basket ID
        { // Opens Details scope
            if (id == null) // Checks if the ID parameter was omitted from the URL
            { // Opens null check
                return NotFound(); // Returns a 404 status code
            } // Closes null check

            var basket = await _context.Basket // Accesses the Basket table
                .FirstOrDefaultAsync(m => m.BasketId == id); // Finds the specific basket matching the provided ID

            if (basket == null) // Checks if no basket was found with that ID
            { // Opens null check
                return NotFound(); // Returns a 404 status code
            } // Closes null check

            return View(basket); // Renders the Details view with the single basket record
        } // Closes Details scope

        // GET: Baskets/Create
        [HttpGet] // Marks this as a GET request
        [Authorize(Roles = "Developer")] // Restricts this to Developers
        public IActionResult Create() // Action to load the create form
        { // Opens method
            return RedirectToAction(nameof(Index)); // Instantly redirects to Index as baskets are handled automatically
        } // Closes method

        // POST: Baskets/Create
        [HttpPost] // Marks this as a POST request for data submission
        [ValidateAntiForgeryToken] // Verifies the request contains a valid security token to prevent CSRF
        public async Task<IActionResult> Create([Bind("BasketId,Status,BasketCreatedAt,UserId")] Basket basket) // Receives form data
        { // Opens method
            if (ModelState.IsValid) // Validates the submitted model against its data annotations
            { // Opens validation check
                _context.Add(basket); // Adds the new basket object to the DB context
                await _context.SaveChangesAsync(); // Saves the new record to the database
                return RedirectToAction(nameof(Index)); // Redirects to the index page on success
            } // Closes validation check

            return View(basket); // If validation fails, returns the same view with error messages
        } // Closes method

        // GET: Baskets/Edit/5
        [HttpGet] // Marks this as a GET request
        [Authorize(Roles = "Developer")] // Security restriction for Developers only
        public async Task<IActionResult> Edit(int? id) // Loads the edit form for a basket
        { // Opens method
            if (id == null) // Checks if ID is missing
            { // Opens check
                return NotFound(); // Returns 404
            } // Closes check

            var basket = await _context.Basket.FindAsync(id); // Searches the database for a basket with the matching primary key

            if (basket == null) // Checks if record was found
            { // Opens check
                return NotFound(); // Returns 404
            } // Closes check

            return View(basket); // Renders the Edit view with the basket data
        } // Closes method

        // POST: Baskets/Edit/5
        [HttpPost] // Marks this as a POST request
        [ValidateAntiForgeryToken] // Security token validation
        [Authorize(Roles = "Developer")] // Restricts saving edits to Developers
        public async Task<IActionResult> Edit(int id, [Bind("BasketId,Status,BasketCreatedAt,UserId")] Basket basket) // Handles the edit form submission
        { // Opens method
            if (id != basket.BasketId) // Security check to ensure the ID in URL matches the ID in the submitted form
            { // Opens mismatch check
                return NotFound(); // Returns 404 if there is a data mismatch
            } // Closes mismatch check

            if (ModelState.IsValid) // Validates the edited data
            { // Opens validation check
                try // Begins a try block to catch database concurrency errors
                { // Opens try
                    _context.Update(basket); // Marks the basket record as modified in the context
                    await _context.SaveChangesAsync(); // Commits the updates to the database
                } // Closes try
                catch (DbUpdateConcurrencyException) // Triggered if the record was modified by another user simultaneously
                { // Opens catch
                    if (!BasketExists(basket.BasketId)) // Helper check to see if the record was actually deleted
                    { // Opens check
                        return NotFound(); // Returns 404
                    } // Closes check
                    else // If the record exists but a different DB error occurred
                    { // Opens else
                        throw; // Rethrows the error to be handled by the global error handler
                    } // Closes else
                } // Closes catch
                return RedirectToAction(nameof(Index)); // Redirects back to the index on success
            } // Closes validation check

            return View(basket); // Returns the view with errors if validation failed
        } // Closes method

        // GET: Baskets/Delete/5
        [HttpGet] // Marks as a GET request
        [Authorize(Roles = "Developer")] // Security restriction
        public async Task<IActionResult> Delete(int? id) // Loads the delete confirmation page
        { // Opens method
            if (id == null) // Checks for missing ID
            { // Opens check
                return NotFound(); // Returns 404
            } // Closes check

            var basket = await _context.Basket // Accesses table
                .FirstOrDefaultAsync(m => m.BasketId == id); // Finds the specific basket

            if (basket == null) // Checks if not found
            { // Opens check
                return NotFound(); // Returns 404
            } // Closes check

            return View(basket); // Renders confirmation view
        } // Closes method

        // POST: Baskets/Delete/5
        [HttpPost, ActionName("Delete")] // Marks as POST and maps the "Delete" action name to this method
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Final deletion process
        { // Opens method
            var basket = await _context.Basket.FindAsync(id); // Retrieves the record one last time
            if (basket != null) // Checks if record exists
            { // Opens check
                _context.Basket.Remove(basket); // Marks record for removal
            } // Closes check

            await _context.SaveChangesAsync(); // Deletes record from the DB
            return RedirectToAction(nameof(Index)); // Returns to list
        } // Closes method

        private bool BasketExists(int id) // Helper method to verify record existence
        { // Opens helper
            return _context.Basket.Any(e => e.BasketId == id); // Returns true if any basket matches the ID
        } // Closes helper

        // GET: Baskets/GetTotals
        [HttpGet] // GET request usually used by JavaScript (Fetch/AJAX)
        public async Task<IActionResult> GetTotals() // Calculates basket totals for dynamic UI updates
        { // Opens method
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Identifies the current user
            if (userId == null) return Unauthorized(); // Rejection if not logged in

            var basket = await _context.Basket // Finds active basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Locates active cart

            if (basket == null) // Rejection if no cart exists
                return Json(new { subtotal = "0.00", discountAmount = "0.00", total = "0.00" }); // Returns zeroes in JSON format

            var basketProducts = await _context.BasketProducts // Gets cart items
                .Where(x => x.BasketId == basket.BasketId) // Filters by ID
                .Include(x => x.Products) // Joins pricing info
                .ToListAsync(); // Converts to list

            decimal subtotal = basketProducts.Sum(x => x.Products.ProductPrice * x.ProductQuantity); // LINQ method to sum up all line item costs

            var loyaltyAccount = await _context.LoyaltyAccount // Checks loyalty
                .FirstOrDefaultAsync(x => x.UserId == userId); // Finds record

            decimal discountPercent = loyaltyAccount?.Tier switch // Determines discount rate
            { // Opens switch
                "Bronze" => 0.05m, // 5%
                "Silver" => 0.10m, // 10%
                "Gold" => 0.15m, // 15%
                _ => 0m // 0%
            }; // Closes switch

            decimal discountAmount = subtotal * discountPercent; // Calculates cash value of discount
            decimal total = subtotal - discountAmount; // Calculates final payable amount

            return Json(new // Returns a JSON object to the client for instant UI update
            { // Opens JSON object
                subtotal = subtotal.ToString("0.00"), // Formats number with 2 decimal places
                discountAmount = discountAmount.ToString("0.00"), // Formats discount as string
                total = total.ToString("0.00") // Formats total as string
            }); // Closes JSON object
        } // Closes method
    } // Closes class scope
} // Closes namespace scope