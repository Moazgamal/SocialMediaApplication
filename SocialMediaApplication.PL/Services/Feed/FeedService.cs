using Microsoft.EntityFrameworkCore;
using SocialMediaApplication.DAL.Data;
using SocialMediaApplication.PL.ViewModels.Post;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Services.Feed
{
    public class FeedService : IFeedService
    {
        private readonly ApplicationDbContext _dbContext;

        public FeedService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<PostToReturnViewModel>> GetFeedAsync(string currentUserId, int? page = null, int? pageSize = null)
        {
            var query =await  _dbContext.Posts
                .Where(p =>
                    p.creatingUserId == currentUserId ||
                    _dbContext.UserAddFriendUsers.Any(f =>
                        (f.UserId == currentUserId && f.FriendUserId == p.creatingUserId) ||
                        (f.FriendUserId == currentUserId && f.UserId == p.creatingUserId)
                    )
                    
                ).Include(p => p.creatingUser)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.DateOfCreation)
                .ToListAsync();
            var returnedPosts = query.Select(p => new PostToReturnViewModel
            {
                Id = p.Id,
                postText = p.postText,
                postImageName = p.postImageName,
                creatingUserName = p.creatingUser.UserName,
                creatingUserImageName = p.creatingUser.profilePictureName,
                NumberOfLikes = p.Likes.Count(),
                IsLikedByCurrentUser = p.Likes.Any(l => l.userId == currentUserId),
            })
                .ToList();
            //.Skip((page - 1) * pageSize)
            //.Take(pageSize);

            return   returnedPosts;
        }
    }

}
