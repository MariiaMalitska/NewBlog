using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewBlog.Models;

namespace NewBlog.Pages.Blog
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly NewBlog.Models.BlogDbContext _context;

        public CreateModel(NewBlog.Models.BlogDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Post Post { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Post.UserId = int.Parse(User.Identity.GetUserId());
            Post.DatePosted = DateTime.Now;
            //Post.PostContent = Post.PostContent.Replace(Environment.NewLine, "<br/>");

            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
