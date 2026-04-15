using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.DAL.Models
{
    public class CommentLike
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public int CommentId { get; set; }

        public ApplicationUser User { get; set; }
        public Comment Comment { get; set; }
    }
}
