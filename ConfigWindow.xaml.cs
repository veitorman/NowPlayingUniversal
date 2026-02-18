using System.Linq;
using System.Windows;
using System.Windows.Media;
using NowPlayingUniversal.Models;
using NowPlayingUniversal.Services;

namespace NowPlayingUniversal
{
    public partial class ConfigWindow : Window
    {
        private OverlaySettings settings;

        public ConfigWindow()
        {
            InitializeComponent();

            settings = SettingsService.Load();

            // Monitor
            var screens = System.Windows.Forms.Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
                MonitorBox.Items.Add("Monitor " + i);

            MonitorBox.SelectedIndex = settings.ScreenIndex;

            // Position
            PositionBox.SelectedItem = PositionBox.Items
                .Cast<System.Windows.Controls.ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == settings.Position);

            // Animation
            AnimationBox.SelectedItem = AnimationBox.Items
                .Cast<System.Windows.Controls.ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == settings.AnimationDirection);

            // Seconds
            SecondsBox.Text = settings.VisibleSeconds.ToString();

            // Opacity
            OpacitySlider.Value = settings.BackgroundOpacity;
            GlowBox.Text = settings.GlowIntensity.ToString();

            //Background radius
            BgRadiusBox.Text = settings.BackgroundRadius.ToString();

            //Cover art Radius
            CoverRadiusBox.Text = settings.CoverRadius.ToString();


