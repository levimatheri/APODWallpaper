using ApodWallpaper.Core;
using ApodWallpaper.Core.Models;
using ApodWallpaper.Core.RequestHandlers;
using ApodWallpaper.Service;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    //.ConfigureAppConfiguration((hostingContext, configuration) =>
    //{
    //    configuration.Sources.Clear();

    //    IHostEnvironment env = hostingContext.HostingEnvironment;

    //    configuration
    //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
    //})
    .ConfigureServices((hostingContext, services) =>
    {
        //services.AddOptions();

        services.Configure<ApodApiConfig>(hostingContext.Configuration.GetSection(ApodApiConfig.ConfigName));
        services.AddHostedService<Worker>();
        services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(GetApodRequestHandler).Assembly);
        services.AddHttpClient();
    })
    .Build();

host.Run();
