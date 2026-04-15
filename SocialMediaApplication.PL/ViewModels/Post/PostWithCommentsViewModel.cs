using SocialMediaApplication.PL.ViewModels.Comment;
using System.Collections.Generic;

namespace SocialMediaApplication.PL.ViewModels.Post
{
    public class PostWithCommentsViewModel
    {
        public PostToReturnViewModel Post { get; set; }
        public List<CommentToReturnViewModel> Comments { get; set; }
        public int CommentsCount { get; set; } 
    }   
}
