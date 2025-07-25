WarboundBase90 = {}

local alphabet = "rO'Y{5Lu#2.WzXECaFcVRbqT9oKHN86G/sA~^3@;MJP)gZUf:&h]1ekQlxmI+di4$_np70D,?!Bv-[jt*=y%"
local base = 90
local max_value = base ^ 3 - 1

local char_array = {}
for i = 1, #alphabet do
    char_array[i - 1] = alphabet:sub(i, i)
end

local function encode(n)
    if type(n) ~= "number" or n < 0 or n > max_value then
        error("Base90: input must be a number between 0 and " .. max_value)
    end

    local encoded = {}
    repeat
        local remainder = n % base
        table.insert(encoded, 1, char_array[remainder])
        n = math.floor(n / base)
    until n == 0

    while #encoded < 3 do
        table.insert(encoded, 1, " ")
    end

    return table.concat(encoded)
end

setmetatable(WarboundBase90, {
    __call = function(_, n)
        return encode(n)
    end
})