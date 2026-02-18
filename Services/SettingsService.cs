using System.IO;
using System.Text.Json;
using NowPlayingUniversal.Models;

namespace NowPlayingUniversal.Services
{
    public static class SettingsService
    {
        private static string FilePath => "settings.json";

        public static OverlaySettings Load()
        {
            if (!File.Exists(FilePath))
            {
                var defaults = new OverlaySettings();
                Save(defaults);
                return defaults;
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<OverlaySettings>(json);
        }

        public static void Save(OverlaySettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }
    }
}
