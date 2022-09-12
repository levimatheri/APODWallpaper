using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApodWallpaper.Core.Models;
public class ApodApiConfig
{
    public const string ConfigName = nameof(ApodApiConfig);

    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
}
