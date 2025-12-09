use std::{fs::read_to_string, process::exit};

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };

    let battery_banks: Vec<Vec<u32>> = get_battery_banks(&file_content);

    let mut total_joltage_with_2: u32 = 0;
    for battery_bank in battery_banks.iter() {
        total_joltage_with_2 += get_max_joltage_with_2(battery_bank);
    }
    println!("Total joltage with 2: {}", total_joltage_with_2);

    let mut total_joltage_with_12: u128 = 0;
    for battery_bank in battery_banks.iter(){
        total_joltage_with_12 += get_max_joltage_with_k(battery_bank, 12);
    }
    println!("Total joltage with 12: {}", total_joltage_with_12)   
}

fn get_max_joltage_with_k(battery_bank: &Vec<u32>, k: usize) -> u128 {
    let mut joltage_bank: Vec<u32> = Vec::new();
    let mut total_index: usize = 0;
    for i in (0..k).rev() {
        let battery_bank_slize = &battery_bank[total_index..(&battery_bank.len()-i)];
        let max_joltage_single_battery = battery_bank_slize
            .iter()
            .max_by_key(|j| **j)
            .unwrap();
        let current_index = battery_bank_slize
            .iter()
            .position(|j| *j == *max_joltage_single_battery)
            .unwrap();
        total_index += current_index + 1; // dont take fount battery in next slice
        joltage_bank.push(*max_joltage_single_battery);
    }
    println!("For battery bank {:?}, found best batteries {:?}", battery_bank, joltage_bank);
    vector_to_number(joltage_bank)
}

fn vector_to_number(vector: Vec<u32>) -> u128{
    let mut total: u128 = 0;
    for number in vector{
        total *= 10;
        total += (number as u128);
    }
    total
}

fn get_max_joltage_with_2(battery_bank: &Vec<u32>) -> u32 {
    let default_value: u32 = u32::MIN;
    let mut left_joltage: u32 = 0;
    let mut right_joltage: u32 = 0;

    // find max value and index of
    let (index, max_value_1) = battery_bank
        .iter()
        .enumerate()
        .max_by_key(|(_, joltage)| **joltage)
        .map(|(index, value)| (index, value))
        .unwrap();

    // split left and right from index
    let left_slice = &battery_bank[..index];
    let right_slice = &battery_bank[index+1..];
    // find max values on either side, or return 0
    let max_left = left_slice.iter().max().unwrap_or(&default_value);
    let max_right = right_slice.iter().max().unwrap_or(&default_value);
    
    // combine to joltage value, either maxright first or max_value_1 first. 
    if max_left > &default_value {
        left_joltage = 10*max_left + max_value_1;
    }
    if max_right > &default_value {
        right_joltage = 10*max_value_1 + max_right;
    }
    // return highest joltage
    return left_joltage.max(right_joltage);
}

fn get_battery_banks(file_contents: &str) -> Vec<Vec<u32>> {
    let mut battery_banks: Vec<Vec<u32>> = Vec::new();
    for line in file_contents.lines() {
        let mut battery_bank: Vec<u32> = Vec::new();
        for character in line.chars(){
            let number: u32 = character.to_digit(10).unwrap();
            battery_bank.push(number);
        }
        battery_banks.push(battery_bank)
    }

    battery_banks
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