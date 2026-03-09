using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Services.Post
{
    public interface IPostService
    {
        Task<int> ToggleLikeAsync(string userId, int postId);
        Task<bool> CanUserInteractAsync(string userId, int postId);
    }
}
