WarboundItemExpansionMapping = {
    itemQueue      = {},
    ItemExpansionMapping = {},
    loadedCount    = 0,
    totalItems     = 0,
    maxItemID      = 300000,
    batchSize      = 200,
    ticker         = nil,
    lastStatusTime = 0,
    currentID      = 1,
    startItemID    = 200000,
}

local function debug(msg)
    DEFAULT_CHAT_FRAME:AddMessage("|cFF33FF99[ITEMEXP]|r " .. msg)
end

function WarboundItemExpansionMapping:ProcessBatch()
    local now = GetTime()

    -- Load batch of itemIDs
    local toLoad = math.min(self.batchSize, #self.itemQueue)
    for i = 1, toLoad do
        local itemID = table.remove(self.itemQueue, 1)
        if C_Item.DoesItemExistByID(itemID) then
            local item = Item:CreateFromItemID(itemID)
            item:ContinueOnItemLoad(function()
                local _, _, _, _, _, _, _, _, _, _, _, _, _, _, expansionID = C_Item.GetItemInfo(itemID)
                if expansionID then
                    self.ItemExpansionMapping[itemID] = expansionID
                end
            end)
        else
            debug(("ItemID %d does not exist. Skipping."):format(itemID))
        end
    end

    -- Status logging every second
    if now - self.lastStatusTime >= 1 then
        self.lastStatusTime = now
        local pct = 100 * (1 - (#self.itemQueue / self.totalItems))
        debug(("Loading: %.1f%% complete (%d of %d)"):format(pct, self.totalItems - #self.itemQueue, self.totalItems))
    end

    -- If queue is empty, stop ticker and finalize after a short delay
    if #self.itemQueue == 0 and self.ticker then
        self.ticker:Cancel()
        self.ticker = nil
        C_Timer.After(2, function()
            debug("All itemIDs processed.")
            WarboundIO.dataExpansionItemIdMapping = self:GetSerialized()
        end)
    end
end

function WarboundItemExpansionMapping:Start()
    if self.ticker then
        debug("Mapping already in progress.")
        return
    end

    self.itemQueue = {}
    self.ItemExpansionMapping = {}
    self.loadedCount = 0
    self.totalItems = 0
    self.lastStatusTime = GetTime()

    debug("Queuing itemIDs " .. self.startItemID .. " to " .. self.maxItemID)
    for itemID = self.startItemID, self.maxItemID do
        table.insert(self.itemQueue, itemID)
    end
    self.totalItems = #self.itemQueue

    debug(("Starting expansionID mapping for %d items..."):format(self.totalItems))
    self.ticker = C_Timer.NewTicker(0.1, function() self:ProcessBatch() end)
end

function WarboundItemExpansionMapping:GetSerialized()
    local expansionToItems = {}
    for itemID, expansionID in pairs(self.ItemExpansionMapping) do
        if expansionID then
            expansionToItems[expansionID] = expansionToItems[expansionID] or {}
            table.insert(expansionToItems[expansionID], itemID)
        end
    end

    local parts = {}
    for expansionID, itemIDs in pairs(expansionToItems) do
        table.insert(parts, "|" .. tostring(expansionID) .. "|" .. table.concat(itemIDs, ";") .. ";")
    end

    return table.concat(parts)
end
