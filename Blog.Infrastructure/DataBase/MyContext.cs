using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.DataBase
{
    public class MyContext:DbContext
    {
        public MyContext(DbContextOptions<MyContext> options):base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }
    }
}
