using Apod;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APODWallpaper.Helpers
{
    public class APODImageProvider
    {
        public async Task<APODImage> GetImage(string apiKey)
        {
            using var client = new ApodClient($"{apiKey}");
            var response = await client.FetchApodAsync(DateTime.Now);

            if (response.StatusCode != ApodStatusCode.OK)
            {
                throw new Exception(response.Error.ErrorMessage);
            }

            if (response.Content.MediaType == MediaType.Video)
            {
                throw new APODWallpaperException("Today's astronomy picture of the day is a video. Cannot set wallpaper");
            }

            using var httpClient = new HttpClient();
            using var imgStream = await httpClient.GetStreamAsync(new Uri(response.Content.ContentUrlHD));
            return new APODImage { Image = Image.FromStream(imgStream), 
                Copyright = response.Content.Copyright, Title = response.Content.Title, Explanation = response.Content.Explanation };
        }
    }

    public class APODImage
    {
        public Image Image { get; set; }
        public string Copyright { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
    }
}
