using System; // Provides fundamental classes and base types like DateTime
using System.Collections.Generic; // Contains interfaces and classes for defining collections
using System.Linq; // Provides classes and interfaces that support Language-Integrated Query
using System.Threading.Tasks; // Provides types that simplify writing concurrent and asynchronous code
using Microsoft.AspNetCore.Mvc; // Provides the Model-View-Controller framework for building web apps
using Microsoft.AspNetCore.Mvc.Rendering; // Provides tools to render HTML elements like dropdown lists
using Microsoft.EntityFrameworkCore; // Provides the Entity Framework Core ORM for database operations
using GreenFieldLocalHub.Data; // Accesses the application's database context and data layer
using GreenFieldLocalHub.Models; // Accesses the data models like Products, Basket, and BasketProducts
using System.Security.Claims; // Provides classes to handle user identity and claim types
using Microsoft.AspNetCore.Authorization; // Provides attributes for role-based and policy-based access control

namespace GreenFieldLocalHub.Controllers // Groups the related controller logic under a specific namespace
{ // Opens the namespace
    public class BasketProductsController : Controller // Defines the controller responsible for managing basket line items
    { // Opens the class
        private readonly ApplicationDbContext _context; // Declares a private field for the database context

        public BasketProductsController(ApplicationDbContext context) // Injecting the database context through the constructor
        { // Opens the constructor
            _context = context; // Assigns the injected context to the private class field
        } // Closes the constructor

        [HttpGet] // Specifies that this action only responds to HTTP GET requests
        [Authorize(Roles = "Developer")] // Restricts access to this page to users with the 'Developer' role
        public async Task<IActionResult> Index() // Asynchronously retrieves all basket items for display
        { // Opens the method
            var applicationDbContext = _context.BasketProducts.Include(b => b.Basket).Include(b => b.Products); // Loads basket items while joining related Basket and Product data
            return View(await applicationDbContext.ToListAsync()); // Converts the query to a list and sends it to the Index view
        } // Closes the method

        [HttpGet] // Marks this action as a GET request
        [Authorize(Roles = "Developer")] // Ensures only Developers can view individual item details
        public async Task<IActionResult> Details(int? id) // Retrieves details for a specific item based on an ID
        { // Opens the method
            if (id == null) // Checks if no ID was provided in the URL
            { // Opens the if-block
                return NotFound(); // Returns a 404 error if the ID is missing
            } // Closes the if-block

            var basketProducts = await _context.BasketProducts // Queries the BasketProducts table
                .Include(b => b.Basket) // Joins the Basket table to the query
                .Include(b => b.Products) // Joins the Products table to the query
                .FirstOrDefaultAsync(m => m.BasketProductsId == id); // Finds the first record matching the provided ID

            if (basketProducts == null) // Checks if no record was found in the database
            { // Opens the if-block
                return NotFound(); // Returns a 404 error if the record doesn't exist
            } // Closes the if-block

            return View(basketProducts); // Sends the specific item data to the Details view
        } // Closes the method

        [HttpGet] // Marks this as a GET request to load a form
        [Authorize(Roles = "Developer")] // Only allows Developers to access the manual creation form
        public IActionResult Create() // Prepares the page for creating a new basket item
        { // Opens the method
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId"); // Provides a list of Baskets for a dropdown menu
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId"); // Provides a list of Products for a dropdown menu
            return View(); // Displays the Create view
        } // Closes the method

        [HttpPost] // Specifies that this action handles data submission via HTTP POST
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery (CSRF) attacks
        public async Task<IActionResult> Create(int ProductsId) // Processes adding a specific product to a user's basket
        { // Opens the method
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductsId == ProductsId); // Finds the product being added to the basket

            if (product == null) // Checks if the product ID provided actually exists
            { // Opens the if-block
                return NotFound(); // Returns 404 if the product is not found
            } // Closes the if-block

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieves the ID of the currently logged-in user

            if (userId == null) // Checks if the user is not signed in
            { // Opens the if-block
                return Unauthorized(); // Returns 401 if the user is not authenticated
            } // Closes the if-block

            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status == true); // Looks for an active basket for this user

