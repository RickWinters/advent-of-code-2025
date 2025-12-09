-- https://adventofcode.com/2025/day/2

local pl_utils = require("pl.utils")

-- read by line, return first line only
local input = assert(pl_utils.readlines("thijs-schreijer/day-2a-input.txt")[1])
-- split line in indivdual ranges
local ranges = pl_utils.split(input, ",")
-- split ranges in start and end ranges
local sranges, eranges = {}, {}
for i, range in ipairs(ranges) do
    sranges[i], eranges[i] = pl_utils.splitv(range,'-', false, 2)
end

local function isValidId(id)
    for n = 1, #id-1 do -- Skip n = #id, as that would always match!
        if #id % n == 0 then
            local pattern = id:sub(1, n)
            local repeated = pattern:rep(#id / n)
            if repeated == id then
                return false
            end
        end
    end
    return true
end

local result = 0
for i, range in ipairs(ranges) do
    for id = tonumber(sranges[i]), tonumber(eranges[i]) do
        if not isValidId(tostring(id)) then
            result = result + id
        end
    end
end

print("code: ", result)
