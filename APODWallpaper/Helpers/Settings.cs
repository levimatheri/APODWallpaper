﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace APODWallpaper.Helpers
{
    public class Settings
    {
        private Options _options;
        private string _settingsPath;

        public Settings()
        {
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.txt");

            try
            {
                using var stream = new FileStream(_settingsPath, FileMode.Open);
                var ser = new DataContractJsonSerializer(typeof(Options));
                _options = (Options)ser.ReadObject(stream);
            }
            catch (FileNotFoundException)
            {
                _options = new Options();
            }
            catch (SerializationException)
            {
                _options = new Options();
            }
        }

        #region User settings

        public bool LaunchOnStartup
        {
            get { return _options.LaunchOnStartup; }
            set
            {
                _options.LaunchOnStartup = value;
                Save();
            }
        }

        public string ImageCopyright
        {
            get { return _options.ImageCopyright; }
            set
            {
                _options.ImageCopyright = value;
                Save();
            }
        }

        public string ImageTitle
        {
            get { return _options.ImageTitle; }
            set
            {
                _options.ImageTitle = value;
                Save();
            }
        }

        #endregion

        private void Save()
        {
            using (var stream = new FileStream(_settingsPath, FileMode.Create))
            {
                var ser = new DataContractJsonSerializer(typeof(Options));
                ser.WriteObject(stream, _options);
            }
        }

        [DataContract]
        private class Options
        {
            [DataMember]
            public bool LaunchOnStartup = true;
            [DataMember]
            public string ImageCopyright = "APOD Wallpaper";
            [DataMember]
            public string ImageTitle = "https://apod.nasa.gov";
        }
    }
}
