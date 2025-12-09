use std::{fs::read_to_string, process::exit};

// Keeps track of the level (y position) and position (x) of beam splitters
#[derive(Debug)]
struct BeamSplitter {
    level: usize,
    position: usize
}

// keeps track of beam paths on a specific level, keeping count on how many routes (number of paths) come to this position
#[derive(Debug)]
struct BeamPath {
    position: usize,
    num_routes: usize,
}

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let beam_splitters: Vec<BeamSplitter>;
    let start_pos: usize;
    (beam_splitters,start_pos) = get_beam_splitters_and_start_pos(&file_content);

    let n_beam_paths: usize;
    let n_beam_splits: usize;
    (n_beam_paths, n_beam_splits) = calculate_beam_paths(&beam_splitters, &start_pos);
    println!("Number of beam splits {}", n_beam_splits);
    println!("Number of beam paths {}", n_beam_paths);

}

fn calculate_beam_paths(beam_splitters: &Vec<BeamSplitter>, start_pos: &usize) -> (usize, usize){
    let mut current_beam_paths: Vec<BeamPath> = Vec::new();
    let mut nr_beam_splits: usize = 0;
    // Set max level down
    let max_level: usize = beam_splitters
        .iter()
        .max_by_key(|splitter| splitter.level)
        .unwrap()
        .level;
    // Add initial beam path
    current_beam_paths.push(BeamPath { position: *start_pos, num_routes: 1 });
    for level in 0..=max_level {
        // Beam splitters on this level
        let beam_splitters_on_level: Vec<&BeamSplitter> = beam_splitters
            .iter()
            .filter(|splitter| splitter.level == level)
            .collect();
        // and their locations
        let splitter_locations: Vec<usize> = beam_splitters_on_level
            .iter()
            .map(|splitter| splitter.position)
            .collect();
        // locations of beam paths on the same level
        let beam_locations: Vec<usize> = current_beam_paths
            .iter()
            .map(|path| path.position)
            .collect();
        // loop over all splitter locations and check if their present in the beam locations -> a beam has reached a splitter
        for location in splitter_locations{
            if beam_locations.contains(&location){
                nr_beam_splits += 1; // keep track of beam splits

                // Find the beam path in the list that reached current location
                // calculate positions to the left and the right
                // check if a BeamPath object already exists with that location
                // - if so, add this ones 'num_path' to that one to keep track of the number of routes to that position
                // - else. create new BeamPath object with current one num_path to keep track of the routes to this position
                let index = current_beam_paths
                    .iter()
                    .map(|path| path.position)
                    .position(|beam_position| beam_position == location)
                    .unwrap();
                let old_position = current_beam_paths.remove(index);
                let left_pos = old_position.position - 1;
                let right_pos = old_position.position + 1;
                // check if position to the left already in the list
                if let Some(beam_path) = current_beam_paths
                    .iter_mut()
                    .find(|beam_path| beam_path.position == left_pos){
                        beam_path.num_routes += old_position.num_routes // add number of routes to that position
                    }
                else {
                    current_beam_paths.push(BeamPath { 
                        position: left_pos, 
                        num_routes: old_position.num_routes 
                    }) //otherwise create new BeamPath with old number of routes 
                }
                // check if position to the right already in the list
                if let Some(beam_path) = current_beam_paths
                    .iter_mut()
                    .find(|beam_path| beam_path.position == right_pos){
                        beam_path.num_routes += old_position.num_routes // add number of routes to that position
                    }
                else {
                    current_beam_paths.push(BeamPath { 
                        position: right_pos, 
                        num_routes: old_position.num_routes 
                    }) // otherwise create new BeamPath with old number of routes
                }
            }
        }
        println!("Level {}:{}", level, max_level)
    }
    let n_beam_paths = current_beam_paths
        .iter()
        .map(|beam_path| beam_path.num_routes)
        .sum(); //calculate total number of beam paths
    (n_beam_paths, nr_beam_splits)
}

fn get_beam_splitters_and_start_pos(file_contents: &str) -> (Vec<BeamSplitter>, usize) {
    let mut beam_splitters: Vec<BeamSplitter> = Vec::new();
    let mut start_pos: usize = 0;
    // loop over all lines
    for (level, line) in file_contents.split('\n').enumerate(){
        // if character = ^, add beamSplitter, set start_pos as well
        for (position, char) in line.chars().enumerate(){
            if char == '^' {
                beam_splitters.push(BeamSplitter { 
                    level, 
                    position 
                })
            }
            if char == 'S' {
                start_pos = position
            }
        }
    }
    (beam_splitters, start_pos)
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