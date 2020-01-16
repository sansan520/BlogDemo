using Blog.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.HelperServices
{
    public abstract class PropertyMapping<TSource, TDestination> : IPropertyMapping where TDestination : IEntityBase
    {
        public Dictionary<string, List<MappedProperty>> MappingDictionary { get; }
        public PropertyMapping(Dictionary<string, List<MappedProperty>> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
            MappingDictionary[nameof(IEntityBase.Id)] = new List<MappedProperty>
            {
                new MappedProperty { Name = nameof(IEntityBase.Id), Revert = false}
            };
        }
    }
}
