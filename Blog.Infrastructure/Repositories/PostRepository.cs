using Blog.Core.Entities;
using Blog.Core.Repositories;
using Blog.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private MyContext _myContext;
        public PostRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _myContext.Posts.ToListAsync();
        }
    }
}
