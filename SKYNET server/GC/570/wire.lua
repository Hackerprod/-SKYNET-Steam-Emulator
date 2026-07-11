PB = PB or {}

function PB.cat(...)
    return gc.Concat(...)
end

function PB.v(field, value)
    return gc.FieldVarint(field, value or 0)
end

function PB.vs(field, value)
    return gc.FieldVarintString(field, tostring(value or "0"))
end

function PB.f32(field, value)
    return gc.FieldFixed32(field, value or 0)
end

function PB.f64(field, value)
    return gc.FieldFixed64(field, value or 0)
end

function PB.f64s(field, value)
    return gc.FieldFixed64String(field, tostring(value or "0"))
end

function PB.bool(field, value)
    return gc.FieldBool(field, value == true)
end

function PB.str(field, value)
    return gc.FieldString(field, value or "")
end

function PB.bytes(field, payload)
    return gc.FieldBytes(field, payload or "")
end

function PB.result(value)
    return gc.Result(value or 0)
end

function PB.now()
    return runtime.NowUnix()
end

function PB.next_id()
    if gc.NextObjectIdString ~= nil then
        return gc.NextObjectIdString()
    end

    return runtime.NextObjectId()
end

