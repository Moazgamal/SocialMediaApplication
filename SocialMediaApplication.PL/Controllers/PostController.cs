using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.BLL.Interfaces;
using SocialMediaApplication.BLL.Specifications;
using SocialMediaApplication.BLL.Specifications.PostSpecs;
using SocialMediaApplication.DAL.Data;
using SocialMediaApplication.DAL.Models;
using SocialMediaApplication.PL.Helpers;
using SocialMediaApplication.PL.Services.Post;
using SocialMediaApplication.PL.ViewModels.Comment;
using SocialMediaApplication.PL.ViewModels.Post;
using System;
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

            var postToReturn = new PostToReturnViewModel
            {
                Id = post.Id,
                creatingUserName = post.creatingUser.UserName,
                creatingUserImageName = post.creatingUser.profilePictureName,
                postText = post.postText,
                postImageName = post.postImageName,
                DateOfCreation = post.DateOfCreation,
                NumberOfLikes = await _dbContext.UserLikePost.CountAsync(x => x.postId == Id),
                NumberOfComments = totalCommentsCount,
                IsLikedByCurrentUser = await _postService.IFUserLikePostAsync(user.Id, Id) is null ? false : true,
            };

            var result = new PostWithCommentsViewModel
            {
                Post = postToReturn,
                Comments = comments,
                CommentsCount = totalCommentsCount
            };

            return  PartialView("~/Views/Home/HomePartialViews/PostWithComments.cshtml", result);
        }


        // ADD COMMENT

        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel commentVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int Id;

            bool isValid = int.TryParse(commentVM.postId, out Id);    

            if (!isValid)
            {
                Response.StatusCode = 400;
                return BadRequest();
            }

            var post = await _unitOfWork.Repository<Post>().GetAsync(Id);

            if (post is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            var newComment = new Comment
            {
                CommentText = commentVM.Content,
                UserId = user.Id,
                PostId = Id,
            };
            try
            {
                _unitOfWork.Repository<Comment>().Add(newComment);
                var count = await _unitOfWork.Complete();
                if(count==0)
                {
                    Response.StatusCode = 400;
                    ModelState.AddModelError(string.Empty, "An Error Has Occured Adding Comment");
                    return BadRequest(ModelState);
                }
            }
            catch (System.Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                ModelState.AddModelError(string.Empty, "An Error Has Occured Adding Comment");
                Response.StatusCode = 400;
            }
            var commentToReturn = new CommentToReturnViewModel
            {
                Id = newComment.Id,
                Content = newComment.CommentText,   
                UserName = user.UserName,
                UserImageName = user.profilePictureName,
                NumberOfLikes = 0,
                IsLikedByCurrentUser = false,
                DateOfCreation = newComment.DateOfCreation
            };
            return PartialView("~/Views/Home/HomePartialViews/Comment.cshtml", commentToReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetPrivileges(int commentId)
        {
            var user = await _userManager.GetUserAsync(User);

            var comment = _dbContext.Comments.FirstOrDefault(c => c.Id == commentId);

            if (comment == null)
                return NotFound();

            bool IsOwner = comment.UserId == user.Id;

            if (IsOwner)
            {
                return Json(new { isOwner = true });
            }

            return  Json(new { isOwner = true });
        }



        // UPDATE COMMENT

        [HttpPost]

        public async Task<IActionResult> UpdateComment(int commentId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false });

            var user = await _userManager.GetUserAsync(User);
            var comment = await _unitOfWork.Repository<Comment>().GetAsync(commentId);

            if (comment == null || comment.UserId != user.Id)
                return Json(new { success = false });

            comment.CommentText = content;

            var count = await _unitOfWork.Complete();

            return Json(new { success = count > 0 });
        }


        // DELETE COMMENT

        [HttpPost]
        public async Task<IActionResult> DeleteComment([FromBody] string? commentId)
        {
            if (commentId is null)
                return Json(new
                {
                    success = false,
                });
            int Id;

            bool isValid = int.TryParse(commentId, out Id);

            if (!isValid)
                return Json(new
                {
                    success = false,
                });

            var user = await _userManager.GetUserAsync(User);
            var comment = await _unitOfWork.Repository<Comment>().GetAsync(Id);
            if (comment is null || comment.UserId != user.Id)
                return Json(new
                {
                    success = false
                });

            try
            {
                var likes = _dbContext.UserLikeComment
                                .Where(l => l.CommentId == Id);

                _dbContext.UserLikeComment.RemoveRange(likes);

                _unitOfWork.Repository<Comment>().Delete(comment);
                var count = await _unitOfWork.Complete();
                if (count == 0)
                {
                    return Json(new
                    {
                        success = false
                    });
                }
                return Json(new
                {
                    success = true,
                });

            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                ModelState.AddModelError(string.Empty, "An Error Has Occured Deleting Comment");
                return Json(new
                {
                    success = false
                });
            }
        }


        // TOGGLE COMMENT LIKE

        [HttpPost]
        
        public async Task<IActionResult> ToggleCommentLike(int commentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (postId is null)
            //    return NotFound();

            var comment = await _unitOfWork.Repository<Comment>().GetAsync(commentId);

            if (comment is null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);


            var existingLike = await _dbContext.UserLikeComment
                .FirstOrDefaultAsync(l => l.UserId == user.Id && l.CommentId == commentId);

            if (existingLike is null)
            {
                // Add like
                var commentlike = new CommentLike { UserId = user.Id, CommentId = comment.Id };
                _dbContext.UserLikeComment.Add(commentlike);
                await _dbContext.SaveChangesAsync();
                return Json(new { liked = true }); // liked
            }
            // Remove like
            _dbContext.UserLikeComment.Remove(existingLike);
            await _dbContext.SaveChangesAsync();

            return Json(new { liked = false });
        }


    }
}
