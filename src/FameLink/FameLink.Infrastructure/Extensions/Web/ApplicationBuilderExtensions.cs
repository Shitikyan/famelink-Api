using FameLink.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace FameLink.Infrastructure.Extensions.Services
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }

        public static IApplicationBuilder UseSwashbuckleSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            return app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "FameLink API V1");
            });
        }
    }
}
