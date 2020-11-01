using Markdig;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewBlog.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Posted At")]
        public DateTime CommentDate { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The comment must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Comment")]
        public string CommentContent { get; set; }

        public virtual Post Post { get; set; }
        public virtual BlogUser User { get; set; }

        public string HtmlBody()
        {
            var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
            return Markdown.ToHtml(CommentContent, pipeline);
        }
    }
}
