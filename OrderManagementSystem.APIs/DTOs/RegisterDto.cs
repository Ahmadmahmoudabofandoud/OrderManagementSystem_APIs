using OrderManagementSystem.Core.Entities.UserAggregate;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.APIs.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserRole Role { get; set; }
    }
}
