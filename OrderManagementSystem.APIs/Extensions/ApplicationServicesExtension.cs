using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.ProductService;
using OrderManagementSystem.Core.Services.Contract;
using OrderManagementSystem.Core;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Core.Repositories.Contract;
using OrderManagementSystem.Application.TokenService;
using OrderManagementSystem.Application.UserService;
using OrderManagementSystem.Application.OrderService;
using OrderManagementSystem.Application.CustomerService;
using OrderManagementSystem.APIs.EmailService;
using OrderManagementSystem.Application.InvoiceService;
using OrderManagementSystem.APIs.Helpers;

namespace OrderManagementSystem.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfiles)); //allow DI for auto mapper

            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            services.AddScoped(typeof(IProductService), typeof(ProductService));

            services.AddScoped(typeof(ITokenService), typeof(TokenService));

            services.AddScoped(typeof(IUserService), typeof(UserService));

            services.AddScoped(typeof(IOrderService), typeof(OrderService));

            services.AddScoped(typeof(ICustomerService), typeof(CustomerService));

            services.AddScoped(typeof(IInvoiceService), typeof(InvoiceService));

            services.AddSingleton(typeof(IEmailService), typeof(SendEmailService));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();

                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
