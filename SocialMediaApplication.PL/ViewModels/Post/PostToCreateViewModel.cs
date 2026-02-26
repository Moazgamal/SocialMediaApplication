using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApplication.PL.ViewModels.Post
{
    public class PostToCreateViewModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(200)]
        public string postText { get; set; }
        public IFormFile postImage { get; set; }
    }
}
