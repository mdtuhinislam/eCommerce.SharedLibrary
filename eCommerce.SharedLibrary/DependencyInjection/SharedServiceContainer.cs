using eCommerce.SharedLibrary.MiddleWare;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedService<TContext>(this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            //Add generic database

            services.AddDbContext<TContext>(option=> 
            option.UseSqlServer(config.GetConnectionString("eCommerceConnectionString"), sqlServerOptions =>
            sqlServerOptions.EnableRetryOnFailure()));

            //Serilog config
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [Level:u3] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Add JWT Authentication
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);

            return services;
        }
        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //use global exception middleware

            app.UseMiddleware<GlobalException>();

            //Register middleware to block all calls coming from out side of the API gateway 
            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}
