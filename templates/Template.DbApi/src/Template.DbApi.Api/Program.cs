using Template.DbApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Serilog;
using Microsoft.IdentityModel.Logging;
using Template.DbApi.Api.Middleware;
using Template.DbApi.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.WithSerilog(builder.Configuration, "Template.DbApi API");
builder.Services.WithPostgres(builder.Configuration);
builder.Services.WithMediatr();
builder.Services.WithAuthentication(builder.Configuration, Keys.AppKey);
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirectionExcluding("/_system");

// https://github.com/prometheus-net/prometheus-net

app.UseMetricServer();
app.UseAuthentication();
app.UseRouting();
app.UseHttpMetrics();
app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics("_system/metrics");
    endpoints.MapHealthChecks("/_system/health");
    endpoints.MapGet("/_system/ping", () => Results.Ok("pong"));
});

app.MapControllers();

Log.Logger.Information("Starting Application");

app.Run();
