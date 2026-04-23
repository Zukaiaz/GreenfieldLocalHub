using System; // Imports basic system functionality like DateTime
using System.Collections.Generic; // Imports support for Lists and Collections
using System.Linq; // Imports data querying tools like .Any() and .Where()
using System.Threading.Tasks; // Imports support for asynchronous programming (Tasks)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for rendering HTML SelectLists (dropdowns)
using Microsoft.EntityFrameworkCore; // Imports the database engine for C# (ORM)
using GreenFieldLocalHub.Data; // Imports your project's database context
using GreenFieldLocalHub.Models; // Imports your data models (LoyaltyTransactions, etc.)
using Microsoft.AspNetCore.Authorization; // Imports the security attribute for role-based access

namespace GreenFieldLocalHub.Controllers // Defines the container for this specific controller
{ // Start of namespace scope
    [Authorize(Roles = "Developer")] // Security: Restricts this entire controller to users with the 'Developer' role
    public class LoyaltyTransactionsController : Controller // Defines the class for managing loyalty point history
    { // Start of class scope
        private readonly ApplicationDbContext _context; // Declares a private variable for the database connection

        public LoyaltyTransactionsController(ApplicationDbContext context) // Constructor to inject the database context
        { // Start of constructor scope
            _context = context; // Stores the database connection in the local variable for use in methods
        } // End of constructor scope

        // GET: LoyaltyTransactions
        [HttpGet] // GET: Identifies this as a request to display the transaction history list
        public async Task<IActionResult> Index() // Method to show all loyalty transactions in the system
        { // Start of Index method
            var applicationDbContext = _context.LoyaltyTransactions // Starts a query on the Transactions table
                .Include(l => l.LoyaltyAccount) // Joins with the LoyaltyAccount table to display account owner info
                .Include(l => l.Orders); // Joins with the Orders table to show the specific purchase link
            return View(await applicationDbContext.ToListAsync()); // Executes the query and sends the resulting list to the Index View
        } // End of Index method

        // GET: LoyaltyTransactions/Details/5
        [HttpGet] // GET: Identifies this as a request for specific transaction details
        public async Task<IActionResult> Details(int? id) // Method to show one specific transaction record
        { // Start of Details method
            if (id == null) // Checks if the ID was missing from the URL path
            { // Start if block
                return NotFound(); // Returns a 404 error if no ID was provided
            } // End if block

            var loyaltyTransactions = await _context.LoyaltyTransactions // Accesses the Transactions table
                .Include(l => l.LoyaltyAccount) // Joins the account data
                .Include(l => l.Orders) // Joins the order data
                .FirstOrDefaultAsync(m => m.LoyaltyTransactionsId == id); // Finds the first record matching the unique ID

            if (loyaltyTransactions == null) // Checks if the database search returned no results
            { // Start if block
                return NotFound(); // Returns a 404 error if the transaction doesn't exist
            } // End if block

            return View(loyaltyTransactions); // Sends the specific transaction data object to the Details View
        } // End of Details method

        // GET: LoyaltyTransactions/Create
        [HttpGet] // GET: Loads the empty form for a manual transaction entry
        public IActionResult Create() // Method to display the creation form
        { // Start of Create method
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId"); // Populates a dropdown with existing Account IDs
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId"); // Populates a dropdown with existing Order IDs
            return View(); // Returns the blank Create View to the user
        } // End of Create method

