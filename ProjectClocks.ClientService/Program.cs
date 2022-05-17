using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpLogging; // HttpLoggingFields
using Swashbuckle.AspNetCore.SwaggerUI;
using ProjectClocks.Common.EntityModels.SqlServer;
using ProjectClocks.ClientService.Repositories;

using static System.Console;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// This service will run on port 5002
builder.WebHost.UseUrls("https://localhost:5002");

// Add services to the container.

// Add the ProjectClocks DB context
builder.Services.AddProjectClocksContext();

builder.Services
    .AddControllers()
    .AddXmlDataContractSerializerFormatters()
    .AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Client Service API", Version = "v1" });

    // using System.Reflection;
    /*
     * Ensure .csproj file includes the following <PropertyGroup> item for Swagger documentation on methods
     *  <GenerateDocumentationFile>true</GenerateDocumentationFile>
	    <NoWarn>$(NoWarn);1591</NoWarn>
     */
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Register the ClientRepository for use at runtime as a scoped dependency
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; // default is 32k
    options.ResponseBodyLogLimit = 4096; // default is 32k
});

// Allow services from a different origin to access this service (cross origin)
builder.Services.AddCors();

builder.Services.AddHealthChecks().AddDbContextCheck<ProjectClocksContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Clocks - Client Service API Version 1");
        c.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete });
    });
}

// Uses CORS to allow GET, POST, PUT, and DELETE requests from any website that has an origin of https://localhost:5001
app.UseCors(configurePolicy: options =>
{
    options.WithMethods("GET", "POST", "PUT", "DELETE");
    options.WithOrigins(
    "https://localhost:5001" // allow requests from the MVC website client only
    );
});

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

// checks the ProjectClocksContext to determine if the database is healthy
app.UseHealthChecks(path: "/healthcheck");

app.MapControllers();

app.Run();
