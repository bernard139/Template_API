using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Identity.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder) 
        {
            builder.HasData(
                new IdentityRole()
                {
                    Id = "a4f78d09-86e3-4e96-a91b-3713e8043c7c",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"

                },
                new IdentityRole()
                {
                    Id = "e3f7a8c1-b55c-4e4e-8893-89e440da1bbd",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
        }
    }
}
