using System; // Imports base system types
using System.Collections.Generic; // Imports support for lists and collections
using System.Linq; // Imports data querying tools (Sum, Where, etc.)
using System.Threading.Tasks; // Imports support for async/await
using Microsoft.AspNetCore.Mvc; // Imports MVC controller functionality
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for SelectLists and dropdowns
using Microsoft.EntityFrameworkCore; // Imports the database engine (EF Core)
using GreenFieldLocalHub.Data; // Imports your data context
using GreenFieldLocalHub.Models; // Imports your Basket and Product models
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; // Imports tools to check user identity

namespace GreenFieldLocalHub.Controllers // Defines the container for this controller
{ // Start of namespace
    public class BasketsController : Controller // Defines the Baskets Controller class
    { // Start of class
        private readonly ApplicationDbContext _context; // Declares the database context variable

        public BasketsController(ApplicationDbContext context) // Constructor to inject the database context
        { // Start of constructor
            _context = context; // Stores the database connection in the private variable
        } // End of constructor

        // GET: Baskets
        [Authorize(Roles = "Standard,Admin,Developer")]
        public async Task<IActionResult> Index() // Main method to display the user's shopping basket
        { // Start of Index
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the ID of the logged-in user

            if (userId == null) // Checks if the user is not logged in
            { // Start if
                return Unauthorized(); // Stops and returns a 401 error if not logged in
            } // End if

            var basket = await _context.Basket // Searches for the user's current basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Finds an active (Status=true) basket for this user

            if (basket == null) // If no active basket is found in the database
            { // Start if
                { // Start scope
                    basket = new Basket // Create a new basket object
                    { // Start object assignment
                        Status = true, // Sets the new basket as the active one
                        UserId = userId, // Links the basket to the current user
                        BasketCreatedAt = DateTime.UtcNow // Sets the creation timestamp
                    }; // End object assignment

                    _context.Basket.Add(basket); // Adds the new basket to the DB tracking
                    await _context.SaveChangesAsync(); // Saves changes to generate a BasketId
                } // End scope
            } // End if

            var basketProducts = await _context.BasketProducts // Gets all products linked to this basket
                .Where(x => x.BasketId == basket.BasketId) // Filters by the current basket ID
                .Include(x => x.Basket) // Joins the Basket table info
                .Include(x => x.Products) // Joins the Products table info (price, name)
                .ToListAsync(); // Converts the results into a list

            decimal subtotal = 0m; // Initializes the subtotal variable at zero

            foreach (var basketProduct in basketProducts) // Loops through each item in the basket
            { // Start loop
                var productTotal = basketProduct.Products.ProductPrice * basketProduct.ProductQuantity; // Multiplies price by quantity
                subtotal += productTotal; // Adds this product's total to the running subtotal
            } // End loop

            // Get the users loyalty account
            var loyaltyAccount = await _context.LoyaltyAccount // Looks for the user's loyalty details
                .FirstOrDefaultAsync(x => x.UserId == userId); // Finds record matching the user's ID

            // Work out discount based on their tier
            decimal discountPercent = 0m; // Initializes discount percentage at 0%

            if (loyaltyAccount != null) // If the user has a loyalty record
            { // Start if
                discountPercent = loyaltyAccount.Tier switch // Uses a switch expression to choose discount
                { // Start switch
                    "Bronze" => 0.05m,  // 5% off
                    "Silver" => 0.10m,  // 10% off
                    "Gold" => 0.15m,  // 15% off
                    _ => 0m      // No discount for standard users
                }; // End switch
            } // End if

            decimal discountAmount = subtotal * discountPercent; // Calculates how much money is taken off
            decimal total = subtotal - discountAmount; // Subtracts discount from subtotal for the final price

            // Pass values to the view
            ViewBag.Subtotal = subtotal; // Stores subtotal in ViewBag for the HTML page
            ViewBag.DiscountAmount = discountAmount; // Stores discount in ViewBag for the HTML page
            ViewBag.Total = total; // Stores final total in ViewBag for the HTML page
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None"; // Stores tier name or "None" if null

            return View(basketProducts); // Sends the list of items to the Index View
        } // End of Index

        // GET: Baskets/Details/5
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> Details(int? id) // Method to show details for one specific basket
        { // Start of Details
            if (id == null) // Checks if ID is missing from URL
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            var basket = await _context.Basket // Looks for the basket in the database
                .FirstOrDefaultAsync(m => m.BasketId == id); // Finds the record matching the ID
            if (basket == null) // If no basket was found
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            return View(basket); // Sends basket data to the View
        } // End of Details

        // GET: Baskets/Create
        [Authorize(Roles = "Developer")]
        public IActionResult Create() // Method to load the "Create Basket" form
        { // Start of Create
            return RedirectToAction(nameof(Index)); // Returns the blank view
        } // End of Create

        // POST: Baskets/Create
        [HttpPost] // Marks this as a POST request
        [ValidateAntiForgeryToken] // Security check to prevent CSRF attacks
        public async Task<IActionResult> Create([Bind("BasketId,Status,BasketCreatedAt,UserId")] Basket basket) // Method to save a new basket
        { // Start of Create POST
            if (ModelState.IsValid) // Checks if the data submitted is valid
            { // Start if
                _context.Add(basket); // Prepares the basket for addition
                await _context.SaveChangesAsync(); // Saves to the database
                return RedirectToAction(nameof(Index)); // Redirects back to the list
            } // End if

            return View(basket); // If invalid, returns the form with the current data
        } // End of Create POST

