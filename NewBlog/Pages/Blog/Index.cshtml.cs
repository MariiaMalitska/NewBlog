using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewBlog.Models;

namespace NewBlog.Pages.Blog
{
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
                .Include(p => p.User).OrderByDescending(p=>(p.DateEdited==null ? p.DatePosted : p.DateEdited)).ToListAsync();
        }
    }
}
