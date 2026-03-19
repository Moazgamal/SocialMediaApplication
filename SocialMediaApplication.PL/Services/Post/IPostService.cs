using SocialMediaApplication.DAL.Models;
using SocialMediaApplication.PL.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Services.Post
{
    public interface IPostService
    {
        Task<int> ToggleLikeAsync(string userId, int postId);
        Task<bool> CanUserInteractAsync(string userId, int postId);
        Task<UserLikePost?> IFUserLikePostAsync(string userId, int postId);
        Task<List<UserLikeViewModel>> GetPostLikesAsync(int postId);
    }
}
