using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.Entities
{
    /// <summary>
    /// 博客文章类
    /// </summary>
    public class Post: EntityBase
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }

        public DateTime LastModified { get; set; }
    }
}
