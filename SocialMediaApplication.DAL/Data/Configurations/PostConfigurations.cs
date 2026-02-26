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
    internal class PostConfigurations : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(post => post.Id);

            builder.Property(post => post.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(post => post.postText)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(post => post.postImageName)
                   .IsRequired(false);

            builder.Property(post => post.creatingUserId)
                   .IsRequired();

            builder.Property(post => post.DateOfCreation)
                   .HasDefaultValueSql("GETDATE()");

            // creator relation (one user creates many c1)
            builder.HasOne(post => post.creatingUser)
                   .WithMany(u => u.createdPosts)
                   .HasForeignKey(post => post.creatingUserId)
                   .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}