            // Set color previews
            SetButtonColor(BgColorButton, settings.BackgroundColor);
            SetButtonColor(GlowColorButton, settings.GlowColor);
            SetButtonColor(TextColorButton, settings.TextColor);
        }

        private void SetButtonColor(System.Windows.Controls.Button button, string hex)
        {
            var brush = (SolidColorBrush)new BrushConverter().ConvertFrom(hex);
            button.Background = brush;
            button.Content = hex;
        }

        private string OpenColorDialog(string currentHex)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            dialog.Color = System.Drawing.ColorTranslator.FromHtml(currentHex);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return $"#{dialog.Color.R:X2}{dialog.Color.G:X2}{dialog.Color.B:X2}";
            }

            return currentHex;
        }

        private void BgColor_Click(object sender, RoutedEventArgs e)
        {
            settings.BackgroundColor = OpenColorDialog(settings.BackgroundColor);
            SetButtonColor(BgColorButton, settings.BackgroundColor);
        }

        private void GlowColor_Click(object sender, RoutedEventArgs e)
        {
            settings.GlowColor = OpenColorDialog(settings.GlowColor);
            SetButtonColor(GlowColorButton, settings.GlowColor);
        }

        private void TextColor_Click(object sender, RoutedEventArgs e)
        {
            settings.TextColor = OpenColorDialog(settings.TextColor);
            SetButtonColor(TextColorButton, settings.TextColor);
        }

        ///SKINSISTEM
        private void SkinBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SkinBox.SelectedItem == null)
                return;

            string skin = ((System.Windows.Controls.ComboBoxItem)SkinBox.SelectedItem).Content.ToString();

            switch (skin)
            {
                // =============================
                // BASIC BLACK / WHITE
                // =============================

                case "Squared Black":
                    settings.BackgroundColor = "#000000";
                    settings.TextColor = "#FFFFFF";
                    settings.BackgroundOpacity = 0.8;
                    settings.GlowIntensity = 0;
                    settings.BackgroundRadius = 0;
                    settings.CoverRadius = 0;
                    break;

                case "Rounded Black":
                    settings.BackgroundColor = "#000000";
                    settings.TextColor = "#FFFFFF";
                    settings.BackgroundOpacity = 0.8;
                    settings.GlowIntensity = 0;
                    settings.BackgroundRadius = 50;
                    settings.CoverRadius = 50;
                    break;

                case "Squared White":
                    settings.BackgroundColor = "#FFFFFF";
                    settings.TextColor = "#000000";
                    settings.BackgroundOpacity = 0.8;
                    settings.GlowIntensity = 0;
                    settings.BackgroundRadius = 0;
                    settings.CoverRadius = 0;
                    break;

                case "Rounded White":
                    settings.BackgroundColor = "#FFFFFF";
                    settings.TextColor = "#000000";
                    settings.BackgroundOpacity = 0.8;
                    settings.GlowIntensity = 0;
                    settings.BackgroundRadius = 50;
                    settings.CoverRadius = 50;
                    break;

                // =============================
                // WHITE VARIANTS
                // =============================

                case "Ice White":
                    settings.BackgroundColor = "#E8F6FF";
                    settings.TextColor = "#003344";
                    settings.GlowColor = "#AEEFFF";
                    settings.BackgroundOpacity = 0.9;
                    settings.GlowIntensity = 30;
                    settings.BackgroundRadius = 30;
                    settings.CoverRadius = 20;
                    break;

                case "White Red Accent":
                    settings.BackgroundColor = "#FFFFFF";
                    settings.TextColor = "#880000";
                    settings.GlowColor = "#FF0000";
                    settings.BackgroundOpacity = 0.9;
                    settings.GlowIntensity = 40;
                    settings.BackgroundRadius = 20;
                    settings.CoverRadius = 10;
                    break;

                case "White Blue Accent":
                    settings.BackgroundColor = "#FFFFFF";
                    settings.TextColor = "#0033AA";
                    settings.GlowColor = "#0066FF";
                    settings.BackgroundOpacity = 0.9;
                    settings.GlowIntensity = 35;
                    settings.BackgroundRadius = 25;
                    settings.CoverRadius = 15;
                    break;

                // =============================
                // BLACK VARIANTS
                // =============================

                case "Black Red Neon":
                    settings.BackgroundColor = "#000000";
                    settings.TextColor = "#FF4444";
                    settings.GlowColor = "#FF0000";
                    settings.BackgroundOpacity = 0.85;
                    settings.GlowIntensity = 60;
                    settings.BackgroundRadius = 20;
                    settings.CoverRadius = 10;
                    break;

                case "Black Violet Neon":
                    settings.BackgroundColor = "#000000";
                    settings.TextColor = "#C084FF";
                    settings.GlowColor = "#8B00FF";
                    settings.BackgroundOpacity = 0.85;
                    settings.GlowIntensity = 70;
                    settings.BackgroundRadius = 25;
                    settings.CoverRadius = 20;
                    break;

                case "Black Orange Neon":
                    settings.BackgroundColor = "#000000";
                    settings.TextColor = "#FF9900";
                    settings.GlowColor = "#FF6600";
                    settings.BackgroundOpacity = 0.85;
                    settings.GlowIntensity = 65;
                    settings.BackgroundRadius = 25;
                    settings.CoverRadius = 15;
                    break;
            }

            // Update UI visually
            GlowBox.Text = settings.GlowIntensity.ToString();
            BgRadiusBox.Text = settings.BackgroundRadius.ToString();
            CoverRadiusBox.Text = settings.CoverRadius.ToString();
            SecondsBox.Text = settings.VisibleSeconds.ToString();
            OpacitySlider.Value = settings.BackgroundOpacity;

            SetButtonColor(BgColorButton, settings.BackgroundColor);
            SetButtonColor(GlowColorButton, settings.GlowColor);
            SetButtonColor(TextColorButton, settings.TextColor);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            settings.ScreenIndex = MonitorBox.SelectedIndex;

            if (PositionBox.SelectedItem != null)
            {
                settings.Position =
                    ((System.Windows.Controls.ComboBoxItem)PositionBox.SelectedItem)
                    .Content.ToString();
            }

            if (int.TryParse(SecondsBox.Text, out int seconds))
                settings.VisibleSeconds = seconds;

            settings.BackgroundOpacity = OpacitySlider.Value;

            if (int.TryParse(GlowBox.Text, out int glow))
            {
                settings.GlowIntensity = glow;
            }

            //Radius
            if (int.TryParse(BgRadiusBox.Text, out int bgRadius))
                settings.BackgroundRadius = bgRadius;

            if (int.TryParse(CoverRadiusBox.Text, out int coverRadius))
                settings.CoverRadius = coverRadius;


            // Animation
            if (AnimationBox.SelectedItem != null)
            {
                settings.AnimationDirection =
                    ((System.Windows.Controls.ComboBoxItem)AnimationBox.SelectedItem)
                    .Content.ToString();
            }


            SettingsService.Save(settings);
            this.Close();
        }
    }
}
