-- https://adventofcode.com/2025/day/3

local pl_utils = require("pl.utils")

-- read by line, return first line only
local input = assert(pl_utils.readlines("thijs-schreijer/day-3a-input.txt"))


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


local jolts = {}
local total = 0
for i, line in ipairs(input) do
    local first, idx = highest_digit(line:sub(1, -2)) -- skip last number because we need 2 digits
    local last, _ = highest_digit(line:sub(idx + 1, -1))
    jolts[i] = first * 10 + last
    total = total + jolts[i]
end



print("code: ", total)
