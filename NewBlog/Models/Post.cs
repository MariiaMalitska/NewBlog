using Markdig;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewBlog.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public int PostId { get; set; }
        public int UserId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The title must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Title")]
        public string PostTitle { get; set; }

        [Display(Name = "Posted At")]
        public DateTime DatePosted { get; set; }

        [Display(Name = "Edited At")]
        public DateTime? DateEdited { get; set; }

        [Required]
        [StringLength(4000, ErrorMessage = "The content must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Content")]
        public string PostContent { get; set; }

        [StringLength(4000, ErrorMessage = "The image URL must be at max {1} characters long.")]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        public virtual BlogUser User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public string HtmlBody()
        {
            var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
            return Markdown.ToHtml(PostContent, pipeline);
        }
    }
}
