using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.HelperServices
{
    public class MappedProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否相反（soucrce,destination 排序是否相反）
        /// </summary>
        public bool Revert { get; set; }
    }
}
