using System; // Imports basic system functionality like DateTime
using System.Collections.Generic; // Imports support for Lists and Collections
using System.Linq; // Imports data querying tools like .Any() and .Where()
using System.Threading.Tasks; // Imports support for asynchronous programming (Tasks)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for rendering HTML elements
using Microsoft.EntityFrameworkCore; // Imports the database engine for C#
using GreenFieldLocalHub.Data; // Imports your project's database context
using GreenFieldLocalHub.Models; // Imports your data models (LoyaltyAccount, etc.)
using Microsoft.AspNetCore.Identity; // Imports tools for managing users and passwords
using Microsoft.AspNetCore.Authorization; // Imports the [Authorize] attribute for security
using System.Security.Claims; // Imports tools to access user login claims/IDs

namespace GreenFieldLocalHub.Controllers // Defines the container for this controller
{ // Start of namespace scope
    public class LoyaltyAccountsController : Controller // Defines the class for managing Loyalty accounts
    { // Start of class scope
        private readonly ApplicationDbContext _context; // Declares a private variable for the database connection
        private readonly UserManager<IdentityUser> _userManager; // Declares a private variable for the Identity user manager

        public LoyaltyAccountsController(ApplicationDbContext context, UserManager<IdentityUser> userManager) // Constructor to inject dependencies
        { // Start of constructor scope
            _context = context; // Stores the database connection
            _userManager = userManager; // Stores the user manager tool
        } // End of constructor scope

        // GET: LoyaltyAccounts
        [Authorize(Roles = "Admin,Developer")]
        [HttpGet] // GET: Identifies this as a request to retrieve the full list of accounts
        public async Task<IActionResult> Index() // Method to show all loyalty accounts in the system
        { // Start of index method
            var accounts = await _context.LoyaltyAccount.ToListAsync();

            var users = await _userManager.Users.ToListAsync(); // Gets all users from the identity table
            ViewBag.UserEmails = users.ToDictionary(u => u.Id, u => u.Email); // Creates a lookup dictionary of userId -> email

            return View(accounts);
        }

        // GET: LoyaltyAccounts/Details/5
        [Authorize(Roles = "Admin,Developer")]
        [HttpGet] // GET: Identifies this as a request for specific account details
        public async Task<IActionResult> Details(int? id) // Method to show details for one specific loyalty account
        { // Start of Details method
            if (id == null) // Checks if the ID was missing from the URL
            { // Start if block
                return NotFound(); // Returns 404 error
            } // End if block

            var loyaltyAccount = await _context.LoyaltyAccount // Searches in the LoyaltyAccount table
                .FirstOrDefaultAsync(m => m.LoyaltyAccountId == id); // Finds the first record matching the provided ID
            if (loyaltyAccount == null) // Checks if the record was not found
            { // Start if block
                return NotFound(); // Returns 404 error
            } // End if block

            return View(loyaltyAccount); // Sends the specific account data to the Details View
        } // End of Details method

        // GET: LoyaltyAccounts/Create
        [Authorize(Roles = "Admin,Developer")]
        [HttpGet] // GET: Identifies this as a request for a blank creation form
        public IActionResult Create() // Method to display the initial account creation form
        { // Start of Create method
            return View(); // Returns the blank Create View
        } // End of Create method

        // POST: LoyaltyAccounts/Create
        [Authorize(Roles = "Admin,Developer")]
        [HttpPost] // POST: Identifies this as a submission of new data
        [ValidateAntiForgeryToken] // Security layer to prevent CSRF attacks
        public async Task<IActionResult> Create([Bind("LoyaltyAccountId,UserId,Points,Tier,CreatedAt")] LoyaltyAccount loyaltyAccount) // Method to save a new account
        { // Start of Create POST method
            if (ModelState.IsValid) // Checks if the form data follows the rules in the model
            { // Start if block
                _context.Add(loyaltyAccount); // Marks the new account for addition
                await _context.SaveChangesAsync(); // Commits the record to the database
                return RedirectToAction(nameof(Index)); // Returns to the list page
            } // End if block
            return View(loyaltyAccount); // If invalid, shows the form again with error messages
        } // End of Create POST method

