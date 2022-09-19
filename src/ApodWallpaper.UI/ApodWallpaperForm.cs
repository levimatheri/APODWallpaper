using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ApodWallpaper.UI;

public partial class ApodWallpaperForm : Form
{
    private readonly ILogger<ApodWallpaperForm> _logger;
    private readonly IMediator _mediator;

    private readonly ContextMenuStrip _trayMenu;
    public ApodWallpaperForm(ILogger<ApodWallpaperForm> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
        InitializeComponent();

        _trayMenu = new();
        SetupTrayIcon();
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

        trayIcon.MouseUp += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                _trayMenu.Show(Control.MousePosition);
            }
        };
        
    }
}
