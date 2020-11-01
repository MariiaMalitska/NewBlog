using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NewBlog.Models;

namespace NewBlog.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly BlogUserManager _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(BlogUserManager userManager, 
                        ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(32, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await AuthenticateUser(Input.Email, Input.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                else
                {
                    _userManager.SignInWithCookies(HttpContext, user, Input.RememberMe);
                    
                    _logger.LogInformation("User logged in.");
                }

                if (!Url.IsLocalUrl(returnUrl))
                {
                    returnUrl = Url.Content("~/");
                }

                return LocalRedirect(returnUrl);

            }

            _logger.LogInformation("Login failed");
            // Something failed. Redisplay the form.
            return Page();
        }

        private async Task<BlogUser> AuthenticateUser(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var result = await _userManager.GetUserAsync(email, password);

            return result;
        }
    }
}
