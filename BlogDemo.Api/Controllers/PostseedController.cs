using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Core.Repositories;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostseedController : Controller
    {
        private IPostRepository _postRepository;
        public PostseedController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [HttpGet]
        public async Task<IActionResult>  Get()
        {
            var posts = await _postRepository.GetAllPostsAsync();
            return Ok(posts);
        }
    }
}