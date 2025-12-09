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



-- merges overlapping ranges
local function merge_ranges(ranges)
    table.sort(ranges, function(a, b) return a[1] < b[1] end)
    local merged = {}
    local current = ranges[1]
    for i = 2, #ranges do
        local next_range = ranges[i]
        if next_range[1] <= current[2] + 1 then
            -- overlapping or contiguous ranges, merge them
            current[2] = math.max(current[2], next_range[2])
        else
            table.insert(merged, current)
            current = next_range
        end
    end
    table.insert(merged, current)
    return merged
end


ranges = merge_ranges(ranges)

local fresh_count = 0
for i, range in ipairs(ranges) do
    fresh_count = fresh_count + (range[2] - range[1] + 1)
end

print("fresh ingredient ids: ", fresh_count)
