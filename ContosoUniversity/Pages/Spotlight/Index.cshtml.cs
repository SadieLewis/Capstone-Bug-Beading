using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugBeading.Data;
using BugBeading.Models;

namespace BugBeading.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly BugBeading.Data.InventoryContext _context;

        public IndexModel(BugBeading.Data.InventoryContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            
        }
    }
}
