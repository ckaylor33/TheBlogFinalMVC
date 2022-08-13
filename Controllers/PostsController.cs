using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogFinalMVC.Data;
using TheBlogFinalMVC.Models;
using TheBlogFinalMVC.Services;
using TheBlogFinalMVC.Enums;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using TheBlogFinalMVC.ViewModels;

namespace TheBlogFinalMVC.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;


        public PostsController(ApplicationDbContext context, ISlugService slugService, IImageService imageService, UserManager<BlogUser> userManager, BlogSearchService blogSearchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;
            ViewData["MainText"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 6;

            var posts = _blogSearchService.Search(searchTerm);

            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Posts
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AllPosts(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            var allPosts = await _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser)
                                         .ToList()
                                         .OrderByDescending(p => p.Created)
                                         .ToPagedListAsync(pageNumber, pageSize);


            return View(allPosts);
        }

        // GET: Posts
        [HttpGet]
        public async Task<IActionResult> ProductionReady(int? page)
        {

            var pageNumber = page ?? 1;
            var pageSize = 6;

            IPagedList<Post> productionPosts = await _context.Posts.Include(p => p.Blog)
                                                     .Include(p => p.BlogUser)
                                                     .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
                                                     .ToList()
                                                     .OrderByDescending(p => p.Created)
                                                     .ToPagedListAsync(pageNumber, pageSize);


            return View(productionPosts);
        }

        //BlogPostIndex
        public async Task<IActionResult> BlogPostIndex(int? id, int? page)
        {
            if (id is null)
            {
                return NotFound();
            }

            var pageNumber = page ?? 1;
            var pageSize = 6;

            var blogs = await _context.Blogs.Include(p => p.Posts).FirstOrDefaultAsync(p => p.Id == id);

            var posts = await _context.Posts.Include(p => p.BlogUser)
                .Where(p => p.BlogId == id && p.ReadyStatus == ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .ToPagedListAsync(pageNumber, pageSize);

            ViewData["MainText"] = blogs?.Name;
            ViewData["SubText"] = blogs?.Description;

            return View(posts);
        }

        // GET: Posts/Details/5

        public async Task<IActionResult> Details(string slug)
        {
            ViewData["Title"] = "Post Details Page";
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var post = await _context.Posts
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null) return NotFound();

            var dataVM = new PostDetailViewModel()
            {
                Post = post,
                Tags = _context.Tags.Select(t => t.Text.ToLower()).Distinct().ToList()

            };

            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;

            return View(dataVM);
        }


        /*        public async Task<IActionResult> Details(string slug)
                {
                    if (string.IsNullOrEmpty(slug))
                    {
                        return NotFound();
                    }

                    var post = await _context.Posts
                        .Include(p => p.Blog)
                        .Include(p => p.BlogUser) //Author of post
                        .Include(p => p.Tags)
                        .Include(p => p.Comments)
                        .ThenInclude(c => c.BlogUser) //Author of comment
                        .FirstOrDefaultAsync(m => m.Slug == slug);
                    if (post == null)
                    {
                        return NotFound();
                    }

                    return View(post);
                }*/

        [Authorize(Roles = "Administrator")]
        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid) //if not valid, show create view again below
            {
                post.Created = DateTime.Now;

                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                //use the _imageService to store the incoming user specified image
                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                //Create the slug and determine if it's unique
                var slug = _slugService.UrlFriendly(post.Title);

                //Create a variable to store whether an error has occurred
                var validationError = false;

                //Detect empty title/slug
                if (string.IsNullOrEmpty(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("", "The Title you provided cannot be used as it results in an empty slug.");
                }

                //Detect incoming duplicate slugs
                if (!_slugService.IsUnique(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("Title", "The Title you provided cannot be used as it results in a duplicate slug.");
                }

                //returns View back to user along with errors found
                if (validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();

                //How do I loop over the incoming list of string?
                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText,
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { slug = post.Slug });
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post); //will load combination of existing post and ViewData
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post post, IFormFile NewImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //The original post - gets a copy before any edits applied (preserves data that doesn't change)
                    //Opportunity to update data that has changed
                    var originalPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);

                    originalPost.Updated = DateTime.Now;
                    originalPost.Title = post.Title;
                    originalPost.Abstract = post.Abstract;
                    originalPost.Content = post.Content;
                    originalPost.ReadyStatus = post.ReadyStatus;

                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if (newSlug != originalPost.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            originalPost.Title = post.Title;
                            originalPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This Title cannot be used as it results in a duplicate slug.");
                            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
                            return View(post);
                        }
                    }

                    if (NewImage is not null)
                    {
                        originalPost.ImageData = await _imageService.EncodeImageAsync(NewImage);
                        originalPost.ContentType = _imageService.ContentType(NewImage);
                    }

                    //Remove all Tags previously associated with this Post
                    _context.Tags.RemoveRange(originalPost.Tags);

                    //Add new tags from the Edit form
                    foreach (var tagText in tagValues)
                    {
                        _context.Tags.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = originalPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllPosts));
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
