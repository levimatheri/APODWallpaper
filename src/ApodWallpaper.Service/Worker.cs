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
        while (!stoppingToken.IsCancellationRequested)
        {
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var data = await _mediator.Send(new GetApodRequest(EntryDate: new DateOnly(2022, 9, 9)), stoppingToken);
            _logger.LogInformation(JsonConvert.SerializeObject(data));
            await Task.Delay(5000, stoppingToken);
        }
    }
}
