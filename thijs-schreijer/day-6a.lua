-- https://adventofcode.com/2025/day/6

local pl_utils = require("pl.utils")
local pl_stringx = require("pl.stringx")

-- read by line, return first line only
local input = assert(pl_utils.readfile("thijs-schreijer/day-6a-input.txt"))

-- replace all double spaces with a single one, in a loop until done
local n = -1
while n ~= 0 do
    input, n = input:gsub("  ", " ")
end

-- split into lines
input = pl_stringx.splitlines(input)

-- trim leading/trailing spaces from each line
for i, line in ipairs(input) do
    input[i] = pl_stringx.strip(line)
end

-- extract the operations, last line of input
local operations = input[#input]
table.remove(input, #input) -- drop operations line
operations = pl_utils.split(operations) -- split into individual operations as array

-- do the math
local results = {}
for _, data in ipairs(input) do
    local elements = pl_utils.split(data)
    for i, element in ipairs(elements) do
        local value = tonumber(element)
        if operations[i] == "+" then
            results[i] = (results[i] or 0) + value
        elseif operations[i] == "*" then
            results[i] = (results[i] or 1) * value
        else
            -- sanity check
            error("unknown operation: " .. operations[i])
        end
    end
end

-- add results into a single number
local final_result = 0
for i, value in ipairs(results) do
    final_result = final_result + value
end

print("final result: ", final_result)
