use std::{fs::read_to_string, process::exit};

#[derive(Debug, Clone)]
struct Tile {
    id: usize,
    x: usize,
    y: usize,
}

#[derive(Debug)]
struct TileCombination {
    area: usize
}

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let mut tiles: Vec<Tile> = get_tiles(&file_content);
    // print_tiles(&tiles);
    let mut tile_combinations: Vec<TileCombination> = get_tile_combinations_part_a(&tiles);
    tile_combinations.sort_by_key(|combination| std::cmp::Reverse(combination.area)); // sort descending by area

    let highest_area = &tile_combinations[0].area;
    println!{"Highest area: {}", highest_area};

    let green_tiles: Vec<Tile> = get_green_tiles_on_edge(&mut tiles);
    println!("Filled in edges with green tiles");


    let mut tile_bool_matrix: Vec<Vec<bool>> = Vec::new();
    let max_y = green_tiles.iter().map(|tile| tile.y).max().unwrap_or(0);
    let max_x = green_tiles.iter().map(|tile| tile.x).max().unwrap_or(0);
    tile_bool_matrix = vec![vec![false; max_x]; max_y];
    fill_green_tiles(&green_tiles, &tiles, &mut tile_bool_matrix);
    println!("Filled green tile matrix");
    let mut tile_combinations_on_green_edge: Vec<TileCombination> = get_tile_combinations_part_on_green(&tiles, &green_tiles);
    tile_combinations_on_green_edge.sort_by_key(|combination| std::cmp::Reverse(combination.area)); // sort descending by area

    let highest_area_part_b = &tile_combinations_on_green_edge[0].area;
    println!{"Highest area part B: {}", highest_area_part_b};
}

fn print_tiles(tiles: &Vec<Tile>) {
    let max_y = tiles.iter().map(|tile| tile.y).max().unwrap_or(0);
    let max_x = tiles.iter().map(|tile| tile.x).max().unwrap_or(0);
    for y in 0..=max_y {
        for x in 0..=max_x {
            if let Some(_) = tiles.iter().find(|tile| tile.x == x && tile.y == y) {
                print!("X");
            } else {
                print!(".");
            }
        }
        println!();
    }
}

fn get_missing_coord(tile_1: &Tile, tile_2: &Tile, tile_3: &Tile) -> (usize, usize) {
    // brute force find missing coord
    // by counting occurrences of x and y coords
    // the missing coord will be the one that occurs only once
    let mut missing_coord: (usize, usize) = (0,0);
    let mut x_coord_1: usize = 0;
    let mut x_coord_2: usize = 0;
    let mut y_coord_1: usize = 0;
    let mut y_coord_2: usize = 0;
    let mut x_coord_1_count: usize = 0;
    let mut y_coord_1_count: usize = 0;

    x_coord_1 = tile_1.x;
    y_coord_1 = tile_1.y;
    x_coord_1_count += 1;
    y_coord_1_count += 1;

    if tile_2.x == x_coord_1 {
        x_coord_1_count += 1;
    } else {
        x_coord_2 = tile_2.x;
    }
    if tile_2.y == y_coord_1 {
        y_coord_1_count += 1;
    } else {
        y_coord_2 = tile_2.y;
    }
    if tile_3.x == x_coord_1 {
        x_coord_1_count += 1;
    } else {
        x_coord_2 = tile_3.x;
    }
    if tile_3.y == y_coord_1 {
        y_coord_1_count += 1;
    } else {
        y_coord_2 = tile_3.y;
    }

    if x_coord_1_count == 1 {
        missing_coord.0 = x_coord_1;
    } else {
        missing_coord.0 = x_coord_2;
    }
    if y_coord_1_count == 1 {
        missing_coord.1 = y_coord_1;
    } else {
        missing_coord.1 = y_coord_2;
    }

    missing_coord
}

// If a square must be inside the 'green' area this means that atleast 3 corners of the square must be red tiles, 
// and the 4th corner must be a green tile on the edge between 2 red tiles.
// The puzzel sais atleast 2 red tiles, but to find the missing tile we need three red tiles to determine the position 
// of the 4th tile. 
fn get_tile_combinations_part_on_green(tiles: &Vec<Tile>, green_tiles: &Vec<Tile>) -> Vec<TileCombination> {
    let mut tile_combinations: Vec<TileCombination> = Vec::new();
    let green_tile_coords: Vec<(usize, usize)> = green_tiles.iter().map(|tile| (tile.x, tile.y)).collect();

    for i in 0..tiles.len(){
        // get 3 tiles, wrap around the indices 
        let mut index_2 = i + 1;
        let mut index_3 = i + 2;
        if index_2 > tiles.len()-1 {
            index_2 -= tiles.len();
        }
        if index_3 > tiles.len()-1 {
            index_3 -= tiles.len();
        }
        let tile_1: &Tile = &tiles[i];
        let tile_2: &Tile = &tiles[index_2];
        let tile_3: &Tile = &tiles[index_3];
        // missing_coords is always the opposite corner of the rectangle formed by tile_1, tile_2 and tile_3
        let missing_coords: (usize, usize) = get_missing_coord(tile_1, tile_2, tile_3);

        // check if missing coords is in the list of green tiles
        if green_tile_coords.contains(&missing_coords) {
                // if so, calculate area and store combination
                let x_diff: usize = tile_1.x.abs_diff(tile_3.x)+1; // +1 to include both edges
                let y_diff: usize = tile_1.y.abs_diff(tile_3.y)+1; // +1 to include both edges
                let area: usize = x_diff * y_diff;
                tile_combinations.push(TileCombination {
                    area: area
                });
            }
    }
    tile_combinations
}


