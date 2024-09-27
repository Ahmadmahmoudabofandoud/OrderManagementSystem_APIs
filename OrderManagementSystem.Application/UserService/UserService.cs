using OrderManagementSystem.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.UserService
{
    public class UserService : IUserService
    {
        public bool IsUserAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }
    }
}
