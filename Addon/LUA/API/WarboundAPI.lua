WarboundAPI = {}

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
    local pos = C_Map.GetPlayerMapPosition(mapID, "player")

    local npcInfo = {
        npcID = tonumber(npcID),
        name = name,
        type = creatureType,
        faction = faction,
        mapID = mapID,
        x = math.floor(pos.x * 1000),
        y = math.floor(pos.y * 1000)
    }

    return npcInfo
end
