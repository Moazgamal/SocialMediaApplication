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
    internal class UserAddFriendConfigurations : IEntityTypeConfiguration<UserAddFriend>
    {
        public void Configure(EntityTypeBuilder<UserAddFriend> builder)
        {
            // Composite PK (يمنع duplicate fav)
            builder.HasKey(x => new { x.UserId, x.FriendUserId });

            // Relation: User -> FavUsers
            builder.HasOne(x => x.User)
                   .WithMany(u => u.Friends)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relation: FavUser -> FavByUsers
            builder.HasOne(x => x.FriendUser)
                   .WithMany(u => u.FriendsBy)
                   .HasForeignKey(x => x.FriendUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            //// Default FavDate
            //builder.Property(x => x.FavDate)
            //       .HasDefaultValueSql("GETDATE()");

            // منع user يفضل نفسه
            builder.HasCheckConstraint(
                "CK_UserFriendUser_NoSelfFriend",
                "[UserId] <> [FriendUserId]"
            );

            builder.ToTable("UserAddFriendUsers");
        }
    }
}
