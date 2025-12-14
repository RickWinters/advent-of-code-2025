use std::{fs::read_to_string, process::exit, vec};

#[derive(Debug)]
struct Machine {
    current_lights: Vec<bool>,
    lights: Vec<bool>,
    buttons: Vec<Vec<usize>>,
    current_joltage: Vec<usize>,
    joltage: Vec<usize>,
}

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let mut machines: Vec<Machine> = get_machines_from_input(&file_content);
    let n_machines = file_content.split('\n').count();

    let mut total_button_presses_required_to_turn_on_machines: usize = 0;
    for (index, machine) in &mut machines.iter_mut().enumerate() {
        print!{"turning on machine {}/{}...", index, n_machines};
        let n_presses_required = turn_on_machine(machine);
        if n_presses_required == usize::MAX {
            println!("No solution found for machine: \n{:?}", machine);
            return
        } else {
            println!(" Done! Required {} button presses.", n_presses_required);
            total_button_presses_required_to_turn_on_machines += n_presses_required;
        }
    }
    println!("Total button presses required to turn on all machines: {}", total_button_presses_required_to_turn_on_machines);

    let mut total_button_presses_required_to_set_joltage: usize = 0;
    for (index, machine) in & mut machines.iter_mut().enumerate() {
        println!("setting joltage correctly for machine {}/{}...", index+1, n_machines);
        let n_presses_required = set_joltage_correctly(machine);
        if n_presses_required == usize::MAX {
            println!("No solution found for machine: \n{:?}", machine);
            return
        } else {
            println!(" Done! Required {} button presses.", n_presses_required);
            total_button_presses_required_to_set_joltage += n_presses_required;
        }
    }
    println!("Total button presses required to set joltage correctly for all machines: {}", total_button_presses_required_to_set_joltage);
}

fn set_joltage_correctly(machine: &mut Machine) -> usize {
    let mut button_sequence: Vec<usize>;
    let max_joltage_required = machine.joltage.iter().max().unwrap();
    for n_button_presses in *max_joltage_required..=100 {
        println!("Testing {} buttons presses on {} buttons", n_button_presses, machine.buttons.len());
        // reset button sequence
        button_sequence = vec![0; 1];
        // calculate all the button press permutations for n_button_presses
        while get_next_butten_press_sequence(& mut button_sequence, n_button_presses, machine.buttons.len()){
            // println!("    testing sequence: {:?}", button_sequence);
            // reset lights_changed
            machine.current_joltage = vec![0; machine.joltage.len()];

            if machine_button_presses(machine, &button_sequence, false) {
                return n_button_presses
            }
        }
    }
    return usize::MAX
}

fn turn_on_machine(machine: &mut Machine) -> usize {
    let mut button_sequence: Vec<usize>;
    for n_button_presses in 1..=machine.buttons.len() {
        // reset button sequence
        button_sequence = vec![0; 0];
        // calculate all the button press permutations for n_button_presses
        while get_next_butten_press_sequence(& mut button_sequence, n_button_presses, machine.buttons.len()){
            // reset lights_changed
            machine.current_lights = vec![false; machine.lights.len()];

            if machine_button_presses(machine, &button_sequence, true) {
                return n_button_presses
            }
        }
    }
    return usize::MAX
}

fn get_next_butten_press_sequence(current_sequence: &mut Vec<usize>, n_button_presses: usize, n_buttons: usize) -> bool {
    let final_sequence = vec![n_buttons - 1; n_button_presses];
    if final_sequence.eq(current_sequence) {
        return false;
    }
    if current_sequence.len() == n_button_presses {
        let last_button_press = current_sequence.pop().unwrap();
        if last_button_press + 1 < n_buttons {
            current_sequence.push(last_button_press + 1);
            return true;
        } else {
            get_next_butten_press_sequence(current_sequence, n_button_presses - 1, n_buttons);
            current_sequence.push(0);
            return true;
        }   
    }
    while current_sequence.len() < n_button_presses {
        current_sequence.push(0);
    }
    return true;
}

fn machine_button_presses(machine: &mut Machine, button_sequence: &Vec<usize>, test_on_lights: bool) -> bool {
    for button_index in button_sequence.iter(){
        let button = &machine.buttons[*button_index];
        for index in button {
            machine.current_lights[*index] = !machine.current_lights[*index];
            machine.current_joltage[*index] += 1;
            if !test_on_lights && machine.current_joltage[*index] > machine.joltage[*index]{
                // println!("Skipped sequnce prematurely");
                return false
            }
        }
    }
    if test_on_lights && machine.lights.eq(&machine.current_lights) {
        return true;
    }
    else if machine.joltage.eq(&machine.current_joltage) {
        return true;
    }
    return false;
}
// Simulate a button press on a machine and button index, mutates the machine state and returns if the lights match the target state
fn machine_button_press(machine: &mut Machine, butten_index: usize, test_on_lights: bool) -> bool {
    let button = &machine.buttons[butten_index];
    for index in button {
        machine.current_lights[*index] = !machine.current_lights[*index];
        machine.current_joltage[*index] += 1;
    }
    if test_on_lights && machine.lights.eq(&machine.current_lights) {
        return true;
    }
    else if machine.joltage.eq(&machine.current_joltage) {
        return true;
    }
    return false;
}

fn get_machines_from_input(input: &str) -> Vec<Machine> {
    let mut machines: Vec<Machine> = Vec::new();

    for line in input.split("\n") {
        let mut lights: Vec<bool> = Vec::new();
        let mut buttons: Vec<Vec<usize>> = Vec::new();
        let mut joltage: Vec<usize> = Vec::new();

        let lights_to_index = line.chars().position(|c| c == ']').unwrap();
        let joltage_from_index = line.chars().position(|c| c == '{').unwrap();

        let lights_str = &line[1..lights_to_index];
        let buttons_str = &line[(lights_to_index + 2)..(joltage_from_index - 1)];
        let joltage_str = &line[(joltage_from_index + 1)..(line.len() - 1)];

        for c in lights_str.chars() {
            match c {
                '.' => lights.push(false),
                '#' => lights.push(true),
                _ => {}
            }
        }
        
        let max_u32: usize = u32::MAX as usize;
        for button_group in buttons_str.split(" ") {
            let mut button_indices: Vec<usize> = Vec::new();
            for c in button_group.chars() {
                let index = c.to_digit(10).unwrap_or(u32::MAX) as usize;
                if index < max_u32 {
                    button_indices.push(index);
                }
            }
            buttons.push(button_indices);
        }
        for joltage_substring in joltage_str.split(",") {
            if let Some(j) = joltage_substring.trim().parse::<usize>().ok() {
                joltage.push(j);
            }
        }

        machines.push(Machine {
            current_lights: vec![false; lights.len()],
            lights: lights,
            buttons: buttons,
            current_joltage: vec![0; joltage.len()],
            joltage: joltage,
        });
        }
    machines
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