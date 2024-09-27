
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderManagementSystem.APIs.Extensions;
using OrderManagementSystem.APIs.Middlewares;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Application Services
            // Add services to the container.

            builder.Services.AddContextsServices(builder.Configuration);

            builder.Services.AddAuthServices(builder.Configuration);

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            builder.Services.AddSwaggerServices();

            builder.Services.AddApplicationServices();

            #endregion

            var app = builder.Build();

            #region ASK CLR to create Object from DbContext Explicitly

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<OrderManagementDbContext>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();


            try
            {
                await _dbContext.Database.MigrateAsync();
                await OrderManagementContextSeed.SeedAsync(_dbContext);

                
            }
            catch (Exception ex)
            {

                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error has been occured during apply the Migration");
            }

            #endregion

            #region Application Middlewares

            app.UseMiddleware<ExceptionMiddleware>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}"); //Not Found Endpoints


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run(); 
            #endregion
        }
    }
}
