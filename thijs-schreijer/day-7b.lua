-- https://adventofcode.com/2025/day/7

local pl_utils = require("pl.utils")

local input = assert(pl_utils.readlines("thijs-schreijer/day-7a-input.txt"))
local rows = #input
local cols = #input[1]

local function get_cell(r, c)
    if r < 1 or r > rows or c < 1 or c > cols then
        return nil, "out of bounds"
    end
    return input[r]:sub(c, c)
end


local counts = {} -- array with counts per column
counts[input[1]:find("S")] = 1  -- start with one beam at S

for row = 2, rows do -- start at row 2
    local new_counts = {}
    for col = 1, cols do
        local ct = get_cell(row, col) -- cell type

        local value = 0
        if ct == "." then -- is empty, so can hold a value
            value = counts[col] or 0 -- the value right above
            if get_cell(row, col - 1) == "^" then
                -- splitter to my left, so add upper left value
                value = (value or 0) + (counts[col - 1] or 0)
            end
            if get_cell(row, col + 1) == "^" then
                -- splitter to my right, so add upper right value
                value = (value or 0) + (counts[col + 1] or 0)
            end
        end
        new_counts[col] = value
    end
    counts = new_counts
end

-- add all values
local count = 0
for i = 1, cols do
    count = count + (counts[i] or 0)
end

print("number of paths: ", count)
