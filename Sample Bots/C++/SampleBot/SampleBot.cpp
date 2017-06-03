// C++ Sample Bot for Entelect Challenge 2017


#include "stdafx.h"
#include <random>
#include <vector>
#include <iostream>
#include <fstream>
#include <sstream>

#include "rapidjson/document.h"

using namespace std;

constexpr char state_filename[] = "state.json";
constexpr char place_filename[] = "place.txt";
constexpr char command_filename[] = "command.txt";

struct point {
	int x;
	int y;
};


rapidjson::Document parse_state(const string working_directory) {
	// Long-winded parsing because Nuget version of
	// rapidjson (1.0.2) requires a C-style string

	ifstream ifs(working_directory + "/" + state_filename);
	stringstream buffer;
	buffer << ifs.rdbuf();
	auto buffer_string = buffer.str();
	rapidjson::Document json_doc;
	json_doc.Parse(buffer_string.c_str());
	return json_doc;
}


void fire_shot(const string working_directory, rapidjson::Document& state) {
	// Get cells that haven't already been shot at
	const auto& cells = state["OpponentMap"]["Cells"];
	vector<point> valid_points;
	for (auto it = cells.Begin(); it != cells.End(); it++) {
		const auto& cell = (*it);
		if (!cell["Damaged"].GetBool() && !cell["Missed"].GetBool()) {
			point p { cell["X"].GetInt(), cell["Y"].GetInt() };
			valid_points.push_back(p);
		}
	}

	// Select among them randomly
	random_device rd;
	default_random_engine rng(rd());
	uniform_int_distribution<int> cell_dist(0, valid_points.size() - 1);
	auto shot = valid_points[cell_dist(rng)];

	// Output shot
	ofstream ofs(working_directory + "/" + command_filename);
	ofs << "1," << shot.x << "," << shot.y << "\n";
}


void place_ships(const string working_directory) {
	ofstream ofs(working_directory + "/" + place_filename);
	ofs << "Carrier 1 1 East\n";
	ofs << "Battleship 7 7 South\n";
	ofs << "Cruiser 9 0 North\n";
	ofs << "Submarine 4 4 West\n";
	ofs << "Destroyer 0 9 East\n";
}


int main(int argc, char** argv)
{
	if (argc != 3) {
		cout << "Usage: SampleBot.exe PlayerKey WorkingDirectory" << endl;
		return 1;
	}
	string working_directory(argv[2]);
	auto state = parse_state(working_directory);

	if (state["Phase"].GetInt() == 1) {
		place_ships(working_directory);
	} else {
		fire_shot(working_directory, state);
	}
	
    return 0;
}
