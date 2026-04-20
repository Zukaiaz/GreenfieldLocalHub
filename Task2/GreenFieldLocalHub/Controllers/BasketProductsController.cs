using System; // Imports basic system functions like DateTime
using System.Collections.Generic; // Imports support for lists and collections
using System.Linq; // Imports data querying tools (like .Where or .Any)
using System.Threading.Tasks; // Imports support for asynchronous programming (async/await)
using Microsoft.AspNetCore.Mvc; // Imports the MVC framework (Controllers, Views, etc.)
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for HTML dropdowns (SelectList)
using Microsoft.EntityFrameworkCore; // Imports the database ORM (Entity Framework)
using GreenFieldLocalHub.Data; // Imports your project's specific data folder
using GreenFieldLocalHub.Models; // Imports your data models (Basket, Product, etc.)
using System.Security.Claims; // Imports tools to identify the logged-in user

namespace GreenFieldLocalHub.Controllers // Defines the organized "box" where this code lives
{ // Start of the namespace
    public class BasketProductsController : Controller // Defines the controller class for basket items
    { // Start of the class
        private readonly ApplicationDbContext _context; // Declares a private variable to hold the DB connection

        public BasketProductsController(ApplicationDbContext context) // Constructor that runs when the controller is made
        { // Start of constructor
            _context = context; // Stores the database connection into the private variable for later use
        } // End of constructor

        [HttpGet] // Defines this as a GET request (loading a page)

        //GET: BasketProducts
        public async Task<IActionResult> Index() // Method to show the list of all items in baskets
        { // Start of Index
            var applicationDbContext = _context.BasketProducts.Include(b => b.Basket).Include(b => b.Products); // Prepares a query to get basket items + their related details
            return View(await applicationDbContext.ToListAsync()); // Executes the query and sends the list to the View
        } // End of Index

        public async Task<IActionResult> Details(int? id) // Method to show details for one specific item
        { // Start of Details
            if (id == null) // Check if the user didn't provide an ID
            { // Start if
                return NotFound(); // Return a 404 error page
            } // End if

            var basketProducts = await _context.BasketProducts // Start looking in the BasketProducts table
                .Include(b => b.Basket) // Join the Basket table to see owner info
                .Include(b => b.Products) // Join the Products table to see names/prices
                .FirstOrDefaultAsync(m => m.BasketProductsId == id); // Find the first item that matches the ID

            if (basketProducts == null) // Check if no item was found with that ID
            { // Start if
                return NotFound(); // Return a 404 error page
            } // End if

            return View(basketProducts); // Send the found item data to the Details View
        } // End of Details


        //Get:BasketProducts/Create
        public IActionResult Create() // Method to load the "Create" page
        { // Start of Create
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId"); // Create a list of Baskets for a dropdown menu
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId"); // Create a list of Products for a dropdown menu
            return View(); // Show the form to the user
        } // End of Create


        //POST:BasketProducts/Create
        [HttpPost] // Defines this as a POST request (sending data to the server)
        [ValidateAntiForgeryToken] // Prevents hackers from submitting forms from other sites
        public async Task<IActionResult> Create(int ProductsId) // Method to handle adding a product to the basket
        { // Start of Create logic
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductsId == ProductsId); // Look for the product in the database

            if (product == null) // Check if the product actually exists
            { // Start if
                return NotFound(); // Return 404 if product is missing
            } // End if

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the unique ID of the currently logged-in user

            if (userId == null) // Check if the user is not logged in
            { // Start if
                return Unauthorized(); // Stop them and return a 401 Unauthorized error
            } // End if

            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status == true); // Find the user's current "Active" basket

            if (basket == null) // If the user doesn't have an active basket yet
            { // Start if
                basket = new Basket // Prepare a new Basket object
                { // Start object assignment
                    Status = true, // Set the basket as active
                    UserId = userId, // Assign it to the current user
                    BasketCreatedAt = DateTime.UtcNow, // Set the creation time to right now
                }; // End object assignment

                _context.Basket.Add(basket); // Tell the database we want to add this new basket
                await _context.SaveChangesAsync(); // Save it now so we get a BasketId back from the DB
            } // End if

            var basketProduct = await _context.BasketProducts // Check if this specific product is already in the basket
                .FirstOrDefaultAsync(bp => bp.BasketId == basket.BasketId && bp.ProductsId == ProductsId); // Match by BasketId and ProductId

            if (basketProduct != null) // If the product is already in the basket
            { // Start if
                basketProduct.ProductQuantity++; // Simply add 1 to the existing quantity
            } // End if
            else // If the product is NOT in the basket yet
            { // Start else
                basketProduct = new BasketProducts // Create a new record for this product-basket link
                { // Start object assignment
                    BasketId = basket.BasketId, // Link it to the active basket
                    ProductsId = ProductsId, // Link it to the chosen product
                    ProductQuantity = 1 // Set the initial quantity to 1
                }; // End object assignment

                _context.BasketProducts.Add(basketProduct); // Tell the DB to add this new item record

            } // End else

