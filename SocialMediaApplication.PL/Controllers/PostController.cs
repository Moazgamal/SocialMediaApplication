using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.BLL.Interfaces;
using SocialMediaApplication.BLL.Specifications;
using SocialMediaApplication.BLL.Specifications.PostSpecs;
using SocialMediaApplication.DAL.Data;
using SocialMediaApplication.DAL.Models;
using SocialMediaApplication.PL.Services.Post;
using SocialMediaApplication.PL.ViewModels.Comment;
using SocialMediaApplication.PL.ViewModels.Post;
using System.Collections;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IPostService _postService;
        private readonly ApplicationDbContext _dbContext;

        public PostController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IPostService postService,
            ApplicationDbContext dbContext
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _env = env;
            _postService = postService;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        //[Route("Post/ToggleLike")]
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (postId is null)
            //    return NotFound();

            var post = await _unitOfWork.Repository<Post>().GetAsync(postId);

            if (post is null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);



            int result = await _postService.ToggleLikeAsync(user.Id, postId);

            if (result == 0)
                return Forbid("User cannot interact with this post");
            

            if (result == 1)
            {
                return Json(new { liked = true });
            }
            
            return Json(new { liked = false });
        }

        [HttpPost]
        public async Task<IActionResult> GetPostLikes(string postId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int Id;

            bool isValid = int.TryParse(postId, out Id);

            if (!isValid)
            {
                Response.StatusCode = 400;
                return BadRequest();
            }

            var post = await _unitOfWork.Repository<Post>().GetAsync(Id);

            if (post is null)
                return NotFound();
            var likes = await _postService.GetPostLikesAsync(Id);
            return PartialView("~/Views/Home/HomePartialViews/PostLikes.cshtml", likes);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostWithComments(string? postId)
        {
            if (!ModelState.IsValid || postId is null)  
                return BadRequest(ModelState);

            int Id;

            bool isValid = int.TryParse(postId, out Id);

            if (!isValid)
            {
                Response.StatusCode = 400;
                return BadRequest();
            }

            var post = await _unitOfWork.Repository<Post>().GetAsync(Id);

            if (post is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            var postToReturn = new PostToReturnViewModel
            {
                Id = post.Id,
                creatingUserName = post.creatingUser.UserName,
                creatingUserImageName = post.creatingUser.profilePictureName,
                postText = post.postText,
                postImageName = post.postImageName,
                DateOfCreation = post.DateOfCreation,
                NumberOfLikes = await _dbContext.UserLikePost.CountAsync(x => x.postId == Id),
                IsLikedByCurrentUser = await _postService.IFUserLikePostAsync(user.Id, Id) is null ? false : true,
            };

            var comments =  await _dbContext.Comments.AsNoTracking()
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Where(c => c.PostId == Id)
                .OrderByDescending(c => c.DateOfCreation)
                .Take(5)
                .Select(c => new CommentToReturnViewModel
                {
                    Id = c.Id,
                    Content = c.CommentText,
                    UserName = c.User.UserName,
                    UserImageName = c.User.profilePictureName,
                    NumberOfLikes = c.Likes.Count(),
                    IsLikedByCurrentUser =  c.Likes
                        .Any(l => l.UserId == user.Id),
                    DateOfCreation = c.DateOfCreation
                })
                .ToListAsync();

            var totalCommentsCount = await _dbContext.Comments.AsNoTracking()
                .CountAsync(c => c.PostId == Id);


            var result = new PostWithCommentsViewModel
            {
                Post = postToReturn,
                Comments = comments,
                CommentsCount = totalCommentsCount
            };

            return  PartialView("~/Views/Home/HomePartialViews/PostWithComments.cshtml", result);
        }
    }
}
