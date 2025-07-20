-- WarboundIO Addon
-- A minimal World of Warcraft addon for warbound.io

-- Initialize WarboundData SavedVariables table with default schema
if not WarboundData then
    WarboundData = {
        transmogs = "",
        pets = "",
        mounts = "",
        recipes = "",
        toys = "",
        version = "1.0",
        lastUpdated = date("%Y-%m-%dT%H:%M:%SZ")
    }
    print("WarboundIO: Initialized new WarboundData SavedVariables")
else
    print("WarboundIO: Loaded existing WarboundData SavedVariables (version: " .. (WarboundData.version or "unknown") .. ")")
end

-- Validate and repair WarboundData structure if needed
local function validateAndRepairData()
    local changed = false
    
    -- Ensure all required fields exist
    if not WarboundData.transmogs then
        WarboundData.transmogs = ""
        changed = true
    end
    if not WarboundData.pets then
        WarboundData.pets = ""
        changed = true
    end
    if not WarboundData.mounts then
        WarboundData.mounts = ""
        changed = true
    end
    if not WarboundData.recipes then
        WarboundData.recipes = ""
        changed = true
    end
    if not WarboundData.toys then
        WarboundData.toys = ""
        changed = true
    end
    if not WarboundData.version then
        WarboundData.version = "1.0"
        changed = true
    end
    if not WarboundData.lastUpdated then
        WarboundData.lastUpdated = date("%Y-%m-%dT%H:%M:%SZ")
        changed = true
    end
    
    if changed then
        print("WarboundIO: Repaired WarboundData structure")
    end
end

-- Perform validation on load
validateAndRepairData()

print("WarboundIO addon loaded successfully!")