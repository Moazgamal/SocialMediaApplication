using Microsoft.EntityFrameworkCore;
using SocialMediaApplication.BLL.Interfaces;
using SocialMediaApplication.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.BLL.Specifications.PostSpecs
{
    public class CanWatchOrInteractWithPostSpecification : BaseSpecifications<Post>
    {
        public CanWatchOrInteractWithPostSpecification(string currentUserId, int postId)
        : base(p =>
            p.Id == postId &&
            (
                p.creatingUserId == currentUserId ||

                p.creatingUser.Friends.Any(f =>
                    f.FriendUserId == currentUserId
                ) ||

                p.creatingUser.FriendsBy.Any(f =>
                    f.UserId == currentUserId
                )
            )
        )
        {
        }
        public CanWatchOrInteractWithPostSpecification(string currentUserId, IGenericRepository<UserAddFriend> UserAddFriendsTable)
            : base(
                 p =>
        p.creatingUserId == currentUserId ||
         UserAddFriendsTable.AnyWithSpec(new BaseSpecifications<UserAddFriend>(f =>
            (f.UserId == currentUserId && f.FriendUserId == p.creatingUserId) ||
            (f.FriendUserId == currentUserId && f.UserId == p.creatingUserId)
                 ))
                 )
        {
        }
                 
                 
            
        
                 

    }
}
