using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.PropertyMappingServices
{
    public interface IPropertyMapping
    {
        Dictionary<string, List<MappedProperty>> MappingDictionary { get; }
    }
}
