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
    internal class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
               .IsRequired()
               .HasMaxLength(10);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(u => u.profilePictureName)
                   .IsRequired();

            builder.Property(u => u.IsAgree)
                   .IsRequired();

           
        }
    }
}
