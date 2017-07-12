# 2017-Battleships

The current release is version [1.1.2](https://github.com/EntelectChallenge/2017-Battleships/releases/latest).

For more information about the challenge see the [Challenge website](http://challenge.entelect.co.za/) .

## TL;DR

1.	Download the latest release files
2.	Build your own bot
3.	Submit your bot 
4.	Prepare to fight for the title: Ruler of the high seas.

# Project Structure

In this project you will find everything you need to build and run a bot on your local machine.  This project contains the following:

1. Game Engine - The game engine is responsible for running matches between players.
2. Sample Bots - Sample bots can be used a starting point for your bot.
3. Reference Bots - The reference bot contains some AI logic that will play the game based on predefined rules.  You can use this to play against your bot for testing purposes.
4. Sample State Files - Can be used as a starting point to get the parsing working for your bot.

This project can be used to get a better understanding of the rules and to help debug your bot.

Improvements and enhancements will be made to the game engine code over time.  The game engine will also evolve during the competition after every battle, so be prepared. Any changes made to the game engine or rules will be updated here, so check in here now and then to see the latest changes and updates.

The game engine has been made available to the community for peer review and bug fixes, so if you find any bugs or have any concerns, please e-mail challenge@entelect.co.za, discuss it with us on the [Challenge forum](http://forum.entelect.co.za/), alternatively submit a pull request on Github and we will review it.

## Usage

The easiest way to start using the game engine is to download the [Game Engine zip](https://github.com/EntelectChallenge/2017-Battleships/releases/latest). You will also need the .NET framework if you don't have it installed already - you can get the offline installer for [.NET Framework here](https://www.microsoft.com/en-us/download/details.aspx?id=53344).

Once you have installed .NET and downloaded the binary release zip file, extract it and open a new Command Prompt in the Binaries/{version}/Game Engine folder.

We have included the reference bot in the binaries version folder, so at this point you can simply run the Run.bat to see the bots play a match.

Once you have written your own bot you can you can use the command line arguments to specify the bots that should be run. You can see the available command line arguments by running `Battleship.exe --help`:

```powershell
-b, --bot (Default: Empty String Array) Relative path to the folder containing the 		bot player. You can add multiple bots by separating each with a space.

-c, --console (Default: 0) The amount of console players to add to the game.

--clog (Default: False) Enables Console Logging.

--pretty (Default: False) Draws the game map to console for every round instead of showing logs.

-l, --log (Default: ) Relative path where you want the match replay log files to be output (instead of the default Replays/{matchSeed}).

-s, --size (Default: 2) The map size to use for the match. 1 = small (7 x 7), 2 = medium (10 x 10), 3 = large (14 x 14).

--nolimit (Default: false) Disables the time limit for bot execution.

--debug (Default: false) Halts the game engine when a bot writes to the error stream.                                 

--tournamentMode (Default: false) Will run the game engine in tournament mode, this will delete all game files after every bot execution to prevent cheating and will write all the files again at the end of the match.

--help Display this help screen.
````

So for example you can do something like this to run your bot against the bundled reference bot: `Battleship.exe --pretty -b "../Reference Bot" "../My Bot That will win"`.

You might have to change the configuration file depending on your system in order to run the game.  The configuration file is in the game engine folder called `Battleship.exe.config`.  You can modify the file to update where the game engine looks for the various runtime executables such as the java runtime to use.  All paths have to be absolute (unless the executable is in the system path).

## Sample Bots

Entelect will provide Sample Bots for C# and Java. Sample bots are basic started projects that can read game files and make random moves. Reference bots that are capable of playing a game from start to finish are also included for contestants wishing to have something more to work from.  

For any additional languages we will be relying on the community contributing a sample bot for the language of their choice.  If you would like your language to be supported you will have to submit a sample bot based on the rules outlined in the [Sample Bot](https://github.com/EntelectChallenge/2017-Battleships/tree/master/Sample%20Bots) page.  Sample bot submissions will close at Midnight on the 26th of May, after this no additional sample bots will be accepted.

Calibration bots will be included into the game engine before the first battle (after sample bot submission have closed) and will be based on the sample bot for each language.

## Your Bot

Same as last year, the system will compile your bot, based on the bot meta file that you included.

Sample bot project files can be downloaded [here.](https://github.com/EntelectChallenge/2017-Battleships/releases/latest)

The game engine requires that you have `bot.json` file.  This will tell the game engine how to compile and run your bot.  The file must contain the following:

```json
{
    "Author":"John Doe",
    "Email":"john.doe@example.com",
    "NickName" :"John",
    "BotType": "CSharp",
    "ProjectLocation" : "",
    "RunFile" : "Reference\\bin\\Debug\\Reference.exe",
    "RunArgs" : ""
}
```

The property fields are as follows:

1. Author - Your Name
2. Email - Your Email Address,
3. Nickname - The nickname you want the visualizer to use
4. Bot Type - The type of bot (The language you used to code the bot)
	* CSharp
	* Java
	* JavaScript
	* CPlusPlus
	* Python2
	* Python3
	* FSharp
5. Project Location - The root location of the project file.  For instance in C# solutions, that will point to folder containing the solution (.sln) file.  This will be used for bot compilation when you submit your bot.
6. Run File - This is the main entry point file for your bot that will be executed to play the game and must be relative to the root location of this meta file.
	* Java users have to ensure that the main class is specified in the manifest file, also ensure that maven can compile your project as a fat-jar containing all of your libraries you might use.
7. RunArgs - (Optional) Any additional arguments you would like to send your bot.  This will be the 3rd argument sent to your bot (if provided).

The game engine might set additional runtime parameter in some scenarios, for instance specifying minimum memory allocation for java bots.

The following package managers are supported by the game engine:
* Microsoft Languages - Nuget.  (Requires the nuget.exe to be present in the project location path)
* Java - Maven.  (Requires that the project contains a pom file in the project location path)
* JavaScript - NPM.  (Requires that project contains a package.json file in the project location path)
* Python - Python Package Index.  (Requires that the project contains a requirements.txt file in the project location path)

Your bot will receive two arguments on every execution round of the game:

1. Your player key registered in the game
2. The directory for the current game files

The game will store game files during a match in the following directory format
````
...Replays/
..........{Match Time Stamp}/
...................{Phase and Round Number}/
........................................ engine.log
........................................ A - map.txt
........................................ B - map.txt
........................................ state.json
........................................ roundinfo.json
........................................ {Player Key}/
................................................... log.txt
................................................... map.txt
................................................... state.json
................................................... place.txt
................................................... command.txt
````

The engine has two modes, normal mode or debug mode, and tournament mode. During normal mode no files are deleted, during tournament mode after each round, all files that could leak information to the opposing player is deleted as soon as the round ends, the information is stored in memory and dumped into the appropriate files at the end of match. The rules below will be explained using tournament mode.

### Game Phases

There are 2 phases to the game, phase 1 being the place ships phase, during this phase each player will have the `map.txt` and `state.json` files in their `Player Key` folder until they placed their ships in the `place.txt` file, phase 1 will only end if both players have successfully placed all their ships, if a player fails to place their ships for 5 continues rounds they will be killed off and the other player will win by default. After a successful round in phase 1, the game will proceed to phase 2, the great battle, during this phase the player will have the same files as mentioned above to work from, but this time they will have to place their shot command in the `command.txt` file. At the end of the round the files will be deleted as well to prevent opposing bots from snooping where they shouldn't...

The `engine.log` file contains information from the engine while processing the round.

The `log.txt` file in the player file contains player specific logs, such as the console output from player bots.  If your bot is misbehaving this should be the first place to go have a look.  In this file you can also view additional information such as bot run time and bot processor time.

The `round.info` folder is mainly for GUI submissions, and reports player stats and the leaderboard.

The `map.txt` file will use the following characters to represent the game state:
```
'~': An empty water block
'!': A missed shot
'*': A succesfully landed shot
'B,C,R,S,D': A healthy ship cell
'b,c,r,s,d': A damaged ship cell
'B-b': BattleShip
'C-c': Carrier
'R-r': Cruiser
'S-s': Submarine
'D-d': Destroyer

The `map.txt` will also have sections underneath the map for each player to give more information about each player like the points, shots fired, shots landed and which ships are still remaining.
```

### Game Engine Upgrades

Duing the competition the game engine will be upgraded after each battle to support new features to make the game more complex and challenging.  Please make sure that you follow the releases to make sure you are always working with the latest features and that your bot can handle the latest changes to the game to ensure you stand a winning chance.  Each version of the game engine will be backwards compatible with older bots.

### Rules

These are the simplified rules.  More in depth rules are further down.

1. A player can only pass through one command during a round.
2. During the first phase a player has to place their ships.
3. After a successful phase 1, a player can pass through one of the following commands
	* Do Nothing - nothing will happen
	* Fire Shot- Fires a shot at the given location
	* Fire Double Shot - Fires two shots given a center location
	* Fire Corner Shot - Fires four shots given a center location
	* Fire Cross Shot Diagonal - Fires five shots given a center location
	* Fire Cross Shot Horizontal - Fires five shots given a center location
	* Fire Seeker Missile - Finds the nearst ship with an eclidian distance of 2 units or less away, given a center location 
4. After each round energy is added to the player depending on the size of the map.
	* Small Map = 2 Energy per round
	* Medium Map = 3 Energy per round
	* Large Map = 4 Energy per round
5. Each ship has two weapons, a single shot weapon and a special weapon, unique to that ship, each weapon requires energy to use, the unique weapon can only be used if the ship is not destroyed and the player has enough energy.
	* Single Shot - All ships - Requires 1 energy to use
	* Double Shot - Destroyer - Requires 8 rounds worth of energy to use
	* Corner Shot - Carrier - Requires 10 rounds worth of energy to use
	* Cross Shot Diagonal - BattleShip - Requires 12 rounds worth of energy to use
	* Cross Shot Horizontal - Crusier - Requires 14 rounds worth of energy to use
	* Seeker Missile - Submarine  - Requires 10 rounds worth of energy to use
6. A shot will damage a ship if it hits, else it will just hit the water and do nothing.
7. A player earns points for each successful shot landed and for completely destroying an enemy's ship.
8. A player is victorious if he destroy's all of the enemy's ships first, in the case of a tie the winner will be the player who landed the first shot successfully, if it is still a tie it will be the player who had the least amount of first phase failed commands.

# The Rules to rule them all
### Map Generation

The map will be square sized based on the given size rules. Please note that each player has their own map and this is the size of the player's map and not the entire game map.

1. 7 x 7 if small is selected.
2. 10 x 10 if medium is selected.
3. 14 x 14 if large is selected.

Each player will only see where they hit and missed on their opponent's map.

### Player Rules

Players can either be console players or bots.  Both follow the same game engine rules.

1. Players will only be able submit one command per round.  The game engine will reject any additional commands sent by the player.
2. Phase 1 will be a maximum of 5 rounds long, if a player is unable to place their ships during these 5 turns they will be destroyed and the opposing player will win.
3. During the first phase a player can only pass through the PlaceShipCommand, if another Command is sent through or the player fails to place their ships, their FailedFirstPhaseCommands counter will be incremented, if this reaches 5 they will be killed off.
4. After the first phase is done, each player can send through one of the following commands.
	* Do Nothing - This will skip their turn, after 20 DoNothing Commands the player will be killed off to protect the game engine from faulty bots.
	* Fire Shot Command - This will fire a shot at the enemy's map, if the shot is successful and hits an opposing ship, the player will be awarded points, if the shot destroys an enemy ship, they will be awarded additinal points for sinking the ship. (3 x 1 cells) or (1 x 3 cells)
	* Fire Double Shot Command - This will fire two shots at the enemy's map given a center point and a direction, the direction will determine whether the shots are horizontal or vertical. The Shots will be one block to the west and east or the north and south of the center point. (3 x 3 cells)
	* Fire Corner Shot Command - This will fire four shots at the enemy's map, given a center point the shots will be one block to the north-west, north-east, south-east and south-west of the center point. (3 x 3 cells)
	* Fire Cross Shot Diagonal Command - This will fire five shots givn a center point, with four being the same as the corner shot and inclusive of the center point. (3 x 3 cells)
	* Fire Cross Shot Horizontal Command - This will fire five shots, same as the CrossShotDiagonal, but in a horizontal and vertical cross. (3 x 3 cells)
	* Fire Seeker Missile Command - This will fire a missile at target area, finding the nearest ship cell with an eclidian distance of 2 units or less away from the center point given , if there is no ship the center point given will be the target of the missile. 
5. Bot players will have the following additional rules
	* Bot processes will be forcefully terminated after 4 seconds
	* Bots will not be allowed to exceed a total execution time of 2 seconds
	* Bots processes might be run with elevated processor priority. (For this reason the game has to be run with administrator privileges) 
	* Calibrations will be done at the start of a game to determine additional processor time.  So if the calibration bot takes 200ms to read the files and make a move decision then your bot will be allowed an additional 200ms to complete.
	* Malfunctioning bots or bots that exceed their time limit will send back a do nothing command.
	* Bot players that post more than 20 do nothing commands in a row will be assumed broken and will automatically be killed.
	* Players must ensure that the bot process exits gracefully within the allotted time (Rule 5-B). 
	* All bot logic processing must be done within the source code submitted for your bot.  You may not use network calls such as web services to aid in your bots decision making. No child processes will be allowed and should it be discovered you will be disqualified.
	* The nickname used on the `bot meta` file is used in the `map.txt` file, for this reason you will only be allowed alphanumeric characters as your nickname, and no special characters such as `Carriage Return`, `Line Feed` and `New Line` will not be allowed.
 6. Entelect reserves the right to change/update these rules at any point during the competition.

### Game Engine Rules

The following rules describe how the game engine will run and process the game

1. The game is split up into 2 phases:
	* Phase 1: the placing of the ships phase. During this phase, each player has 5 chances to place their ships on the map. If a player is not successful they will receive an increase to their failed first phase counter, if this reaches 5 they will be killed off.
	* Phase 2: firing shots at the opponent's map. During this phase each player takes a shot at the opposing player's map                 in hope of hitting a ship and destroying all of the enemy's ships first.
2. After each round of phase 2, each player will receive energy depending on the size of the map.
	* Small Map = 2 Energy per round
	* Medium Map = 3 Energy per round
	* Large Map = 4 Energy per round
3. The game contains the following entities:
	* Empty space
	* Player Ship
3. An empty space can be occupied by a ship.
4. A space occupied by a ship that is hit will be marked as hit (The player who landed the shot will have their shots landed counter increased).
5. A space not occupied by a ship that is fired at will be marked as missed (The player who missed the shot will have their shots fired counter increased, regardless if it is a hit or a miss).
6. The game engine will process rounds in the following order:
	* Process Player Commands (Depending on the phase)
	* Add Energy to players, given the size of the map
	* Kill off Players (A player will be killed if all of his ships are destroyed)
7. If all of a player's ships are destroyed they will be eliminated and the opposing player will be the winner.
8. If both players are killed at the same time, the following will be used to determine a winner:
	* Player who still has ships remaining
	* Player with the most points.
	* Player who landed a shot first.
	* Player who had the least amount of failed place ship commands.

### Points

Players wills be awarded points during the game based on the following.
1. For each successful shot you will be awarded 10 points.
1. For destroying an enemy ship you will be awarded 30 points.
1. For killing the enemy player you will be awarded an additional 100 points.

## Release Notes

### Version 1.1.2 - 07 July 2017
Change Log:
1. Changed order of searching for a seeker missile target
2. Fixed issue with starting energy not lining up with mapsize

### Version 1.1.1 - 22 June 2017
Change Log:

1. Added new weapons to each ship.
	* Double Shot - Destroyer - Requires 8 rounds worth of energy to use
	* Corner Shot - Carrier - Requires 10 rounds worth of energy to use
	* Cross Shot Diagonal - BattleShip - Requires 12 rounds worth of energy to use
	* Cross Shot Horizontal - Crusier - Requires 14 rounds worth of energy to use
	* Seeker Missile - Submarine  - Requires 10 rounds worth of energy to use
2. Added new commands and command codes.
	* FireDoubleShotCommand
	* FireCornerShotCommand
	* FireCrossShotCommand
	* FireSeekerMissileCommand
3. Added energy to players at the end of each round, depending on the map size selected.
	* Small Map - 2 Energy
	* Medium Map - 3 Energy 
	* Large Map - 4 Energy
4. Updated Console Harness to allow the new commands
5. Updated C# and Java Reference Bot to make use of the double (Each time it has energy and the destroyer is still available)
6. Fixed readme for seeker missile
7. Added Tests for all weapon commands

### Version 1.0.5 - 04 June 2017
Change Log:

1. Added C++ bot support.

### Version 1.0.4 - 29 May 2017
Change Log:

1. Fixed a bug where a bot could not fire a shot in the same round that it's last ship was destroyed.

### Version 1.0.3 - 28 May 2017
Change Log:

1. Added Julia sample bot and support.
2. Added support for calibration bots.

### Version 1.0.2 - 21 May 2017
Change Log:

1. Improved support for Golang bots.
2. Fixed a bug where the game engine would continue with the match if a bot fails to write a place file.
3. Fixed a bug where ships could not be placed on the border of the map in an easternly or nothernly direction.
4. Minor updates to ensure the game engine runs correctly on the tournament server.

### Version 1.0.1 - 04 May 2017
Change Log:

1. Fixed some typo's in the game engine and output files.
2. Updated Java bot to use Java 8 features.
3. Fixed a bug where the game engine would throw an InvalidOperationException if a player tries to fire a weapon with no remaining ships.
4. Changed Opponent Ship Types in the JSON to be string values instead of integer.
5. Updated Game Engine to only pause on exit if it is running in a pretty session.
6. Fixed a bug where the game engine could throw exceptions when placing ships in some scenarios.
7. Fixed a bug where repeatedly hitting the same block with a ship on would continually award points.
8. Fixed a bug where the game engine would not output files for different players with the same name correctly.
9. Added additional sample bots.

### Version 1.0.0 - 18 April 2017
Change Log:

1. Initial release


