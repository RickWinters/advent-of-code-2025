use std::{fs::read_to_string, process::exit};
use num_bigint::{BigUint, BigInt};
use num_traits::One;

#[derive(Debug)]
struct Shape {
    width: usize,
    height: usize,
    rotated_none: u128,
    rotated_once: u128,
    rotated_twice: u128,
    rotated_thrice: u128,
}

#[derive(Debug)]
struct Region {
    id: usize,
    width: usize,
    height: usize,
    occupied: u128,
    required_shapes: Vec<usize>,
}

fn main() {
    let input_path: &str = "example_input.txt";
    // let input_path: &str = "example_part_b.txt";
    // let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let shapes: Vec<Shape>;
    let mut regions: Vec<Region>;
    (shapes, regions) = get_present_shapes_and_regions(&file_content);

    // println!("Hello, world!");
    // println!("Shapes: {:?}", shapes);
    // println!("Regions: {:?}", regions);

    let mut total_fitted_regions= 0;
    // let region = &mut regions[1];
    // if fill_region(region, &shapes){
    //     total_fitted_regions += 1;
    // };
    for region in regions.iter_mut() {
        if fill_region(region, &shapes){
            total_fitted_regions += 1;
            println!("Fitted region \n");
            print_region(&region.occupied, region.width, region.height);
        };
    }
    println!("Total fitted regions: {}", total_fitted_regions);
}

fn fill_region(region: &mut Region, shapes: &Vec<Shape>) -> bool {
    let mut shapes_required: Vec<&Shape> = Vec::new();
    for (id, _) in region.required_shapes.iter().enumerate() {
        for _ in 0..region.required_shapes[id] {
            shapes_required.push(&shapes[id]);
        }
    }
    return try_placements(region, &shapes_required, region.id);
}


fn transform_shape_vector_for_region(shape_mask: u128, region: &Region, shape: &Shape) -> u128 {
    let mut result = 0;
    let shape_bits = shape.width * shape.height;
    let padding = region.width - shape.width;
    
    for i in 0..shape_bits {
        if (shape_mask & (1 << i)) != 0 {
            let new_pos = i + (i / shape.width) * padding;
            result |= 1 << new_pos;
        }
    }
    result
}

fn get_shape_variants_for_region(shape: &Shape, region: &Region) -> Vec<u128> {
    let mut variants: Vec<u128> = Vec::new();

    variants.push(transform_shape_vector_for_region(shape.rotated_none, region, shape));
    variants.push(transform_shape_vector_for_region(shape.rotated_once, region, shape));
    variants.push(transform_shape_vector_for_region(shape.rotated_twice, region, shape));
    variants.push(transform_shape_vector_for_region(shape.rotated_thrice, region, shape));

    return variants;
}

fn try_placements(region: &mut Region, shapes_required: &Vec<&Shape>, region_id: usize) -> bool{
    if shapes_required.is_empty() {
        return true;
    }
    let shape = shapes_required[0];
    let variants = get_shape_variants_for_region(shape, region);
    let max_index = (region.width - shape.width) + (region.height - shape.height) * region.width;
    for variant in variants {
        for index in 0..=max_index {
            if index % region.width > region.width - shape.width {
                // println!("Skipping index {} due to width overflow", index);
                continue;
            }
            if check_shape_fits(&region.occupied, &variant, index, region) {
                place_shape(&mut region.occupied, &variant, index);
                // println!("--------------");
                // print_region(&region.occupied, region.width, region.height);
                // println!("--------------");
                if try_placements(region, &shapes_required[1..].to_vec(), region_id) {
                    return true;
                }
                remove_shape(&mut region.occupied, &variant, index);
            }
        }
    }
    return false;

}

fn print_shape(shape: u128, width: usize, height: usize) {
    for i in 0..(width * height) {
        if i % width == 0 && i != 0 {
            println!();
        }
        if (shape & (1 << i)) != 0 {
            print!("#");
        } else {
            print!(".");
        }
    }
    println!();
}

fn print_region(region: &u128, region_width: usize, region_height: usize) {
    for i in 0..(region_width * region_height) {
        if i % region_width == 0 && i != 0 {
            // println!("finish line at index {}", i);
            println!();
        }
        if (region & (1 << i)) != 0 {
            print!("#");
        } else {
            print!(".");
        }
    }
    println!();
}

