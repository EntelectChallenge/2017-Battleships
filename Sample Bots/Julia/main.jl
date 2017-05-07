import JSON

const commandFileName = "command.txt"
const placeShipFileName = "place.txt"
const stateFileName = "state.json"

type GameContext
  state::Dict
  playerKey::String
  workingDirectory::String
  GameContext()=new(Dict(), "", "")
end

function getFilePath(gameContext::GameContext, fileName::AbstractString)
  ret = string(gameContext.workingDirectory, '/', fileName)
  println("** $ret")
  ret
end
function getPhase(gameContext::GameContext)
  gameContext.state["Phase"]
end

function makeYourMove(gameContext::GameContext)
  phase = getPhase(gameContext)
  println("* Phase $phase")
  if phase == 1
    deployShips(gameContext)
  elseif phase == 2
    bombsAway(gameContext)
  else
    error("Unsupported phase: '$phase'")
  end
end

function deployShips(gameContext::GameContext)
  println("* Deploying ships..")

  placementString = """
Carrier 0 0 North
Battleship 1 0 North
Cruiser 2 0 North
Submarine 3 0 North
Destroyer 4 0 North
"""

	 write(getFilePath(gameContext, placeShipFileName), placementString)
   println("* Aye aye Captain, the ships were deployed \\o/")
end

function bombsAway(gameContext::GameContext)
  println("* Bombs away!")

  fire = 1
  xCoordinate = rand(1:10)
  yCoordinate = rand(1:10)

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
  if length(args) < 2
      printUsage()
      error("Error: Expected 2 args but got: $args")
      exit(1)
  end
  run(args...)
end

function printUsage()
  println("""
  Julia SampleBot usage: main.jl <PlayerKey> <WorkingDirectoryFilename>

  \tPlayerKey\tThe key assigned to your bot.
  \tWorkingDirectory\tThe working directory folder where the match runner will output map and state files and look for the move file.

  """)
end

println("ARGS=$ARGS")
main(ARGS...)

#TODO: facture out bot module
#TODO: write some basic tests..
