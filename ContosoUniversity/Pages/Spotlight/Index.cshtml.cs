using System;
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
    public class IndexModel : PageModel
    {
        private readonly BugBeading.Data.InventoryContext _context;

        public IndexModel(BugBeading.Data.InventoryContext context)
        {
            _context = context;
        }

        public IList<Product> BestSellers { get; set; }
            = new List<Product>();

        public IList<Product> OnSale { get; set; }
            = new List<Product>();

        public async Task OnGetAsync()
        {
            BestSellers = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsBestSeller)
                .OrderBy(p => p.Name)
                .ToListAsync();

            OnSale = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsOnSale)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
