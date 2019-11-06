using Forums.Data;
using Forums.Data.Models;
using Forums.Models.ApplicationUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Forums.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IUpload _uploadService;
        private readonly IConfiguration _configuration;
        public ProfileController(UserManager<ApplicationUser> userManager, IApplicationUser userService, IUpload uploadService, IConfiguration configuration)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
            _configuration = configuration;
        }

        public IActionResult Detail(string id)
        {
            var user = _userService.GetUserById(id);
            var userRoles = _userManager.GetRolesAsync(user).Result;
            var model = new ProfileModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRating =user.Rating.ToString(),
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file) {
            var userId =_userManager.GetUserId(User);
            //connect  to an Azure Storage Account Container when the the connection is valid
            var connectionString = _configuration.GetConnectionString("AzureStorageAccount");
            //get Blob container
            var container = _uploadService.GetBlobContainer(connectionString, "profile-images");
            //Parse the Content Disposition responser header
            var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            //grab the filename
            var filename = contentDisposition.FileName.Trim('"');
            //get a reference to a block blob
            var blockBlob = container.GetBlockBlobReference(filename);
            //on the block blon, upload our file
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
            //set the user's profile imag to the URI
            await _userService.SetProfileImage(userId, blockBlob.Uri);
            //redirect to the user's profile page
            return RedirectToAction("Detail", "Profile", new { id = userId });
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Index() {
            var profiles = _userService.GetAll().
                OrderByDescending(user => user.Rating).
                Select(u => new ProfileModel {
                    Email = u.Email,
                    UserName = u.UserName,
                    ProfileImageUrl = u.ProfileImageUrl,
                    UserRating = u.Rating.ToString(),
                    MemberSince = u.MemberSince
                });
            var model = new ProfileListModel { Profiles = profiles };

            return View(model);
        }
    }
}