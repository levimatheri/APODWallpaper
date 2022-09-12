using Newtonsoft.Json;

namespace ApodWallpaper.Core.Models;

public record ApodEntry(
    string Url,
    string Title,
    [property: JsonProperty("service_version")]
    string ServiceVersion,
    string HdUrl,
    [property: JsonProperty("media_type")]
    MediaType mediaType,
    string Explanation,
    string Date,
    [property: JsonProperty("thumbnail_url")]
    string? ThumbnailUrl,
    string? Copyright
);

public enum MediaType
{
    Image,
    Video
}
