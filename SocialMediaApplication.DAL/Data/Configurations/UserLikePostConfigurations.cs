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
    internal class UserLikePostConfigurations : IEntityTypeConfiguration<UserLikePost>
    {
        public void Configure(EntityTypeBuilder<UserLikePost> builder)
        {
            // Composite PK => prevent duplicate favs
            builder.HasKey(x => new { x.userId, x.postId });

            // User relation
            builder.HasOne(x => x.user)
                   .WithMany(u => u.likedPosts)
                   .HasForeignKey(x => x.userId)
                   .OnDelete(DeleteBehavior.Restrict);

            // C1 relation
            builder.HasOne(x => x.post)
                   .WithMany(p => p.Likes)
                   .HasForeignKey(x => x.postId)
                   .OnDelete(DeleteBehavior.Restrict);

            //// Default FavDate
            //builder.Property(x => x.FavDate)
            //       .HasDefaultValueSql("GETDATE()");

            builder.ToTable("UserLikePost");
        }
    }
}