            if (basket == null) // If the user doesn't have an active basket, create one
            { // Opens the if-block
                basket = new Basket // Instantiates a new Basket object
                { // Opens the object initializer
                    Status = true, // Sets the status as active
                    UserId = userId, // Links the basket to the current user
                    BasketCreatedAt = DateTime.UtcNow, // Sets the current UTC time as the creation date
                }; // Closes the initializer

                _context.Basket.Add(basket); // Adds the new basket to the context tracking
                await _context.SaveChangesAsync(); // Saves the new basket to the database to generate an ID
            } // Closes the if-block

            var basketProduct = await _context.BasketProducts // Checks if the product is already in the basket
                .FirstOrDefaultAsync(bp => bp.BasketId == basket.BasketId && bp.ProductsId == ProductsId); // Matches by both basket and product IDs

            if (basketProduct != null) // If the product is already present in the basket
            { // Opens the if-block
                basketProduct.ProductQuantity++; // Increments the quantity of the existing item
            } // Closes the if-block
            else // If the product is being added to this basket for the first time
            { // Opens the else-block
                basketProduct = new BasketProducts // Creates a new linking record
                { // Opens the initializer
                    BasketId = basket.BasketId, // Links it to the active basket
                    ProductsId = ProductsId, // Links it to the specific product
                    ProductQuantity = 1 // Sets the initial quantity to one
                }; // Closes the initializer

                _context.BasketProducts.Add(basketProduct); // Tracks the new record in the context
            } // Closes the else-block

            await _context.SaveChangesAsync(); // Commits all changes (updates or additions) to the database

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Checks if the request was made via AJAX
                return Ok(); // Returns a success status without redirecting for background requests

