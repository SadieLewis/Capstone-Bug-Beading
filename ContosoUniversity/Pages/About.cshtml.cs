using BugBeading.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugBeading.Models;

namespace BugBeading.Pages
{
    public class AboutModel : PageModel
    {
        private readonly InventoryContext _context;

        public async Task OnGetAsync()
        {
           
        }
    }
}