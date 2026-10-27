using BugBeading.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BugBeading.Pages.Inventory
{
    public class DeleteModel : PageModel
    {
        private readonly BugBeading.Data.InventoryContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(BugBeading.Data.InventoryContext context,
                           ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Product Product { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ID == id.Value);

            if (product == null)
            {
                return NotFound();
            }

            Product = product;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ID == id.Value);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to delete product with ID {ProductId}.",
                    id.Value);

                Product = product;

                ErrorMessage =
                    "This product could not be deleted. " +
                    "It may be referenced by an existing order.";

                return Page();
            }
        }
    }
}