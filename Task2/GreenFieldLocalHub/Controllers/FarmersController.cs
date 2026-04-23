using GreenFieldLocalHub.Data; // Imports the database context namespace
using GreenFieldLocalHub.Models; // Imports the data models (Farmers, etc.)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for rendering HTML elements
using Microsoft.EntityFrameworkCore; // Imports the database engine for C#
using System; // Imports basic system functionality like types and dates
using System.Collections.Generic; // Imports support for lists and collections
using System.Linq; // Imports data querying tools (like .Any())
using System.Security.Claims; // Imports tools to read User Claims (like User ID)
using System.Threading.Tasks; // Imports support for asynchronous tasks (async/await)
using Microsoft.AspNetCore.Authorization; // Imports security attributes

namespace GreenFieldLocalHub.Controllers // Defines the container for this controller
{ // Start of namespace
    public class FarmersController : Controller // Defines the class that handles Farmer profiles
    { // Start of class
        private readonly ApplicationDbContext _context; // Declares a private variable for the database connection

        public FarmersController(ApplicationDbContext context) // Constructor: Runs when the controller is made
        { // Start of constructor
            _context = context; // Stores the database connection in the local variable
        } // End of constructor

        // GET: Farmers
        [HttpGet] // Retrieves the list of farmers
        public async Task<IActionResult> Index() // Method to show a list of all farmers
        { // Start of Index
            return View(await _context.Farmers.ToListAsync()); // Fetches all farmers from the DB and sends them to the Index page
        } // End of Index

        // GET: Farmers/Details/5
        [HttpGet] // Retrieves details for one farmer
        public async Task<IActionResult> Details(int? id) // Method to show specific info for one farmer
        { // Start of Details
            if (id == null) // Checks if the ID was missing from the URL
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            var farmers = await _context.Farmers // Looks into the Farmers table
                .FirstOrDefaultAsync(m => m.FarmersId == id); // Finds the first farmer matching the ID
            if (farmers == null) // Checks if the database failed to find a record
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            return View(farmers); // Sends the farmer data to the Details page
        } // End of Details

        // GET: Farmers/Create
        [HttpGet] // Loads the form to add a new farmer
        [Authorize(Roles = "Admin,Developer")] // Only high-level users can manually create farmer profiles
        public IActionResult Create() // Method to load the "Add Farmer" form
        { // Start of Create
            return View(); // Returns the blank form view
        } // End of Create

        // GET: Farmers/Edit/5
        [HttpGet] // Loads the edit form
        [Authorize(Roles = "Farmer,Admin,Developer")] // Farmers can edit themselves; Admins/Devs can edit anyone
        public async Task<IActionResult> Edit(int? id)
        { // Start of Edit GET
            if (id == null) // Check if ID is missing
                return NotFound();

            var farmers = await _context.Farmers.FindAsync(id); // Find farmer by ID
            if (farmers == null)
                return NotFound();

            // SECURITY: Check the logged-in user owns this record
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // If the user isn't an Admin/Dev and doesn't own the profile, block access
            if (farmers.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Developer"))
                return Forbid();

            return View(farmers); // Show the edit page
        } // End of Edit GET

        // POST: Farmers/Edit/5
        [HttpPost] // Handles the form submission for editing
        [ValidateAntiForgeryToken] // Security check against cross-site attacks
        public async Task<IActionResult> Edit(int id, Farmers farmers, IFormFile? ImageFile)
        { // Start of Edit POST
            if (id != farmers.FarmersId) // Ensure the ID in the URL matches the form data
                return NotFound();

            // GET EXISTING FARMER: Use AsNoTracking so we can compare without DB conflicts
            var existingFarmer = await _context.Farmers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FarmersId == id);

            if (existingFarmer == null)
                return NotFound();

            // SECURITY: Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Logic check: Must own the profile or be an Admin/Developer to save changes
            if (existingFarmer.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Developer"))
                return Forbid();

            // IMAGE UPLOAD LOGIC
            if (ImageFile != null && ImageFile.Length > 0) // Check if a new file was uploaded
            { // Start image logic
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" }; // List of safe file types
                var extension = Path.GetExtension(ImageFile.FileName).ToLower(); // Get file extension

                if (!allowedExtensions.Contains(extension)) // Validate file type
                { // Start error check
                    ViewData["ImageError"] = "Only .jpg, .png, and .webp files are allowed.";
                    return View(farmers);
                } // End error check

                var fileName = Guid.NewGuid() + extension; // Generate a unique name for the image
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/farmers", fileName); // Set save location

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!); // Ensure the folder exists

                using (var stream = new FileStream(savePath, FileMode.Create)) // Create the file
                {
                    await ImageFile.CopyToAsync(stream); // Save the image to the server
                }

                farmers.ImagePath = "/images/farmers/" + fileName; // Update the record with the new path
            } // End image logic
            else
            { // If no new image was uploaded
                farmers.ImagePath = existingFarmer.ImagePath; // Keep the old image path
            }

            if (ModelState.IsValid) // Check if all other form data follows the rules
            { // Start save
                _context.Update(farmers); // Mark record for update
                await _context.SaveChangesAsync(); // Commit changes to DB
                return RedirectToAction(nameof(Index)); // Go back to the list
            } // End save

            return View(farmers); // Return the form if validation failed
        } // End of Edit POST

        // GET: Farmers/Delete/5
        [HttpGet] // Loads delete confirmation
        [Authorize(Roles = "Admin,Developer")] // Only Admins or Developers should be allowed to delete profiles
        public async Task<IActionResult> Delete(int? id) // Method to load delete confirmation page
        { // Start of Delete
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var farmers = await _context.Farmers // Searches for the farmer
                .FirstOrDefaultAsync(m => m.FarmersId == id); // Finds matching record
            if (farmers == null) // If not found
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            return View(farmers); // Shows the confirmation page
        } // End of Delete

        // POST: Farmers/Delete/5
        [HttpPost, ActionName("Delete")] // Final deletion process
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Final logic to remove farmer
        { // Start of DeleteConfirmed
            var farmers = await _context.Farmers.FindAsync(id); // Finds the record one last time
            if (farmers != null) // If it exists
            { // Start if
                _context.Farmers.Remove(farmers); // Marks it for deletion
            } // End if

            await _context.SaveChangesAsync(); // Saves the removal in the database
            return RedirectToAction(nameof(Index)); // Goes back to the list
        } // End of DeleteConfirmed

        private bool FarmersExists(int id) // Private tool to check if a record is in the DB
        { // Start helper
            return _context.Farmers.Any(e => e.FarmersId == id); // Returns true if ID is found
        } // End helper
    } // End of class
} // End of namespace