using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugBeading.Data;
using BugBeading.Models;

namespace BugBeading.Pages.Inventory
{
    public class DetailsModel : PageModel
    {
        private readonly BugBeading.Data.InventoryContext _context;

        public DetailsModel(BugBeading.Data.InventoryContext context)
        {
            _context = context;
        }

        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
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
    }
}
