using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace prjSocialMediaPlatform.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "貼文內容不得超過 1000 字")]
        public string Content { get; set; }

        public string Image { get; set; }

        public string CreatedAt { get; set; }

        public string UserName { get; set; }
    }
}