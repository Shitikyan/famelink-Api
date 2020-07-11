using FameLink.Infrastructure.Extensions.Services;
using FameLink.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FameLink.Domain.Helpers;
using System.Reflection;

namespace FameLink.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddIdentity<FameLinkContext>(Configuration);

            services.AddEFCore<FameLinkContext>(Configuration.GetConnectionString("Default"));

            services.AddOptions(Configuration);

            services.AddServices<FameLinkContext>();

            services.AddJwtAuthentication();

            services.AddSwagger();

            services.AddMediatR(DomainAssemblyIndicator.GetAssembly());

            services.AddFluentValidations(Assembly.GetExecutingAssembly());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwashbuckleSwagger();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthorization();

            app.UseExceptionHandlerMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
