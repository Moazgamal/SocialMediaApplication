using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SocialMediaApplication.DAL.Data;
using SocialMediaApplication.DAL.Models;
using SocialMediaApplication.PL.ViewModels.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Services.Post
{
    public class PostService : IPostService 
    {
        private readonly ApplicationDbContext _dbContext;

        public PostService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UserLikePost?> IFUserLikePostAsync(string userId, int postId)
        {
            var existingLike = await _dbContext.UserLikePost
                .FirstOrDefaultAsync(l => l.userId == userId && l.postId == postId);
            return existingLike;
        }
        public async Task<int> ToggleLikeAsync(string userId, int postId)
        {
            // 1️⃣ Check if user can interact
            bool canInteract = await CanUserInteractAsync(userId, postId);
            if (!canInteract)
                return 0; 

            // 2️⃣ Check if like already exists
            var existingLike = await IFUserLikePostAsync(userId,postId);   

            if (existingLike == null)
            {
                // Add like
                var like = new UserLikePost { userId = userId, postId = postId };
                _dbContext.UserLikePost.Add(like);
                await _dbContext.SaveChangesAsync();
                return 1; // liked
            }
            else
            {
                // Remove like
                _dbContext.UserLikePost.Remove(existingLike);
                await _dbContext.SaveChangesAsync();
                return 2; // unliked
            }
        }

        public async Task<bool> CanUserInteractAsync(string userId, int postId)
        {
            return await _dbContext.Posts.AnyAsync(p =>
                p.Id == postId &&
                (
                    p.creatingUserId == userId ||  // Owner
                    _dbContext.UserAddFriendUsers.Any(f =>
                        (f.UserId == userId && f.FriendUserId == p.creatingUserId) ||
                        (f.FriendUserId == userId && f.UserId == p.creatingUserId))
                )
            );
        }
        public async Task<List<UserLikeViewModel>> GetPostLikesAsync(int postId)
        {
            var likes =  await _dbContext.UserLikePost
                    .AsNoTracking()
                    .Where(l => l.postId == postId)
                    .Select(l => new UserLikeViewModel
                    {
                        UserName = l.user.UserName,
                        ProfilePictureName = l.user.profilePictureName
                    })
                    .ToListAsync();
            return likes;
        }


    }
}
