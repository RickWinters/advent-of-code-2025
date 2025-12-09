-- https://adventofcode.com/2025/day/7

local pl_utils = require("pl.utils")

local input = assert(pl_utils.readlines("thijs-schreijer/day-7a-input.txt"))
local rows = #input
local cols = #input[1]



local cell_types = setmetatable({
    ["."] = "empty",
    ["S"] = "beam",
    ["^"] = "splitter",
    ["|"] = "beam",
}, {
    __index = function(_, key)
        error("unknown cell type: " .. tostring(key))
    end
})



local function get_cell_type(r, c)
    if r < 1 or r > rows or c < 1 or c > cols then
        return nil, "out of bounds"
    end
    return cell_types[input[r]:sub(c, c)]
end


-- returns true if cell was, or was set to a beam
local function set_beam(r, c)
    local curr = get_cell_type(r, c)
    if curr == "empty" then
        input[r] = input[r]:sub(1, c - 1) .. "|" .. input[r]:sub(c + 1, -1)
        return true
    end
    if curr == "beam" then
        return true
    end
    return false
end


local splits = 0
for row = 2, rows do -- start at row 2
    for col = 1, cols do
        local ct = get_cell_type(row, col) -- cell type
        local uct = get_cell_type(row - 1, col) -- upper cell type
        if uct == "beam" then
            if ct == "empty" then
                set_beam(row, col)
            elseif ct == "splitter" then
                set_beam(row, col - 1)
                set_beam(row, col + 1)
                splits = splits + 1
            end
        end
    end
end


print("number of splits: ", splits)
