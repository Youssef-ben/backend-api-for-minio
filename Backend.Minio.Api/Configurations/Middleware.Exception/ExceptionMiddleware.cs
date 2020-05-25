namespace Backend.Minio.Api.Configurations.Middleware.Exception
{
    using System;
    using System.Threading.Tasks;
    using Backend.Minio.Manager.Helpers.Api.Response.Custom;
    using Facilis.Api.Helpers.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (BaseCustomError ex)
            {
                this.logger.LogError($"Something went wrong: {ex}");
                await this.HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Something went wrong: {ex}");
                await this.HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, BaseCustomError ex)
        {
            return ex.LogBadRequest(context, this.logger);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            return ex.LogInternalServerError(context, this.logger);
        }
    }
}
