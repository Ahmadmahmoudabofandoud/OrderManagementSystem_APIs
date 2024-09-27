using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Entities.UserAggregate
{
    public enum UserRole
    {
        [EnumMember(Value = "Customer")]
        Customer,
        [EnumMember(Value = "Admin")]
        Admin,
    }
}
