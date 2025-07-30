WarboundMountItemIdMapping = {
    itemQueue           = {},
    MountItemIdMapping  = {},
    loadedCount         = 0,
    totalItems          = 0,
    maxItemID           = 400000,
    batchSize           = 100,
    ticker              = nil,
    lastStatusTime      = 0,
    currentID           = 1,
    startItemID         = 300000,
}

local function debug(msg)
    DEFAULT_CHAT_FRAME:AddMessage("|cFF33FF99[MOUNTITEMMAP]|r " .. msg)
end

function WarboundMountItemIdMapping:ProcessBatch()
    local now = GetTime()

    local toLoad = math.min(self.batchSize, #self.itemQueue)
    for i = 1, toLoad do
        local itemID = table.remove(self.itemQueue, 1)

        local item = Item:CreateFromItemID(itemID)
        item:ContinueOnItemLoad(function()
            local mountID = C_MountJournal.GetMountFromItem(itemID)
            if mountID and mountID > 0 then
                self.MountItemIdMapping[itemID] = mountID
                --debug('ITEMID has mount')
            else
                --debug(("ItemID %d has no mount."):format(itemID))
            end
        end)
    end

    if now - self.lastStatusTime >= 1 then
        self.lastStatusTime = now
        local pct = 100 * (1 - (#self.itemQueue / self.totalItems))
        debug(("Loading: %.1f%% complete (%d of %d)"):format(pct, self.totalItems - #self.itemQueue, self.totalItems))
    end

    if #self.itemQueue == 0 and self.ticker then
        self.ticker:Cancel()
        self.ticker = nil
        C_Timer.After(2, function()
            debug("All mount itemIDs processed.")
            WarboundIO.dataMountItemIdMapping = self:GetSerialized()
        end)
    end
end

function WarboundMountItemIdMapping:Test()
    local itemID = 228760
    local item = Item:CreateFromItemID(itemID)
    item:ContinueOnItemLoad(function()
        print("Mount ID:", C_MountJournal.GetMountFromItem(itemID))
    end)

end

function WarboundMountItemIdMapping:Start()
    if self.ticker then
        debug("Mapping already in progress.")
        return
    end

    self.itemQueue = {}
    self.MountItemIdMapping = {}
    self.loadedCount = 0
    self.totalItems = 0
    self.lastStatusTime = GetTime()

    debug("Queuing itemIDs " .. self.startItemID .. " to " .. self.maxItemID)
    for itemID = self.startItemID, self.maxItemID do
        table.insert(self.itemQueue, itemID)
    end
    self.totalItems = #self.itemQueue

    debug(("Starting mountID mapping for %d items..."):format(self.totalItems))
    self.ticker = C_Timer.NewTicker(0.1, function() self:ProcessBatch() end)
end

function WarboundMountItemIdMapping:GetSerialized()
    local parts = {}
    for itemID, mountID in pairs(self.MountItemIdMapping) do
        if mountID then
            table.insert(parts, tostring(itemID) .. ":" .. tostring(mountID))
        end
    end
    return table.concat(parts, "|")
end
