using System;
using System.Collections.Generic;

namespace NewBlog.Models
{
    public partial class Role
    {
        public Role()
        {
            BlogUsers = new HashSet<BlogUser>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<BlogUser> BlogUsers { get; set; }
    }
}
