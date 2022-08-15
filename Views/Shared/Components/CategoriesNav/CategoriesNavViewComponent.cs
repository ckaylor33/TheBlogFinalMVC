using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogFinalMVC.Data;
using TheBlogFinalMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheBlogFinalMVC.Enums;

namespace TheBlogFinalMVC.Views.Shared.Categories
{
    public class CategoriesNavViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoriesNavViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await GetItemsAsync();
            ViewData["Tags"] = new SelectList(_context.Tags);

            return View(items);
        }
        private Task<List<Blog>> GetItemsAsync()
        {
            return _context.Blogs.Where(b => b.Posts.Any(p => p.ReadyStatus == ReadyStatus.ProductionReady))
                                 .OrderByDescending(p => p.Created)
                                 .ToListAsync();
        }
    }
}

