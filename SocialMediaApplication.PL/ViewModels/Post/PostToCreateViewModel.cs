using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApplication.PL.ViewModels.Post
{
    public class PostToCreateViewModel
    {
        [Required(ErrorMessage ="Post Text Is Required")]
        [MinLength(1)]
        [MaxLength(200)]
        public string postText { get; set; }
        public IFormFile? postImage { get; set; }
        public string? postImageName { get; set; }
    }
}
