using ApodWallpaper.Core.Models.NasaApi;
using ApodWallpaper.Core.RequestHandlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ApodWallpaper.UI;

static class Program
{
    static IServiceProvider ServiceProvider { get; set; }
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var host = CreateHostBuilder().Build();
        ServiceProvider = host.Services;
        Application.Run(ServiceProvider.GetRequiredService<ApodWallpaperForm>());
    }    

    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((hostingContext, services) =>
            {
                services.Configure<ApodApiConfig>(hostingContext.Configuration.GetSection(ApodApiConfig.ConfigName));
                services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(GetApodRequestHandler).Assembly);
                services.AddHttpClient();
                services.AddTransient<ApodWallpaperForm>();
            })
            .ConfigureLogging((context, logging) =>
            {
                // See: https://github.com/dotnet/runtime/issues/47303
                //logging.AddConfiguration(
                //    context.Configuration.GetSection("Logging"));
                logging.AddEventLog();
            });
    }
}