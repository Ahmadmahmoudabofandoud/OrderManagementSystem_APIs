using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } //Navigational Property [ONE]
        public DateTimeOffset OrderDate { get; set; } 
        public decimal TotalAmount { get; set; }


    }
}
