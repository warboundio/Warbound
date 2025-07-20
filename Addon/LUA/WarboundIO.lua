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
        lastUpdated = time()
    }
    print("WarboundIO: Initialized new WarboundData SavedVariables")
else
    print("WarboundIO: Loaded existing WarboundData SavedVariables (version: " .. (WarboundData.version or "unknown") .. ")")
end

print("WarboundIO addon loaded successfully!")