        [Authorize(Roles = "Developer")]

        // GET: Baskets/Edit/5
        public async Task<IActionResult> Edit(int? id) // Method to load the edit form for a basket
        { // Start of Edit
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var basket = await _context.Basket.FindAsync(id); // Finds the basket by primary key
            if (basket == null) // If basket doesn't exist
            { // Start if
                return NotFound(); // Returns 404
            } // End if
            return View(basket); // Shows the edit form
        } // End of Edit

        [Authorize(Roles = "Developer")]

        // POST: Baskets/Edit/5
        [HttpPost] // Marks this as a POST request
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> Edit(int id, [Bind("BasketId,Status,BasketCreatedAt,UserId")] Basket basket) // Method to save changes to a basket
        { // Start of Edit POST
            if (id != basket.BasketId) // Security check to ensure IDs match
            { // Start if
                return NotFound(); // Returns 404 if mismatch
            } // End if

            if (ModelState.IsValid) // Checks if edited data is valid
            { // Start if
                try // Tries to update the database
                { // Start try
                    _context.Update(basket); // Marks the basket as modified
                    await _context.SaveChangesAsync(); // Commits changes to the DB
                } // End try
                catch (DbUpdateConcurrencyException) // Handles errors if record was changed elsewhere
                { // Start catch
                    if (!BasketExists(basket.BasketId)) // Checks if the basket was actually deleted
                    { // Start if
                        return NotFound(); // Returns 404
                    } // End if
                    else // If a different error occurred
                    { // Start else
                        throw; // Rethrows the error
                    } // End else
                } // End catch
                return RedirectToAction(nameof(Index)); // Returns to list after success
            } // End if
            return View(basket); // Returns form if data is invalid
        } // End of Edit POST

        [Authorize(Roles = "Developer")]
        // GET: Baskets/Delete/5
        public async Task<IActionResult> Delete(int? id) // Method to show delete confirmation page
        { // Start of Delete
            if (id == null) // If ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var basket = await _context.Basket // Looks for the basket
                .FirstOrDefaultAsync(m => m.BasketId == id); // Finds match by ID
            if (basket == null) // If not found
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            return View(basket); // Shows the confirmation view
        } // End of Delete

        [Authorize(Roles = "Developer")]
        // POST: Baskets/Delete/5
        [HttpPost, ActionName("Delete")] // POST request mapped to the "Delete" action
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Final method to remove the basket
        { // Start of DeleteConfirmed
            var basket = await _context.Basket.FindAsync(id); // Finds the record
            if (basket != null) // If it exists
            { // Start if
                _context.Basket.Remove(basket); // Marks it for deletion
            } // End if

            await _context.SaveChangesAsync(); // Saves the removal to the database
            return RedirectToAction(nameof(Index)); // Returns to the list
        } // End of DeleteConfirmed

        private bool BasketExists(int id) // Private helper tool to verify if an ID exists
        { // Start helper
            return _context.Basket.Any(e => e.BasketId == id); // Returns true if ID is in DB
        } // End helper

        [HttpGet] // Marks this as a GET request for retrieving data
        public async Task<IActionResult> GetTotals() // Method for AJAX calls to update totals instantly
        { // Start GetTotals
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current user's ID
            if (userId == null) return Unauthorized(); // Stops if user is logged out

            var basket = await _context.Basket // Looks for active basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Finds record

            if (basket == null) // If no basket exists
                return Json(new { subtotal = "0.00", discountAmount = "0.00", total = "0.00" }); // Return zeroes as JSON

            var basketProducts = await _context.BasketProducts // Gets the products in the basket
                .Where(x => x.BasketId == basket.BasketId) // Filters by basket
                .Include(x => x.Products) // Includes product data for prices
                .ToListAsync(); // Converts to list

            decimal subtotal = basketProducts.Sum(x => x.Products.ProductPrice * x.ProductQuantity); // Calculates sum of all items

            var loyaltyAccount = await _context.LoyaltyAccount // Gets loyalty info for calculation
                .FirstOrDefaultAsync(x => x.UserId == userId); // Finds by user ID

            decimal discountPercent = loyaltyAccount?.Tier switch // Calculates percentage based on tier
            { // Start switch
                "Bronze" => 0.05m, // 5%
                "Silver" => 0.10m, // 10%
                "Gold" => 0.15m, // 15%
                _ => 0m // 0%
            }; // End switch

            decimal discountAmount = subtotal * discountPercent; // Calculates cash discount value
            decimal total = subtotal - discountAmount; // Calculates final total

            return Json(new // Returns a JSON object for the front-end JavaScript to read
            { // Start object
                subtotal = subtotal.ToString("0.00"), // Formats subtotal as string with 2 decimals
                discountAmount = discountAmount.ToString("0.00"), // Formats discount as string
                total = total.ToString("0.00") // Formats total as string
            }); // End object
        } // End GetTotals

    } // End of class
} // End of namespace