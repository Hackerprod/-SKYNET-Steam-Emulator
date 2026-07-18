// CI gate: the committed dist/gc.js and proto-descriptor.json must match a
// fresh build of the current TS + proto sources. Run AFTER `npm run build`;
// fails if the build left the working tree dirty (bundle out of sync).
import { execSync } from "node:child_process";

const paths = [
  "SKYNET server/GC/570/js/dist/gc.js",
  "SKYNET server/GC/570/js/proto-descriptor.json",
];

const diff = execSync(`git status --porcelain -- ${paths.map((p) => `"${p}"`).join(" ")}`, {
  cwd: new URL("../..", import.meta.url),
  encoding: "utf8",
}).trim();

if (diff) {
  console.error("GATE FAIL: committed GC JS artifacts are out of sync with the sources:");
  console.error(diff);
  console.error("Run `npm run build` inside gc-js/ and commit the result.");
  process.exit(1);
}
console.log("bundle-sync gate OK: dist/gc.js and proto-descriptor.json match the sources");
