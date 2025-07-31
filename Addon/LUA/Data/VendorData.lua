WarboundEventManager:On("MERCHANT_SHOW", function()
    C_Timer.After(0.5, WarboundScanVendor)
end)

function WarboundScanVendor()
    local npcInfo = WarboundAPI:GetTargetData()
    if not npcInfo or not npcInfo.npcID then
        WarboundLogger:Debug("Failed to get vendor NPC info.")
        return
    end

    local npcID = npcInfo.npcID
    WarboundIO.dataVendor[npcID] = npcInfo

    local numItems = GetMerchantNumItems()
    for i = 1, numItems do
        local itemID = GetMerchantItemID(i)
        local _, _, priceCopper, quantity = GetMerchantItemInfo(i)

        local cost = 0
        local costID = nil
        local costType = "G" -- default to gold

        local altCount = GetMerchantItemCostInfo(i)
        if altCount and altCount > 0 then
            local _, amount, link = GetMerchantItemCostItem(i, 1)
            cost = amount

            if link then
                costID = tonumber(link:match("item:(%d+):"))
                if costID then
                    costType = "I"
                else
                    costID = tonumber(link:match("currency:(%d+):"))
                    if costID then
                        costType = "C"
                    end
                end
            end
        else
            cost = priceCopper
            costID = 0
        end

        local itemObj = {
            vendorID = npcID,
            itemID = itemID,
            quantity = quantity,
            cost = cost,
            factionID = WarboundAPI:GetPlayerFaction(),
            costID = costID,
            costType = costType
        }

        table.insert(WarboundIO.dataVendorItems, itemObj)
    end

    WarboundLogger:Debug(string.format("Vendor [%d] scanned and stored (%d items).", npcID, numItems))
end