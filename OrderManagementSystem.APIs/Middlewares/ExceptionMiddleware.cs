using Microsoft.Extensions.Logging;
using OrderManagementSystem.APIs.Errors;
using System.Net;
using System.Text.Json;

namespace OrderManagementSystem.APIs.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment env
            )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext); //Go to the next Middleware 

                //Take an action with the response
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message); //Development Environment 
                //Log Exception in Database | Files //Production Environment

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //500 
                httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment() ?
                    new ApiExceptionResponse(
                        (int)HttpStatusCode.InternalServerError,
                        ex.Message,
                        ex.StackTrace.ToString()
                    )
                    :
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString());

                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(json);

            }

        }
    }
}
