using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.BLL.Interfaces;
using SocialMediaApplication.DAL.Models;
using SocialMediaApplication.PL.Helpers;
using SocialMediaApplication.PL.ViewModels;
using SocialMediaApplication.PL.ViewModels.Post;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _env = env;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            //ViewData["currentUserName"] = user.UserName;
            //ViewData["currentUserProfilePicture"] = user.profilePictureName;
            var posts = await _unitOfWork.Repository<Post>().GetAllAsync() as IEnumerable<Post>;
            List<PostToReturnViewModel> returnedPosts = new List<PostToReturnViewModel>();
            
            foreach ( var post in posts)
            {
                var newPost = new PostToReturnViewModel
                {
                    Id= post.Id,
                    creatingUserImageName = user.profilePictureName,
                    creatingUserName = user.UserName,
                    postImageName = post.postImageName,
                    postText = post.postText,
                    DateOfCreation = post.DateOfCreation
                };
                returnedPosts.Add(newPost); 
            }
            return View("Index", returnedPosts);
        }
        [HttpPost]
        public async Task<IActionResult> GetCreateFormAsync([FromBody]string? postId)
        {
            if(postId is null)
                return Json(new
                {
                    success = false,
                });
            if(postId == "Create")
                return PartialView("HomePartialViews/CreatePost");

            int Id;

            bool isValid = int.TryParse(postId, out Id);

            if (!isValid)
                return Json(new
                {
                    success = false,
                });

            var user = await _userManager.GetUserAsync(User);
            var post = await _unitOfWork.Repository<Post>().GetAsync(Id);
            if (post is null || post.creatingUserId != user.Id)
                return Json(new
                {
                    success = false
                });
            var postImage = post.postImageName;
            
            var postToCreate = new PostToCreateViewModel
            {
                postText = post.postText,
                postImageName = postImage
            };
            
            return PartialView("HomePartialViews/CreatePost", postToCreate);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdatePost(PostToCreateViewModel model, string? postId)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return PartialView("HomePartialViews/CreatePost", model);
            }
            if(postId !="undefined" && postId is not null)
            {
                int Id;

                bool isValid = int.TryParse(postId, out Id);

                if (!isValid)
                {
                    Response.StatusCode = 400;
                    return PartialView("HomePartialViews/CreatePost", model);
                }
                var user = await _userManager.GetUserAsync(User);
                var post = await _unitOfWork.Repository<Post>().GetAsync(Id);
                if (post is null || post.creatingUserId != user.Id)
                {
                    Response.StatusCode = 400;
                    return PartialView("HomePartialViews/CreatePost", model);
                }
                var postImage = "";
                if(post.postImageName is not null && post.postImageName != "")
                    postImage = post.postImageName;
                 var newImageName = "";
                try
                {
                    post.postText = model.postText;
                    if (model.postImage is not null)
                        newImageName = DocumentSettings.UploadFile(model.postImage, "images");
                    post.postImageName = newImageName;
                    //_unitOfWork.Repository<Post>().Update(post);
                    var count = await _unitOfWork.Complete();
                    if (count == 0)
                    {
                        DocumentSettings.DeleteFile(newImageName, "images");
                        Response.StatusCode = 400;
                        return PartialView("HomePartialViews/CreatePost", model);
                    }
                }
                catch (Exception ex)
                {
                    DocumentSettings.DeleteFile(newImageName, "images");
                    if (_env.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    ModelState.AddModelError(string.Empty, "An Error Has Occured Adding Post");
                    Response.StatusCode = 400;
                    return PartialView("CreatePost", model);
                }
                if(postImage is not null && postImage != "")
                    DocumentSettings.DeleteFile(postImage, "images");
                
                var postToReturn = new PostToReturnViewModel
                {
                    Id = post.Id,
                    creatingUserName = user.UserName,
                    creatingUserImageName = user.profilePictureName,
                    postText = post.postText,
                    postImageName = post.postImageName,
                    DateOfCreation = post.DateOfCreation
                };
                return PartialView("HomePartialViews/Post", postToReturn);
            }
            if (ModelState.IsValid)
            {
                string postName = "";
                if(model.postImage is not null)
                    postName = DocumentSettings.UploadFile(model.postImage, "images");
                var user =  await _userManager.GetUserAsync(User);
                var post = new Post {
                    creatingUserId = user.Id,
                    postText = model.postText,
                    postImageName= postName
                };
                try
                {
                    _unitOfWork.Repository<Post>().Add(post);
                    var count = await _unitOfWork.Complete();
                    if(count == 0)
                    {
                        // delete the image
                        if(postName != "")
                            DocumentSettings.DeleteFile(postName, "images");
                        Response.StatusCode = 400;
                        return PartialView("HomePartialViews/CreatePost", model);
                    }
                }
                catch (Exception ex)
                {
                    // delete the image
                    if (postName != "")
                        DocumentSettings.DeleteFile(postName, "images");
                    if (_env.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    ModelState.AddModelError(string.Empty, "An Error Has Occured Adding Post");
                    Response.StatusCode = 400;
                    return PartialView("CreatePost", model);
                }
                var postToReturn = new PostToReturnViewModel { 
                    Id = post.Id,
                    creatingUserName = user.UserName,
                    creatingUserImageName = user.profilePictureName,
                    postText = post.postText, 
                    postImageName=post.postImageName,
                    DateOfCreation = post.DateOfCreation
                };
                return PartialView("HomePartialViews/Post", postToReturn);
            }
            Response.StatusCode = 400;
            return PartialView("HomePartialViews/CreatePost", model);
         }

        [HttpPost]
        public async Task<IActionResult> DeletePost([FromBody]string? postId)
        {
            if (postId is null)
                return Json(new
                {
                    success = false,
                });
            int Id;

            bool isValid = int.TryParse(postId, out Id);

            if (!isValid)
                return Json(new
                {
                    success = false,
                });

            var user = await _userManager.GetUserAsync(User);
            var post = await _unitOfWork.Repository<Post>().GetAsync(Id);
            if (post is null || post.creatingUserId != user.Id)
                return Json(new
                {
                    success = false
                });
            var postImage = post.postImageName;
            try
            {
                _unitOfWork.Repository<Post>().Delete(post);
                var count = await _unitOfWork.Complete();
                if (count == 0)
                {
                    return Json(new
                    {
                        success = false
                    });
                }
                if (postImage is not null && postImage != "")
                    DocumentSettings.DeleteFile(postImage, "images");
                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                ModelState.AddModelError(string.Empty, "An Error Has Occured Deleting Post");
                return Json(new
                {
                    success = false
                });
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> checkPostPrivilege([FromBody]string? postId)
        {
            if (postId is null)
                return Json(new
                {
                    success = false,
                });
            int Id;

            bool isValid = int.TryParse(postId, out Id);

            if (!isValid)
                return Json(new
                {
                    success = false,
                    IsPostCreator = false
                });
            var user = await _userManager.GetUserAsync(User);
            var post = await _unitOfWork.Repository<Post>().GetAsync(Id);
            if (post is null || post.creatingUserId != user.Id)
                return Json(new
                {
                    success = false,
                    IsPostCreator = false
                });
            return Json(new
            {
                success = true,
                IsPostCreator = true
            });
        }



    }
}
