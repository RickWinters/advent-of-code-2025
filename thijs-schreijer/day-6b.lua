-- https://adventofcode.com/2025/day/6

local pl_utils = require("pl.utils")
local pl_stringx = require("pl.stringx")

-- read input file as lines
local input = assert(pl_utils.readlines("thijs-schreijer/day-6a-input.txt"))

-- make lines equal length by padding with spaces
local max_length = 0
for _, line in ipairs(input) do
  if #line > max_length then
    max_length = #line
  end
end
for i, line in ipairs(input) do
  if #line < max_length then
    input[i] = line .. string.rep(" ", max_length - #line)
  end
end

-- transpose the array of strings
local transposed = {}
local operations = {}
for pos = max_length, 1, -1 do
    local line = ""
    for _, str in ipairs(input) do
        line = line .. str:sub(pos, pos)
    end
    transposed[pos] = line
end

-- rebuild strings
local problems = {}
local line = ""
for i = #transposed, 1, -1 do
    local elem = pl_stringx.strip(transposed[i])
    if elem == "" then
        -- separator
        table.insert(problems,1, line)
        line = ""
    else
        line = elem .. " " .. line
    end
end
table.insert(problems,1, line)

-- extract operations
local ops = {}
for i, problem in ipairs(problems) do
    for op in problem:gmatch("([%*%+])") do
        table.insert(ops, op)
    end
    operations[i] = ops
    problems[i] = problem:gsub("[%*%+]", ""):gsub("%s+", " "):gsub("^%s+", ""):gsub("%s+$", "")
end

-- do the math
local results = {}
for i, line in ipairs(problems) do
    local op = ops[i]
    local numbers = pl_utils.split(line, " ")

    for j, num in ipairs(numbers) do
        num = tonumber(num)
        if op == "*" then
            results[i] = (results[i] or 1) * num
        elseif op == "+" then
            results[i] = (results[i] or 0) + num
        else
            print("title: ", require("pl.pretty").write(op))
            error("unknown operation: " .. tostring(op))
        end
    end
end

-- sum it up
local total = 0
for _, res in ipairs(results) do
    total = total + res
end

print("Total:", total)
