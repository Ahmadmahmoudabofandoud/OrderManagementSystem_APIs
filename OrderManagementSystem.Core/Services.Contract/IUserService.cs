using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Services.Contract
{
    public interface IUserService
    {
        bool IsUserAdmin(ClaimsPrincipal user);
    }
}
