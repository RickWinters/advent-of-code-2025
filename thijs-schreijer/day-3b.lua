-- https://adventofcode.com/2025/day/3

local pl_utils = require("pl.utils")

-- read by line, return first line only
local input = assert(pl_utils.readlines("thijs-schreijer/day-3a-input.txt"))

local digits = 12



local function highest_digit(s)
    local highest = 0
    local idx = 0
    for i = 1, #s do
        local c = tonumber(s:sub(i, i))
        if c > highest then
            highest = c
            idx = i
        end
    end
    return highest, idx
end



local function find_highest(s)
    local idx = 1
    local result = ""
    while #result < digits do
        local highest, new_idx = highest_digit(s:sub(idx, -(digits - #result)))
        result = result .. tostring(highest)
        idx = idx + new_idx
    end
    return tonumber(result)
end



local jolts = {}
local total = 0
for i, line in ipairs(input) do
    local n = find_highest(line)
    jolts[i] = n
    total = total + jolts[i]
end



print("code: ", total)
