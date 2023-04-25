using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using static Dapper.SqlMapper;
using Microsoft.Extensions.Options;
using Template.DbApi.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Template.DbApi;
public static class ConfigurationExtensions
{
    public static IApplicationBuilder UseHttpsRedirectionExcluding(this IApplicationBuilder builder, string excluding)
    {
        builder.UseWhen(
            context => !context.Request.Path.StartsWithSegments(excluding),
            builder => builder.UseHttpsRedirection());

        return builder;
    }

    public static IServiceCollection WithMediatr(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ConfigurationExtensions)));
        return services;
    }

    public static IServiceCollection WithAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-7.0
        // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn?view=aspnetcore-7.0&tabs=windows
        // https://jasonwatmore.com/post/2021/12/14/net-6-jwt-authentication-tutorial-with-example-api

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options => configuration.Bind("JwtSettings", options))
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options => configuration.Bind("CookieSettings", options));

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAppUserRole",
                 policy => policy.RequireRole("AppUser"));
        });

        return services;
    }

    public static IServiceCollection WithPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var writeConn = configuration.GetSection("Postgres:WriteConnection").Value;
        var readConn = configuration.GetSection("Postgres:ReadConnection").Value;
        services.AddSingleton<IConnectionFactory>(new PostgresConnectionFactory(writeConn, readConn));
        
        return services;
    }

    public static IServiceCollection WithSerilog(this IServiceCollection services, IConfiguration configuration, string appName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .Enrich.WithProperty("App", appName)
            .CreateLogger();
        services.AddSingleton(Log.Logger);
        services.AddLogging(lb => {
            lb.ClearProviders();
            lb.AddSerilog(Log.Logger);
        });

        return services;
    }
}
