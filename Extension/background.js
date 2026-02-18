chrome.runtime.onMessage.addListener((message) => {
    fetch("http://127.0.0.1:32145/nowplaying", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(message)
    }).catch(() => { });
});
