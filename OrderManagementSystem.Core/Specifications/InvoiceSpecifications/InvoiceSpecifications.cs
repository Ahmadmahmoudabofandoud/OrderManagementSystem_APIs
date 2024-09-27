using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Specifications.InvoiceSpecifications
{
    public class InvoiceSpecifications : BaseSpecifications<Invoice>
    {
        public InvoiceSpecifications()
            :base() 
        {
            Includes.Add(I => I.Order);
        }

        public InvoiceSpecifications(int invoiceId)
            : base(I => I.InvoiceId == invoiceId)
        {
            Includes.Add(I => I.Order);
        }
    }
}
