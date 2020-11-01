using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewBlog.Models;

namespace NewBlog.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly NewBlog.Models.BlogDbContext _context;

        public IndexModel(NewBlog.Models.BlogDbContext context)
        {
            _context = context;
        }

        public IList<Post> Post { get;set; }

        public async Task OnGetAsync()
        {
            Post = await _context.Posts
                .Include(p => p.User).ToListAsync();
        }
    }
}
