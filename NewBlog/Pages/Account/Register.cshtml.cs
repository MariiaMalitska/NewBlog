using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NewBlog.Models;

namespace NewBlog.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly BlogUserManager _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(BlogUserManager userManager,
                            ILogger<RegisterModel> logger)
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
            [StringLength(50, ErrorMessage = "The name must be less than 50 characters long.")]
            [Display(Name = "Login")]
            public string Login { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new BlogUser { Login = Input.Login, Email = Input.Email };

                var errors = await _userManager.CreateUserAsync(user, Input.Password);
                if (errors.Count==0)
                {
                    _logger.LogInformation("User created a new account with password.");

                    //await _userManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
