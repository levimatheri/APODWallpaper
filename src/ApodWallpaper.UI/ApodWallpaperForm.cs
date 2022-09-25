using ApodWallpaper.Core.RequestHandlers;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using ApodWallpaper.UI.WindowsApi;
using ApodWallpaper.UI.Extensions;

namespace ApodWallpaper.UI;

public partial class ApodWallpaperForm : Form
{
    private readonly ILogger<ApodWallpaperForm> _logger;
    private readonly IMediator _mediator;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ContextMenuStrip _trayMenu;
    public ApodWallpaperForm(ILogger<ApodWallpaperForm> logger, IMediator mediator, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _mediator = mediator;
        _httpClientFactory = httpClientFactory;
        InitializeComponent();

        _trayMenu = new();
        SetupTrayIcon();
        
        SetWallpaper();
    }

    protected override void OnLoad(EventArgs e)
    {
        Visible = false; // Hide form window.
        ShowInTaskbar = false; // Remove from taskbar.

        base.OnLoad(e);
    }

    //protected override void Dispose(bool isDisposing)
    //{
    //    if (isDisposing)
    //    {
    //        _trayMenu.Dispose();
    //    }

    //    base.Dispose(isDisposing);
    //}

    private async void SetWallpaper()
    {
        try
        {
            var apodResponse = await _mediator
                .Send(new GetApodRequest(EntryDate: new DateOnly(2022, 9, 24)));
            var apodImage = await apodResponse
                .DownloadImage(_httpClientFactory.CreateClient());

            Wallpaper.Set(apodImage, Wallpaper.Style.Stretched);
        }
        catch
        {
            //ShowErrorNotification();
            _logger.LogError("Error occurred while setting wallpaper");
        }
    }

    private void SetupTrayIcon()
    {
        var copyrightLabel = new ToolStripMenuItem("NASA Astronomy Picture of the Day");
        copyrightLabel.Click += (s, e) =>
        {
            var url = "https://apod.nasa.gov/apod/lib/about_apod.html#srapply";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not launch copyright URL: {Message}", ex.Message);
            }
        };
        _trayMenu.Items.Add(copyrightLabel);

        // separator
        _trayMenu.Items.Add("-");

        var exitLabel = new ToolStripMenuItem("Quit Apod");
        exitLabel.Click += (s, e) =>
        {
            Application.Exit();
        };
        _trayMenu.Items.Add(exitLabel);

        var trayIcon = new NotifyIcon
        {
            Text = "Apod",
            Icon = new Icon("img/apod.ico"),
            ContextMenuStrip = _trayMenu,
            Visible = true
        };

        //trayIcon.MouseUp += (s, e) =>
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        _trayMenu.Show(Control.MousePosition);
        //    }
        //};

        // open tray icon on left click
        trayIcon.MouseUp += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo? mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (mi == null)
                {
                    _logger.LogError("Could not find method ShowContextMenu");
                    return;
                }
                mi?.Invoke(trayIcon, null);
            }
        };
    }
}
