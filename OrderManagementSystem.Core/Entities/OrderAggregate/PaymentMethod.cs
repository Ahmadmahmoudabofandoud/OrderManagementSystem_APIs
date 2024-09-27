using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Entities.OrderAggregate
{
    public enum PaymentMethod
    {
        [EnumMember(Value = "CreditCard")]
        CreditCard,
        [EnumMember(Value = "PayPal")]
        PayPal,

    }
}
