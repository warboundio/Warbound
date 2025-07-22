local function TrackNewVignettesAndSetTomTom()
    local mapID = C_Map.GetBestMapForUnit("player") or 0
    if mapID == 0 then return end

    local vignetteGUIDs = C_VignetteInfo.GetVignettes()
    for _, guid in ipairs(vignetteGUIDs) do
        local info = C_VignetteInfo.GetVignetteInfo(guid)
        local pos = C_VignetteInfo.GetVignettePosition(guid, mapID)

        if info and pos and TomTom then
            local x = pos.x
            local y = pos.y

            TomTom:AddWaypoint(mapID, x, y, {
                title = info.name or "Vignette",
                persistent = false,
                minimap = true,
                world = true,
            })
        end
    end
end

WarboundEventManager:On("VIGNETTE_MINIMAP_UPDATED", TrackNewVignettesAndSetTomTom)
