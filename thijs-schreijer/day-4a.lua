-- https://adventofcode.com/2025/day/4

local pl_utils = require("pl.utils")

-- read by line, return first line only
local input = assert(pl_utils.readlines("thijs-schreijer/day-4a-input.txt"))
local rows = #input
local cols = #input[1]



local function is_paper_roll(r, c)
    if r < 1 or r > rows or c < 1 or c > cols then
        return nil, "out of bounds"
    end
    local cell = input[r]:sub(c, c)
    return cell == "@"
end


local is_movable do

    local adjacent_directions = {
        {-1, -1}, {-1, 0}, {-1, 1},
        {0, -1},           {0, 1},
        {1, -1},  {1, 0},  {1, 1},
    }

    function is_movable(r, c)
        local count = 0
        for _, dir in ipairs(adjacent_directions) do
            local dr = dir[1]
            local dc = dir[2]
            local adj_r = r + dr
            local adj_c = c + dc
            if is_paper_roll(adj_r, adj_c) then
                count = count + 1
                if count >= 4 then
                    -- too many adjacent rolls, not movable
                    return false
                end
            end
        end
        return true
    end
end



local movable_count = 0
for r = 1, rows do
    for c = 1, cols do
        if is_paper_roll(r, c) then
            if is_movable(r, c) then
                movable_count = movable_count + 1
            end
        end
    end
end

print("movable paper rolls: ", movable_count)
