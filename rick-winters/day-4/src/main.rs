use rayon::prelude::*;
use std::{fs::read_to_string, process::exit, usize};

#[derive(Debug, Clone)]
struct PaperRoll{
    x: usize,
    y: usize,
    n_neigbours: usize,
    accessable: bool,
}

impl PaperRoll {
    fn add_neighbour(&mut self) {
        self.n_neigbours += 1
    }
}

struct FieldSize{
    width: usize,
    height: usize,
}

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let field_size: FieldSize = get_field_size(&file_content);
    let mut paper_rolls: Vec<PaperRoll> = get_paper_rolls(&file_content);
    find_n_neighbours(&mut paper_rolls, &field_size);
    // println!("Paper rolls {:#?}", paper_rolls)
    let mut number_accessable = count_number_accessable(&mut paper_rolls, &field_size);
    let mut total_accessable = number_accessable;
    println!("Part A: Number of accessable paper rolls: {}", number_accessable);
    while number_accessable > 0 {
        remove_paper_rolls(&mut paper_rolls);  
        println!("Remaining paper rolls: {}, removed {}, total removed {}", paper_rolls.len(), number_accessable, total_accessable);
        find_n_neighbours(&mut paper_rolls, &field_size);
        number_accessable = count_number_accessable(&mut paper_rolls, &field_size);
        total_accessable += number_accessable;
    }
    println!("Part B: Total number of accessable paper rolls: {}", total_accessable);

}

fn remove_paper_rolls(paper_rolls: &mut Vec<PaperRoll>) {
    paper_rolls.retain(|roll| !roll.accessable);
    for paper_roll in paper_rolls.iter_mut() {
        paper_roll.n_neigbours = 0;
        paper_roll.accessable = false;
    }
}

fn count_number_accessable(paper_rolls: &mut Vec<PaperRoll>, field_size: &FieldSize) -> usize {
    let rolls_clone = paper_rolls.clone(); //clone to avoid multiple mutable borrows for memory safety
    let mut number_accessable: usize = 0;
    for y in 0..field_size.height {
        for x in 0..field_size.width {
            if let Some(j) = rolls_clone.iter().position(|roll| roll.x == x && roll.y == y) {
                if paper_rolls[j].n_neigbours <= 3 {
                    number_accessable += 1;
                    paper_rolls[j].accessable = true;
                }
            }
        }
        // println!();
    }
    number_accessable
}

fn find_n_neighbours(paper_rolls: &mut Vec<PaperRoll>, field_size: &FieldSize) {
    let rolls_clone = paper_rolls.clone();

    paper_rolls.par_iter_mut().for_each(|paper_roll| {
        let mut start_x: usize = 0;
        let mut start_y: usize = 0;
        if paper_roll.x > 0{
            start_x = (paper_roll.x - 1).max(0);
        }
        if paper_roll.y > 0{
            start_y = (paper_roll.y - 1).max(0);
        }
        let end_x: usize = (paper_roll.x + 1).min(field_size.width);
        let end_y: usize = (paper_roll.y + 1).min(field_size.height);

        for y in start_y..=end_y {
            for x in start_x..=end_x {
                if y == paper_roll.y && x == paper_roll.x {
                    continue;
                }
                if let Some(_) = rolls_clone.iter().position(|roll| roll.x == x && roll.y == y){
                    paper_roll.add_neighbour();
                }
            }
        }
    });
}

fn get_paper_rolls(file_content: &str) -> Vec<PaperRoll> {
    let mut paper_rolls: Vec<PaperRoll> = Vec::new();
    let lines: Vec<&str> = file_content.split('\n').collect();
    let mut y: usize = 0;
    let mut x: usize = 0;
    for line in lines {
        x = 0;
        for character in line.chars() {
            if character == '@' {
                let paper_roll: PaperRoll = PaperRoll {
                    x, 
                    y, 
                    n_neigbours: 0,
                    accessable: false
                };
                paper_rolls.push(paper_roll);
            }
            x += 1
        }
        y += 1
    }

    return paper_rolls
}

fn get_field_size(file_content: &str) -> FieldSize {
    let lines: Vec<&str> = file_content.split('\n').collect();
    let height: usize = lines.len();
    let width: usize = lines[0].chars().count();

    FieldSize { width, height }
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