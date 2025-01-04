using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Identity.Models;

namespace Template.Identity.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder) 
        {
            builder.HasData(
                new ApplicationRole()
                {
                    Id = "a4f78d09-86e3-4e96-a91b-3713e8043c7c",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    RoleDescription = "Administrative role",
                    DateCreated = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false

                },
                new ApplicationRole()
                {
                    Id = "e3f7a8c1-b55c-4e4e-8893-89e440da1bbd",
                    Name = "User",
                    NormalizedName = "USER",
                    RoleDescription = "User role",
                    DateCreated = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                }
            );
        }
    }
}
