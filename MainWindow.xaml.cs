
using System.Net;
using System.IO;
using Microsoft.Web.WebView2.Core;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NowPlayingUniversal.Models;
using NowPlayingUniversal.Services;

namespace NowPlayingUniversal
{
    public partial class MainWindow : Window
    {
        private string lastTitle = "";
        private string lastArtist = "";
        private OverlaySettings settings;

        // ================= WIN32 DRAG =================
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        // =============================================

        public MainWindow() {
            InitializeComponent();

            settings = SettingsService.Load();

            this.Width = settings.Width;
            this.Height = settings.Height;

            StartLocalListener();

            this.KeyDown += Window_KeyDown;

            this.Loaded += (s, e) =>
            {
                WebView.MouseLeftButtonDown += (sender, args) =>
                {
                    ReleaseCapture();
                    SendMessage(
                        new System.Windows.Interop.WindowInteropHelper(this).Handle,
                        WM_NCLBUTTONDOWN,
                        HTCAPTION,
                        0);
                };

                ApplyPosition();
            };

            Init();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (e.Key == Key.F2)
            {
                var config = new ConfigWindow();
                config.Owner = this;
                config.ShowDialog();

                settings = SettingsService.Load();
                ApplyPosition();
                WebView.CoreWebView2.NavigateToString(GetHtml());
            }
        }

        async void Init()
        {
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.NavigateToString(GetHtml());
            _ = MonitorBrowser();
        }

        // ================= MONITOR + POSITION =================
        private void ApplyPosition()
        {
            var screens = System.Windows.Forms.Screen.AllScreens;

            if (settings.ScreenIndex >= screens.Length)
                settings.ScreenIndex = 0;

            var screen = screens[settings.ScreenIndex];
            var area = screen.WorkingArea;

            double left = 0;
            double top = 0;

            switch (settings.Position)
            {
                case "TopLeft":
                    left = area.Left;
                    top = area.Top;
                    break;

                case "TopRight":
                    left = area.Right - this.Width;
                    top = area.Top;
                    break;

                case "BottomRight":
                    left = area.Right - this.Width;
                    top = area.Bottom - this.Height;
                    break;

                default:
                    left = area.Left;
                    top = area.Bottom - this.Height;
                    break;
            }

            this.Left = left;
            this.Top = top;
        }
        // ======================================================

        async Task MonitorBrowser()
        {
            while (true)
            {
                try
                {
                    var client = new HttpClient();
                    var json = await client.GetStringAsync("http://localhost:9222/json");

                    var pages = JsonDocument.Parse(json).RootElement;

                    foreach (var page in pages.EnumerateArray())
                    {
                        if (!page.TryGetProperty("webSocketDebuggerUrl", out var wsProp))
                            continue;

                        var wsUrl = wsProp.GetString();
                        if (string.IsNullOrEmpty(wsUrl))
                            continue;

                        var metadata = await GetMediaMetadata(wsUrl);

                        if (metadata != null)
                        {
                            var title = metadata.Value.title;
                            var artist = metadata.Value.artist;
                            var artwork = metadata.Value.artwork;

                            if (!string.IsNullOrEmpty(title) &&
                                (title != lastTitle || artist != lastArtist))
                            {
                                lastTitle = title;
                                lastArtist = artist;

                                await ShowOverlay(title, artist, artwork);
                                break;
                            }
                        }
                    }
                }
                catch { }

                await Task.Delay(2000);
            }
        }

        async Task<(string title, string artist, string artwork)?> GetMediaMetadata(string wsUrl)
        {
            try
            {
                using var ws = new ClientWebSocket();
                await ws.ConnectAsync(new Uri(wsUrl), CancellationToken.None);

                var command = @"{
                    ""id"":1,
                    ""method"":""Runtime.evaluate"",
                    ""params"":{
                        ""expression"":""(function(){
                            if(navigator.mediaSession && navigator.mediaSession.metadata){
                                let m = navigator.mediaSession.metadata;
                                return JSON.stringify({
                                    title: m.title || '',
                                    artist: m.artist || '',
                                    artwork: m.artwork && m.artwork.length > 0 ? m.artwork[0].src : ''
                                });
                            }
                            return null;
                        })()"",
                        ""returnByValue"":true
                    }
                }";

                var buffer = Encoding.UTF8.GetBytes(command);
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                var recv = new byte[8192];
                var result = await ws.ReceiveAsync(recv, CancellationToken.None);

                var response = Encoding.UTF8.GetString(recv, 0, result.Count);

                if (!response.Contains("value"))
                    return null;

                using var doc = JsonDocument.Parse(response);

                var value = doc.RootElement
                    .GetProperty("result")
                    .GetProperty("result")
                    .GetProperty("value")
                    .GetString();

                if (string.IsNullOrEmpty(value))
                    return null;

                var metadata = JsonDocument.Parse(value).RootElement;

                return (
                    metadata.GetProperty("title").GetString() ?? "",
                    metadata.GetProperty("artist").GetString() ?? "",
                    metadata.GetProperty("artwork").GetString() ?? ""
                );
            }
            catch
            {
                return null;
            }
        }

