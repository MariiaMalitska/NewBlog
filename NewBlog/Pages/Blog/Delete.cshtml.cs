using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewBlog.Models;

namespace NewBlog.Pages.Blog
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly NewBlog.Models.BlogDbContext _context;

        public DeleteModel(NewBlog.Models.BlogDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Post Post { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post = await _context.Posts
                .Include(p => p.User).FirstOrDefaultAsync(m => m.PostId == id);

            if (Post == null)
            {
                return NotFound();
            }
            if (!(User.Identity.GetUserId() == Post.UserId.ToString() || User.IsInRole("admin")))
            {
                return RedirectToPage("/Errors/Unauthorized");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!(User.Identity.GetUserId() == Post.UserId.ToString() || User.IsInRole("admin")))
            {
                return RedirectToPage("/Errors/Unauthorized");
            }
            if (id == null)
            {
                return NotFound();
            }

            Post = await _context.Posts.FindAsync(id);

            if (Post != null)
            {
                _context.Posts.Remove(Post);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
