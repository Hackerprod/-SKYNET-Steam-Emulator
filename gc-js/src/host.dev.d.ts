// Dev/test-only host capabilities. NEVER part of the production contract:
// the production host does not inject `runtime` into the sandbox at all, so
// referencing it in shipped handler code fails at runtime and is rejected by
// the CI gate (tools/check-gates.mjs greps the production bundle).

export interface DevRuntime {
  fixture(path: string): Uint8Array;
}

export {};
