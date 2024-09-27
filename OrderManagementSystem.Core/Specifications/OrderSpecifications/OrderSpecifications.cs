using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Specifications.OrderSpecifications
{
    public class OrderSpecifications : BaseSpecifications<Order>
    {
        public OrderSpecifications()
            :base() 
        {
            Includes.Add(O => O.Invoice);
            Includes.Add(O => O.OrderItems);
        }
        public OrderSpecifications( int orderId)
            : base(O => O.OrderId == orderId)
        {
            Includes.Add(O => O.Invoice);
            Includes.Add(O => O.OrderItems);

        }


    }
}
