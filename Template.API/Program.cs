using Template.Application;
using Template.Persistence;
using Template.Infrastructure;
using Template.Identity;
using Microsoft.OpenApi.Models;
using EllipticCurve;
using Microsoft.AspNetCore.DataProtection;
using Template.API.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region Add services to the container.
AddSwaggerDoc(builder.Services);

builder.Services.ConfigureApplicationServices();
builder.Services.ConfigureInfrastructureServices(builder.Configuration);
builder.Services.ConfigurePersistenceServices(builder.Configuration);
builder.Services.ConfigureIdentityServices(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod() 
        .AllowAnyHeader());
});

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("DPAPI-key"));

WebApplication app = builder.Build();

#endregion

#region Configure the HTTp request pipeline.

if (app.Environment.IsDevelopment())
{
    
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();
app.Run();

#endregion

void AddSwaggerDoc(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });

        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Template API"
        });
    });
}