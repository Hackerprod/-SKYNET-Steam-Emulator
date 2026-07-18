// Dev helper: decode a nethook fixture with the curated protos to validate
// field numbers while iterating on proto/ (Fase 2 curation loop).
// Usage: node tools/dev-decode-fixture.mjs <TypeName> <fixture-relative-path> [--econ]
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import protobuf from "protobufjs";

const here = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(here, "..", "..");
const gcDir = path.join(repoRoot, "SKYNET server", "GC", "570");

const root = await protobuf.load(
  fs.readdirSync(path.join(repoRoot, "proto")).map((f) => path.join(repoRoot, "proto", f)),
);

const [typeName, fixture, flag] = process.argv.slice(2);
const type = root.lookupType(typeName);
const bytes = fs.readFileSync(path.join(gcDir, fixture));
const msg = type.decode(bytes);
const obj = type.toObject(msg, { longs: String, bytes: String, enums: Number });
console.log(JSON.stringify(obj, null, 2).slice(0, 6000));

if (flag === "--econ") {
  // decode first few CSOEconItem blobs inside a SOCacheSubscribed (type_id 1)
  const econ = root.lookupType("CSOEconItem");
  for (const st of msg.objects ?? []) {
    console.log(`type_id=${st.typeId} count=${st.objectData.length}`);
    if (st.typeId === 1) {
      for (const data of st.objectData.slice(0, 3)) {
        const item = econ.decode(data);
        console.log("  item:", JSON.stringify(econ.toObject(item, { longs: String })));
      }
    }
  }
}

// re-encode and compare sizes (unknown fields in the fixture are dropped by
// protobuf.js, so a size delta signals unmodeled fields)
const re = type.encode(type.fromObject(obj)).finish();
console.log(`original=${bytes.length} bytes, re-encoded=${re.length} bytes, byteExact=${Buffer.compare(bytes, re) === 0}`);
