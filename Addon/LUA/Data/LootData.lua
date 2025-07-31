WarboundEventManager:On("LOOT_OPENED", function()
    local mapID = C_Map.GetBestMapForUnit("player") or 0
    local posX, posY = 0, 0
    if mapID > 0 then
        local pos = C_Map.GetPlayerMapPosition(mapID, "player")
        if pos then
            posX, posY = pos:GetXY()
        end
    end
    local x = math.floor(posX * 1000 + 0.5)
    local y = math.floor(posY * 1000 + 0.5)

    for slot = 1, GetNumLootItems() do
        local link = GetLootSlotLink(slot)
        if link then
            local itemID = tonumber(link:match("item:(%d+):")) or 0
            local srcInfo = { GetLootSourceInfo(slot) }
            --WarboundLogger:Debug(string.format("Raw GUID from slot %d: %s", slot, tostring(srcInfo[i])))

            for i = 1, #srcInfo, 2 do
                local guid = srcInfo[i]
                local sourceQty = srcInfo[i+1] or 0
                local _, _, _, _, _, npcID = strsplit("-", guid)
                npcID = tonumber(npcID) or 0

                local entry = {
                    itemID   = itemID,
                    npcID    = npcID,
                    quantity = sourceQty,
                    zoneID   = mapID,
                    x        = x,
                    y        = y,
                }
                table.insert(WarboundIO.dataLoot, entry)
                WarboundLogger:Debug(string.format(
                    "itemID=%d, npcID=%d, quantity=%d, zoneID=%d, x=%d, y=%d",
                    entry.itemID, entry.npcID, entry.quantity, entry.zoneID, entry.x, entry.y
                ))                
            end
        end
    end
end)

WarboundEventManager:On("COMBAT_LOG_EVENT_UNFILTERED", function()
    local _, subevent, _, _, _, _, _, destGUID, destName = CombatLogGetCurrentEventInfo()
    if subevent == "PARTY_KILL" then
        local _, _, _, _, _, npcID = strsplit("-", destGUID)
        npcID = tonumber(npcID) or 0

        -- Create or update entry
        WarboundIO.dataKill[npcID] = WarboundIO.dataKill[npcID] or { name = destName or "Unknown", count = 0 }
        WarboundIO.dataKill[npcID].count = WarboundIO.dataKill[npcID].count + 1

        WarboundLogger:Debug(string.format(
            "Killed NPC %d (%s), total kills: %d",
            npcID, destName or "Unknown", WarboundIO.dataKill[npcID].count
        ))
    end
end)