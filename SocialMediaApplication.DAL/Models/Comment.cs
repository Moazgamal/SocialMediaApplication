using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.DAL.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentText { get; set; } = string.Empty;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();
        public int LikesCount { get; set; } = 0; 

        public DateTime DateOfCreation { get; set; }


    }
}
