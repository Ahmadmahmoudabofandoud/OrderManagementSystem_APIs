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
    internal class InvoiceConfigurations : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {


            builder.HasOne(i => i.Order)
               .WithOne(o => o.Invoice)
               .HasForeignKey<Invoice>(i => i.OrderId);

            builder.Property(I => I.TotalAmount)
                .HasColumnType("decimal(12 , 2)");

            


        }
    }
}
