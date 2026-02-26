using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.DAL.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        
        public string postText { get; set; }
        public string? postImageName { get; set; }
        [Required]
        public string creatingUserId { get; set; }
        public DateTime DateOfCreation { get; set; }  
        public ApplicationUser creatingUser { get; set; }

        public ICollection<UserLikePost> Likes { get; set; }
        //public ICollection<ApplicationUser>? Likes { get; set; }

    }
}