        async Task ShowOverlay(string title, string artist, string artwork)
        {
            await WebView.ExecuteScriptAsync(
                $"showNowPlaying('{title.Replace("'", "")}','{artist.Replace("'", "")}','{artwork.Replace("'", "")}')"
            );
        }

        private string ConvertHexToRgba(string hex, double opacity)
        {
            hex = hex.Replace("#", "");
            var r = Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = Convert.ToInt32(hex.Substring(4, 2), 16);

            return $"rgba({r},{g},{b},{opacity.ToString(CultureInfo.InvariantCulture)})";
        }


        private async void StartLocalListener()
        {
            try
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://127.0.0.1:32145/");
                listener.Start();

                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        var context = await listener.GetContextAsync();

                        //  CORS headers
                        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                        context.Response.AddHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
                        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                        //  Handle preflight
                        if (context.Request.HttpMethod == "OPTIONS")
                        {
                            context.Response.StatusCode = 200;
                            context.Response.Close();
                            continue;
                        }

                        if (context.Request.Url.AbsolutePath == "/nowplaying")
                        {
                            using var reader = new StreamReader(context.Request.InputStream);
                            var body = await reader.ReadToEndAsync();

                            try
                            {
                                var json = JsonDocument.Parse(body).RootElement;

                                string title = json.GetProperty("title").GetString() ?? "";
                                string artist = json.GetProperty("artist").GetString() ?? "";
                                string artwork = json.GetProperty("artwork").GetString() ?? "";

                                await Dispatcher.InvokeAsync(() =>
                                {
                                    _ = ShowOverlay(title, artist, artwork);
                                });
                            }
                            catch { }

                            context.Response.StatusCode = 200;
                            context.Response.ContentType = "text/plain";

                            var buffer = Encoding.UTF8.GetBytes("OK");
                            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                            context.Response.Close();
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            context.Response.Close();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        string GetHtml()
        {
            var bg = ConvertHexToRgba(settings.BackgroundColor, settings.BackgroundOpacity);

            var bodyBg = settings.BackgroundMode == "Alpha"
                ? "transparent"
                : "#00FF00";

            string hiddenTransform = settings.AnimationDirection switch
            {
                "Right" => "translateX(120%)",
                "Up" => "translateY(-120%)",
                "Down" => "translateY(120%)",
                _ => "translateX(-120%)"
            };

            return $@"
<!DOCTYPE html>
<html>
<head>
<style>
body {{
    margin:0;
    padding:{settings.GlowIntensity + 40}px;
    background:{bodyBg};
    overflow:hidden;
}}
.card {{
    position:absolute;
    bottom:20px;
    left:20px;
    display:flex;
    align-items:center;
    background:{bg};
    padding:15px 20px;
    border-radius:{settings.BackgroundRadius}px;
    color:{settings.TextColor};
    font-family:Segoe UI;
    box-shadow:0 0 {settings.GlowIntensity}px {settings.GlowColor};
    transform:{hiddenTransform};
    opacity:0;
    transition:all .4s ease;
}}
.card.show {{
    transform:translateX(0);
    opacity:1;
}}
.cover {{
    width:60px;
    height:60px;
    border-radius:{settings.CoverRadius}px;
    margin-right:15px;
    object-fit:cover;
    display:{(settings.ShowCover ? "block" : "none")};
}}
</style>
</head>
<body>
<div id='card' class='card'>
    <img id='cover' class='cover'/>
    <div>
        <div id='title'></div>
        <div id='artist'></div>
    </div>
</div>

<script>
const VISIBLE_MS = {settings.VisibleSeconds * 1000};

function showNowPlaying(title, artist, artwork){{
    const card = document.getElementById('card');
    document.getElementById('title').innerText = title;
    document.getElementById('artist').innerText = artist;

    if(artwork){{
        document.getElementById('cover').src = artwork;
    }}

    card.classList.add('show');

    setTimeout(()=>{{
        card.classList.remove('show');
    }},VISIBLE_MS);
}}
</script>
</body>
</html>";
        }
    }
}

