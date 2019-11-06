using Forums.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Forums.Data
{
    public interface IApplicationUser
    {
        ApplicationUser GetUserById(string id);
        IEnumerable<ApplicationUser> GetAll();
        Task SetProfileImage(string id, Uri uri);
        Task UpdateUserRating(String id, Type type);
    }
}
