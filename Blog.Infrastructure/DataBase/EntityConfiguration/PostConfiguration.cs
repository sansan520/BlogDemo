using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.DataBase.EntityConfiguration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Post> builder)
        {
            builder.Property(x => x.Author).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.Body).HasMaxLength(2000);
        }
    }
}
