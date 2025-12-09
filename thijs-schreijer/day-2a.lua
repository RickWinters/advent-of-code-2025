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

local function isValidId(i)
    local l = i:sub(1,math.floor(#i/2))
    local r = i:sub(math.floor(#i/2)+1, -1)
    return l ~= r
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
