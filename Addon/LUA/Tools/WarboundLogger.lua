WarboundLogger = { debug = true }
local wl = WarboundLogger

local function yellow(text)
    return "|cffffff00" .. text .. "|r"
end

function wl:Log(message)
    DEFAULT_CHAT_FRAME:AddMessage(yellow("WarboundIO") .. ": " .. tostring(message))
end

function wl:Debug(message)
    if not wl.debug then return end
    wl:Log(message)
end
