using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.DAL.Models
{
    public class UserLikePost
    {
        public string userId { get; set; }
        public ApplicationUser user { get; set; }

        public int postId { get; set; }
        public Post post { get; set; }
    }
}
