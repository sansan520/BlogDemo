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
using Microsoft.AspNetCore.JsonPatch;

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
            if (!_typeService.TypeHasProperties<PostResource>(parameters.Fields))
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
            return Ok(result);
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
                return BadRequest("Can't finds fields for sorting.");
            }

            if (!_typeService.TypeHasProperties<PostResource>(parameters.Fields))
            {
                return BadRequest("Fields not exist.");
            }

            var postList = await _postRepository.GetAllPostsAsync(parameters);

            var postResources = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResource>>(postList);

            var previousPageLink = postList.HasPrevious ?
                CreatePostUri(parameters,
                    PaginationResourceUriType.PreviousPage) : null;

            var nextPageLink = postList.HasNext ?
                CreatePostUri(parameters,
                    PaginationResourceUriType.NextPage) : null;

            var meta = new
            {
                postList.TotalItemsCount,
                postList.PageSize,
                postList.PageIndex,
                postList.PageCount,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            // 根据参数field 进行定制返回
            return Ok(postResources.ToDynamicIEnumerable(parameters.Fields));
        }

        [HttpGet("{id}", Name = "GetPost")]
        public async Task<IActionResult> Get(int id,string fields=null)
        {
            if (!_typeService.TypeHasProperties<PostResource>(fields))
            {
                return BadRequest("Fields not exist.");
            }
            var post = await _postRepository.GetPostByIdAsync(id);
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

        [HttpPost(Name = "CreatePost")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.huaisan.post.create+json" })]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.huaisan.hateoas+json" })]
        public async Task<IActionResult> Post([FromBody] PostAddResource postAddResource)
        {
            if (postAddResource == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid) {
                return new MyUnprocessableEntityObjectResult(ModelState); //自定义错误输出（比较符合）
                //return UnprocessableEntity(ModelState); // 原生错误类型输出
            }

            var newPost = _mapper.Map<PostAddResource, Post>(postAddResource);
            newPost.Author = "admin";
            newPost.LastModified = DateTime.Now;
            _postRepository.AddPost(newPost);
            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("添加保存失败！");
            }
            var resultResource = _mapper.Map<Post, PostResource>(newPost);
            var links = CreateLinksForPost(newPost.Id);
            var linkedPostResource = resultResource.ToDynamic() as IDictionary<string, object>;
            linkedPostResource.Add("links", links);

            return CreatedAtRoute("GetPost", new { id = linkedPostResource["Id"] }, linkedPostResource);

        }

        [HttpDelete("{id}", Name = "DeletePost")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _postRepository.Delete(post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Deleting post {id} failed when saving.");
            }

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdatePost")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.huaisan.post.update+json" })]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateResource postUpdate)
        {
            if (postUpdate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.LastModified = DateTime.Now;
            _mapper.Map(postUpdate, post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Updating post {id} failed when saving.");
            }
            return NoContent();
        }


        [HttpPatch("{id}", Name = "PartiallyUpdatePost")]
        public async Task<IActionResult> PartiallyUpdateCityForCountry(int id, [FromBody] JsonPatchDocument<PostUpdateResource> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var postToPatch = _mapper.Map<PostUpdateResource>(post);

            patchDoc.ApplyTo(postToPatch, ModelState);

            TryValidateModel(postToPatch);

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(postToPatch, post);
            post.LastModified = DateTime.Now;
            _postRepository.Update(post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Patching city {id} failed when saving.");
            }

            return NoContent();
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
                links.Add(new LinkResource( _urlHelper.Link("GetPost", new { id }), "self", "GET"));
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