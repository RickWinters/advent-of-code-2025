#input_file_path = "./example.txt"
input_file_path = "./input.txt"

def get_import_as_array(file_path) 

  input = File.read(file_path)

  arr = []

  input.split(",").each_with_index { |sequence|
    split_sequence = sequence.split('-')

    start =  split_sequence[0].to_i
    sequence_end =  split_sequence[1].to_i

    tempArr = []

    (sequence_end - start + 1).times { |index| 
      tempArr << (start + index).to_s
    }

    arr << tempArr
  }

  return arr
end 

def is_repeating(number)
  return false if number.length % 2 == 1 

  return number[0, number.length/ 2] == number[number.length/ 2,  number.length]
end 

def is_repeatingPart2(number)
  s = number.to_s
  
  return false if s.length == 1

  (s.length / 2).times{|index|
    return true if parts_are_equal(s, index + 1)
  }

  return false
end 

def parts_are_equal(number, part_length )
  # Return if the parts are not all equal length
  return false if number.length % part_length != 0

  last_part = number[number.length - part_length, number.length]

  # check if all the parts of the same length are equal to each other 
  (number.length / part_length - 1).times {|index|
    current_part = number[part_length * index, part_length ]

    return false if last_part != current_part
  }

  return true

end 

arr = get_import_as_array input_file_path

answer1 = 0
answer2 = 0

arr.each {|sequence|
  sequence.each {|number|
    if is_repeating number 
      answer1 += number.to_i
    end 

    if is_repeatingPart2 number 
      answer2 += number.to_i
    end 
  }
}

puts "Answer 1: #{answer1}"
puts "Answer 2: #{answer2}"