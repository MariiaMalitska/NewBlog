using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewBlog.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Web.Helpers;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net;
using System.Security.Principal;

namespace NewBlog.Pages.Blog
{
    [Authorize]
    public class PostModel : PageModel
    {
        private readonly NewBlog.Models.BlogDbContext _context;
        private readonly ILogger<PostModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public PostModel(NewBlog.Models.BlogDbContext context,
            ILogger<PostModel> logger,
            IConfiguration configuration,
            IHttpClientFactory clientFactory)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public Post Post { get; set; }
        public IList<Comment> Comments { get; set; }

        [BindProperty]
        public Comment NewComment { get; set; }

        [FromRoute(Name = "id")]
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(m => m.PostId == id);

            if (Post == null)
            {
                return NotFound();
            }

            Comments = await _context.Comments.Where(c=>c.PostId == Post.PostId).Include(c => c.User).OrderByDescending(c=>c.CommentDate).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int id)
        {
            Post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(m => m.PostId == id);

            //// ReCAPTCHA verification; responds with "invalid-input-response" :(
            //string recaptchaResponse = this.Request.Form["__RequestVerificationToken"];

            //var client = _clientFactory.CreateClient();
            //try
            //{
            //    var parameters = new Dictionary<string, string>
            //{
            //    {"secret", _configuration["reCAPTCHA:SecretKey"]},
            //    {"response", recaptchaResponse},
            //    {"remoteip", this.HttpContext.Connection.RemoteIpAddress.ToString()}
            //};

            //    HttpResponseMessage response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(parameters));
            //    //HttpResponseMessage response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify" + "?secret=" + parameters["secret"] + "&response=" + parameters["response"], new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded"));
            //    response.EnsureSuccessStatusCode();

            //    string apiResponse = await response.Content.ReadAsStringAsync();
            //    dynamic apiJson = JObject.Parse(apiResponse);
            //    if (apiJson.success != true)
            //    {
            //        this.ModelState.AddModelError(string.Empty, "There was an unexpected problem processing this request. Please try again.");
            //    }
            //}
            //catch (HttpRequestException ex)
            //{
            //    // Something went wrong with the API. Let the request through.
            //    _logger.LogError(ex, "Unexpected error calling reCAPTCHA api.");
            //}

            if (!ModelState.IsValid)
            {
                _logger.LogError("Comment post error");
                return RedirectToPage();
            }

            NewComment.UserId = int.Parse(User.Identity.GetUserId());
            NewComment.PostId = Post.PostId;
            NewComment.CommentDate = DateTime.Now;
            //NewComment.CommentContent = NewComment.CommentContent.Replace(Environment.NewLine, "<br/>");
            _context.Comments.Add(NewComment);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment.UserId!= int.Parse(User.Identity.GetUserId()))
            {
                return RedirectToPage("/Errors/Unauthorized");
            }

            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Post", new { id = comment.PostId });
        }

    }
}
