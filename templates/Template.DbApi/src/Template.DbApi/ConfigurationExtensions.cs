using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using static Dapper.SqlMapper;
using Microsoft.Extensions.Options;
using Template.DbApi.Model;
using Microsoft.AspNetCore.Builder;

namespace Template.DbApi;
public static class ConfigurationExtensions
{
    public static IApplicationBuilder UseHttpsRedirectionExcluding(this IApplicationBuilder builder, string excluding)
    {
        builder.UseWhen(
            context => !context.Request.Path.StartsWithSegments("/_system/metrics"),
            builder => builder.UseHttpsRedirection());

        return builder;
    }

    public static IServiceCollection WithPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Postgres:WriteConnection").Value;
        services.AddSingleton<IConnectionFactory>(new PostgresConnectionFactory(section));
        
        // Add Storage Services
        services.AddSingleton<IItemStore, ItemStore>();

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
