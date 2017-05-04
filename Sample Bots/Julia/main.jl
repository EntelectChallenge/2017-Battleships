import JSON

commandFileName = "command.txt"
placeShipFileName = "place.txt"
#stateFileName = "test_data/Phase 1 - Round 0/state.json"
stateFileName = "test_data/Phase 2 - Round 1/state.json"
#stateFileName = "state.json"

state = JSON.parsefile(stateFileName)

#print(state)

function makeYourMove()
  phase = state["Phase"]
  println("* Phase $phase")
  if phase == 1
    deployShips()
  elseif phase == 2
    println("Bombs away!..")
    bombsAway()
  else
    throw(Exception("Unsupported phase: '$phase'"))
  end
end

function deployShips()
  println("* Deploying ships..")
end

function bombsAway()
  println("* Bombs away!")
end

makeYourMove()
