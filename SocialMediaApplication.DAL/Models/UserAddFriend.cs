using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.DAL.Models
{
    public class UserAddFriend
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string FriendUserId { get; set; }
        public ApplicationUser FriendUser { get; set; }

    }
}
