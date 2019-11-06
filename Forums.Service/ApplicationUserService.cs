using Forums.Data;
using Forums.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forums.Service
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(ApplicationDbContext context) {
            _context = context;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.ApplicationUsers;
        }

        public ApplicationUser GetUserById(string id)
        {
            return GetAll().FirstOrDefault(user=>user.Id == id);
        }

        public async Task UpdateUserRating(string id, Type type)
        {
            var user = GetUserById(id);
            user.Rating = CaculateUserRating(type, user.Rating);
            await _context.SaveChangesAsync();
        }

        private int CaculateUserRating(Type type, int userRating) {
            var inc = 0;
            if (type == typeof(Post))
                inc = 1;

            if (type == typeof(PostReply))
                inc = 3;
            return userRating+inc;
        }

        public async Task SetProfileImage(string id, Uri uri)
        {
            var user = GetUserById(id);
            user.ProfileImageUrl = uri.AbsoluteUri;
            _context.Update(user);
            await _context.SaveChangesAsync();
            
        }
    }
}
