using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.HelperServices
{
    public interface ITypeService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
