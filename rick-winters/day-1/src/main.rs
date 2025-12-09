use std::{fs::read_to_string, process::exit};

#[derive(Debug)]
enum Direction {
    Left,
    Right,
}

#[derive(Debug)]
struct Instruction {
    direction: Direction,
    amount: isize
}

struct Position {
    number: isize,
    n_at_zero: isize,
    n_clicked_to_zero: isize,
}

impl Position {
    fn add_number(&mut self, mut number: isize, direction: &Direction) {
        println!("Current number: {}, adding {} {:?}", self.number, number, direction);
        if number.abs() > 100 {
            println!("debug here")
        }
        while number > 0 {
            match direction {
                Direction::Left => {
                    self.number -= 1;
                }
                Direction::Right => {
                    self.number += 1;
                }
            }
            if self.number < 0 {
                self.number += 100;
            }
            if self.number > 99 {
                self.number -= 100;
            }
            if self.number == 0 {
                self.n_clicked_to_zero += 1;
            }
            number -= 1;
        }
        if self.number == 0 {
            self.n_at_zero += 1;
        }
        // println!{"n_at_zero: {}", self.n_at_zero};
    }
}

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let instructions: Vec<Instruction> = get_instructions(&file_content);
    // println!("Hello, world! {:#?}", instructions);
    let mut position: Position = Position { 
        number: 50, 
        n_at_zero: 0,
        n_clicked_to_zero: 0
    };
    for instruction in instructions.iter() {
        position.add_number(
            instruction.amount, 
            &instruction.direction);
    }
    println!("Number of stops at zero: {}", position.n_at_zero);
    println!("Number of times crossed zero: {}", position.n_clicked_to_zero);
}

fn get_instructions(file_content: &str) -> Vec<Instruction> {
    let mut instructions: Vec<Instruction> = Vec::new();
    let text_lines: Vec<&str> = file_content.split('\n').collect();
    for line in text_lines {
        let direction: Direction;
        let amount: isize;
        if line.starts_with('L') {
            direction = Direction::Left;
        } else {
            direction = Direction::Right;
        }
        amount = line[1..].parse::<isize>().unwrap();
        instructions.push(Instruction { direction, amount });
    }
    instructions
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