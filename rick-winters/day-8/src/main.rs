use std::{fs::read_to_string, process::exit};

#[derive(Debug)]
struct JunctionBox{
    id: usize,
    x: usize,
    y: usize,
    z: usize,
}

#[derive(Debug)]
struct Circuit{
    junction_boxes: Vec<usize>, // IDs of junction boxes in the circuit
    length: usize, // number of junction boxes in the circuit
}

#[derive(Debug)]
struct JunctionBoxDistance{
    from: usize, // ID of the junction box the connection starts from
    to: usize, // Id of the junction box the connection goes to
    distance: f64
}

// The below implementations allow to sort JunctionBoxDistance by distance
// Implement ordering traits for JunctionBoxDistance based on distance
impl PartialOrd for JunctionBoxDistance {
    fn partial_cmp(&self, other: &Self) -> Option<std::cmp::Ordering> {
        self.distance.partial_cmp(&other.distance)
    }
}

// Implement Ord and Eq for JunctionBoxDistance comparing instances by the distance field
impl Ord for JunctionBoxDistance {
    fn cmp(&self, other: &Self) -> std::cmp::Ordering {
        self.distance.partial_cmp(&other.distance).unwrap()
    }
}

// Implement PartialEq for JunctionBoxDistance comparing instances by the distance field
impl PartialEq for JunctionBoxDistance {
    fn eq(&self, other: &Self) -> bool {
        self.distance == other.distance
    }
}

impl Eq for JunctionBoxDistance {}


fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    // Get junction boxes from file and number of connections required
    let junction_boxes: Vec<JunctionBox>;
    let use_n_connections: usize;
    (junction_boxes, use_n_connections) = get_junction_boxes(&file_content);

    // calculate distances between all junction boxes
    let mut junction_box_distances: Vec<JunctionBoxDistance> = get_junction_box_distances(&junction_boxes);
    junction_box_distances.sort(); //sort the array by distance field ascending --> also shows the connections

    // Calculate circuits from the junction box distances, using only the required number of connections
    let mut circuits: Vec<Circuit>;
    let last_two_connected_ids: (usize, usize);
    (circuits, _) = calculate_circuits(&junction_box_distances, use_n_connections, junction_boxes.len());
    circuits.sort_by_key(|c| std::cmp::Reverse(c.length)); // sort circuits by length descending
    
    // sum the number of junction boxes in the largest three circuits
    let top_three_circuits = &circuits[..3];
    let top_three_lengths: Vec<usize> = top_three_circuits.iter().map(|c| c.length).collect();
    let top_three_product: usize = top_three_lengths.iter().product();
    // println!("Top three circuits: {:?}", top_three_circuits);
    println!("Top three circuit lengths: {:?}, product: {}", top_three_lengths, top_three_product);

    // relcalculate circuits using all connections to find the last two connected junction boxes that connect all junction boxes
    (_, last_two_connected_ids)  = calculate_circuits(&junction_box_distances, junction_box_distances.len(), junction_boxes.len());
    let junction_box_one = &junction_boxes[last_two_connected_ids.0];
    let junction_box_two = &junction_boxes[last_two_connected_ids.1];
    let x_pos_product = junction_box_one.x * junction_box_two.x;
    println!("Product of x position of the last two connected junction boxes ({:?} and {:?}): {}", junction_box_one, junction_box_two, x_pos_product);
}

