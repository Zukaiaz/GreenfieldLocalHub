using System; // Imports basic system functionality
using System.Collections.Generic; // Imports support for Lists and Collections
using System.Linq; // Imports LINQ for filtering and selecting data
using System.Threading.Tasks; // Imports support for asynchronous programming
using Microsoft.AspNetCore.Mvc; // Imports MVC controller and action result classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for rendering HTML elements like dropdowns
using Microsoft.EntityFrameworkCore; // Imports Entity Framework for database operations
using GreenFieldLocalHub.Data; // Imports your ApplicationDbContext
using GreenFieldLocalHub.Models; // Imports your data models
using System.Security.Claims; // Imports tools to retrieve the logged-in user's ID
using Microsoft.AspNetCore.Authorization; // Imports authorization attributes
using Microsoft.AspNetCore.Identity; // Imports Identity management tools

namespace GreenFieldLocalHub.Controllers // Defines the namespace for this controller
{ // Start of namespace
    [Authorize] // Security: Ensures only logged-in users can access any part of this controller
    public class OrdersController : Controller // Defines the OrdersController class inheriting from Controller
    { // Start of class
        private readonly ApplicationDbContext _context; // Private variable for database access
        private readonly UserManager<IdentityUser> _userManager; // Private variable for managing user data

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager) // Constructor to inject dependencies
        { // Start of constructor
            _context = context; // Assigns the injected context to the private variable
            _userManager = userManager; // Assigns the injected user manager to the private variable
        } // End of constructor

        // GET: Orders
        [HttpGet] // Defines this as a GET request to view the order list
        public async Task<IActionResult> Index() // Method to list orders based on user role
        { // Start of Index
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the unique ID of the current user
            if (userId == null) // Checks if the user session is valid
            { // Start if
                return Unauthorized(); // Returns a 401 Unauthorized status
            } // End if
            if (User.IsInRole("Admin")) // Logic for Administrator users
            { // Start if Admin
                var allOrders = await _context.Orders // Queries all orders in the system
                    .Include(o => o.OrderProducts) // Includes the link to products
                    .ThenInclude(op => op.Products) // Includes the actual product details
                    .ToListAsync(); // Executes the query and returns the list

                var adminUsers = await _userManager.Users.ToListAsync(); // Gets all registered users from the identity table
                ViewBag.UserEmails = adminUsers.ToDictionary(u => u.Id, u => u.Email); // Builds a userId to email lookup dictionary for the view

                return View(allOrders); // Sends all orders to the view
            } // End if Admin
            else if (User.IsInRole("Farmer")) // Logic for Farmer/Supplier users
            { // Start if Farmer
                var supplierProducts = await _context.Products // Queries the Products table
                    .Where(p => p.Farmers.UserId == userId) // Filters for products belonging to this farmer
                    .Select(p => p.ProductsId) // Only grabs the IDs of those products
                    .ToListAsync(); // Executes query to find supplier products first
                var supplierOrders = await _context.OrderProducts // Queries the linking table
                    .Where(op => supplierProducts.Contains(op.ProductsId)) // Finds rows where the product belongs to this farmer
                    .Include(op => op.Orders) // Joins the parent Order info
                        .ThenInclude(o => o.OrderProducts) // For each order, also loads all its linked OrderProducts rows
                            .ThenInclude(op => op.Products) // For each of those OrderProducts, loads the full product details
                    .Include(op => op.Products) // Joins the Product info
                    .ToListAsync(); // Executes query to find relevant orders

                var farmerUsers = await _userManager.Users.ToListAsync(); // Gets all registered users from the identity table
                ViewBag.UserEmails = farmerUsers.ToDictionary(u => u.Id, u => u.Email); // Builds a userId to email lookup dictionary so the farmer can see whose order it is

                return View(supplierOrders.Select(op => op.Orders).Distinct().ToList()); // Returns a unique list of orders containing the farmer's products
            } // End if Farmer
            else // Logic for standard Customers
            { // Start else
                var userOrders = await _context.Orders // Queries the Orders table
                    .Where(o => o.UserId == userId) // Filters only for orders matching the current user's ID
                    .Include(o => o.OrderProducts) // Includes the products in those orders
                    .ThenInclude(op => op.Products) // Includes full product details
                    .ToListAsync(); // Executes query
                return View(userOrders); // Sends the user's personal order history to the view
            } // End else
        } // End of Index