fn fill_green_tiles(green_tiles: &Vec<Tile>, red_tiles: &Vec<Tile>, bool_matrix: &mut Vec<Vec<bool>>) {
    let mut coords: Vec<(usize, usize)> = Vec::new();
    for tile in green_tiles.iter() {
        coords.push((tile.x, tile.y));
    }
    for tile in red_tiles.iter() {
        coords.push((tile.x, tile.y));
    }
    let max_y = coords.iter().map(|coord| coord.1).max().unwrap_or(0);
    for y in 0..=max_y {
        let min_x = green_tiles
            .iter()
            .filter(|tile| tile.y == y)
            .map(|tile| tile.x)
            .min()
            .unwrap_or(0);
        let max_x: usize = green_tiles
            .iter()
            .filter(|tile| tile.y == y)
            .map(|tile| tile.x)
            .max()
            .unwrap_or(0);
        println!("Filling row y={}/{} from x={} to x={}", y, max_y, min_x, max_x);
        for x in min_x..=max_x {
            bool_matrix[y][x] = true;
        }
    }
}

// Get all green tiles on the edges between red tiles
fn get_green_tiles_on_edge(tiles: &Vec<Tile>) -> Vec<Tile> {
    let mut green_tiles: Vec<Tile> = Vec::new();
    for (id, i) in (0..tiles.len()).enumerate() {
        let tile = &tiles[i];
        let mut index2 = i + 1;
        if index2 > tiles.len()-1 {
            index2 -= tiles.len();
        }
        let other_tile = &tiles[index2];
        let x_abs_diff: usize = tile.x.abs_diff(other_tile.x);
        let y_abs_diff: usize = tile.y.abs_diff(other_tile.y);
        println!("Filling edges between tile {:?} and tile {:?}", tile, other_tile);
        if x_abs_diff > 0 {
            let min_x = tile.x.min(other_tile.x);
            let max_x = tile.x.max(other_tile.x);
            for x in min_x..=max_x {
                green_tiles.push(Tile {
                    id: id,
                    x: x,
                    y: tile.y,
                });
            }
        }
        if y_abs_diff > 0 {
            let min_y: usize = tile.y.min(other_tile.y);
            let max_y: usize = tile.y.max(other_tile.y);
            for y in min_y..=max_y {
                green_tiles.push(Tile {
                    id: id,
                    x: tile.x,
                    y: y,
                });
            }
        }
    }
    green_tiles
}

fn get_tile_combinations_part_a(tiles: &Vec<Tile>) -> Vec<TileCombination> {
    let mut tile_combinations: Vec<TileCombination> = Vec::new();
    let mut already_calculated: Vec<Vec<bool>> = Vec::new();
    // create matrix of already combined tiles
    for i in 0..tiles.len() {
        already_calculated.push(Vec::new());
        for _ in 0..tiles.len() {
            already_calculated[i].push(false);
        }
    }

    for tile in tiles {
        for other_tile in tiles {
            if tile.id == other_tile.id {
                continue;
            }
            if already_calculated[other_tile.id][tile.id] {
                continue;
            }
            already_calculated[tile.id][other_tile.id] = true;
            let x_diff: usize = tile.x.abs_diff(other_tile.x)+1; // +1 to include both edges
            let y_diff: usize = tile.y.abs_diff(other_tile.y)+1; // +1 to include both edges
            let area: usize = x_diff * y_diff;
            tile_combinations.push(TileCombination {
                area: area
            });
        }
    }
    tile_combinations
}

fn get_tiles(file_content: &str) -> Vec<Tile> {
    let mut tiles: Vec<Tile> = Vec::new();
    let lines: Vec<&str> = file_content.split('\n').collect();
    for (id, line) in lines.iter().enumerate() {
        let x = line.split(',').nth(0).unwrap().parse::<usize>().unwrap();
        let y = line.split(',').nth(1).unwrap().parse::<usize>().unwrap();
        if x == 7 && y == 3 {
            println!("debug");
        }
        tiles.push(Tile {
            id: id,
            x: x,
            y: y,
        })
    }
    tiles
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