// Pure-JS base64 for the host boundary. Jint has no atob/btoa/Buffer, so the
// host passes payloads as base64 strings and this shim turns them into
// Uint8Array before any handler sees them (the contract type is bytes).

const ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
const REVERSE: Record<string, number> = {};
for (let i = 0; i < ALPHABET.length; i++) {
  REVERSE[ALPHABET[i]] = i;
}

export function b64decode(input: string): Uint8Array {
  const clean = input.replace(/[=\s]+$/g, "");
  const out = new Uint8Array(Math.floor((clean.length * 3) / 4));
  let o = 0;
  for (let i = 0; i + 1 < clean.length; i += 4) {
    const a = REVERSE[clean[i]];
    const b = REVERSE[clean[i + 1]];
    const c = i + 2 < clean.length ? REVERSE[clean[i + 2]] : -1;
    const d = i + 3 < clean.length ? REVERSE[clean[i + 3]] : -1;
    out[o++] = (a << 2) | (b >> 4);
    if (c >= 0) out[o++] = ((b & 15) << 4) | (c >> 2);
    if (d >= 0) out[o++] = ((c & 3) << 6) | d;
  }
  return out.subarray(0, o);
}

export function b64encode(bytes: Uint8Array): string {
  let out = "";
  for (let i = 0; i < bytes.length; i += 3) {
    const a = bytes[i];
    const b = i + 1 < bytes.length ? bytes[i + 1] : -1;
    const c = i + 2 < bytes.length ? bytes[i + 2] : -1;
    out += ALPHABET[a >> 2];
    out += ALPHABET[((a & 3) << 4) | (b >= 0 ? b >> 4 : 0)];
    out += b >= 0 ? ALPHABET[((b & 15) << 2) | (c >= 0 ? c >> 6 : 0)] : "=";
    out += c >= 0 ? ALPHABET[c & 63] : "=";
  }
  return out;
}
