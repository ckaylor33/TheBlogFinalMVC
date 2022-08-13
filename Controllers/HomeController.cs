using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheBlogFinalMVC.Data;
using TheBlogFinalMVC.Models;
using TheBlogFinalMVC.Services;
using TheBlogFinalMVC.ViewModels;
using X.PagedList;

namespace TheBlogFinalMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
                              IBlogEmailSender emailSender,
                              ApplicationDbContext context)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Homepage(int? page)
        {

            HomepageViewModel homepage = new();


            homepage.Blogs = await _context.Blogs
                .Where(b => b.Posts.Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady))
                .Include(b => b.BlogUser)
                .OrderByDescending(p => p.Created)
                .Take(6)
                .ToListAsync();

            homepage.Posts = await _context.Posts.Include(p => p.BlogUser)
                                                 .Where(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady)
                                                 .OrderByDescending(p => p.Created)
                                                 .Take(6)
                                                 .ToListAsync();


            /*            var blogs = _context.Blogs.Where(
                b => b.Posts.Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady))
                .OrderByDescending(p => p.Created)
                .ToPagedListAsync(pageNumber, pageSize);*/


            ViewData["MainText"] = "CK Blogs";
            ViewData["SubText"] = "Follow my Coding Journey";

            return View(homepage);
        }

        public IActionResult About()
        {
            ViewData["HeaderImage"] = Url.Content("~/images/home-bg.jpg");
            ViewData["MainText"] = "About Me";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["HeaderImage"] = Url.Content("~/images/home-bg.jpg");
            ViewData["MainText"] = "Contact Me";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            //This is where I will be emailing...
            model.Message = $"{model.Message} <hr/> Phone : {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
