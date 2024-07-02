function arrayBufferToBase64(buffer) {
    // https://stackoverflow.com/a/9458996
    let binary = "";
    let bytes = new Uint8Array(buffer);
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
};

module.exports = {
    arrayBufferToBase64
};