        // GET: Orders/Details/5
        [HttpGet] // Defines this as a GET request for order details
        public async Task<IActionResult> Details(int? id) // Method to display the full details of a specific order
        { // Start of Details
            if (id == null) // Checks if no ID was provided in the URL
                return NotFound(); // Returns a 404 error if ID is missing

            var orders = await _context.OrderProducts // Queries the OrderProducts linking table
                .Where(op => op.OrdersId == id) // Filters for rows that belong to this specific order ID
                .Include(op => op.Orders) // Joins the parent Order record so we can access order details
                .Include(op => op.Products) // Joins the Product record so we can access product name and price
                .ToListAsync(); // Executes the query and returns the results as a list

            if (orders == null || !orders.Any()) // Checks if no order products were found
                return NotFound(); // Returns a 404 error if the order doesn't exist

            var userId = orders.First().Orders.UserId; // Gets the ID of the user who placed this order from the first row

            var loyaltyAccount = await _context.LoyaltyAccount // Queries the LoyaltyAccount table
                .FirstOrDefaultAsync(x => x.UserId == userId); // Finds the loyalty record belonging to this order's user

            decimal discountPercent = loyaltyAccount?.Tier switch // Uses a switch to pick the discount based on their tier
            {
                "Bronze" => 0.05m, // Bronze members get 5% off
                "Silver" => 0.10m, // Silver members get 10% off
                "Gold" => 0.15m, // Gold members get 15% off
                _ => 0m     // Anyone else gets no discount
            };

            decimal total = orders.First().Orders.TotalAmount; // Gets the final total that was saved when the order was placed
            decimal subtotal = discountPercent > 0 ? total / (1 - discountPercent) : total; // Reverses the discount math
            decimal discountAmount = subtotal - total; // Calculates the actual cash amount that was discounted
            ViewBag.DiscountAmount = discountAmount; // Passes the discount amount to the view
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None"; // Passes the tier name to the view
            return View(orders); // Sends the list of order products to the Details view
        } // End of Details

        // GET: Orders/Create
        [HttpGet] // Defines this as a GET request for the checkout page
        public async Task<IActionResult> Create(int basketId) // Method to load the checkout/order creation page
        { // Start of Create GET
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets current user ID

            var basket = await _context.Basket // Looks for the user's active basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == true); // Checks ID and that the basket is "Open"

            if (basket == null) // If no active basket is found
            { // Start if
                return RedirectToAction("Index", "Products"); // Redirects back to shopping
            } // End if

            var basketProducts = await _context.BasketProducts // Gets items currently in the basket
                .Where(x => x.BasketId == basket.BasketId) // Filters by basket ID
                .Include(x => x.Products) // Joins product info to get prices
                .ToListAsync(); // Executes query

            decimal subtotal = 0.00m; // Initializes subtotal variable
            foreach (var basketProduct in basketProducts) // Loops through each item in basket
            { // Start foreach
                subtotal += basketProduct.Products.ProductPrice * basketProduct.ProductQuantity; // Calculates line total and adds to subtotal
            } // End foreach

            var loyaltyAccount = await _context.LoyaltyAccount // Looks for the user's loyalty record
                .FirstOrDefaultAsync(x => x.UserId == userId); // Matches by User ID

            decimal discountPercent = loyaltyAccount?.Tier switch // Determines discount percentage based on tier name
            { // Start switch
                "Bronze" => 0.05m, // 5% discount
                "Silver" => 0.10m, // 10% discount
                "Gold" => 0.15m, // 15% discount
                _ => 0m // 0% for "None" or unknown
            }; // End switch

            decimal discountAmount = subtotal * discountPercent; // Calculates the cash value of the discount
            decimal total = subtotal - discountAmount; // Subtracts discount from subtotal for the final price

            ViewBag.BasketId = basket.BasketId; // Passes Basket ID to the view
            ViewBag.Subtotal = subtotal; // Passes Subtotal to the view
            ViewBag.DiscountAmount = discountAmount; // Passes Discount to the view
            ViewBag.Total = total; // Passes Final Total to the view
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None"; // Passes Tier name to the view
            ViewBag.BasketProducts = basketProducts; // Passes the list of items to the view

            return View(); // Returns the checkout page view
        } // End of Create GET