fn place_shape(region: &mut u128, shape: &u128, index: usize) {
    *region |= shape << index;
}

fn remove_shape(region: &mut u128, shape: &u128, index: usize) {
    *region |= shape << index;
}

fn check_shape_fits(region_occupied: &u128, shape: &u128, index: usize, region: &Region) -> bool {
    // println!("Current region occupied:");
    // print_region(region_occupied, region.width, region.height); // Assuming region size is 6x6 for display
    // println!("Shape to fit:");
    // print_region(shape, region.width, region.height); // Assuming shape size is 3x3 for display
    let max_start = 128 - shape.trailing_zeros() as usize;
    if index >= max_start {
        return false;
    }

    let shifted_shape = *shape << index;
    // println!("Shifted shape by {}:", index);
    // print_region(&shifted_shape, region.width, region.height); // Assuming shape size is 3x3 for display
    let fits = (region_occupied & shifted_shape) == 0;
    // println!(" Fits: {}", fits);
    return fits;
}

fn rotate_mask(mask: u128) -> u128 {
    // original     new 
    // 0 1 2       6 3 0
    // 3 4 5  -->  7 4 1
    // 6 7 8       8 5 2
    let mut rotated_mask: u128 = 0;
    rotated_mask |= ((mask >> 0) & 1) << 6;
    rotated_mask |= ((mask >> 1) & 1) << 3;
    rotated_mask |= ((mask >> 2) & 1) << 0;
    rotated_mask |= ((mask >> 3) & 1) << 7;
    rotated_mask |= ((mask >> 4) & 1) << 4;
    rotated_mask |= ((mask >> 5) & 1) << 1;
    rotated_mask |= ((mask >> 6) & 1) << 8;
    rotated_mask |= ((mask >> 7) & 1) << 5;
    rotated_mask |= ((mask >> 8) & 1) << 2;
    rotated_mask
}

fn get_present_shapes_and_regions(file_content: &str) -> (Vec<Shape>, Vec<Region>) {
    let mut shapes: Vec<Shape> = Vec::new();
    let mut regions: Vec<Region> = Vec::new();

    let blocks: Vec<&str> = file_content.split("\n\n").collect();
    let blocks_len = blocks.len();
    let shape_lines = &blocks[0..&blocks_len-1];
    let region_lines = &blocks[blocks_len-1];

    for shape_line in shape_lines {
        let lines: Vec<&str> = shape_line.split('\n').collect();
        // let id_line = lines[0];
        // let id = id_line.strip_suffix(':').unwrap().parse::<usize>().unwrap();
        let mut mask: u128 = 0;
        let mut pos = 0;
        let height = lines.len() - 1;
        let width = lines[1].len();
        for line in &lines[1..] {
            for ch in line.chars() {
                match ch {
                    '#' => mask |= 1 << pos,
                    '.' => {},
                    _ => {}
                }
                pos += 1;
            }
        }
        let rotated_once: u128 = rotate_mask(mask); // Placeholder for actual rotation logic
        let rotated_twice: u128 = rotate_mask(rotated_once);
        let rotated_thrice: u128 = rotate_mask(rotated_twice);
        
        shapes.push(Shape {
            width: width,
            height: height,
            rotated_none: mask,
            rotated_once,
            rotated_twice,
            rotated_thrice,
        });
    }

    for (id, region_line) in region_lines.lines().enumerate() {
        let line_split: Vec<&str> = region_line.split(':').collect();
        let size_part = line_split[0];
        let required_shapes_part = line_split[1].trim();
        let size_split: Vec<&str> = size_part.split('x').collect();
        let size_x = size_split[0].parse::<usize>().unwrap();
        let size_y = size_split[1].parse::<usize>().unwrap();
        let occupied: u128 = 0;
        let required_shapes: Vec<usize> = required_shapes_part
            .split(' ')
            .map(|s| s.trim().parse::<usize>().unwrap())
            .collect();
        regions.push(Region {
            id,
            width: size_x,
            height: size_y,
            occupied,
            required_shapes,
        });
    }

    (shapes, regions)
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