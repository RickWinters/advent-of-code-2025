use std::{fs::read_to_string, process::exit, time::{SystemTime, UNIX_EPOCH}};

#[derive(Debug)]
struct Range {
    start: usize,
    end: usize,
}

fn main() {    
    let start = SystemTime::now().duration_since(UNIX_EPOCH).unwrap();
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    // Parse ranges
    let ranges: Vec<Range> = get_ranges(&file_content);
    // analyse ranges and return vec<usize> of invalid IDs per range, append to all_invalid_ids
    let mut all_invalid_ids_repeat_halve: Vec<usize> = Vec::new();
    for range in ranges.iter() {
        all_invalid_ids_repeat_halve.extend(analyse_range_by_half_repeat(range));
    }
    // calculate total
    let mut total_repeat_halve: usize = 0;
    for invalid_id in all_invalid_ids_repeat_halve.iter() {
        total_repeat_halve += invalid_id;
    }
    let halve_end_time = SystemTime::now().duration_since(UNIX_EPOCH).unwrap();
    let time_taken_halve = halve_end_time.as_secs_f64() - start.as_secs_f64();

    println!("Total by halve repeat: {}, time taken: {}", total_repeat_halve, time_taken_halve);
    let mut all_invalid_ids_any_repeat: Vec<usize> = Vec::new();
    for range in ranges.iter() {
        all_invalid_ids_any_repeat.extend(analyse_range_by_any_repeat(range));
    }
    let mut total_repeat_any: usize = 0;
    for invalid_id in all_invalid_ids_any_repeat.iter() {
        total_repeat_any += invalid_id;
    }

    let end = SystemTime::now().duration_since(UNIX_EPOCH).unwrap();
    let time_taken = end.as_secs_f64() - halve_end_time.as_secs_f64();
    let time_total = end.as_secs_f64() - start.as_secs_f64();
    println!("Total by any repeat: {}, time taken {}", total_repeat_any, time_taken);
    println!("Total time taken: {:?}", time_total)
}

fn analyse_range_by_any_repeat(range: &Range) -> Vec<usize>{
    let numbers: Vec<usize> = (range.start..=range.end).collect();
    let mut invalid_ids: Vec<usize> = Vec::new();
    for number in numbers.iter() {
        let number_string: String = number.to_string();
        let number_length: usize = number_string.len();
        let mut current_length: usize = 1;
        while current_length < number_length  {
            if number_length % current_length != 0 {
                // pattern length does not exactly divide number length
                current_length += 1;
                continue;
            }
            // create pattern to search from 0 to current_length
            let pattern = &number_string[0..current_length];
            // create rest string from current_length to end
            let reststring = &number_string[current_length..number_length];
            // create an array of sub patterns from rest string matching the current length
            let mut rest_sub_patterns: Vec<&str> = Vec::new();
            let n_sub_patterns = reststring.len() / current_length;
            for i in 0..n_sub_patterns {
                let start_index = i * current_length;
                let end_index = start_index + current_length;
                let sub_pattern = &reststring[start_index..end_index];
                rest_sub_patterns.push(sub_pattern);
            }

            // if each sub pattern from the rest string matches the pattern to search, mark as invalid
            if rest_sub_patterns.iter().all(|&x| x == pattern) {
                // println!("Number: {}, Pattern: {}, Reststring: {}, Subpatterns: {:?}", number, pattern, reststring, rest_sub_patterns);
                invalid_ids.push(*number);
                break;
            }

            current_length += 1;
        }
    }
    invalid_ids
    // Placeholder for range analysis logic
}

fn analyse_range_by_half_repeat(range: &Range) -> Vec<usize>{
    let numbers: Vec<usize> = (range.start..=range.end).collect();
    let mut invalid_ids: Vec<usize> = Vec::new();
    for number in numbers.iter() {
        let number_string = number.to_string();
        let number_length = number_string.len();
        if number_length % 2 != 0 {
            continue;
        }
        let number_string_first_halve = &number_string[0..number_length / 2];
        let number_string_second_halve = &number_string[number_length / 2..number_length];
        if number_string_first_halve == number_string_second_halve {
            invalid_ids.push(*number);
        }
        // println!("Number: {}, First halve {}, Second Halve {}", number, number_string_first_halve, number_string_second_halve);
    }
    invalid_ids
    // Placeholder for range analysis logic
}

fn get_ranges(file_content: &str) -> Vec<Range> {
    let mut ranges: Vec<Range> = Vec::new();
    let text_lines: Vec<&str> = file_content.split(',').collect();
    for line in text_lines.iter() {
        let parts: Vec<&str> = line.split('-').collect();
        let start: usize = match parts[0].parse::<usize>() {
            Ok(num) => num,
            Err(e) => {
                eprintln!("Error parsing start of range: {}, Error: {:?}", parts[0], e);
                exit(1)
            }
        };
        let end: usize = match parts[1].parse::<usize>() {
            Ok(num) => num,
            Err(e) => {
                eprintln!("Error parsing end of range: {}, Error: {:?}", parts[1], e);
                exit(1)
            }
        };
        ranges.push(Range { start, end });
    }
    ranges
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