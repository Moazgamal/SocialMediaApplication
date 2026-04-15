using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApplication.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public bool IsAgree { get; set; }
        public string profilePictureName { get; set; }

        public ICollection<Post>? createdPosts { get; set; } // Navigational Property Many

        public ICollection<UserLikePost> likedPosts { get; set; }
        //public ICollection<Post>? likedPosts { get; set; } // Navigational Property Many

        public ICollection<UserAddFriend>? Friends { get; set; }
        public ICollection<UserAddFriend>? FriendsBy { get; set; }

        public ICollection<Comment>? Comments { get; set; }

    }
}
