use std::{fs::read_to_string, process::exit};

#[derive(Debug, Clone)]
struct Device {
    name: String,
    to_string: String,
    id: usize,
    output_ids: Vec<usize>,
    is_start: bool,
    is_end: bool,
    path: Vec<usize>,
    visited_ids: Vec<usize>,
    ids_to_exit: Vec<usize>,
    n_routes_to_exit: usize,
    correct_path: bool,
}

fn main() {
    let input_path: &str = "example_input.txt";
    // let input_path: &str = "example_part_b.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };

    // let mut devices: Vec<Device> = get_devices(&file_content, "you");
    // let start_device_id = devices.iter_mut().find(|d| d.is_start).unwrap().id;
    // // println!("Starting from device id: {}", start_device_id);
    // let mut visited_devices: Vec<String> = Vec::new();
    // let mut n_routes: usize = 0;
    // let mut n_total_checks: usize = 0;
    // let mut route_has_fft = false;
    // let mut route_has_dac = false;
    // route_to_out(
    //     &mut devices,
    //     start_device_id, 
    //     &mut n_routes, 
    //     &mut n_total_checks, 
    //     &mut route_has_fft, 
    //     &mut route_has_dac, 
    //     false);
    // // println!("Total number of routes: {}", n_routes);
    // println!("start device {} has {} routes to exit", devices[start_device_id].name, devices[start_device_id].n_routes_to_exit);
    // println!("Total number of route checks: {}", n_total_checks);

    let mut n_routes = 0;
    let mut n_total_checks = 0;
    let mut devices: Vec<Device> = get_devices(&file_content, "svr");
    let start_device_id = devices.iter_mut().find(|d| d.is_start).unwrap().id;
    let mut route_has_fft = false;
    let mut route_has_dac = false;
    let fft_id = devices.iter().position(|d| d.name == "fft").unwrap();
    let dac_id = devices.iter().position(|d| d.name == "dac").unwrap();
    println!("fft id: {}, dac id: {}", fft_id, dac_id);
    route_to_out(
        &mut devices, 
        start_device_id, 
        &mut n_routes, 
        &mut n_total_checks,
        &mut route_has_fft,
        &mut route_has_dac,
        true);
    println!("start device {} has {} routes to exit (crosses both fft and dac)", devices[start_device_id].name, devices[start_device_id].n_routes_to_exit);
    println!("Total number of route checks: {}", n_total_checks);

    let mut total_correct_paths = 0;
    for device in devices.iter() {
        if device.correct_path {
            total_correct_paths += device.n_routes_to_exit;
        }
    }

    println!("total correct paths: {}", total_correct_paths);
}



fn route_to_out(devices: &mut Vec<Device>, 
                current_device_id: usize, 
                n_routes: &mut usize, 
                n_total_checks: &mut usize, 
                route_has_fft: &mut bool, 
                route_has_dac: &mut bool, 
                check_path_crosses_fft_and_dac: bool) -> bool {
    if devices[current_device_id].name == "fft" {
        println!("Found fft in path: {:?}", devices[current_device_id].path);
        *route_has_fft = true;
    }
    if devices[current_device_id].name == "dac" {
        println!("Found dac in path: {:?}", devices[current_device_id].path);
        *route_has_dac = true;
    }
    if *route_has_dac && *route_has_fft {
        println!("Route has both fft and dac: {:?}", devices[current_device_id].path);
    }
    if devices[current_device_id].is_end {
        if check_path_crosses_fft_and_dac {
            if (*route_has_fft && *route_has_dac) {
                devices[current_device_id].correct_path = true;
            }
        }
        devices[current_device_id].correct_path = true;
        *n_routes = *n_routes + 1;
        devices[current_device_id].n_routes_to_exit += 1;
        return true
    }
    for id in devices[current_device_id].output_ids.clone() {
        *n_total_checks += 1;
        if devices[id].n_routes_to_exit > 0{
            devices[current_device_id].n_routes_to_exit += devices[id].n_routes_to_exit;
            continue
        }
        // if devices[current_device_id].visited_ids.contains(&id) {
        //     continue
        // }
        devices[current_device_id].visited_ids.push(id);
        devices[current_device_id].path.push(id);
        devices[id].path = devices[current_device_id].path.clone();
        if route_to_out(
                devices, 
                id, 
                n_routes, 
                n_total_checks,
                route_has_fft,
                route_has_dac,
                check_path_crosses_fft_and_dac){
            devices[current_device_id].ids_to_exit.push(id);
            devices[current_device_id].n_routes_to_exit += devices[id].n_routes_to_exit;
            continue
        };
        devices[id].path.clear();
        devices[current_device_id].path.pop();
    }
    return true
}

fn get_devices(file_contents: &str, start_name: &str) -> Vec<Device> {
    let mut devices: Vec<Device> = Vec::new();
    let lines: Vec<&str> = file_contents.split('\n').collect();
    for (id, line) in lines.iter().enumerate() {
        let parts: Vec<&str> = line.split(':').collect();
        let name = parts[0].trim().to_string();
        let start = name.eq(start_name);
        let to_string = parts[1].trim();
        let device = Device {
            name: name,
            to_string: to_string.to_string(),
            id: id,
            output_ids: Vec::new(),
            is_start: start,
            is_end: to_string.contains("out"),
            path: Vec::new(),
            visited_ids: Vec::new(),
            ids_to_exit: Vec::new(),
            n_routes_to_exit: 0,
            correct_path: false,
        };
        devices.push(device);
    }
    let other_devices = devices.clone();
    for device in devices.iter_mut() {
        let output_strings: Vec<&str> = device.to_string.split(' ').collect();
        for output_str in output_strings {
            for other_device in other_devices.iter() {
                if output_str == other_device.name {
                    device.output_ids.push(other_device.id);
                }
            }
        }
    }
    devices
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