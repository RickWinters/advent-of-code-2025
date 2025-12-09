-- https://adventofcode.com/2025/day/5

local pl_utils = require("pl.utils")

-- read by line, return first line only
local input = assert(pl_utils.readlines("thijs-schreijer/day-5a-input.txt"))

local mark = 0
for i, line in ipairs(input) do
    if line == "" then
        mark = i
        break;
    end
end
table.remove(input, mark) -- drop empty line

-- ingredients follow on next lines, copy to a new table
local ingredients = {}
local l = input[mark]
while l do
    table.remove(input, mark)
    table.insert(ingredients, tonumber(l))
    l = input[mark]
end

-- parse ranges
local ranges = {}
for i, line in ipairs(input) do
    ranges[i] = {pl_utils.splitv(line, "-", true, 2)} -- value split into two parts
    ranges[i][1] = tonumber(ranges[i][1])
    ranges[i][2] = tonumber(ranges[i][2])
end



local function is_fresh(ingredient)
    for i, range in ipairs(ranges) do
        if ingredient >= range[1] and ingredient <= range[2] then
            return true
        end
    end
    return false
end



local fresh_ingredients = 0
for i, line in ipairs(ingredients) do
    if is_fresh(line) then
        fresh_ingredients = fresh_ingredients + 1
    end
end



print("fresh ingredients: ", fresh_ingredients)
