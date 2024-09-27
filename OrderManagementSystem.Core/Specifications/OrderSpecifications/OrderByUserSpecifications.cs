using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Specifications.OrderSpecifications
{
    public class OrderByUserSpecifications : BaseSpecifications<Order>
    {
        public OrderByUserSpecifications(int userId)
            :base()
        {
            Includes.Add(O => O.Invoice);
            Includes.Add(O => O.OrderItems);
        }
    }
}
