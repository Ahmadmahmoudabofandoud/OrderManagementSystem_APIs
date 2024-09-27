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
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(P => P.ProductName).IsRequired().HasMaxLength(64);
            builder.Property(P => P.Price)
                .HasColumnType("decimal(12 , 2)");

            builder.HasMany(P => P.OrderItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}
