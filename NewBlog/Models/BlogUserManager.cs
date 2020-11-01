using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace NewBlog.Models
{
    public class BlogUserManager
    {
        private readonly BlogDbContext _context;
        private readonly IConfiguration _configuration;

        public BlogUserManager(BlogDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async void SignInWithJWT(BlogUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async void SignInWithCookies(HttpContext httpContext, BlogUser user, bool isPersistent)
        {
            var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.RoleName)
                    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(
                httpContext,
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent
                });
        }

        public async Task<BlogUser> GetUserAsync(string email, string password)
        {
            var user = await _context.BlogUsers.Include(b => b.Role).FirstOrDefaultAsync(x => x.Email == email);
            bool verified = (user != null ? BCrypt.Net.BCrypt.Verify(password, user.Password) : false);

            return (verified ? user : null);
        }

        public async Task<List<string>> CreateUserAsync(BlogUser user, string password)
        {
            var errors = new List<string>();

            if (user.Email.Length == 0 || user.Email.Length > 50)
                errors.Add("Email length is invalid");
            if (user.Login.Length == 0 || user.Login.Length > 50)
                errors.Add("Login length is invalid");

            var sameEmail = await _context.BlogUsers.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (sameEmail!=null)
                errors.Add("User with this email already exists");
            var sameLogin = await _context.BlogUsers.FirstOrDefaultAsync(x => x.Login == user.Login);
            if (sameLogin != null)
                errors.Add("User with this login already exists");

            if (errors.Count == 0)
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(password);
                BlogUser newuser = new BlogUser { Email = user.Email, Login = user.Login, Password = hash };
                _context.BlogUsers.Add(newuser);
                var result = await _context.SaveChangesAsync();
            }

            return errors;
        }


    }
}
