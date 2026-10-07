using System.Text.Json;
using Microsoft.EntityFrameworkCore;

using Wharf.Api.Data;
using Wharf.Api.Endpoints;
using Wharf.Api.K8s;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
            .WithMethods("GET", "POST", "PATCH", "DELETE", "OPTIONS")
            .WithHeaders("Content-Type");
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddScoped<IClusterAccess, ClusterAccess>();

builder.Services.AddDbContext<WharfDbContext>(options => {
        options.UseNpgsql(builder.Configuration.GetConnectionString("WharfDatabase"));
});

var app = builder.Build();

app.MapEnvironmentEndpoints();
app.MapClusterEndpoints();

app.UseCors("VueDev");
app.Run();

public partial class Program { }
