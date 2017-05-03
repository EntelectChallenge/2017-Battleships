extern crate json;
extern crate rand;

use std::env;
use std::path::Path;
use std::path::PathBuf;
use std::io::prelude::*;
use std::fs::File;

use rand::distributions::{IndependentSample, Range};

const COMMAND_FILE: &'static str = "command.txt";
const PLACE_FILE: &'static str = "place.txt";
const STATE_FILE: &'static str = "state.json";


fn main() {
    let working_dir = env::args()
        .nth(2)
        .map(|pwd| PathBuf::from(pwd))
        .expect("Requires game state folder to be passed as the second parameter");
    
    let state = read_file(&working_dir).expect("Failed to read state.json");
    println!("{}", state);
    let place_phase = state["Phase"] == 1;
    let map_dimension = state["MapDimension"].as_u32().expect("Could not read map dimension from the state");
        
    if place_phase {
        place_ships(&working_dir, map_dimension).expect("Failed to write placement");
    }
    else {
        shoot(&working_dir, map_dimension).expect("Failed to wrote shot");
    }
}

fn read_file(working_dir: &Path) -> Result<json::JsonValue, String> {
    let state_path = working_dir.join(STATE_FILE);
    let mut file = File::open(state_path.as_path()).map_err(|e| e.to_string())?;
    let mut content = String::new();
    file.read_to_string(&mut content).map_err(|e| e.to_string())?;
    json::parse(content.as_ref()).map_err(|e| e.to_string())
}

fn place_ships(working_dir: &Path, map_dimension: u32) -> Result<(), String> {
    let place_path = working_dir.join(PLACE_FILE);
    let mut file = File::create(place_path.as_path()).map_err(|e| e.to_string())?;
    file.write_all(b"Battleship 1 0 North\n").map_err(|e| e.to_string())?;
    file.write_all(b"Carrier 3 1 East\n").map_err(|e| e.to_string())?;
    file.write_all(b"Cruiser 4 2 North\n").map_err(|e| e.to_string())?;
    file.write_all(b"Destroyer 7 3 North\n").map_err(|e| e.to_string())?;
    file.write_all(b"Submarine 1 8 East\n").map_err(|e| e.to_string())?;
    Ok(())
}

fn shoot(working_dir: &Path, map_dimension: u32) -> Result<(), String> {
    let mut rng = rand::thread_rng();
    let between = Range::new(0, map_dimension);
    let x = between.ind_sample(&mut rng);
    let y = between.ind_sample(&mut rng);
    
    let shoot_path = working_dir.join(COMMAND_FILE);
    let mut file = File::create(shoot_path.as_path()).map_err(|e| e.to_string())?;
    write!(file, "1,{},{}", x, y).map_err(|e| e.to_string())?;
    Ok(())
}