        // POST: LoyaltyTransactions/Create
        [HttpPost] // POST: Identifies this as a submission of new transaction data
        [ValidateAntiForgeryToken] // Security layer to prevent Cross-Site Request Forgery (CSRF) attacks
        public async Task<IActionResult> Create([Bind("LoyaltyTransactionsId,LoyaltyAccountId,OrdersId,PointsChange,Reason,CreatedAt")] LoyaltyTransactions loyaltyTransactions) // Logic to save a new transaction
        { // Start of Create POST method
            if (ModelState.IsValid) // Checks if the submitted form data matches the requirements defined in the Model
            { // Start if block
                _context.Add(loyaltyTransactions); // Adds the new transaction object to the database tracker
                await _context.SaveChangesAsync(); // Commits the changes and saves the record to the database
                return RedirectToAction(nameof(Index)); // Redirects the user back to the transaction history list
            } // End if block
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId); // Reloads the account dropdown if there was an error
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId); // Reloads the order dropdown if there was an error
            return View(loyaltyTransactions); // Returns the form View with the current data and error messages
        } // End of Create POST method

        // GET: LoyaltyTransactions/Edit/5
        [HttpGet] // GET: Loads existing transaction data into a form for modification
        public async Task<IActionResult> Edit(int? id) // Method to load the edit form
        { // Start of Edit method
            if (id == null) // Checks if the ID is missing from the URL
            { // Start if block
                return NotFound(); // Returns a 404 error
            } // End if block

            var loyaltyTransactions = await _context.LoyaltyTransactions.FindAsync(id); // Searches for the record by its primary key
            if (loyaltyTransactions == null) // Checks if the record exists in the database
            { // Start if block
                return NotFound(); // Returns a 404 error if not found
            } // End if block
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId); // Pre-selects the correct account in the dropdown
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId); // Pre-selects the correct order in the dropdown
            return View(loyaltyTransactions); // Returns the edit form View populated with the existing data
        } // End of Edit method

        // POST: LoyaltyTransactions/Edit/5
        [HttpPost] // POST: Handles the submission of updated transaction data
        [ValidateAntiForgeryToken] // Security layer to verify the request's authenticity
        public async Task<IActionResult> Edit(int id, [Bind("LoyaltyTransactionsId,LoyaltyAccountId,OrdersId,PointsChange,Reason,CreatedAt")] LoyaltyTransactions loyaltyTransactions) // Logic to save edited data
        { // Start of Edit POST method
            if (id != loyaltyTransactions.LoyaltyTransactionsId) // Security check: Does the ID in the URL match the ID in the form?
            { // Start if block
                return NotFound(); // Returns a 404 if there is a mismatch
            } // End if block

            if (ModelState.IsValid) // Checks if the modified data is valid
            { // Start if block
                try // Begins a try-catch block to handle database update issues
                { // Start try block
                    _context.Update(loyaltyTransactions); // Marks the existing record as modified
                    await _context.SaveChangesAsync(); // Commits the updates to the database
                } // End try block
                catch (DbUpdateConcurrencyException) // Catches errors if the record was modified by another user simultaneously
                { // Start catch block
                    if (!LoyaltyTransactionsExists(loyaltyTransactions.LoyaltyTransactionsId)) // Checks if the record was deleted during the process
                    { // Start if block
                        return NotFound(); // Returns a 404 if the record no longer exists
                    } // End if block
                    else // If a different database error occurred
                    { // Start else block
                        throw; // Rethrows the exception to be handled by the global error page
                    } // End else block
                } // End catch block
                return RedirectToAction(nameof(Index)); // Returns to the transaction list on success
            } // End if block
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId); // Reloads dropdowns for the error view
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId); // Reloads dropdowns for the error view
            return View(loyaltyTransactions); // Returns the form with validation error messages
        } // End of Edit POST method

        // GET: LoyaltyTransactions/Delete/5
        [HttpGet] // GET: Loads a confirmation page before permanently deleting a record
        public async Task<IActionResult> Delete(int? id) // Method to load the delete confirmation view
        { // Start of Delete method
            if (id == null) // Checks if the ID is missing
            { // Start if block
                return NotFound(); // Returns 404
            } // End if block

            var loyaltyTransactions = await _context.LoyaltyTransactions // Starts database search
                .Include(l => l.LoyaltyAccount) // Includes account info for clarity in the confirmation
                .Include(l => l.Orders) // Includes order info
                .FirstOrDefaultAsync(m => m.LoyaltyTransactionsId == id); // Finds the matching transaction record

            if (loyaltyTransactions == null) // Checks if the record was found
            { // Start if block
                return NotFound(); // Returns 404
            } // End if block

            return View(loyaltyTransactions); // Shows the confirmation View to the Developer
        } // End of Delete method

        // POST: LoyaltyTransactions/Delete/5
        [HttpPost, ActionName("Delete")] // POST: Triggered when the Developer clicks the final 'Delete' button
        [ValidateAntiForgeryToken] // Security layer to prevent unauthorized deletions
        public async Task<IActionResult> DeleteConfirmed(int id) // Final logic to remove the transaction record
        { // Start of DeleteConfirmed method
            var loyaltyTransactions = await _context.LoyaltyTransactions.FindAsync(id); // Finds the record one last time
            if (loyaltyTransactions != null) // Checks if the record still exists
            { // Start if block
                _context.LoyaltyTransactions.Remove(loyaltyTransactions); // Marks the record for deletion in the context
            } // End if block

            await _context.SaveChangesAsync(); // Executes the deletion SQL command in the database
            return RedirectToAction(nameof(Index)); // Redirects back to the history list
        } // End of DeleteConfirmed method

        private bool LoyaltyTransactionsExists(int id) // Private helper tool to check for existence in the database
        { // Start helper method
            return _context.LoyaltyTransactions.Any(e => e.LoyaltyTransactionsId == id); // Returns true if the ID is found in the table
        } // End helper method
    } // End of class scope
} // End of namespace scope