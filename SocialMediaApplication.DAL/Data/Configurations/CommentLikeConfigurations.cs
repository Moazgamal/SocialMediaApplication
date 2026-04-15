using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMediaApplication.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.DAL.Data.Configurations
{
    public class CommentLikeConfigurations : IEntityTypeConfiguration<CommentLike>
    {
        public void Configure(EntityTypeBuilder<CommentLike> builder)
        {
            builder.HasKey(cl => cl.Id);

            builder.HasOne(cl => cl.User)
                   .WithMany()
                   .HasForeignKey(cl => cl.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cl => cl.Comment)
                   .WithMany(c => c.Likes)
                   .HasForeignKey(cl => cl.CommentId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(cl => new { cl.UserId, cl.CommentId })
                .IsUnique();
        }
    }
}
