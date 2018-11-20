using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Core.Repositories;
using Blog.Core;
using Blog.Core.Entities;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Blog.Infrastructure.DTOResources;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostseedController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public PostseedController(IPostRepository postRepository, IUnitOfWork unitOfWork,ILoggerFactory loggerFactory, IMapper mapper, IUrlHelper urlHelper)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger("BlogDemo.Api.Controllers.PostseedController");
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name ="GetPosts")]
        public async Task<IActionResult>  Get(PostParameters parameters)
        {
            var posts = await _postRepository.GetAllPostsAsync(parameters);
            var postsDto = _mapper.Map<IEnumerable<Post>,IEnumerable<PostResource>>(posts);
            //_logger.LogInformation("Get all posts....hello...");
            //添加返回header - 分页信息
            var previousPageLink = posts.HasPrevious ?
                CreatePostUri(parameters, PaginationResourceUriType.PreviousPage) : null;

            var nextPageLink = posts.HasNext ?
                CreatePostUri(parameters, PaginationResourceUriType.NextPage) : null;
            var meta = new
            {
                PageSize = posts.PageSize,
                PageIndex = posts.PageIndex,
                TotalItemsCount = posts.TotalItemsCount,
                PageCount = posts.PageCount,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink,
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            return Ok(postsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var post = await _postRepository.GetPostByid(id);
            if (null==post) {
                return NotFound();
            }
            var postDto = _mapper.Map<Post,PostResource>(post);
            return Ok(postDto);
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


        private string CreatePostUri(PostParameters parameters, PaginationResourceUriType uriType)
        {
            switch (uriType)
            {
                case PaginationResourceUriType.PreviousPage:
                    var previousParameters = new
                    {
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", nextParameters);
                default:
                    var currentParameters = new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", currentParameters);
            }
        }
    }
}