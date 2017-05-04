import JSON

commandFileName = "command.txt"
placeShipFileName = "place.txt"
#stateFileName = "test_data/Phase 1 - Round 0/state.json"
stateFileName = "test_data/Phase 2 - Round 1/state.json"
#stateFileName = "state.json"
workingDirectory = "."

state = JSON.parsefile(stateFileName)

#print(state)

function makeYourMove()
  phase = state["Phase"]
  println("* Phase $phase")
  if phase == 1
    deployShips()
  elseif phase == 2
    bombsAway()
  else
    throw(Exception("Unsupported phase: '$phase'"))
  end
end

function deployShips()
  println("* Deploying ships..")

  placementString = """
Carrier 0 0 North
Battleship 1 0 North
Cruiser 2 0 North
Submarine 3 0 North
Destroyer 4 0 North
""";

	 write(placeShipFileName, placementString)
   println("* Aye aye Captain, the ships were deployed \\o/");
end

function bombsAway()
  println("* Bombs away!")

  fire = 1
  xCoordinate = rand(1:10)
  yCoordinate = rand(1:10)

  payload = string(fire, ",", xCoordinate, ",", yCoordinate, "\n")

  write(commandFileName, payload)
  println("* A shot was fired at [$xCoordinate,$yCoordinate]");
end

makeYourMove()
