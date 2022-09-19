using ApodWallpaper.Core.Extensions;
using ApodWallpaper.Core.Models.NasaApi;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApodWallpaper.Core.RequestHandlers;
public class GetApodRequestHandler : IRequestHandler<GetApodRequest, ApodEntry>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApodApiConfig _apodApiConfig;

    public GetApodRequestHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<ApodApiConfig> apodApiConfig)
    {
        _httpClientFactory = httpClientFactory;
        _apodApiConfig = apodApiConfig.Value;
    }

    public async Task<ApodEntry> Handle(GetApodRequest request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();

        var queryParams = new Dictionary<string, string>
        {
            { "api_key", _apodApiConfig.ApiKey }
        };

        if (request.IncludeThumbnailLink is true)
        {
            queryParams.Add("thumbs", "true");
        }

        if (request.EntryDate.HasValue)
        {
            queryParams.Add("date", request.EntryDate.Value.ToString("yyyy-MM-dd"));
        }

        var uriBuilder = new UriBuilder($"{_apodApiConfig.BaseUrl}");
        uriBuilder.AddParameters(queryParams);

        var resp = await client.GetAsync(uriBuilder.Uri, cancellationToken);

        resp.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<ApodEntry>(await resp.Content.ReadAsStringAsync(cancellationToken))!;
    }
}

public record GetApodRequest(bool? IncludeThumbnailLink = null, DateOnly? EntryDate = null) : IRequest<ApodEntry>;
