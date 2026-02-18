let lastTitle = "";

function sendNowPlaying(data) {
    chrome.runtime.sendMessage(data);
}

function checkMedia() {
    if (navigator.mediaSession && navigator.mediaSession.metadata) {
        const m = navigator.mediaSession.metadata;

        if (m.title && m.title !== lastTitle) {
            lastTitle = m.title;

            sendNowPlaying({
                title: m.title || "",
                artist: m.artist || "",
                artwork: (m.artwork && m.artwork.length > 0)
                    ? m.artwork[0].src
                    : ""
            });
        }
    }
}

setInterval(checkMedia, 1000);

let lastUrl = location.href;
setInterval(() => {
    if (location.href !== lastUrl) {
        lastUrl = location.href;
        lastTitle = "";
    }
}, 1000);
