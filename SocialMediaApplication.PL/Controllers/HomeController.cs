using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddPost(PostToCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postName = DocumentSettings.UploadFile(model.postImage, "images");
                var user = await _userManager.GetUserAsync(User);
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
                        DocumentSettings.DeleteFile(postName, "images");
                        Response.StatusCode = 400;
                        return PartialView("CreatePost", model);
                    }
                }
                catch (Exception ex)
                {
                    // delete the image
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
                return PartialView("Post", postToReturn);
            }
            Response.StatusCode = 400;
            return PartialView("CreatePost", model);
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

    }
}
