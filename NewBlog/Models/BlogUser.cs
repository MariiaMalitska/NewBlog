using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewBlog.Models
{
    public partial class BlogUser
    {
        public BlogUser()
        {
            Comments = new HashSet<Comment>();
            Posts = new HashSet<Post>();
        }

        public int UserId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The login must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
