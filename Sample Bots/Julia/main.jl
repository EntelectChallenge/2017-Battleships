import JSON

commandFileName = "command.txt"
placeShipFileName = "place.txt"
#stateFileName = "test_data/Phase 1 - Round 0/state.json"
#stateFileName = "test_data/Phase 2 - Round 1/state.json"
stateFileName = "state.json"
playerKey = "007"
workingDirectory = "."

function makeYourMove(state)
  phase = state["Phase"]
  println("* Phase $phase")
  if phase == 1
    deployShips(state)
  elseif phase == 2
    bombsAway(state)
  else
    throw(Exception("Unsupported phase: '$phase'"))
  end
end

function deployShips(state)
  println("* Deploying ships..")

  placementString = """
Carrier 0 0 North
Battleship 1 0 North
Cruiser 2 0 North
Submarine 3 0 North
Destroyer 4 0 North
"""

	 write(placeShipFileName, placementString)
   println("* Aye aye Captain, the ships were deployed \\o/")
end

function bombsAway(state)
  println("* Bombs away!")

  fire = 1
  xCoordinate = rand(1:10)
  yCoordinate = rand(1:10)

  payload = string(fire, ",", xCoordinate, ",", yCoordinate, "\n")

  write(commandFileName, payload)
  println("* Shot fired: [$xCoordinate,$yCoordinate]")
end

function run(playerKey, workPathArray...)
  #I'm not sure why args containing strings are split :(
  workingDirectory = join(workPathArray, " ")
  println("* playerKey:        '$playerKey'")
  println("* workingDirectory: '$workingDirectory'")
  if !isdir(workingDirectory)
      printUsage()
      println("Error: Working directory \"$workingDirectory\" does not exist.")
      exit(1)
  end

  qstateFileName = string(workingDirectory, '/', stateFileName)
  println(qstateFileName)
  state = JSON.parsefile(qstateFileName)
  #print(state)
  makeYourMove(state)
end

function main(args...)
  println("args=$args")
  if length(args) < 2
      printUsage()
      println("Error: Expected 2 args but got: $args")
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
