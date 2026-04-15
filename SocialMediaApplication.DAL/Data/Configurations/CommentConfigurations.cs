using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMediaApplication.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.DAL.Data.Configurations
{
    public class CommentConfigurations : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId).IsRequired();

            builder.Property(c => c.PostId).IsRequired();

            builder.Property(c => c.CommentText)
               .IsRequired()
               .HasMaxLength(1000);

            builder.HasOne(c => c.User)
               .WithMany(u => u.Comments)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Post)
               .WithMany(p => p.Comments)
               .HasForeignKey(c => c.PostId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.DateOfCreation)
               .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.LikesCount)
                .HasDefaultValue(0);

            builder.HasIndex(c => c.PostId);
            builder.HasIndex(c => c.UserId); // will not be useful until now.
            
            builder.HasIndex(c => new { c.PostId, c.DateOfCreation });
        }
    }
}
