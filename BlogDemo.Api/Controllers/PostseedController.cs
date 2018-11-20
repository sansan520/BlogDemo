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
using Blog.Infrastructure.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Blog.Infrastructure.Extensions;
using Blog.Infrastructure.HelperServices;
using BlogDemo.Api.Helpers;

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
        private readonly ITypeService _typeService;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public PostseedController(IPostRepository postRepository, IUnitOfWork unitOfWork,ILoggerFactory loggerFactory, IMapper mapper, IUrlHelper urlHelper,ITypeService typeService, IPropertyMappingContainer propertyMappingContainer)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger("BlogDemo.Api.Controllers.PostseedController");
            _mapper = mapper;
            _urlHelper = urlHelper;
            _typeService = typeService;
            _propertyMappingContainer = propertyMappingContainer;
        }

        /// <summary>
        /// Hateoas相关： Accept 请求头 为供应商自定义 格式application/vnd.huaisan.hateoas+json
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetPosts")]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.huaisan.hateoas+json" })]
        public async Task<IActionResult> GetHateoas(PostParameters parameters)
        {
            if (!_propertyMappingContainer.ValidateMappingExistsFor<PostResource, Post>(parameters.OrderBy))
            {
                return BadRequest("Can't finds fields for sorting."); //若请求排序参数中有dto类不存在的字段属性，则返回400错误
            }
            if (_typeService.TypeHasProperties<PostResource>(parameters.Fields))
            {
                return BadRequest("Fields not exist."); ; //若请求参数中有dto类不存在的字段属性，则返回400错误
            }
            var posts = await _postRepository.GetAllPostsAsync(parameters);
            IEnumerable<PostResource> postsDto = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResource>>(posts);
            var shapedPostResources = postsDto.ToDynamicIEnumerable(parameters.Fields);

            var shapedWithLinks = shapedPostResources.Select(x=> {
                var dict = x as IDictionary<string, object>;
                var postlinks = CreateLinksForPost((int)dict["Id"],parameters.Fields);
                dict.Add("links",postlinks);
                return dict;
            });

            #region 添加额外的header信息
            var links = CreateLinksForPosts(parameters, posts.HasPrevious, posts.HasNext);
            var result = new
            {
                value = shapedWithLinks,
                links
            };
            var meta = new
            {
                posts.PageSize,
                posts.PageIndex,
                posts.TotalItemsCount,
                posts.PageCount,
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            #endregion
            return Ok(shapedPostResources);
        }

        /// <summary>
        /// Accept 为application/json格式
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet(Name ="GetPosts")]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/json" })]
        public async Task<IActionResult>  Get(PostParameters parameters)
        {
            if (!_propertyMappingContainer.ValidateMappingExistsFor<PostResource, Post>(parameters.OrderBy))
            {
                return BadRequest("Can't finds fields for sorting."); //若请求排序参数中有dto类不存在的字段属性，则返回400错误
            }
            if (_typeService.TypeHasProperties<PostResource>(parameters.Fields)) {
                return BadRequest("Fields not exist."); ; //若请求参数中有dto类不存在的字段属性，则返回400错误
            }
            var posts = await _postRepository.GetAllPostsAsync(parameters);
            IEnumerable<PostResource> postsDto = _mapper.Map<IEnumerable<Post>,IEnumerable<PostResource>>(posts);
            //?fields=id,title,body,author 客户端可指定需要返回的字段<-->资源塑型

            var result = postsDto.ToDynamicIEnumerable(parameters.Fields);
            //_logger.LogInformation("Get all posts....hello...");
            #region 添加额外的header信息
            //添加返回header - 分页信息
            var previousPageLink = posts.HasPrevious ?
                CreatePostUri(parameters, PaginationResourceUriType.PreviousPage) : null;

            var nextPageLink = posts.HasNext ?
                CreatePostUri(parameters, PaginationResourceUriType.NextPage) : null;
            var meta = new
            {
                posts.PageSize,
                posts.PageIndex,
                posts.TotalItemsCount,
                posts.PageCount,
                previousPageLink,
                nextPageLink,
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })); 
            #endregion
            return Ok(result);
        }

        [HttpGet("{id}")]
        //[HttpGet("{id}", Name = "GetPost")]
        public async Task<IActionResult> Get(int id,string fields=null)
        {
            if (!_typeService.TypeHasProperties<PostResource>(fields))
            {
                return BadRequest("Fields not exist.");
            }
            var post = await _postRepository.GetPostByid(id);
            if (null==post) {
                return NotFound();
            }
            var postDto = _mapper.Map<Post,PostResource>(post);

            var shapedPostResource = postDto.ToDynamic(fields);
            var links = CreateLinksForPost(id, fields);
            var result = (IDictionary<string, object>)shapedPostResource;
            result.Add("links", links);

            return Ok(result);
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


        private IEnumerable<LinkResource> CreateLinksForPost(int id, string fields = null)
        {
            var links = new List<LinkResource>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add( new LinkResource( _urlHelper.Link("GetPost", new { id }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkResource(_urlHelper.Link("GetPost", new { id, fields }), "self", "GET"));
            }

            links.Add( new LinkResource( _urlHelper.Link("DeletePost", new { id }), "delete_post", "DELETE"));

            return links;
        }

        private IEnumerable<LinkResource> CreateLinksForPosts(PostParameters postResourceParameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkResource>
            {
                new LinkResource(CreatePostUri(postResourceParameters, PaginationResourceUriType.CurrentPage),"self", "GET")
            };

            if (hasPrevious)
            {
                links.Add( new LinkResource(CreatePostUri(postResourceParameters, PaginationResourceUriType.PreviousPage), "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(new LinkResource( CreatePostUri(postResourceParameters, PaginationResourceUriType.NextPage), "next_page", "GET"));
            }

            return links;
        }
    }
}