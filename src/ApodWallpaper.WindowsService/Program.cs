using ApodWallpaper.Core;
using ApodWallpaper.Core.Models.NasaApi;
using ApodWallpaper.Core.RequestHandlers;
using ApodWallpaper.Service;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Apod Wallpaper Service";
    })
    .ConfigureServices((hostingContext, services) =>
    {
        services.Configure<ApodApiConfig>(hostingContext.Configuration.GetSection(ApodApiConfig.ConfigName));
        services.AddHostedService<Worker>();
        services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(GetApodRequestHandler).Assembly);
        services.AddHttpClient();
    })
    .ConfigureLogging((context, logging) =>
    {
        // See: https://github.com/dotnet/runtime/issues/47303
        //logging.AddConfiguration(
        //    context.Configuration.GetSection("Logging"));
        logging.AddEventLog();
    })
    .Build();

await host.RunAsync();
