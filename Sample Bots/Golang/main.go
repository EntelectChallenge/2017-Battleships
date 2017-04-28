package main

// Compilation for Windows on non-windows...:
// GOOS=windows GOARCH=amd64 go build -o some_target_dir/gobot.exe main.go

import "os"
import "fmt"
import "time"
import "encoding/json"
import "io/ioutil"
import "math/rand"
import "path"
import "math"

const commandFileName string = "command.txt"

const placeShipFileName string = "place.txt"

const stateFileName = "state.json"

type Bot struct {
	key              string
	workingDirectory string
}

type Cell struct {
	Occupied bool
	Damaged  bool
	Missed   bool
	Hit      bool
	X        int
	Y        int
}

func (c Cell) String() string {
	return fmt.Sprintf("{O: %v, H: %v, D: %v, M: %v, (X:%d, Y:%d)}", c.Occupied, c.Hit, c.Damaged, c.Missed, c.X, c.Y)
}

type MapType struct {
	Cells []Cell
}

type GameState struct {
	PlayerMap    MapType
	OpponentMap  MapType
	Player1Map   MapType
	Player2Map   MapType
	GameVersion  string
	GameLevel    int
	Round        int
	MapDimension int
	Phase        int
}

const (
	DO_NOTHING = iota
	FIRESHOT
)

type Code int

type Point struct {
	x int
	y int
}

func RandomPoint(maxX, maxY int) *Point {
	return &Point{
		rand.Intn(maxX - 1),
		rand.Intn(maxY - 1),
	}
}

type Command struct {
	coordinate  Point
	commandCode Code
}

func NewCommand(code Code, x int, y int) *Command {
	coordinate := Point{x, y}
	newCommand := Command{
		coordinate,
		code,
	}
	return &newCommand
}

const (
	NORTH = "North"
	EAST  = "East"
	SOUTH = "South"
	WEST  = "West"
)

type Direction string

const (
	BATTLESHIP = "Battleship"
	CARRIER    = "Carrier"
	CRUISER    = "Cruiser"
	DESTROYER  = "Destroyer"
	SUBMARINE  = "Submarine"
)

type ShipType string

type PlaceShipCommand struct {
	shipTypes  []ShipType
	points     []Point
	directions []Direction
} // to be continued

func (c *PlaceShipCommand) String() string {

	output := ""
	for index := 0; index < len(c.shipTypes); index++ {
		elem := fmt.Sprintf("%s %d %d %s",
			c.shipTypes[index],
			c.points[index].x,
			c.points[index].y,
			c.directions[index])
		output += elem
		if index+1 != len(c.shipTypes) {
			output += "\r\n"
		}
	}
	return output
}

func (c *Command) String() string {
	return fmt.Sprintf("%d,%d,%d",
		c.commandCode,
		c.coordinate.x,
		c.coordinate.y)
}

var shipSizes = map[ShipType]int{
	BATTLESHIP: 4,
	CARRIER:    5,
	CRUISER:    3,
	DESTROYER:  2,
	SUBMARINE:  3,
}

func placeShips(bot Bot, gameState GameState) *PlaceShipCommand {
	var placeShipCommand PlaceShipCommand
	counter := 0
	for shipType, shipSize := range shipSizes {
		_ = shipSize
		counter += 1
		placeShipCommand.shipTypes = append(placeShipCommand.shipTypes, shipType)
		var location = Point{0, counter}
		placeShipCommand.points = append(placeShipCommand.points, location)
		placeShipCommand.directions = append(placeShipCommand.directions, EAST)
	}

	return &placeShipCommand
}

func writePlaceShips(bot Bot, placeShipCommand *PlaceShipCommand) {
	f, err := os.Create(path.Join(bot.workingDirectory, placeShipFileName))
	if err != nil {
		panic(err) // handle this better
	}
	defer f.Close()

	_, err = f.WriteString(placeShipCommand.String()) // gets bytecounts, ignored
	if err != nil {
		panic(err) // handle this better
	}
	f.Sync() // flush
}

func makeMove(bot Bot, gameState GameState) *Command {
	// Crazy Ivan
	var command Command

	// get opponent cells already attempted
	newX, newY := 0, 0
	totalCubes := int(math.Pow(float64(gameState.MapDimension), 2))
	for {
		randomIndex := rand.Intn(totalCubes)
		cell := gameState.OpponentMap.Cells[randomIndex]
		if cell.Damaged || cell.Missed {
			continue
		}
		newX, newY = cell.X, cell.Y
		break
	}
	
	
	command.coordinate = Point{newX, newY}
	command.commandCode = FIRESHOT
	return &command
}

func writeMove(bot Bot, command *Command) {
	f, err := os.Create(path.Join(bot.workingDirectory, commandFileName))
	if err != nil {
		panic(err) // handle this better
	}
	defer f.Close()

	_, err = f.WriteString(command.String()) // gets bytecounts, ignored
	if err != nil {
		panic(err) // handle this better
	}
	f.Sync() // flush
}

func (bot Bot) Execute() {
	// read gameState from json stateFileName
	var gameState GameState
	stateFileBytes, err := ioutil.ReadFile(path.Join(bot.workingDirectory, stateFileName)) // path correct?
	if err != nil {
		fmt.Println("could not open statefile")
		os.Exit(1)
	}

	if err := json.Unmarshal(stateFileBytes, &gameState); err != nil {
		fmt.Println("statefile broke", err)
		os.Exit(1)
	}

	if gameState.Phase == 1 {
		fmt.Println("Placing ships!")
		placeShipCommand := placeShips(bot, gameState)
		writePlaceShips(bot, placeShipCommand)
	} else {
		fmt.Println("Making move")
		command := makeMove(bot, gameState)
		writeMove(bot, command)
	}
}

func printUsage() {
	fmt.Println("Go SampleBot usage: main.exe <PlayerKey> <WorkingDirectoryFilename>")
	fmt.Println()
	fmt.Println("\tPlayerKey\tThe key assigned to your bot.")
	fmt.Println("\tWorkingDirectoryFilename\tThe working directory folder where the match runner will output map and state files and look for the move file.")
}

func runBot(args []string) {

	if 2 != len(args) {
		printUsage()
		os.Exit(1)
	}

	if _, err := os.Stat(args[1]); err != nil {
		printUsage()
		fmt.Println()
		fmt.Printf("Error: Working directory \"%s\" does not exist.\n", args[1])
		os.Exit(1)
	}

	var bot = Bot{args[0], args[1]}
	bot.Execute()
}

func main() {
	var args = os.Args[1:]
	fmt.Println("hello ", args, len(args))

	var startTime = time.Now()
	runBot(args)
	var duration = time.Since(startTime)
	fmt.Printf("[Bot]\tBot finished in %v", duration)

}
