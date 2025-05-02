window.cryptoService = {
    async generateKey() {
        const key = await crypto.subtle.generateKey(
            { name: "AES-GCM", length: 256 },
            true,
            ["encrypt", "decrypt"]
        );

        const exportedKey = await crypto.subtle.exportKey("raw", key);
        return Array.from(new Uint8Array(exportedKey));
    },
    async encrypt(data, rawKey) {
        const iv = crypto.getRandomValues(new Uint8Array(12));
        const key = await crypto.subtle.importKey("raw", new Uint8Array(rawKey), { name: "AES-GCM" }, false, ["encrypt"]);
        const enc = await crypto.subtle.encrypt({ name: "AES-GCM", iv }, key, new Uint8Array(data));
        return Array.from(iv).concat(Array.from(new Uint8Array(enc)));
    },
    async decrypt(encryptedData, rawKey) {
        const iv = new UInt8Array(encryptedData.slice(0, 12));
        const data = new UInt8Array(encryptedData.slice(12));
        const key = await crypto.subtle.importKey("raw", new Uint8Array(rawKey), { name: "AES-GCM" }, false, ["decrypt"]);
        const dec = await crypto.subtle.decrypt({ name: "AES-GCM", iv: new Uint8Array(iv) }, key, new Uint8Array(data));
        return Array.from(new Uint8Array(dec));
    }
};