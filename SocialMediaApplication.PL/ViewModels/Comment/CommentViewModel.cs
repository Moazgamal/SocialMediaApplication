using System.ComponentModel.DataAnnotations;

namespace SocialMediaApplication.PL.ViewModels.Comment
{
    public class CommentViewModel
    {
        [Required(ErrorMessage = "Comment is required")]
        [MaxLength(200, ErrorMessage = "Max 200 characters")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string postId { get; set; }
    }
}
