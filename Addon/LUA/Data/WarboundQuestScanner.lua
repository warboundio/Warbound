local WarboundQuestScanner = {}

-- Init entry point
function WarboundQuestScanner:Init()
    WarboundEventManager:On("QUEST_ACCEPTED", function(questID)
        C_Timer.After(0.25, function()
            self:LogQuest(questID, true)
        end)
    end)

    WarboundEventManager:On("QUEST_COMPLETE", function()
        C_Timer.After(0.25, function()
            self:ScanQuestComplete()
        end)
    end)
end

-- Location helper
function WarboundQuestScanner:GetLocationData()
    local mapID = C_Map.GetBestMapForUnit("player") or 0
    local x, y = 0, 0
    if mapID > 0 then
        local pos = C_Map.GetPlayerMapPosition(mapID, "player")
        if pos ~= nil then
            x = tonumber(pos.x) and math.floor(pos.x * 10000) / 100 or 0
            y = tonumber(pos.y) and math.floor(pos.y * 10000) / 100 or 0
        end
    end
    return mapID, x, y
end

-- Core logger
function WarboundQuestScanner:LogQuest(questID, isStart)
    local npcInfo = WarboundAPI:GetTargetData()
    local npcID = npcInfo and npcInfo.npcID or 0
    local npcName = UnitName("target") or "UNKNOWN"
    local mapID, x, y = self:GetLocationData()

    local entry = {
        questID = questID,
        npcID = npcID,
        npcName = npcName,
        mapID = mapID,
        x = x,
        y = y,
        factionID = WarboundAPI:GetPlayerFaction(),
        isStart = isStart
    }

    table.insert(WarboundIO.dataNpcQuests, entry)

    WarboundLogger:Debug(string.format(
        "[QUEST_%s] QuestID=%d | NPCID=%d | NPC='%s' | MapID=%d | X=%.2f Y=%.2f",
        isStart and "DETAIL" or "COMPLETE", questID, npcID, npcName, mapID, x, y
    ))
end

-- Completion handler
function WarboundQuestScanner:ScanQuestComplete()
    local questID = GetQuestID()
    if questID then
        self:LogQuest(questID, false)
    end
end

-- Expose and init
WarboundQuestScanner:Init()
return WarboundQuestScanner
