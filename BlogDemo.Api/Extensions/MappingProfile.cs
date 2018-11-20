using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Core.Entities;
using Blog.Infrastructure.Resources;

namespace BlogDemo.Api.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostResource>().ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.LastModified));

            CreateMap<PostResource, Post>();
        }
    }
}
