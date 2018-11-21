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

        Task<Post> GetPostByIdAsync(int id);
        void AddPost(Post entity);
        void Delete(Post post);
        void Update(Post post);
    }
}