        // POST: Orders/Create
        [HttpPost] // Marks this as a form submission handler for saving an order
        [ValidateAntiForgeryToken] // Security layer to prevent CSRF attacks
        public async Task<IActionResult> Create([Bind("OrdersId,Delivery,Collection,DeliveryType,CollectionDate")] Orders orders, int basketId) // Logic to process the checkout
        { // Start of Create POST
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets User ID

            if (userId == null) // Checks for session loss
            { // Start if
                ViewBag.BasketId = basketId; // Keeps basket ID for reload
                return View(orders); // Returns view with current data
            } // End if

            orders.UserId = userId; // Assigns the order to the current user
            ModelState.Remove("UserId"); // Removes validation requirement for UserId

            orders.OrderDate = DateOnly.FromDateTime(DateTime.Today); // Sets order date to today
            ModelState.Remove("OrderDate"); // Removes validation requirement

            orders.OrderTrackingStatus = "Pending"; // Sets initial status
            ModelState.Remove("OrderTrackingStatus"); // Removes validation requirement

            var basket = await _context.Basket // Finds the basket being checked out
                .FirstOrDefaultAsync(x => x.BasketId == basketId && x.UserId == userId && x.Status); // Ensures it's valid

            if (basket == null) // Error handling for missing basket
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var basketProducts = await _context.BasketProducts // Gets items to convert to order items
                .Where(x => x.BasketId == basketId) // Filter by ID
                .Include(x => x.Products) // Include product data
                .ToListAsync(); // Execute query

            if (!basketProducts.Any()) // Logic for empty basket checkout attempt
            { // Start if
                ModelState.AddModelError("", "Your basket is empty"); // Adds error message
                ViewBag.BasketId = basketId; // Keeps ID for reload
                return View(orders); // Reloads the view
            } // End if

            decimal subtotal = 0.00m; // Subtotal calculation
            foreach (var basketProduct in basketProducts) // Loop through items
            { // Start foreach
                var productTotal = basketProduct.Products.ProductPrice * basketProduct.ProductQuantity; // Calculate line total
                subtotal = productTotal + subtotal; // Accumulate subtotal
            } // End foreach

            var loyaltyAccount = await _context.LoyaltyAccount // Get loyalty data
                .FirstOrDefaultAsync(x => x.UserId == userId); // Filter by user

            decimal discountPercent = loyaltyAccount?.Tier switch // Determine final discount
            { // Start switch
                "Bronze" => 0.05m,
                "Silver" => 0.10m,
                "Gold" => 0.15m,
                _ => 0m
            }; // End switch

            decimal discount = subtotal * discountPercent; // Calculate discount amount
            orders.TotalAmount = subtotal - discount; // Set final amount on the Order object

            ModelState.Remove("subtotal"); // Cleans up validation tracking

            if (!orders.Collection && !orders.Delivery) // Validation: User must pick a method
            { // Start if
                ModelState.AddModelError("Delivery", "Must choose Collection or Delivery"); // Adds error
            } // End if

            if (orders.Collection) // Logic for collection choice
            { // Start if Collection
                ModelState.Remove("DeliveryType"); // Cleans up validation for non-delivery
                if (orders.CollectionDate == null) // Validation: needs a date
                { // Start if
                    ModelState.AddModelError("CollectionDate", "Collection date is Required"); // Adds error
                } // End if
                else // Date range validation
                { // Start else
                    var earliestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)); // Sets limit to 2 days from now
                    if (orders.CollectionDate.Value < earliestDate) // Check if date is too soon
                    { // Start if
                        ModelState.AddModelError("CollectionDate", "Collection must be at least 2 days from now"); // Adds error
                    } // End if
                } // End else
            } // End if Collection

            if (orders.Delivery) // Additional delivery validation
            { // Start if Delivery
                ModelState.Remove("CollectionDate"); // Cleans up validation for non-collection
                if (string.IsNullOrWhiteSpace(orders.DeliveryType)) // Check if type is empty
                { // Start if
                    ModelState.AddModelError("DeliveryType", "Delivery type is required"); // Adds error
                } // End if
            } // End if Delivery

            if (!ModelState.IsValid) // Final check for all validation errors
            { // Start if
                ViewBag.BasketId = basketId; // Retain ID
                return View(orders); // Return view with errors
            } // End if

            _context.Orders.Add(orders); // Adds the Order header to DB tracker
            await _context.SaveChangesAsync(); // Saves Order to get the OrdersId

            foreach (var basketProduct in basketProducts) // Loop to move items from basket to order
            { // Start foreach
                if (basketProduct.Products.StockQuantity < basketProduct.ProductQuantity) // Inventory check
                { // Start if
                    ModelState.AddModelError("", $"Not enough stock for {basketProduct.Products.ProductName}"); // Adds error
                    ViewBag.BasketId = basketId; // Retain ID
                    return View(orders); // Return view
                } // End if

                var orderProduct = new OrderProducts // Create new OrderItem record
                { // Start assignment
                    OrdersId = orders.OrdersId, // Links to the newly created Order ID
                    ProductsId = basketProduct.ProductsId, // Copies product ID
                    ProductsQuantity = basketProduct.ProductQuantity, // Copies quantity
                }; // End assignment

                _context.OrderProducts.Add(orderProduct); // Adds link to DB tracker
                basketProduct.Products.StockQuantity -= basketProduct.ProductQuantity; // Reduces actual store stock
            } // End foreach

            basket.Status = false; // "Closes" the basket so it can't be reused
            await _context.SaveChangesAsync(); // Commits items and stock changes

            if (loyaltyAccount != null) // Award loyalty points
            { // Start if
                int pointsEarned = (int)(subtotal * 10); // 10 points per £1 spent (pre-discount)
                loyaltyAccount.Points += pointsEarned; // Adds to account balance

                loyaltyAccount.Tier = loyaltyAccount.Points switch // Checks for tier upgrades
                { // Start switch
                    >= 1000 => "Gold",
                    >= 600 => "Silver",
                    >= 300 => "Bronze",
                    _ => loyaltyAccount.Tier
                }; // End switch

                var transaction = new LoyaltyTransactions // Log the point gain
                { // Start assignment
                    LoyaltyAccountId = loyaltyAccount.LoyaltyAccountId, // Link to account
                    OrdersId = orders.OrdersId, // Link to this order
                    PointsChange = pointsEarned, // Amount gained
                    Reason = $"Order #{orders.OrdersId} — £{subtotal:F2} spent", // Description
                    CreatedAt = DateTime.UtcNow // Timestamp
                }; // End assignment

                _context.LoyaltyTransactions.Add(transaction); // Adds log to DB tracker
                await _context.SaveChangesAsync(); // Final save for loyalty data
            } // End if

            return RedirectToAction("Index", "Home"); // Redirects to homepage on success
        } // End of Create POST

        // GET: Orders/Edit/5
        [HttpGet] // Defines this as a GET request for the edit form
        [Authorize(Roles = "Farmer, Admin")] // Restricts access to Farmers or Admins
        public async Task<IActionResult> Edit(int? id) // Method to load edit form
        { // Start of Edit GET
            if (id == null) // Check ID
                return NotFound();

            var orders = await _context.Orders.FindAsync(id); // Find order header
            if (orders == null) // Check existence
                return NotFound();

            return View(orders); // Return edit view
        } // End of Edit GET

        // POST: Orders/Edit/5
        [HttpPost] // Submit handler for saving edits
        [Authorize(Roles = "Farmer, Admin")] // Restricts access
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> Edit(int id, [Bind("OrdersId,UserId,TotalAmount,Delivery,Collection,DeliveryType,OrderTrackingStatus,CollectionDate,OrderDate")] Orders orders) // Logic to save edits
        { // Start of Edit POST
            if (id != orders.OrdersId) // ID verification
                return NotFound();

            if (ModelState.IsValid) // Check data validity
            { // Start if
                try // Attempt update
                { // Start try
                    _context.Update(orders); // Mark as modified
                    await _context.SaveChangesAsync(); // Save to DB
                } // End try
                catch (DbUpdateConcurrencyException) // Handle multi-user collision
                { // Start catch
                    if (!OrdersExists(orders.OrdersId)) // Check if deleted
                        return NotFound();
                    else
                        throw;
                } // End catch
                return RedirectToAction(nameof(Index)); // Back to list on success
            } // End if
            return View(orders); // Return form with errors
        } // End of Edit POST

        // GET: Orders/Delete/5
        [HttpGet] // Defines this as a GET request for delete confirmation
        [Authorize(Roles = "Admin")] // Restricts deletion to Admins
        public async Task<IActionResult> Delete(int? id) // Method to load delete confirmation
        { // Start of Delete GET
            if (id == null)
                return NotFound();

            var orders = await _context.Orders.FirstOrDefaultAsync(m => m.OrdersId == id); // Find order
            if (orders == null)
                return NotFound();

            return View(orders); // Return confirmation view
        } // End of Delete GET

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")] // Handler for final delete click
        [Authorize(Roles = "Admin")] // Restricts action to Admins
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> DeleteConfirmed(int id) // Logic to remove order
        { // Start of DeleteConfirmed
            var orders = await _context.Orders.FindAsync(id); // Find record
            if (orders != null)
                _context.Orders.Remove(orders); // Mark for removal

            await _context.SaveChangesAsync(); // Commit removal
            return RedirectToAction(nameof(Index)); // Back to list
        } // End of DeleteConfirmed

        private bool OrdersExists(int id) // Helper for DB checks
        { // Start helper
            return _context.Orders.Any(e => e.OrdersId == id); // Returns true if ID found
        } // End helper

        [HttpPost] // Submit handler for status updates
        [Authorize(Roles = "Farmer, Admin")] // Only allows Farmers or Admins
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> UpdateStatus(int id, string status) // Function to update tracking status
        { // Start of UpdateStatus
            var order = await _context.Orders.FindAsync(id); // Find order in DB
            if (order == null)
                return NotFound();

            order.OrderTrackingStatus = status; // Update the tracking status string
            await _context.SaveChangesAsync(); // Save changes
            return RedirectToAction("Details", new { id = id }); // Return to details page
        } // End of UpdateStatus
    } // End of class
} // End of namespace