        // GET: LoyaltyAccounts/Edit/5
        [Authorize(Roles = "Admin,Developer")]
        [HttpGet] // GET: Identifies this as a request to load existing data for editing
        public async Task<IActionResult> Edit(int? id) // Method to load the edit form
        { // Start of Edit method
            if (id == null) // Checks if ID is missing
            { // Start if block
                return NotFound(); // Returns 404
            } // End if block

            var loyaltyAccount = await _context.LoyaltyAccount.FindAsync(id); // Searches for the record by ID
            if (loyaltyAccount == null) // If the account doesn't exist
            { // Start if block
                return NotFound(); // Returns 404
            } // End if block
            return View(loyaltyAccount); // Returns the edit form with the current data
        } // End of Edit method

        // POST: LoyaltyAccounts/Edit/5
        [HttpPost] // POST: Identifies this as a submission of updated data
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> Edit(int id, [Bind("LoyaltyAccountId,UserId,Points,Tier,CreatedAt")] LoyaltyAccount loyaltyAccount) // Method to save changes
        { // Start of Edit POST method
            if (id != loyaltyAccount.LoyaltyAccountId) // Checks if the URL ID matches the Form ID
            { // Start if block
                return NotFound(); // Returns 404 if mismatch
            } // End if block

            if (ModelState.IsValid) // Checks if the updated data is valid
            { // Start if block
                try // Tries to perform the update
                { // Start try block
                    _context.Update(loyaltyAccount); // Marks the account as modified
                    await _context.SaveChangesAsync(); // Saves changes to the DB
                } // End try block
                catch (DbUpdateConcurrencyException) // Handles errors if record was changed elsewhere
                { // Start catch block
                    if (!LoyaltyAccountExists(loyaltyAccount.LoyaltyAccountId)) // Checks if the record was actually deleted
                    { // Start if block
                        return NotFound(); // Returns 404
                    } // End if block
                    else // If a different error occurred
                    { // Start else block
                        throw; // Rethrows the error to the global handler
                    } // End else block
                } // End catch block
                return RedirectToAction(nameof(Index)); // Returns to list on success
            } // End if block
            return View(loyaltyAccount); // If data is invalid, returns form with errors
        } // End of Edit POST method

        // GET: LoyaltyAccounts/Delete/5
        [Authorize(Roles = "Admin,Developer")]
        [HttpGet] // GET: Identifies this as a request for the delete confirmation page
        public async Task<IActionResult> Delete(int? id) // Method to load the delete page
        { // Start of Delete method
            if (id == null) // Checks if ID is missing
            { // Start if block
                return NotFound(); // Returns 404
            } // End if block

            var loyaltyAccount = await _context.LoyaltyAccount // Searches for the record
                .FirstOrDefaultAsync(m => m.LoyaltyAccountId == id); // Finds the matching record
            if (loyaltyAccount == null) // If not found
            { // Start if block
                return NotFound(); // Returns 404
            } // End if block

            return View(loyaltyAccount); // Shows the confirmation page
        } // End of Delete method

        // POST: LoyaltyAccounts/Delete/5

        [HttpPost, ActionName("Delete")] // POST: Mapped to the Delete action for final removal
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> DeleteConfirmed(int id) // Method to physically remove the record
        { // Start of DeleteConfirmed method
            var loyaltyAccount = await _context.LoyaltyAccount.FindAsync(id); // Finds the record by ID
            if (loyaltyAccount != null) // If it exists
            { // Start if block
                _context.LoyaltyAccount.Remove(loyaltyAccount); // Marks it for removal
            } // End if block

            await _context.SaveChangesAsync(); // Executes the deletion in the database
            return RedirectToAction(nameof(Index)); // Returns to the list
        } // End of DeleteConfirmed method

