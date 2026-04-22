using GreenFieldLocalHub.Data; // Imports your project's database context
using GreenFieldLocalHub.Models; // Imports your data models (Farmers, etc.)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for rendering HTML elements
using Microsoft.EntityFrameworkCore; // Imports the database engine for C#
using System; // Imports basic system functionality like types and dates
using System.Collections.Generic; // Imports support for lists and collections
using System.Linq; // Imports data querying tools (like .Any())
using System.Security.Claims;
using System.Threading.Tasks; // Imports support for asynchronous tasks (async/await)

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
        public async Task<IActionResult> Index() // Method to show a list of all farmers
        { // Start of Index
            return View(await _context.Farmers.ToListAsync()); // Fetches all farmers from the DB and sends them to the Index page
        } // End of Index

        // GET: Farmers/Details/5
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
        public IActionResult Create() // Method to load the "Add Farmer" form
        { // Start of Create
            return View(); // Returns the blank form view
        } // End of Create

        // GET: Farmers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var farmers = await _context.Farmers.FindAsync(id);
            if (farmers == null)
                return NotFound();

            // ADDED - check the logged-in user owns this record
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (farmers.UserId != userId)
                return Forbid(); // blocks anyone editing someone else's profile

            return View(farmers);
        }

        // POST: Farmers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Farmers farmers,
            IFormFile? ImageFile)
        {
            if (id != farmers.FarmersId)
                return NotFound();

            // GET EXISTING FARMER
            var existingFarmer = await _context.Farmers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FarmersId == id);

            if (existingFarmer == null)
                return NotFound();

            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (existingFarmer.UserId != userId)
                return Forbid();

            // IMAGE UPLOAD
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions =
                    new[] { ".jpg", ".jpeg", ".png", ".webp" };

                var extension =
                    Path.GetExtension(ImageFile.FileName)
                        .ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewData["ImageError"] =
                        "Only .jpg, .png, and .webp files are allowed.";

                    return View(farmers);
                }

                var fileName =
                    Guid.NewGuid() + extension;

                var savePath =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/farmers",
                        fileName);

                Directory.CreateDirectory(
                    Path.GetDirectoryName(savePath)!);

                using var stream =
                    new FileStream(savePath, FileMode.Create);

                await ImageFile.CopyToAsync(stream);

                farmers.ImagePath =
                    "/images/farmers/" + fileName;
            }
            else
            {
                // KEEP EXISTING IMAGE
                farmers.ImagePath =
                    existingFarmer.ImagePath;
            }

            if (ModelState.IsValid)
            {
                _context.Update(farmers);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(farmers);
        }
        // GET: Farmers/Delete/5
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
        [HttpPost, ActionName("Delete")] // POST request mapped to the "Delete" confirmation button
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