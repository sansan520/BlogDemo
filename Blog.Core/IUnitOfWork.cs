﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core
{
    public interface IUnitOfWork
    {
         Task<bool> SaveAsync();
    }
}