        // GET: Shows the loyalty sign up form
        [Authorize] // Attribute: Ensures only logged-in users can access the Sign Up page
        [HttpGet] // GET: Loads the Sign Up page
        public async Task<IActionResult> SignUp() // Method to handle loading the sign-up view
        { // Start of SignUp method
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the ID of the current logged-in user

            // If they already have an account, send them straight to MyAccount
            var existing = await _context.LoyaltyAccount // Checks the DB for an existing loyalty account
                .FirstOrDefaultAsync(x => x.UserId == userId); // Filters by the current User's ID

            if (existing != null) // If a record already exists
            { // Start if block
                return RedirectToAction(nameof(MyAccount)); // Redirects them away from sign-up to their account page
            } // End if block

            return View(); // Returns the Sign Up View if they don't have an account yet
        } // End of SignUp method

        // POST: Handles the loyalty sign up form submission
        [HttpPost] // POST: Identifies this as the submission of the sign-up email
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> SignUp(string email) // Method to process the sign-up form
        { // Start of SignUp POST method
            // Find the user by the email they entered
            var user = await _userManager.FindByEmailAsync(email); // Searches the Identity system for that email

            if (user == null) // If no user matches that email
            { // Start if block
                ViewBag.Error = "No account found with that email address."; // Sets an error message for the View
                return View(); // Reloads the page to show the error
            } // End if block

            // Check if they already have a loyalty account
            var existing = await _context.LoyaltyAccount // Searches the database
                .FirstOrDefaultAsync(x => x.UserId == user.Id); // Checks if this specific user already has a record

            if (existing != null) // If a record was found
            { // Start if block
                ViewBag.Error = "You already have a loyalty account!"; // Sets a duplicate error message
                return View(); // Reloads the page
            } // End if block

            // Create their loyalty account
            var loyaltyAccount = new LoyaltyAccount // Initializes a new LoyaltyAccount object
            { // Start object assignment
                UserId = user.Id, // Links the account to the User ID found via email
                Points = 0, // Sets starting balance to zero
                Tier = "None", // Sets initial tier to None
                CreatedAt = DateTime.UtcNow // Sets the creation timestamp to the current time
            }; // End object assignment

            _context.LoyaltyAccount.Add(loyaltyAccount); // Adds the new account to the DB tracker
            await _context.SaveChangesAsync(); // Saves the new account to the database

            ViewBag.Success = "You have successfully signed up to our loyalty scheme!"; // Sets a success message
            return View(); // Returns the view to show the success message
        } // End of SignUp POST method

        [Authorize] // Attribute: Restricts access to logged-in users only
        [HttpGet] // GET: Standard retrieve request for account info
        public async Task<IActionResult> MyAccount() // Method to display the user's loyalty profile
        { // Start of MyAccount method
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current logged-in User ID

            var loyaltyAccount = await _context.LoyaltyAccount // Looks for their loyalty record
                .FirstOrDefaultAsync(x => x.UserId == userId); // Matches against their User ID

            if (loyaltyAccount == null) // If the user isn't in the loyalty scheme yet
            { // Start if block
                return RedirectToAction(nameof(SignUp)); // Redirects them to the sign-up page
            } // End if block

            // Get their transaction history
            var transactions = await _context.LoyaltyTransactions // Queries the transaction history table
                .Where(x => x.LoyaltyAccountId == loyaltyAccount.LoyaltyAccountId) // Filters for this specific account
                .OrderByDescending(x => x.CreatedAt) // Sorts by newest transactions first
                .ToListAsync(); // Converts the results into a List

            ViewBag.Transactions = transactions; // Passes the list of transactions to the view via ViewBag
            return View(loyaltyAccount); // Sends the loyalty account data to the MyAccount View
        } // End of MyAccount method

        private bool LoyaltyAccountExists(int id) // Private helper tool to check for existence
        { // Start of helper method
            return _context.LoyaltyAccount.Any(e => e.LoyaltyAccountId == id); // Returns true if the ID exists in the DB
        } // End of helper method
    } // End of class scope
} // End of namespace scope