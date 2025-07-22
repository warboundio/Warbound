WarboundEventManager:On("PET_BATTLE_OPENING_START", function()
    local mapID = C_Map.GetBestMapForUnit("player") or -1
    local pos = C_Map.GetPlayerMapPosition(mapID, "player")
    local x = pos and math.floor(pos.x * 1000) or 0
    local y = pos and math.floor(pos.y * 1000) or 0
    local speciesID = C_PetBattles.GetPetSpeciesID(2, 1) or -1

    local entry = {
        mapID = mapID,
        x = x,
        y = y,
        id = speciesID
    }

    table.insert(WarboundIO.dataBattlePetLocations, entry)
    WarboundLogger:Debug(string.format("Pet battle: map %d (%d, %d), species %d", mapID, x, y, id))
end)