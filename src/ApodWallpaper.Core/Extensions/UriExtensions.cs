using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApodWallpaper.Core.Extensions;
public static class UriExtensions
{
    public static void AddParameters(this UriBuilder uriBuilder, IReadOnlyDictionary<string, string> parameters)
    {
        uriBuilder.Query = string.Join("&", parameters.Select(item => $"{item.Key}={item.Value}"));
    }
}
