# Tools And Capture

This reference describes the GC capture workflow without binding it to one machine. Read `../../skynet-environment.md` for concrete local paths and commands.

## Required Tools

- Visual Studio Build Tools and .NET SDK for rebuilding emulator components when needed.
- NetHook2 for capturing Steam and Game Coordinator traffic.
- NetHookAnalyzer or equivalent tooling for decoding captures.
- Protobuf tooling compatible with the active server/GC implementation.
- A real Steam and Dota 2 installation for original behavior captures.

## Protobuf Sources

Prefer sources in this order:

1. Current local Dota files or capture-derived schemas when available.
2. SteamDatabase GameTracking-Dota2 for Dota-specific protos.
3. SteamDatabase Protobufs for shared Steam and Dota base protos.
4. Legacy coordinator projects only as behavioral guides, not as authoritative schemas.

Do not paste random `.proto` files from blogs or old sample projects into the active coordinator. Compare old projects against current tracked protos and captures before implementing.

## Capture Discipline

1. Prepare the real Steam/Dota path from the environment file.
2. Restore the original Valve Steam API DLL before real-Steam capture.
3. Capture one focused action at a time, such as welcome, inventory, lobby create, lobby launch, selected-region start, or hero equip.
4. Decode message IDs, request/response payloads, job IDs, routing SteamIDs, and message ordering.
5. Restore the emulator DLL before validating the emulator.
6. Implement the minimum complete protocol flow in TypeScript and document the working flow near the relevant code.

## Comparison Checklist

- Message ID and protobuf type match.
- Request and response job routing are coherent.
- Recipient SteamID and instance are correct.
- Required fields and repeated fields are present.
- SO cache ownership and service IDs match the client expectation.
- Updates are sent in the same phase the client expects them.
- The client-visible result is verified in-game when logs alone are insufficient.
