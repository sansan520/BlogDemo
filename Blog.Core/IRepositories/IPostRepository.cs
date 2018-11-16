using Blog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Repositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
    }
}
