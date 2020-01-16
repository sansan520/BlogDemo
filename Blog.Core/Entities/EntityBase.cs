using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.Entities
{
    public abstract class EntityBase : Interface.IEntityBase
    {
        public int Id { get; set; }
    }
}
