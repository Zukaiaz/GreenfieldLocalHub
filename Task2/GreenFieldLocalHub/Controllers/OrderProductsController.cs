using System; // Imports basic system types and functions
using System.Collections.Generic; // Imports support for lists and collections
using System.Linq; // Imports data querying tools like .Any()
using System.Threading.Tasks; // Imports support for asynchronous tasks (async/await)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for SelectLists (dropdown menus)
using Microsoft.EntityFrameworkCore; // Imports the database engine for C#
using GreenFieldLocalHub.Data; // Imports your project's database context
using GreenFieldLocalHub.Models; // Imports your data models (OrderProducts, etc.)
using Microsoft.AspNetCore.Authorization; // Imports security tools for page access

namespace GreenFieldLocalHub.Controllers // Defines the container for this controller
{ // Start of namespace
    [Authorize(Roles = "Developer")] // Security: Restricts access to all actions in this controller to users in the 'Developer' role
    public class OrderProductsController : Controller // Defines the class that links products to specific orders
    { // Start of class
        private readonly ApplicationDbContext _context; // Declares a private variable for the database connection

        public OrderProductsController(ApplicationDbContext context) // Constructor: Runs when the controller is created
        { // Start of constructor
            _context = context; // Stores the database connection into the local variable
        } // End of constructor

        // GET: OrderProducts
        [HttpGet] // Specifies that this method handles GET requests
        public async Task<IActionResult> Index() // Method to show a list of all order-product links
        { // Start of Index method
            var applicationDbContext = _context.OrderProducts // Starts a query on the OrderProducts table
                .Include(o => o.Orders) // Joins the Orders table to see order details
                .Include(o => o.Products); // Joins the Products table to see product names/prices
            return View(await applicationDbContext.ToListAsync()); // Executes the query and sends the list to the view
        } // End of Index method

        // GET: OrderProducts/Details/5
        [HttpGet] // Specifies that this method handles GET requests
        public async Task<IActionResult> Details(int? id) // Method to show info for one specific item in an order
        { // Start of Details method
            if (id == null) // Checks if the ID was missing from the URL
            { // Start if
                return NotFound(); // Returns 404 error page
            } // End if

            var orderProducts = await _context.OrderProducts // Searches the OrderProducts table
                .Include(o => o.Orders) // Joins Order info
                .Include(o => o.Products) // Joins Product info
                .FirstOrDefaultAsync(m => m.OrderProductsId == id); // Finds the first record matching the ID
            if (orderProducts == null) // Checks if the database failed to find a record
            { // Start if
                return NotFound(); // Returns 404 error page
            } // End if

            return View(orderProducts); // Sends the data to the Details View
        } // End of Details method

        // GET: OrderProducts/Create
        [HttpGet] // Specifies that this method handles GET requests
        public IActionResult Create() // Method to load the "Add Product to Order" form
        { // Start of Create method
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId"); // Prepares a dropdown of Order IDs
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId"); // Prepares a dropdown of Product IDs
            return View(); // Returns the blank form view
        } // End of Create method

        // POST: OrderProducts/Create
        [HttpPost] // Defines this as a POST request (saving data)
        [ValidateAntiForgeryToken] // Security check to prevent cross-site request forgery
        public async Task<IActionResult> Create([Bind("OrderProductsId,OrdersId,ProductsId,ProductsQuantity")] OrderProducts orderProducts) // Logic to save a new item to an order
        { // Start of Create POST method
            if (ModelState.IsValid) // Checks if the submitted data follows the model rules
            { // Start if
                _context.Add(orderProducts); // Prepares the new record for the database
                await _context.SaveChangesAsync(); // Saves the new record to the DB
                return RedirectToAction(nameof(Index)); // Returns user to the list page
            } // End if
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", orderProducts.OrdersId); // Reloads Order dropdown on error
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", orderProducts.ProductsId); // Reloads Product dropdown on error
            return View(orderProducts); // Shows the form again with error messages
        } // End of Create POST method

        // GET: OrderProducts/Edit/5
        [HttpGet] // Specifies that this method handles GET requests
        public async Task<IActionResult> Edit(int? id) // Method to load the edit form
        { // Start of Edit method
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var orderProducts = await _context.OrderProducts.FindAsync(id); // Directly looks for the record by ID
            if (orderProducts == null) // If the record doesn't exist
            { // Start if
                return NotFound(); // Returns 404
            } // End if
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", orderProducts.OrdersId); // Pre-selects the Order in the dropdown
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", orderProducts.ProductsId); // Pre-selects the Product in the dropdown
            return View(orderProducts); // Returns the edit form with current data
        } // End of Edit method

        // POST: OrderProducts/Edit/5
        [HttpPost] // Defines this as a POST request
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> Edit(int id, [Bind("OrderProductsId,OrdersId,ProductsId,ProductsQuantity")] OrderProducts orderProducts) // Logic to save changes
        { // Start of Edit POST method
            if (id != orderProducts.OrderProductsId) // Security check: Does the URL ID match the Form ID?
            { // Start if
                return NotFound(); // Returns 404 if mismatch
            } // End if

            if (ModelState.IsValid) // Checks if the edited data is valid
            { // Start if
                try // Tries to update the database
                { // Start try
                    _context.Update(orderProducts); // Marks the record as modified
                    await _context.SaveChangesAsync(); // Saves the update to the DB
                } // End try
                catch (DbUpdateConcurrencyException) // Handles errors if record was changed elsewhere
                { // Start catch
                    if (!OrderProductsExists(orderProducts.OrderProductsId)) // Checks if record was deleted during edit
                    { // Start if
                        return NotFound(); // Returns 404
                    } // End if
                    else // If a different error occurred
                    { // Start else
                        throw; // Rethrows the error
                    } // End else
                } // End catch
                return RedirectToAction(nameof(Index)); // Goes back to the list on success
            } // End if
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", orderProducts.OrdersId); // Reloads dropdowns on error
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", orderProducts.ProductsId); // Reloads dropdowns on error
            return View(orderProducts); // Stays on the form to show errors
        } // End of Edit POST method

        // GET: OrderProducts/Delete/5
        [HttpGet] // Specifies that this method handles GET requests
        public async Task<IActionResult> Delete(int? id) // Method to load the delete confirmation page
        { // Start of Delete method
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var orderProducts = await _context.OrderProducts // Searches for the record
                .Include(o => o.Orders) // Joins order info
                .Include(o => o.Products) // Joins product info
                .FirstOrDefaultAsync(m => m.OrderProductsId == id); // Finds the matching record
            if (orderProducts == null) // If not found
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            return View(orderProducts); // Shows the confirmation page
        } // End of Delete method

        // POST: OrderProducts/Delete/5
        [HttpPost, ActionName("Delete")] // POST request mapped to the "Delete" confirmation button
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Final logic to remove the item
        { // Start of DeleteConfirmed method
            var orderProducts = await _context.OrderProducts.FindAsync(id); // Finds the record one last time
            if (orderProducts != null) // If it still exists
            { // Start if
                _context.OrderProducts.Remove(orderProducts); // Marks it for deletion
            } // End if

            await _context.SaveChangesAsync(); // Saves the removal in the database
            return RedirectToAction(nameof(Index)); // Goes back to the list
        } // End of DeleteConfirmed method

        private bool OrderProductsExists(int id) // Private tool to check if a record is in the DB
        { // Start helper method
            return _context.OrderProducts.Any(e => e.OrderProductsId == id); // Returns true if ID is found
        } // End helper method
    } // End of class
} // End of namespace