using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugBeading.Data;
using BugBeading.Models;

namespace BugBeading.Pages.Inventory
{
    public class EditModel : PageModel
    {
        private readonly BugBeading.Data.InventoryContext _context;

        public EditModel(BugBeading.Data.InventoryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var productToUpdate = await _context.Products
                .FirstOrDefaultAsync(p => p.ID == id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.Name = Product.Name;
            productToUpdate.ProductType = Product.ProductType;
            productToUpdate.DateAdded = Product.DateAdded;
            productToUpdate.Description = Product.Description;
            productToUpdate.Price = Product.Price;
            productToUpdate.ImageUrl = Product.ImageUrl;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}