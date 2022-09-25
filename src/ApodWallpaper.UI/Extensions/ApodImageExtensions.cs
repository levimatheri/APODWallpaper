using ApodWallpaper.Core.Models.NasaApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ApodWallpaper.UI.Extensions;
public static class ApodImageExtensions
{
    public static async Task<Image> DownloadImage(this ApodEntry apodEntry, HttpClient httpClient)
    {
        //using (var client = new HttpClient())
        //{
            
        //}
        using var stream = await httpClient.GetStreamAsync(apodEntry.HdUrl);
        return Image.FromStream(stream);
    }
}
