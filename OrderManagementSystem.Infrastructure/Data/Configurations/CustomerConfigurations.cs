using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure.Data.Configurations
{
    internal class CustomerConfigurations : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(C => C.CustomerName).IsRequired().HasMaxLength(250);
            builder.Property(C => C.Email).IsRequired();
            builder.HasMany(customer => customer.Orders)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
