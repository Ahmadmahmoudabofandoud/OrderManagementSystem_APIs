using OrderManagementSystem.Core.Entities.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Services.Contract
{
    public interface ITokenService
    {
        string CreateToken(User user);

    }
}
