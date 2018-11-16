using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Core.Repositories;
using Blog.Core;
using Blog.Core.Entities;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostseedController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PostseedController(IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;

        }

        [HttpGet]
        public async Task<IActionResult>  Get()
        {
            var posts = await _postRepository.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var entity = new Post
            {
                Author = "huaisan",
                Body = "hello huaisan",
                Title = "Title of huaisan",
                LastModified = DateTime.Now
            };
            _postRepository.AddPost(entity);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}