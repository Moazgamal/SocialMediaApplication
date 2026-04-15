using System;

namespace SocialMediaApplication.PL.ViewModels.Comment
{
    public class CommentToReturnViewModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime DateOfCreation { get; set; }

        // user info
        public string UserName { get; set; }
        public string UserImageName { get; set; }

        // likes
        public int NumberOfLikes { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
}
