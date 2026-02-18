🎵 NowPlayingUniversal

Minimal, customizable "Now Playing" overlay for streamers, podcasters and radio setups.

Works with YouTube and any Chromium-based browser via a lightweight local bridge extension.

---

[⬇ Download Latest Release](https://github.com/veitorman/NowPlayingUniversal/releases)


## ✨ Features

- 🟢 Real alpha transparency (no chroma required) (It can be used as a chroma changing "alpha" to "chroma" on config.js)
- 🎨 Fully customizable skins
- 💡 Glow control
- 🪟 Multi-monitor support
- 📍 Position selector (TopLeft, TopRight, BottomLeft, BottomRight)
- 🎞 Animation direction control
- 🎧 Chrome extension bridge (no debug flags required)
- 📦 Portable build

---

## 🧩 How It Works

Browser → Chrome Extension → Local HTTP bridge → WPF Overlay → OBS

The extension sends MediaSession metadata to a local listener running in the overlay app.

No browser flags required.

---

## 🚀 Installation

### 1️⃣ Download the Overlay

Download the latest release from the Releases section.

Or build manually:

dotnet publish -c Release -r win-x64 --self-contained true


---

### 2️⃣ Install the Chrome Extension

1. Open Chrome
2. Go to:
chrome://extensions

3. Enable **Developer Mode**
4. Click **Load Unpacked**
5. Select the `extension` folder

Done.

---

## 🎨 Skins Included

- Squared Black
- Rounded Black
- Squared White
- Rounded White
- Ice White
- White Red
- White Soft Blue
- Black Red
- Black Violet
- Black Orange

---

## 🛠 Configuration

Press **F2** inside the overlay to open settings.

You can control:

- Monitor
- Position
- Animation direction
- Glow intensity
- Background opacity
- Border radius
- Cover art radius
- Visible duration
- Color theme
- Presets

---

## 🎥 OBS Setup

Recommended:

**Method 1 (Best):**
- Add Window Capture
- Capture the overlay window
- No chroma needed (Alpha supported)

**Method 2 (Legacy):**
- Enable chroma mode
- Add Chroma Key filter in OBS

---

## 📂 Project Structure

NowPlayingUniversal/
├── Models/
├── Services/
├── Extension/
├── MainWindow.xaml
├── ConfigWindow.xaml
└── settings.json


---

## 🧠 Why This Project Exists

Most "Now Playing" overlays require browser flags, heavy plugins or OBS browser sources.

This project aims to:

- Be lightweight
- Be portable
- Be hackable
- Be fork-friendly

I do a radio show and always need to show mi listeners what song is playing right now.

---

## 📜 License

MIT License.

Free to fork, modify and improve.

---

## ❤️ Contribute

Feel free to fork and create your own skins, animations or integrations.

Pull requests welcome.
