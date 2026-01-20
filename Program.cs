using Configuration.RegistryEnvironmentVariables;
using Scalar.AspNetCore;
using System.Diagnostics;

namespace APITestLoadingRegEnvVar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add/change the value of the environemnt variable: NestedTest__NestedProperty2
            // This wouldn't usually be readable by the Configuration.AddEnvironmentVariables as
            // it didn't exist prior to the process loading.
            // AddRegistryEnvironmentVariables will be able to read it.
            Environment.SetEnvironmentVariable("NestedTest__NestedProperty2", DateTime.Now.ToString("o"), EnvironmentVariableTarget.User);
            
            var builder = WebApplication.CreateBuilder(args);

            // Load environment variables in the registry to pull in any secrets or keys needed when running on a webserver
            builder.Configuration.AddRegistryEnvironmentVariables(EnvironmentVariableTarget.User);

            // Application Configuration Settings Sources
            var appSettings = builder.Configuration.Get<AppSettings>();

            Console.WriteLine(appSettings.Dump());
            Debug.WriteLine(appSettings.Dump());

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(); // Default: /scalar/v1
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/get-appsettings", (HttpContext httpContext) =>
            {
                return appSettings;
            })
            .WithName("GetAppSettings");

            app.Run();
        }
    }
}

