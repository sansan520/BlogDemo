using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostseedController : Controller
    {
        private MyContext _myContext;
        public PostseedController(MyContext myContext)
        {
            _myContext = myContext;
        }

        [HttpGet]
        public async Task<IActionResult>  Get()
        {
            var posts = await _myContext.Posts.ToListAsync();
            return Ok(posts);
        }
    }
}