using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BugBeading.Data;
using BugBeading.Models;

namespace BugBeading.Pages.Inventory
{
    public class IndexModel : PageModel
{
    private readonly InventoryContext _context;
    private readonly IConfiguration Configuration;

    public IndexModel(InventoryContext context, IConfiguration configuration)
    {
        _context = context;
        Configuration = configuration;
    }

        public Product? SelectedProduct { get; set; }
        public string? CurrentFilter { get; set; }
        public string? CurrentSort { get; set; }

    public PaginatedList<Product> Products { get; set; }

    public async Task<IActionResult> OnGetAsync(string? sortOrder, string? currentFilter, string? searchString, int? pageIndex, int? selectedProductId) {
        CurrentSort = sortOrder;
        if (searchString == null)
        {
            searchString = currentFilter;
        }
        else
        {
            pageIndex = 1;
        }
        CurrentFilter = searchString;
        IQueryable<Product> productsQuery =
            _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            productsQuery = productsQuery.Where(p =>
                p.Name.Contains(searchString) ||
                (p.ProductType != null &&
                    p.ProductType.Contains(searchString)));
        }

        // Sort the products.
        switch (sortOrder)
        {
            case "Type":
                productsQuery = productsQuery
                    .OrderBy(p => p.ProductType)
                    .ThenBy(p => p.Name);
                break;

            case "Newest":
                productsQuery = productsQuery
                    .OrderByDescending(p => p.DateAdded)
                    .ThenBy(p => p.Name);
                break;

            case "Price":
                productsQuery = productsQuery
                    .OrderBy(p => p.Price);
                break;

            case "PriceDesc":
                productsQuery = productsQuery
                    .OrderByDescending(p => p.Price);
                break;

            case "NameDesc":
                productsQuery = productsQuery
                    .OrderByDescending(p => p.Name);
                break;

            default:
                productsQuery = productsQuery
                    .OrderBy(p => p.Name);
                break;
        }

        int pageSize = Configuration.GetValue<int>(
            "PageSize", 6);

        Products = await PaginatedList<Product>.CreateAsync( productsQuery,pageIndex ?? 1,pageSize);
        if (selectedProductId.HasValue)
        {
            SelectedProduct = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.ID == selectedProductId.Value);

            if (SelectedProduct == null)
            {
                return NotFound();
            }
        }

        return Page();
    }
    public async Task<IActionResult> OnPostAddToCartAsync(int productId){
        bool productExists = await _context.Products
            .AnyAsync(p => p.ID == productId);

        if (!productExists)
        {
            return NotFound();
        }

        // Cart storage still needs to be implemented.
        TempData["CartMessage"] =
            "Cart wil go here";

        return RedirectToPage("./Index");
    }
    }
}