using APODWallpaper.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APODWallpaper
{
    public partial class MainForm : Form
    {
        private APODImageProvider _provider;
        private Settings _settings;
        private Image _currentWallpaper;
        private IConfigurationRoot _configuration;

        public MainForm(APODImageProvider provider, Settings settings)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            // Register application with registry
            SetStartup(_settings.LaunchOnStartup);

            AddTrayIcons();

            // Set wallpaper every 24 hours
            var timer = new System.Timers.Timer
            {
                Interval = 1000 * 60 * 60 * 24, // 24 hours
                AutoReset = true,
                Enabled = true
            };
            timer.Elapsed += (s, e) => SetWallpaper();
            timer.Start();

            // Set wallpaper on first run
            SetWallpaper();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private string GetApiKey()
        {
            var apiKey = _configuration["APISettings:ApodApiKey"];
            return apiKey;
        }

        /// <summary>
        /// SetStartup will set the application to automatically launch on startup if launch is true,
        /// else it will prevent it from doing so.
        /// </summary>
        public void SetStartup(bool launch)
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (launch)
            {
                if (rk.GetValue("APODWallpaper") == null)
                    rk.SetValue("APODWallpaper", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "APODWallpaper.exe"));
            }
            else
            {
                if (rk.GetValue("APODWallpaper") != null)
                    rk.DeleteValue("APODWallpaper");
            }
        }

        /// <summary>
        /// SetWallpaper fetches the wallpaper from APOD and sets it
        /// </summary>
        public async void SetWallpaper()
        {
            try
            {
                var apodImage = await _provider.GetImage(GetApiKey());

                Wallpaper.Set(apodImage.Image, Wallpaper.Style.Stretched);
                _currentWallpaper = apodImage.Image;
                SetCopyrightTrayLabel(apodImage.Copyright, apodImage.Title, apodImage.Explanation);

                ShowSetWallpaperNotification();
            }
            catch (APODWallpaperException apodEx)
            {
                ShowAPODWallpaperInfoNotification(apodEx.Message);
            }
            catch (Exception ex)
            {
                ShowErrorNotification(ex.Message);
            }
        }

        public void SetCopyrightTrayLabel(string copyright, string title, string explanation)
        {
            _settings.ImageCopyright = copyright;
            _settings.ImageTitle = title;

            _copyrightLabel.Text = title;
            _copyrightLabel.Tag = $"{explanation}";
        }

        #region Tray Icons

        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayMenu;
        private ToolStripMenuItem _copyrightLabel;

        public void AddTrayIcons()
        {
            // Create a simple tray menu with only one item.
            _trayMenu = new ContextMenuStrip();

            // Copyright button
            _copyrightLabel = new ToolStripMenuItem("APOD Wallpaper");
            _copyrightLabel.Click += (s, e) =>
            {
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                var selected = (ToolStripMenuItem)s;
                // Displays the MessageBox.
                result = MessageBox.Show(selected.Tag.ToString(), selected.Text, buttons);
            };
            _trayMenu.Items.Add(_copyrightLabel);

            // Separator
            _trayMenu.Items.Add("-");

            // Force update button
            _trayMenu.Items.Add("Force Update", null, (s, e) => SetWallpaper());

            // Save image button
            var save = new ToolStripMenuItem("Save Wallpaper");
            save.Click += (s, e) =>
            {
                if (_currentWallpaper != null)
                {
                    var fileName = string.Join("_", _settings.ImageCopyright.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                    var dialog = new SaveFileDialog
                    {
                        DefaultExt = "jpg",
                        Title = "Save current wallpaper",
                        FileName = fileName,
                        Filter = "Jpeg Image|*.jpg",
                    };
                    if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != "")
                    {
                        _currentWallpaper.Save(dialog.FileName, ImageFormat.Jpeg);
                        System.Diagnostics.Process.Start(dialog.FileName);
                    }
                }
            };
            _trayMenu.Items.Add(save);

            // Launch on startup button
            var launch = new ToolStripMenuItem("Launch on Startup")
            {
                Checked = _settings.LaunchOnStartup
            };
            launch.Click += OnStartupLaunch;
            _trayMenu.Items.Add(launch);

            // Separator
            _trayMenu.Items.Add("-");

            _trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

            // Create a tray icon. Here we are setting the tray icon to be the same as the application's icon
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "APOD Wallpaper";
            _trayIcon.Icon = new Icon("Resources/apod.ico", 40, 40);
            //_trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // open tray icon on left click
            _trayIcon.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    mi.Invoke(_trayIcon, null);
                }
            };

            // Add menu to tray icon and show it.
            _trayIcon.ContextMenuStrip = _trayMenu;
            _trayIcon.Visible = true;
        }



        private void OnStartupLaunch(object sender, EventArgs e)
        {
            var launch = (ToolStripMenuItem)sender;
            launch.Checked = !launch.Checked;
            SetStartup(launch.Checked);
            _settings.LaunchOnStartup = launch.Checked;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                _trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        #endregion

        #region Notifications

        private void ShowSetWallpaperNotification()
        {
            _trayIcon.BalloonTipText = "Wallpaper has been set to APOD's image of the day!";
            _trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            _trayIcon.Visible = true;
            _trayIcon.ShowBalloonTip(5000);
        }

        private void ShowAPODWallpaperInfoNotification(string message)
        {
            _trayIcon.BalloonTipText = message;
            _trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            _trayIcon.Visible = true;
            _trayIcon.ShowBalloonTip(5000);
        }

        private void ShowErrorNotification(string message = null)
        {
            _trayIcon.BalloonTipText = message != null ? message : "Could not update wallpaper, please check your internet connection.";
            _trayIcon.BalloonTipIcon = ToolTipIcon.Error;
            _trayIcon.Visible = true;
            _trayIcon.ShowBalloonTip(5000);
        }

        #endregion

    }
}
