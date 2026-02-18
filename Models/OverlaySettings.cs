namespace NowPlayingUniversal.Models
{
    public class OverlaySettings
    {
        public string BackgroundColor { get; set; } = "#141414";
        public double BackgroundOpacity { get; set; } = 0.95;
        public string GlowColor { get; set; } = "cyan";
        public string TextColor { get; set; } = "white";
        public bool ShowCover { get; set; } = true;
        public bool CircularCover { get; set; } = true;
        public int VisibleSeconds { get; set; } = 8;
        public int Width { get; set; } = 500;
        public int Height { get; set; } = 160;
        public string BackgroundMode { get; set; } = "Chroma";
    public int ScreenIndex { get; set; } = 0;
    public string Position { get; set; } = "BottomLeft";
        public int GlowIntensity { get; set; } = 30;
        public string AnimationDirection { get; set; } = "Left";
        public int BackgroundRadius { get; set; } = 20;
        public int CoverRadius { get; set; } = 50;



    }
}

