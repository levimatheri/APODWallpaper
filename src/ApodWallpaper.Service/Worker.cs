using ApodWallpaper.Core;
using ApodWallpaper.Core.Models;
using ApodWallpaper.Core.RequestHandlers;
using MediatR;
using Newtonsoft.Json;

namespace ApodWallpaper.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMediator _mediator;

    public Worker(ILogger<Worker> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var data = await _mediator.Send(new GetApodRequest(EntryDate: new DateOnly(2022, 9, 9)), stoppingToken);
                _logger.LogInformation(JsonConvert.SerializeObject(data));
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }
}
