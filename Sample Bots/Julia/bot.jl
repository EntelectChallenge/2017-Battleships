__precompile__()
module Bot
export main

import JSON

const commandFileName = "command.txt"
const placeShipFileName = "place.txt"
const stateFileName = "state.json"

@enum ShipType Carrier Battleship Cruiser Submarine Destroyer
@enum Direction North East South West

type GameContext
  state::Dict
  playerKey::String
  workingDirectory::String
  GameContext()=new(Dict(), "", "")
end

type ShipPlacement
  shipType::ShipType
  x::Int8
  y::Int8
  direction::Direction
end

function getFilePath(gameContext::GameContext, fileName::AbstractString)
  ret = string(gameContext.workingDirectory, '/', fileName)
  println("** $ret")
  ret
end

function getPhase(gameContext::GameContext)
  gameContext.state["Phase"]
end

function getMapDimension(gameContext::GameContext)
  gameContext.state["MapDimension"]
end

function makeYourMove(gameContext::GameContext)
  phase = getPhase(gameContext)
  println("* Phase $phase")
  if phase == 1
    placeShips(gameContext)
  elseif phase == 2
    bombsAway(gameContext)
  else
    error("Unsupported phase: '$phase'")
  end
end

function placeShips(gameContext::GameContext)
  println("* Placing ships..")
  writePlaceShips(gameContext, (
    ShipPlacement(Carrier,0,0,North),
    ShipPlacement(Battleship,1,0,North),
    ShipPlacement(Cruiser,2,0,North),
    ShipPlacement(Submarine,3,0,North),
    ShipPlacement(Destroyer,4,0,North),
  ))
end

function writePlaceShips(gameContext::GameContext, shipPlacements)
  placementString = ""
  for shipPlacement in shipPlacements
    placementString*="$(shipPlacement.shipType) $(shipPlacement.x) $(shipPlacement.y) $(shipPlacement.direction)\n"
  end
  println(placementString)
	write(getFilePath(gameContext, placeShipFileName), placementString)
  println("* Aye aye Captain, the ships were deployed \\o/")
end

function bombsAway(gameContext::GameContext)
  println("* Bombs away!")

  fire = 1
  max = getMapDimension(gameContext)-1
  xCoordinate = rand(0:max)
  yCoordinate = rand(0:max)

  payload = string(fire, ",", xCoordinate, ",", yCoordinate, "\n")

  write(getFilePath(gameContext, commandFileName), payload)
  println("* Shot fired: [$xCoordinate,$yCoordinate]")
end

function run(playerKey, workPathArray...)
  #I'm not sure why args containing strings are split :(
  gameContext = GameContext()
  gameContext.playerKey = playerKey
  gameContext.workingDirectory = join(workPathArray, " ")
  println("* playerKey:        '$playerKey'")
  println("* workingDirectory: '$(gameContext.workingDirectory)'")
  if !isdir(gameContext.workingDirectory)
      printUsage()
      error("Error: Working directory \"$(gameContext.workingDirectory)\" does not exist.")
      exit(1)
  end

  gameContext.state = JSON.parsefile(getFilePath(gameContext, stateFileName))
  #print(state)
  makeYourMove(gameContext)
end

function main(args...)
  println("args=$args")
  if length(args) >= 1 && args[1] == "--compile"
    @time run("007", "test_data/Phase 2 - Round 1")  #just run it in any case to warm up julia with this bot
  elseif length(args) < 2
    printUsage()
    error("Error: Expected 2 args but got: $args")
    exit(1)
  else
    @time run(args...)
  end
end

function printUsage()
  println("""
  Julia SampleBot usage: main.jl <PlayerKey> <WorkingDirectoryFilename>

  \tPlayerKey\tThe key assigned to your bot.
  \tWorkingDirectory\tThe working directory folder where the match runner will output map and state files and look for the move file.

  """)
end

end
