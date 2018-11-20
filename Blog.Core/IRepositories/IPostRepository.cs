using Blog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Repositories
{
    public interface IPostRepository
    {
        Task<PaginatedList<Post>> GetAllPostsAsync(PostParameters parameters);

        Task<Post> GetPostByid(int id);
        void AddPost(Post entity);
    }
}
