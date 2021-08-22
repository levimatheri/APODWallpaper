using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APODWallpaper.Helpers
{
    public class APODWallpaperException : Exception
    {
        public APODWallpaperException(string message) : base(message)
        {
        }

        public APODWallpaperException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