fn calculate_circuits(junction_box_distances: &Vec<JunctionBoxDistance>, use_n_connections: usize, n_junction_boxes: usize) -> (Vec<Circuit>, (usize, usize)) {
    let mut circuits: Vec<Circuit> = Vec::new();
    let mut last_two_connected_ids: (usize, usize) = (usize::MAX, usize::MAX);
    let junction_box_distances_limited = &junction_box_distances[0..use_n_connections];
    let mut from_in_circuit_index: usize;
    let mut to_in_circuit_index: usize;
    // Iterate over the junction box distances and build circuits
    for jbd in junction_box_distances_limited{
        // Get index of circuits containing the from and to junction boxes
        from_in_circuit_index = circuits.iter().position(|c| c.junction_boxes.contains(&jbd.from)).unwrap_or(usize::MAX);
        to_in_circuit_index = circuits.iter().position(|c| c.junction_boxes.contains(&jbd.to)).unwrap_or(usize::MAX);

        // if both junction boxes are already in the same circuit, skip
        if from_in_circuit_index < usize::MAX && to_in_circuit_index == from_in_circuit_index {
            // println!(" -> Both junction boxes already in the same circuit, skipping");
            // continue;
        }
        // if both junction boxes are in different circuits, merge the circuits
        else if from_in_circuit_index < usize::MAX && to_in_circuit_index < usize::MAX {
            let mut other_junction_boxes: Vec<usize> = circuits[to_in_circuit_index].junction_boxes.clone();
            circuits[from_in_circuit_index].junction_boxes.append(&mut other_junction_boxes);
            circuits[from_in_circuit_index].length += circuits[to_in_circuit_index].length;
            circuits.remove(to_in_circuit_index);
        }
        // if from junction box is in a circuit, add to junction box to that circuit
        else if from_in_circuit_index < usize::MAX {
            circuits[from_in_circuit_index].junction_boxes.push(jbd.to);
            circuits[from_in_circuit_index].length += 1;
        }
        // if to junction box is in a circuit, add from junction box to that circuit
        else if to_in_circuit_index < usize::MAX {
            circuits[to_in_circuit_index].junction_boxes.push(jbd.from);
            circuits[to_in_circuit_index].length += 1;
        }
        // if neither junction box is in a circuit, create a new circuit
        else {
            circuits.push(Circuit {
                junction_boxes: vec![jbd.from, jbd.to],
                length: 2
            });
        } 
        // If there is only one circuit and it contains all junction boxes, we can stop here
        if circuits[0].length == n_junction_boxes && circuits.len() == 1 {
            last_two_connected_ids = (jbd.from, jbd.to);
            break;
        }

    }
    (circuits, last_two_connected_ids)
}

fn get_junction_box_distances(junction_boxes: &Vec<JunctionBox>) -> Vec<JunctionBoxDistance> {
    let mut junction_box_distances: Vec<JunctionBoxDistance> = Vec::new();
    let mut already_calculated: Vec<Vec<bool>> = Vec::new();
    // Initialize already_calculated with empty vectors for each junction box
    for i in 0..junction_boxes.len() {
        already_calculated.push(Vec::new());
        for _ in 0..junction_boxes.len() {
            already_calculated[i].push(false);
        }
    }
    for jb in junction_boxes {
        // println!("Calculating distances for junction box {} at position ({}, {}, {})\n", jb.id, jb.x, jb.y, jb.z);
        for other_jb in junction_boxes {
            if jb.id == other_jb.id {
                continue;
            }
            if already_calculated[other_jb.id][jb.id] {
                // println!("Skipping already calculated distance between junction box {} and {}", jb.id, other_jb.id);
                continue;
            }
            already_calculated[jb.id][other_jb.id] = true;
            let distance: f64 = 
                ((jb.x as f64 - other_jb.x as f64).powf(2.0) +
                    (jb.y as f64 - other_jb.y as f64).powf(2.0) +
                    (jb.z as f64 - other_jb.z as f64).powf(2.0)).sqrt();
            junction_box_distances.push(
                JunctionBoxDistance {
                    from: jb.id,
                    to: other_jb.id,
                    distance: distance
                }
            )
        }
    }
    junction_box_distances
}

fn get_junction_boxes(file_content: &str) -> (Vec<JunctionBox>, usize) {
    let mut junction_boxes: Vec<JunctionBox> = Vec::new();
    let mut use_n_connections: usize = 0;
    for (id,line) in file_content.lines().enumerate() {
        if id == file_content.lines().count() - 1 { // if on last line, set use_n_connections
            use_n_connections = line.trim().parse::<usize>().unwrap();
            continue;
        }
        // read coords
        let coords: Vec<usize> = line
            .split(',')
            .map(|s| s.trim().parse::<usize>().unwrap())
            .collect();
        // set JunctionBox
        if coords.len() == 3 {
            junction_boxes.push(JunctionBox {
                id: id,
                x: coords[0],
                y: coords[1],
                z: coords[2],
            });
        }
    }
    (junction_boxes, use_n_connections)
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