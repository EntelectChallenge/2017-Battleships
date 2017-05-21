extern crate json;
extern crate rand;

use std::env;
use std::path::Path;
use std::path::PathBuf;
use std::io::prelude::*;
use std::fs::File;
use std::fmt;

use rand::distributions::{IndependentSample, Range};

const COMMAND_FILE: &'static str = "command.txt";
const PLACE_FILE: &'static str = "place.txt";
const STATE_FILE: &'static str = "state.json";

#[derive(Clone, Copy, PartialEq, Eq, Debug)]
enum Orientation {
    North,
    East,
    South,
    West,
}

impl fmt::Display for Orientation {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        f.write_str(
            match self {
                &Orientation::North => "North",
                &Orientation::East => "East",
                &Orientation::South => "South",
                &Orientation::West => "West"
            }
        )
    }
}

#[derive(Clone, PartialEq, Eq, Debug)]
enum Action {
    PlaceShips(Vec<(String, u32, u32, Orientation)>),
    Shoot(u32, u32)
}

impl fmt::Display for Action {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            &Action::Shoot(x, y) => writeln!(f, "1,{},{}", x, y),
            &Action::PlaceShips(ref ships) => ships.iter().map(|&(ref ship_type, x, y, orientation)| {
                writeln!(f, "{} {} {} {}", ship_type, x, y, orientation)
            }).fold(Ok(()), |acc, next| acc.and(next))
        }
    }
}

fn main() {
    let working_dir = env::args()
        .nth(2)
        .map(|pwd| PathBuf::from(pwd))
        .expect("Requires game state folder to be passed as the second parameter");

    // The state file is read here. Chances are, you'll want to read
    // more values out of it than the sample bot currently does.
    let state = read_file(&working_dir).expect("Failed to read state.json");

    let is_place_phase = state["Phase"] == 1;
    let map_dimension = state["MapDimension"].as_u32().expect("Could not read map dimension from the state");

    let action = if is_place_phase {
        place_ships()
    }
    else {
        shoot_randomly(map_dimension)
    };

    
    write_action(&working_dir, is_place_phase, action).expect("Failed to write action to file");
}

fn read_file(working_dir: &Path) -> Result<json::JsonValue, String> {
    let state_path = working_dir.join(STATE_FILE);
    let mut file = File::open(state_path.as_path()).map_err(|e| e.to_string())?;
    let mut content = String::new();
    file.read_to_string(&mut content).map_err(|e| e.to_string())?;
    json::parse(content.as_ref()).map_err(|e| e.to_string())
}

fn place_ships() -> Action {
    let ships = vec!(
        (String::from("Battleship"), 1, 0, Orientation::North),
        (String::from("Carrier"), 3, 1, Orientation::East),
        (String::from("Cruiser"), 4, 2, Orientation::North),
        (String::from("Destroyer"), 7, 3, Orientation::North),
        (String::from("Submarine"), 1, 8, Orientation::East)
    );
    
    Action::PlaceShips(ships)
}

fn shoot_randomly(map_dimension: u32) -> Action {
    let mut rng = rand::thread_rng();
    let between = Range::new(0, map_dimension);
    let x = between.ind_sample(&mut rng);
    let y = between.ind_sample(&mut rng);
    Action::Shoot(x, y)
}

fn write_action(working_dir: &Path, is_place_phase: bool, action: Action) -> Result<(), String> {
    let filename = if is_place_phase {
        PLACE_FILE
    }
    else {
        COMMAND_FILE
    };
    
    let full_filename = working_dir.join(filename);
    let mut file = File::create(full_filename.as_path()).map_err(|e| e.to_string())?;
    writeln!(file, "{}", action).map_err(|e| e.to_string())?;
    Ok(())
}
