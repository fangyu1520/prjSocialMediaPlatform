using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace prjSocialMediaPlatform.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "評論內容不得超過 500 字")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual LogIn User { get; set; }

        [ForeignKey("Post")]
        public int PostID { get; set; }

        public virtual Post Post { get; set; }
    }
}