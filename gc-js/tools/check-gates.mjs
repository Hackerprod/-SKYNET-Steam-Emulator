// CI build-fail gates (Sec.8 of the migration roadmap):
//   1. The production bundle must not contain fixture-replay capabilities
//      (runtime.fixture / observed_fixture).
//   2. No production C# reads GC/570/fixtures (fixtures are dev/test only).
//   3. main.lua's include graph must not reach routes.generated.lua (the
//      fixture replay table stays out of the live dispatch path).
//   4. Every message id marked as migrated in gc-routing.json must have a
//      golden test (spikes/tests referencing the id) - trivially satisfied
//      while the migrated set is empty, enforced as ids start moving.
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const here = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(here, "..", "..");
const serverRoot = path.join(repoRoot, "SKYNET server");
const gcDir = path.join(serverRoot, "GC", "570");
let failures = 0;

function fail(message) {
  console.error(`GATE FAIL: ${message}`);
  failures++;
}

// ---- 1. production bundle free of fixture capabilities
const bundle = fs.readFileSync(path.join(gcDir, "js", "dist", "gc.js"), "utf8");
for (const needle of ["runtime.fixture", "observed_fixture"]) {
  if (bundle.includes(needle)) {
    fail(`dist/gc.js references '${needle}' (fixtures are dev/test only)`);
  }
}

// ---- 2. no production C# references the fixtures directory
function* walk(dir) {
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      if (["bin", "obj", "node_modules"].includes(entry.name)) continue;
      yield* walk(full);
    } else {
      yield full;
    }
  }
}
for (const file of walk(serverRoot)) {
  if (!file.endsWith(".cs")) continue;
  const text = fs.readFileSync(file, "utf8");
  if (/fixtures[\\/]+nethook|GC[\\/]+570[\\/]+fixtures/i.test(text)) {
    fail(`${path.relative(repoRoot, file)} references GC/570/fixtures from production code`);
  }
}

// ---- 3. live Lua include graph must not reach routes.generated.lua
const included = new Set();
const queue = ["main.lua"];
while (queue.length) {
  const rel = queue.pop();
  if (included.has(rel)) continue;
  included.add(rel);
  const file = path.join(gcDir, rel);
  if (!fs.existsSync(file)) continue;
  for (const m of fs.readFileSync(file, "utf8").matchAll(/include\("([^"]+)"\)/g)) {
    queue.push(m[1]);
  }
}
if (included.has("routes.generated.lua")) {
  fail("main.lua include graph reaches routes.generated.lua (fixture replay in the live path)");
}

// ---- 4. migrated ids require golden coverage
const routingPath = path.join(serverRoot, "GC", "gc-routing.json");
const routing = JSON.parse(fs.readFileSync(routingPath, "utf8"));
const migrated = Object.values(routing.apps ?? {}).flatMap((a) => a.migratedMessageIds ?? []);
if (migrated.length > 0) {
  const testDirs = [path.join(repoRoot, "tests"), path.join(repoRoot, "spikes")];
  const corpus = testDirs
    .filter(fs.existsSync)
    .flatMap((d) => [...walk(d)])
    .filter((f) => f.endsWith(".cs"))
    .map((f) => fs.readFileSync(f, "utf8"))
    .join("\n");
  for (const id of migrated) {
    // convention: a golden test mentions the id (e.g. GoldenTest_4006 / MessageType = 4006)
    if (!new RegExp(`\\b${id}\\b`).test(corpus)) {
      fail(`message id ${id} is marked migrated in gc-routing.json but no golden test mentions it`);
    }
  }
}

if (failures > 0) {
  process.exit(1);
}
console.log(`gates OK (bundle clean, no fixture reads in prod, live include graph clean, ${migrated.length} migrated ids covered)`);
