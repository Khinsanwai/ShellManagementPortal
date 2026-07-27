function downloadFileFromBase64(fileName, base64Data, mimeType) {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}

var _pendingChildToken = null;

function registerChildTokenHandshake(token) {
    _pendingChildToken = token;
}

window.addEventListener("message", function (event) {
    if (event.data && event.data.type === "child_ready" && _pendingChildToken) {
        var iframe = document.querySelector('iframe');
        if (iframe && iframe.contentWindow) {
            iframe.contentWindow.postMessage({
                type: "access_token",
                token: _pendingChildToken
            }, "*");
        }
    }
});
