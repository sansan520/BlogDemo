using Blog.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.PropertyMappingServices
{
    public interface IPropertyMappingContainer
    {
        void Register<T>() where T : IPropertyMapping, new();
        IPropertyMapping Resolve<TSource, TDestination>() where TDestination : IEntityBase;
        bool ValidateMappingExistsFor<TSource, TDestination>(string fields) where TDestination : IEntityBase;
    }
}
