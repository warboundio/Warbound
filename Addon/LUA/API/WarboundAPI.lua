WarboundAPI = {}

function WarboundAPI:GetPlayerFaction()
    local faction = UnitFactionGroup("player")
    if faction == "Alliance" then
        return 1
    elseif faction == "Horde" then
        return 2
    elseif faction == "Neutral" then
        return 3
    else
        return 0
    end
end

function WarboundAPI:GetTargetData()
    local guid = UnitGUID("target")
    if not guid then
        WarboundLogger:Debug("No target selected.")
        return
    end

    local _, _, _, _, _, npcID = strsplit("-", guid)
    local name = UnitName("target") or "Unknown"
    local creatureType = UnitCreatureType("target") or "Unknown"
    local faction = UnitFactionGroup("target") or "Unknown"

    local mapID = C_Map.GetBestMapForUnit("player") or -1
    local x, y = 0, 0
    local pos = C_Map.GetPlayerMapPosition(mapID, "player")

    if pos and pos.x and pos.y then
        x = math.floor(pos.x * 1000)
        y = math.floor(pos.y * 1000)
    end

    local npcInfo = {
        npcID = tonumber(npcID),
        name = name,
        type = creatureType,
        faction = faction,
        mapID = mapID,
        x = x,
        y = y
    }

    return npcInfo
end
