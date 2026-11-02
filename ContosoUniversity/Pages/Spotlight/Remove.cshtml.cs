using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugBeading.Data;
using BugBeading.Models;

namespace BugBeading.Pages.Spotlight
{
    public class RemoveModel : PageModel
    {
        private readonly InventoryContext _context;

        public RemoveModel(InventoryContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; }
            = new List<Product>();

        public string Carousel { get; set; } = "BestSellers";

        public async Task<IActionResult> OnGetAsync(
            string carousel = "BestSellers")
        {
            if (!TryNormalizeCarousel(carousel, out var normalized))
            {
                return BadRequest();
            }
            Carousel = normalized;
            if (Carousel == "BestSellers")
            {
                Products = await _context.Products
                    .AsNoTracking()
                    .Where(p => p.IsBestSeller)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            else
            {
                Products = await _context.Products
                    .AsNoTracking()
                    .Where(p => p.IsOnSale)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(
            int productId,
            string carousel)
        {
            if (!TryNormalizeCarousel(carousel, out var normalized))
            {
                return BadRequest();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ID == productId);

            if (product == null)
            {
                return NotFound();
            }

            if (normalized == "BestSellers")
            {
                product.IsBestSeller = false;
            }
            else
            {
                product.IsOnSale = false;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(
                "/Spotlight/Remove",
                new { carousel = normalized });
        }
        private static bool TryNormalizeCarousel(
            string carousel,
            out string normalized)
        {
            if (string.Equals(
                carousel, "BestSellers",
                System.StringComparison.OrdinalIgnoreCase))
            {
                normalized = "BestSellers";
                return true;
            }

            if (string.Equals(
                carousel, "OnSale",
                System.StringComparison.OrdinalIgnoreCase))
            {
                normalized = "OnSale";
                return true;
            }

            normalized = string.Empty;
            return false;
        }
    }
}