use std::{fs::read_to_string, process::exit};

#[derive(Debug)]
struct MathOperation{
    numbers: Vec<usize>,
    operation: char,
}

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let common_space_indices = find_spaces_indices(&file_content);
    let math_operations_part_a = create_math_operations_part_a(&file_content, &common_space_indices);

    let part_a_results = execute_math_operations(&math_operations_part_a);
    println!("Part A - Sum of all math operations results: {}", part_a_results);

    let math_operations_part_b = create_math_operations_part_b(&file_content, &common_space_indices);
    let part_b_results = execute_math_operations(&math_operations_part_b);
    println!("Part B - Sum of all math operations results: {}", part_b_results);
}

fn execute_math_operations(math_operations: &Vec<MathOperation>) -> usize {
    let mut results: usize = 0;
    for math_operation in math_operations{
        // println!{"Executing math operation: \n{:#?}", math_operation};
        let operation_result: usize = match math_operation.operation {
            '+' => math_operation.numbers.iter().sum(),
            '*' => math_operation.numbers.iter().product(),
            _ => 0,
        };
        results += operation_result;
    }
    results
}

fn create_math_operations_part_b(file_contents: &str, common_space_indices: &Vec<usize>) -> Vec<MathOperation> {
    let mut math_operations: Vec<MathOperation> = Vec::new();
    // revers the indices, starting at the end and add '0' to get the first
    let mut space_indices_reversed: Vec<usize> = common_space_indices.iter().rev().cloned().collect(); 
    space_indices_reversed.push(0);
    let lines: Vec<&str> = file_contents.split('\n').collect();

    // keep track of previous start
    let mut previous_index: usize = lines[0].len();
    let n_lines: usize = lines.len();
    // loop over all indices where spaces (seperators) are located
    for index in space_indices_reversed {
        // initialize math operation
        let mut math_operation = MathOperation{
            numbers: Vec::new(),
            operation: ' ',
        };
        for _ in 0..(previous_index - index) {
            math_operation.numbers.push(0);
        }
        let mut math_operation_number_index: usize = 0;
        // loop over the current segnment in reverse
        for i in (index..previous_index).rev() {
            // loop over the text lines to get a specific character 
            for (linenmuber, line) in lines.iter().enumerate() {
                let character = line.chars().nth(i).unwrap();
                if character == ' ' { // do nothing for spaces
                    continue
                }
                if linenmuber == n_lines -1 { // we're at the last line, so this is the operation
                    math_operation.operation = character;
                    continue
                }
                // build up specific number
                math_operation.numbers[math_operation_number_index] *= 10;
                math_operation.numbers[math_operation_number_index] += character.to_digit(10).unwrap() as usize;      
            }
            math_operation_number_index +=1;
        }
        // theres an off by one error somewhere in the indicies handling, so remove trailing zero if exists
        if math_operation.numbers[math_operation.numbers.len()-1] == 0 {
            math_operation.numbers.pop();
        }
        math_operations.push(math_operation);
        previous_index = index;
    }
    math_operations
}

fn create_math_operations_part_a(file_contents: &str, common_space_indices: &Vec<usize>) -> Vec<MathOperation> {
    let mut math_operations: Vec<MathOperation> = Vec::new();
    let lines: Vec<&str> = file_contents.split('\n').collect();
    let line_length = lines[0].len();
    let mut previous_index: usize = 0;
    // add line length to the indices to capture the last segment
    let mut end_space_indices = common_space_indices.clone();
    end_space_indices.push(line_length);
    // loop over all indices where spaces (seperators) are located
    for space_index in end_space_indices.iter() {
        let mut math_operation = MathOperation{
            numbers: Vec::new(),
            operation: ' ',
        };
        // loop over the text lines to get the specific segment
        for (i, line) in lines.iter().enumerate() {
            // get specific segment
            let segment: &str = &line[previous_index..*space_index];
            if i < lines.len() -1 { // last line means number
                let number: usize = segment.trim().parse().unwrap();
                math_operation.numbers.push(number);
            } else {  // last line means operation
                let op_char: char = segment.trim().chars().next().unwrap();
                math_operation.operation = op_char;
            }
        }
        // keep track of previous index
        previous_index = *space_index;
        // println!("Math operation: {:#?}", math_operation);
        math_operations.push(math_operation);
    }
    math_operations
}


fn find_spaces_indices(file_contents: &str) -> Vec<usize> {
    let mut indices: Vec<usize> = Vec::new();
    let lines: Vec<&str> = file_contents.split('\n').collect();
    // loop over all lines 
    for (i, line) in lines.iter().enumerate() {
        for (index, character) in line.chars().enumerate() {
            // if were at the first line, push all indices of spaces
            if i == 0 {
                if character == ' ' {
                    indices.push(index);
                }
            } else { 
                // for all other lines, remove indices where character is not space
                if character != ' ' {
                    if indices.contains(&index) {
                        indices.remove(indices.iter().position(|i| i == &index).unwrap());
                    }
                }

            }
        }
    }
    indices
}


fn read_file(path: &str) -> Result<String, std::io::Error>{
    match read_to_string(path) {
        Ok(content) => Ok(content),
        Err(err) => {
            eprintln!("Error read file from path {}: {}", path, err);
            exit(1)
        }
    }
}