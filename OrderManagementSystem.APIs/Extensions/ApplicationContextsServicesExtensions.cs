using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.APIs.Extensions
{
    public static class ApplicationContextsServicesExtensions
    {
        public static IServiceCollection AddContextsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderManagementDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

           

            return services;
        }
    }
}