            await _context.SaveChangesAsync(); // Save all changes (updates or additions) to the database

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Check if this was a background "AJAX" call
                return Ok(); // Return a 200 OK status instead of a full page

            return RedirectToAction("Index", "Baskets"); // Otherwise, redirect the user to the main Basket page

        } // End of Create logic

        // GET: BasketProducts/Edit/5
        public async Task<IActionResult> Edit(int? id) // Method to load the Edit page for an item
        { // Start of Edit
            if (id == null) // Check if ID is missing
            { // Start if
                return NotFound(); // Return 404
            } // End if

            var basketProducts = await _context.BasketProducts.FindAsync(id); // Find the item directly by its ID
            if (basketProducts == null) // If the item doesn't exist
            { // Start if
                return NotFound(); // Return 404
            } // End if
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId); // Load basket dropdown with current one selected
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", basketProducts.ProductsId); // Load product dropdown with current one selected
            return View(basketProducts); // Show the Edit form
        } // End of Edit



        // POST: BasketProducts/Edit/5
        [HttpPost] // POST request to save edits
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> Edit(int id, [Bind("BasketProductsId,BasketId,ProductsId,ProductQuantity")] BasketProducts basketProducts) // Method to save changes
        { // Start of Edit save
            if (id != basketProducts.BasketProductsId) // Security check: does the URL ID match the Form ID?
            { // Start if
                return NotFound(); // Return 404 if they don't match
            } // End if

            if (ModelState.IsValid) // Check if all data (like quantity) follows the rules
            { // Start if
                try // Try to save to the database
                { // Start try
                    _context.Update(basketProducts); // Mark the record as updated
                    await _context.SaveChangesAsync(); // Commit the update to the DB
                } // End try
                catch (DbUpdateConcurrencyException) // If two people edited at once and it failed
                { // Start catch
                    if (!BasketProductsExists(basketProducts.BasketProductsId)) // Check if the item was deleted by someone else
                    { // Start if
                        return NotFound(); // Return 404
                    } // End if
                    else // If it was a different database error
                    { // Start else
                        throw; // Let the error happen and log it
                    } // End else
                } // End catch
                return RedirectToAction(nameof(Index));  // Go back to the list if successful
            } // End if
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId); // Reload dropdowns if validation failed
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", basketProducts.ProductsId); // Reload dropdowns if validation failed
            return View(basketProducts); // Show the form again with error messages
        } // End of Edit save


        // GET: BasketProducts/Delete/5
        public async Task<IActionResult> Delete(int? id) // Method to show the "Are you sure you want to delete?" page
        { // Start of Delete
            if (id == null) // If ID is missing
            { // Start if
                return NotFound(); // Return 404
            } // End if

            var basketProducts = await _context.BasketProducts // Look for the item
                .Include(b => b.Basket) // Include basket info for display
                .Include(b => b.Products) // Include product info for display
                .FirstOrDefaultAsync(m => m.BasketProductsId == id); // Find the first match

            if (basketProducts == null) // If item wasn't found
            { // Start if
                return NotFound(); // Return 404
            } // End if

            return View(basketProducts); // Show the confirmation view
        } // End of Delete

        [HttpPost, ActionName("Delete")] // POST request to actually delete (renamed "Delete" for routing)
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Method to perform the final deletion
        { // Start of DeleteConfirmed
            var basketProducts = await _context.BasketProducts.FindAsync(id); // Find the record one last time
            if (basketProducts != null) // If it still exists
            { // Start if
                _context.BasketProducts.Remove(basketProducts); // Mark it for removal
            } // End if

            await _context.SaveChangesAsync(); // Save the deletion to the database
            return RedirectToAction(nameof(Index)); // Return to the list
        } // End of DeleteConfirmed

        private bool BasketProductsExists(int id) // A helper tool to check if a record exists
        { // Start method
            return _context.BasketProducts.Any(e => e.BasketProductsId == id); // Returns true if it finds any item with that ID
        } // End method

        [HttpPost] // POST request for a background deletion
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteAjax(int id) // Method to delete via JavaScript without page refresh
        { // Start DeleteAjax
            var basketProduct = await _context.BasketProducts.FindAsync(id); // Find the item
            if (basketProduct != null) // If it exists
            { // Start if
                _context.BasketProducts.Remove(basketProduct); // Mark it for removal
                await _context.SaveChangesAsync(); // Save changes
            } // End if

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // If it's an AJAX call
                return Ok(); // Return "Success" status only

            return RedirectToAction("Index", "Baskets"); // Otherwise redirect to the main Baskets page
        } // End DeleteAjax
    } // End of class
} // End of namespace