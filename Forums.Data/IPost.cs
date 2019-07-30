using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forums.Data.Models
{
    public interface IPost
    {
        Post GetById(int id);
        IEnumerable<Post> GetAll();
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        IEnumerable<Post> GetPostsByForum(int id);
        IEnumerable<Post> GetLatestPosts(int nPosts);

        Task Add(Post post);
        Task Delete(int id);
        Task UpdatePostContent(int id, string newContent);
    }
}
