using Blog.Core.Entities;
using Blog.Infrastructure.DataBase.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.DataBase
{
    public class MyContext:DbContext
    {
        public MyContext(DbContextOptions<MyContext> options):base(options)
        {}

        /// <summary>
        /// flut api  约束 sql 字段
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PostConfiguration());
        }
        public DbSet<Post> Posts { get; set; }
    }
}
