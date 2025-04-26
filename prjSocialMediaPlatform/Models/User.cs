using System.ComponentModel.DataAnnotations;

namespace prjSocialMediaPlatform.Models
{
    public class Register
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "使用者名稱為必填")]
        [MaxLength(50, ErrorMessage = "使用者名稱最多 50 字")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email 為必填")]
        [EmailAddress(ErrorMessage = "請輸入正確的 Email 格式")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密碼為必填")]
        [MinLength(6, ErrorMessage = "密碼至少需 6 個字")]
        [MaxLength(100)]
        public string PasswordHash { get; set; }

        [MaxLength(255, ErrorMessage = "圖片路徑過長")]
        public string CoverImage { get; set; }

        [MaxLength(300, ErrorMessage = "自我介紹最多 300 字")]
        [DataType(DataType.MultilineText)]
        public string Biography { get; set; }
    }

    public class LogIn
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "密碼為必填")]
        [MinLength(6, ErrorMessage = "密碼至少需 6 個字")]
        [MaxLength(100)]
        public string PasswordHash { get; set; }
    }
}