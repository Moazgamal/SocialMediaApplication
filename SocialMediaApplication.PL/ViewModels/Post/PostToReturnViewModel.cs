using System;

namespace SocialMediaApplication.PL.ViewModels.Post
{
    public class PostToReturnViewModel
    {
        public int Id { get; set; }
        public string creatingUserName { get; set; }
        public string creatingUserImageName { get; set; }
        public string postText { get; set; }
        public string postImageName { get; set; }
        public DateTime DateOfCreation { get; set; }
    }
}
