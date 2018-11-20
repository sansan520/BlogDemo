using Blog.Core.Entities;
using Blog.Core.Repositories;
using Blog.Infrastructure.DataBase;
using Blog.Infrastructure.DTOResources;
using Blog.Infrastructure.Extensions;
using Blog.Infrastructure.PropertyMappingServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly MyContext _myContext;
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        public PostRepository(MyContext myContext, IPropertyMappingContainer propertyMappingContainer)
        {
            _myContext = myContext;
            _propertyMappingContainer = propertyMappingContainer;
        }

        public async Task<PaginatedList<Post>> GetAllPostsAsync(PostParameters parameters)
        {
            var query = _myContext.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(parameters.Title))
            {
                var title = parameters.Title.ToLowerInvariant();
                query = query.Where(x => x.Title.ToLowerInvariant() == title); // 字段查询
            }

            //query = query.OrderBy(x=>x.Id);
            //自定义排序扩展方法
            query = query.ApplySort(parameters.OrderBy, _propertyMappingContainer.Resolve<PostResource, Post>());
            var totalItemsCount = await query.CountAsync();
            var data = await query.Skip(parameters.PageIndex * parameters.PageSize)
                .Take(parameters.PageSize).ToListAsync();
            var paginatedList = new PaginatedList<Post>(parameters.PageIndex, parameters.PageSize,totalItemsCount ,data);
            return paginatedList;
        }

        //[Obsolete]
        //public async Task<IEnumerable<Post>> GetAllPostsAsyncold(PostParameters parameters)
        //{
        //    var postList = _myContext.Posts.AsQueryable();
        //    if(!string.IsNullOrEmpty(parameters.Title)) {
        //        var title = parameters.Title.ToLowerInvariant();
        //        postList = postList.Where(x=>x.Title.ToLowerInvariant() == title);
        //    }
        //    var data = await postList.Skip(parameters.PageIndex*parameters.PageSize)
        //        .Take(parameters.PageSize).ToListAsync();
        //    return data;
        //}

        public void AddPost(Post post)
        {
            _myContext.Posts.Add(post);
        }

        public async Task<Post> GetPostByid(int id)
        {
            return await _myContext.Posts.FindAsync(id);
        }
    }
}