            return RedirectToAction("Index", "Baskets"); // Redirects the user to their basket page
        } // Closes the method

        [HttpGet] // Marks this as a GET request
        [Authorize(Roles = "Developer")] // Restricts the edit form to Developers
        public async Task<IActionResult> Edit(int? id) // Loads an existing basket item for editing
        { // Opens the method
            if (id == null) // Checks if an ID was provided
            { // Opens the if-block
                return NotFound(); // Returns 404 if the ID is missing
            } // Closes the if-block

            var basketProducts = await _context.BasketProducts.FindAsync(id); // Finds the record directly by its primary key
            if (basketProducts == null) // Checks if the record exists
            { // Opens the if-block
                return NotFound(); // Returns 404 if the record is missing
            } // Closes the if-block
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId); // Pre-selects the current basket in the dropdown
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", basketProducts.ProductsId); // Pre-selects the current product in the dropdown
            return View(basketProducts); // Sends the existing record to the Edit view
        } // Closes the method

        [HttpPost] // Handles the submission of the Edit form
        [ValidateAntiForgeryToken] // Security token check
        public async Task<IActionResult> Edit(int id, [Bind("BasketProductsId,BasketId,ProductsId,ProductQuantity")] BasketProducts basketProducts) // Saves the edited item
        { // Opens the method
            if (id != basketProducts.BasketProductsId) // Checks for ID mismatch between the URL and the model
            { // Opens the if-block
                return NotFound(); // Returns 404 if IDs do not match
            } // Closes the if-block

            if (ModelState.IsValid) // Validates the submitted form data
            { // Opens the if-block
                try // Wraps DB operations to handle potential concurrency issues
                { // Opens the try-block
                    _context.Update(basketProducts); // Informs the context that the object has changed
                    await _context.SaveChangesAsync(); // Asynchronously saves updates to the DB
                } // Closes the try-block
                catch (DbUpdateConcurrencyException) // Catches errors where data was changed by another user simultaneously
                { // Opens the catch-block
                    if (!BasketProductsExists(basketProducts.BasketProductsId)) // Checks if the record was deleted while editing
                    { // Opens the if-block
                        return NotFound(); // Returns 404 if it no longer exists
                    } // Closes the if-block
                    else // If it was a different concurrency error
                    { // Opens the else-block
                        throw; // Re-throws the exception to be handled by higher-level error pages
                    } // Closes the else-block
                } // Closes the catch-block
                return RedirectToAction(nameof(Index)); // Redirects to the list of items
            } // Closes the if-block
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId); // Reloads dropdown data if validation failed
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", basketProducts.ProductsId); // Reloads dropdown data if validation failed
            return View(basketProducts); // Returns to the form to show validation errors
        } // Closes the method

        [HttpGet] // Marks this as a GET request
        [Authorize(Roles = "Developer")] // Only Developers can access the delete confirmation page
        public async Task<IActionResult> Delete(int? id) // Loads a confirmation page for deletion
        { // Opens the method
            if (id == null) // Checks if an ID was provided
            { // Opens the if-block
                return NotFound(); // Returns 404 if ID is missing
            } // Closes the if-block

            var basketProducts = await _context.BasketProducts // Queries the BasketProducts table
                .Include(b => b.Basket) // Includes basket info for the confirmation view
                .Include(b => b.Products) // Includes product info for the confirmation view
                .FirstOrDefaultAsync(m => m.BasketProductsId == id); // Finds the record to be deleted

            if (basketProducts == null) // Checks if the record exists
            { // Opens the if-block
                return NotFound(); // Returns 404 if not found
            } // Closes the if-block

            return View(basketProducts); // Displays the Delete confirmation page
        } // Closes the method

        [HttpPost, ActionName("Delete")] // Processes the final deletion
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Actually removes the record from the DB
        { // Opens the method
            var basketProducts = await _context.BasketProducts.FindAsync(id); // Finds the record one last time
            if (basketProducts != null) // Checks if it still exists
            { // Opens the if-block
                _context.BasketProducts.Remove(basketProducts); // Marks the record for removal from the database
            } // Closes the if-block

            await _context.SaveChangesAsync(); // Saves the deletion to the database
            return RedirectToAction(nameof(Index)); // Returns to the list of items
        } // Closes the method

        private bool BasketProductsExists(int id) // A helper method to verify record existence
        { // Opens the method
            return _context.BasketProducts.Any(e => e.BasketProductsId == id); // Checks if any record matches the given ID
        } // Closes the method

        [HttpPost] // Handles background delete requests
        [ValidateAntiForgeryToken] // Security token check
        public async Task<IActionResult> DeleteAjax(int id) // Removes an item without refreshing the whole page
        { // Opens the method
            var basketProduct = await _context.BasketProducts.FindAsync(id); // Locates the item
            if (basketProduct != null) // If the item exists
            { // Opens the if-block
                _context.BasketProducts.Remove(basketProduct); // Marks the item for deletion
                await _context.SaveChangesAsync(); // Commits the deletion to the database
            } // Closes the if-block

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Detects if the call came from JavaScript
                return Ok(); // Returns success status 200 to the script

            return RedirectToAction("Index", "Baskets"); // Fallback redirect if not an AJAX call
        } // Closes the method

        [HttpPost] // Handles quantity change requests
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> UpdateQuantity(int id, string direction) // Adjusts the quantity up or down
        { // Opens the method
            var basketProduct = await _context.BasketProducts.FindAsync(id); // Retrieves the basket item record

            if (basketProduct == null) // Error handling if the record is missing
            { // Opens the if-block
                return NotFound(); // Returns 404 if item doesn't exist
            } // Closes the if-block

            if (direction == "increase") // Logical check for increasing quantity
            { // Opens the if-block
                basketProduct.ProductQuantity++; // Adds 1 to the quantity field
            } // Closes the if-block
            else if (direction == "decrease") // Logical check for decreasing quantity
            { // Opens the else-if block
                basketProduct.ProductQuantity--; // Subtracts 1 from the quantity field

                if (basketProduct.ProductQuantity <= 0) // Checks if the item should be removed entirely
                { // Opens the if-block
                    _context.BasketProducts.Remove(basketProduct); // Deletes the record if quantity reaches zero
                } // Closes the if-block
            } // Closes the else-if block

            await _context.SaveChangesAsync(); // Commits the updated quantity (or deletion) to the database

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Detects AJAX background calls
                return Ok(); // Returns success code to the script

            return RedirectToAction("Index", "Baskets"); // Standard redirect to the basket page
        } // Closes the method
    } // Closes the class
} // Closes the namespace