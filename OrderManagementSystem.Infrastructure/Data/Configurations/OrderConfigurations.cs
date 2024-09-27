using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure.Data.Configurations
{
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(order => order.Status)
                .HasConversion(
                    (OStatus) => OStatus.ToString(),
                    (OStatus) => (OrderStatus)Enum.Parse(typeof(OrderStatus), OStatus)
                );

            builder.Property(order => order.PaymentMethod)
                .HasConversion(
                    (OPaymentMethod) => OPaymentMethod.ToString(),
                    (OPaymentMethod) => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), OPaymentMethod)
                );

            builder.HasMany(order => order.OrderItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Invoice)
               .WithOne(i => i.Order)
               .HasForeignKey<Invoice>(i => i.OrderId);



        }
    }
}
