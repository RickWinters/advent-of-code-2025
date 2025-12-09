use std::{fs::read_to_string, process::exit};

#[derive(Debug, Clone)]
struct FreshRange{
    start: u128,
    end: u128
}

fn main() {
    // let input_path: &str = "example_input.txt";
    let input_path: &str = "puzzel_input.txt";
    // let input_path: &str = "test.txt";
    let file_content = match read_file(input_path) {
        Ok(content) => content,
        Err(_) => unreachable!()
    };
    let fresh_ranges: Vec<FreshRange>;
    let ingredients: Vec<u128>;

    (fresh_ranges, ingredients) = get_fresh_ranges_and_ingredients(&file_content);

    let fresh_ingredients = count_fresh_ingredients(&ingredients, &fresh_ranges);

    println!("Total fresh ingredients = {:?}", {fresh_ingredients});

    let total_fresh_ingredient_ids = count_fresh_ingredient_ids(fresh_ranges);

    println!("Total fresh ingredient id's {:?}", {total_fresh_ingredient_ids})
}

fn count_fresh_ingredient_ids(fresh_ranges: Vec<FreshRange>) -> u128 {
    let mut total_fresh_ids: u128 = 0;
    let mut fresh_ranges_sorted = fresh_ranges.clone();
    fresh_ranges_sorted.sort_by_key(|range| range.start);
    let mut last_id: u128 = 0;
    for range in fresh_ranges_sorted{
        if range.end <= last_id {
            continue
        }
        let end = range.end;
        let start = range.start.max(last_id+1);
        let current_total = end - start + 1;
        last_id = range.end;
        total_fresh_ids += current_total as u128;
    }
    total_fresh_ids
}

fn count_fresh_ingredients(ingredients: &Vec<u128>, fresh_ranges: &Vec<FreshRange>) -> u128{
    let mut total_fresh_ingredients = 0;
    for ingredient in ingredients{
        if let Some(_) = &fresh_ranges.iter().position(|range| range.start <= *ingredient && range.end >= *ingredient) {
            total_fresh_ingredients += 1
        }
    }
    total_fresh_ingredients
}

fn get_fresh_ingredient_ids(fresh_ranges: &Vec<FreshRange>) -> Vec<u128>{
    let mut fresh_ingredient_ids: Vec<u128> = Vec::new();
    for range in fresh_ranges {
        for i in range.start..=range.end {
            fresh_ingredient_ids.push(i);
        }
    }

    fresh_ingredient_ids
}

fn get_fresh_ranges_and_ingredients(file_contents: &str) -> (Vec<FreshRange>, Vec<u128>) {
    let mut fresh_ranges: Vec<FreshRange> = Vec::new();
    let mut ingredients: Vec<u128> = Vec::new();
    let lines = file_contents.split('\n');
    for line in lines{
        if line.is_empty() {
            continue;
        }
        if line.contains('-'){
            let numbers: Vec<&str> = line.split('-').collect();
            let start = numbers[0].parse::<u128>().unwrap();
            let end = numbers[1].parse::<u128>().unwrap();
            fresh_ranges.push(FreshRange { start, end });
        }
        else {
            ingredients.push(line.parse::<u128>().unwrap());
        }
    }

    (fresh_ranges, ingredients)
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