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
    public class EditModel : PageModel
    {
        private readonly NewBlog.Models.BlogDbContext _context;

        public EditModel(NewBlog.Models.BlogDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.Identity.GetUserId() == Post.UserId.ToString() || User.IsInRole("admin")))
            {
                return RedirectToPage("/Errors/Unauthorized");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Post.DateEdited = (DateTime?)DateTime.Now;
            //Post.PostContent = Post.PostContent.Replace(Environment.NewLine, "<br/>");
            _context.Attach(Post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(Post.PostId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
