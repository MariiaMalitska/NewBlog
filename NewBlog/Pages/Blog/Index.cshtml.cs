using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public int CurrentPage { get; set; } = 1;

        public int Count { get; set; }

        public int PageSize { get; set; } = 10;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public bool EnablePrevious => CurrentPage > 1;

        public bool EnableNext => CurrentPage < TotalPages;

        public async Task OnGetAsync(int currentPage)
        {
            CurrentPage = currentPage == 0 ? 1 : currentPage;

            Count = _context.Posts.Count();

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            Post = await _context.Posts.OrderByDescending(p => (p.DateEdited == null ? p.DatePosted : p.DateEdited))
                .Skip((CurrentPage - 1) * PageSize).Take(PageSize)
                .Include(p => p.User).ToListAsync();
        }
    }
}
