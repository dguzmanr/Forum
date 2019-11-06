using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forums.Data;
using Forums.Data.Models;
using Forums.Models.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forums.Controllers
{
    [Authorize]
    public class ReplyController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _applicationUser;

        public ReplyController(IPost postService, UserManager<ApplicationUser> userManager, IApplicationUser applicationUser)
        {
            _postService = postService;
            _userManager = userManager;
            _applicationUser = applicationUser;
        }

        //id particullar for a post
        public async Task<IActionResult> Create(int id)
        {
            var post = _postService.GetById(id);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var model = new PostReplyModel {
                PostContent = post.Content,
                PostTitle = post.Title,
                PostId = post.Id,
                AuthorId = user.Id,
                AuthorName = User.Identity.Name,
                AuthorImageUrl =user.ProfileImageUrl,
                AuthorRating =user.Rating,
                IsAuthorAdmin = User.IsInRole("Admin"),
                ForumId = post.Forum.Id,
                ForumName = post.Forum.Title,
                ForumImageUrl = post.Forum.ImageUrl,
                Created = DateTime.Now

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReply(PostReplyModel model) {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var reply = BuildReply(model, user);
            await _postService.AddReply(reply);
            //Add rating
            await _applicationUser.UpdateUserRating(userId, typeof(PostReply));

            return RedirectToAction("Index", "Post", new { id = model.PostId});
        }

        private PostReply BuildReply(PostReplyModel model, ApplicationUser user)
        {
            var post = _postService.GetById(model.PostId);
            return new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                User = user
            };
        }

    }
}