// Proto.encode/decode over the descriptor generated from proto/ (pbjs -t json).
//
// uint64 policy (Fase 0 decision): inside JS, 64-bit fields are Long.js objects
// (protobuf.js's native representation; there is no BigInt mode in mainline
// protobuf.js). At every JSON/C# boundary they are strings, produced via
// toObject({ longs: String }). One representation end-to-end; never a JS number.
//
// Unknown fields: protobuf.js DROPS unknown fields on decode. That is fine for
// every message we build from scratch (all s2c responses) and for c2s requests
// we only read. Messages that would need pass-through re-emission must keep the
// original bytes instead of decode->encode round-tripping (see the spike README
// for the audited subset).

import * as protobuf from "protobufjs/light";
import Long from "long";

// Force the real Long.js implementation so 64-bit fields never degrade to
// lossy JS numbers (protobuf.js falls back to number without it).
protobuf.util.Long = Long;
protobuf.configure();

let root: protobuf.Root | null = null;

export function initProto(descriptorJson: string): void {
  root = protobuf.Root.fromJSON(JSON.parse(descriptorJson));
}

function lookup(name: string): protobuf.Type {
  if (!root) {
    throw new Error("Proto descriptor not initialised (missing proto-descriptor.json)");
  }
  return root.lookupType(name);
}

export const Proto = {
  // bytes -> message instance (64-bit fields are Long objects)
  decode(name: string, bytes: Uint8Array): any {
    return lookup(name).decode(bytes);
  },

  // plain object or message instance -> wire bytes
  encode(name: string, obj: any): Uint8Array {
    const type = lookup(name);
    const message = obj instanceof protobuf.Message ? obj : type.fromObject(obj);
    return type.encode(message).finish();
  },

  // message/bytes -> plain JSON-safe object; uint64 becomes string (boundary rule)
  toObject(name: string, message: any): any {
    return lookup(name).toObject(message, {
      longs: String,
      bytes: String, // base64 at the JSON boundary
      enums: Number,
      defaults: false,
    });
  },

  decodeToObject(name: string, bytes: Uint8Array): any {
    return this.toObject(name, this.decode(name, bytes));
  },
};

export { Long };
