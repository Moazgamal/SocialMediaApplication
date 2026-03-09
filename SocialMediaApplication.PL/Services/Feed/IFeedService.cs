using SocialMediaApplication.PL.ViewModels.Post;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Services.Feed
{
    public interface IFeedService
    {
        public Task<List<PostToReturnViewModel>> GetFeedAsync(string currentUserId, int? page = null, int? pageSize = null);
    }
}
