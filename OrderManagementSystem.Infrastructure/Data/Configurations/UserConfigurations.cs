using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementSystem.Core.Entities.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure.Data.Configurations
{
    internal class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(U => U.UserName).IsRequired().HasMaxLength(64);
            builder.Property(U => U.PasswordHash).IsRequired().HasMaxLength(64);
            builder.Property(user => user.Role)
                .HasConversion(
                    (URole) => URole.ToString(),
                    (URole) => (UserRole)Enum.Parse(typeof(UserRole), URole)
                );
        }
    }
}
