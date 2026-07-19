import { gc, HandlerContext, RawMessageContext } from "../framework/gc";
import { CMsgClientHello, CMsgClientWelcome, Msg, Routes } from "../generated/dota";

function routeHandler(ctx: HandlerContext<CMsgClientHello, CMsgClientWelcome>): void {
    const encoded = ctx.encode(Routes.ClientHello.response, {
        version: 1,
        gameData: new Uint8Array(),
        outofdateSubscribedCaches: [],
        gcSocacheFileVersion: 1,
        rtime32GcWelcomeTimestamp: ctx.clock.now(),
        currency: 0
    });

    ctx.logger.info("route contract ok");
    ctx.send(Msg.GCClientWelcome, Routes.ClientHello.response, {
        version: 1,
        gameData: encoded,
        outofdateSubscribedCaches: [],
        gcSocacheFileVersion: 1,
        rtime32GcWelcomeTimestamp: ctx.clock.now(),
        currency: 0
    });
}

function rawHandler(ctx: RawMessageContext): boolean {
    ctx.logger.info(`raw ${ctx.messageType}`);
    return true;
}

gc.on(Routes.ClientHello, routeHandler);
gc.onMessage(Msg.ClientToGCAggregateMetrics, rawHandler);
