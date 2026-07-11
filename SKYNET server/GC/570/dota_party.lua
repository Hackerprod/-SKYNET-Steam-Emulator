function dota_party_on_login()
    if gc.DotaPartyEmitCurrent ~= nil then
        gc.DotaPartyEmitCurrent()
    end
end

function dota_party_handle(message_type)
    local handlers = {
        [MSG.GCInviteToParty] = function()
            return gc.DotaPartyInviteToParty()
        end,
        [MSG.GCPartyInviteResponse] = function()
            return gc.DotaPartyInviteResponse()
        end,
        [MSG.ClientToGCPingData] = function()
            return gc.DotaPartyPingData()
        end,
        [MSG.ClientToGCSetPartyLeader] = function()
            return gc.DotaPartySetLeader()
        end,
        [MSG.GCLeaveParty] = function()
            return gc.DotaPartyLeave()
        end,
        [MSG.GCKickFromParty] = function()
            return gc.DotaPartyKick()
        end,
        [MSG.GCPartyMemberSetCoach] = function()
            return gc.DotaPartySetCoach()
        end,
        [MSG.PartyReadyCheckRequest] = function()
            return gc.DotaPartyReadyCheckRequest()
        end,
        [MSG.PartyReadyCheckAcknowledge] = function()
            return gc.DotaPartyReadyCheckAcknowledge()
        end
    }

    local handler = handlers[message_type]
    if handler == nil then
        return false
    end

    return handler()
end
