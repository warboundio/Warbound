WarboundEventManager:On("ADDON_LOADED", function(addonName)
    if addonName ~= "WarboundIO" then return end

    if type(WarboundIO) == "table" then
        for k in pairs(WarboundIO) do
            WarboundIO[k] = nil
        end
    end

    WarboundIO = {
        dataTransmogs = '',
        dataPets = '',
        dataMounts = '',
        dataRecipes = '',
        dataToys = '',
        dataVersion = '1.0',
        dataCreatedAt = time(),
        dataLoot = {},
        dataKill = {},
        dataVendor = {},
        dataVendorItems = {},
        dataBattlePetLocations = {},
        dataExpansionItemIdMapping = '',
        dataMountItemIdMapping = '',
        dataNpcQuests = {},
    }

    WarboundLogger:Log("SavedVariables cleared on load")
end)