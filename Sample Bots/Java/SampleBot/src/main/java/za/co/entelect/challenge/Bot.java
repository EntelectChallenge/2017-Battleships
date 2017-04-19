package za.co.entelect.challenge;

import com.google.gson.Gson;
import za.co.entelect.challenge.domain.command.Command;
import za.co.entelect.challenge.domain.command.PlaceShipCommand;
import za.co.entelect.challenge.domain.command.Point;
import za.co.entelect.challenge.domain.command.code.Code;
import za.co.entelect.challenge.domain.command.direction.Direction;
import za.co.entelect.challenge.domain.command.ship.ShipType;
import za.co.entelect.challenge.domain.state.GameState;

import java.io.*;
import java.util.ArrayList;
import java.util.concurrent.ThreadLocalRandom;

public class Bot {

    private String workingDirectory;

    private String key;

    private final String commandFileName = "command.txt";

    private final String placeShipFileName = "place.txt";

    private final String stateFileName = "state.json";

    private final Gson gson;

    public Bot(String key, String workingDirectory) {

        this.workingDirectory = workingDirectory;
        this.key = key;
        this.gson = new Gson();
    }

    public void execute() throws IOException {

        GameState gameState = gson.fromJson(new StringReader(loadState()), GameState.class);

        if (gameState.Phase == 1) {
            PlaceShipCommand placeShipCommand = placeShips();
            writePlaceShips(placeShipCommand);
        } else {
            Command command = makeMove(gameState);
            writeMove(command);
        }
    }

    private PlaceShipCommand placeShips() {

        ArrayList<ShipType> shipsToPlace = new ArrayList<>();
        shipsToPlace.add(ShipType.Battleship);
        shipsToPlace.add(ShipType.Carrier);
        shipsToPlace.add(ShipType.Cruiser);
        shipsToPlace.add(ShipType.Destroyer);
        shipsToPlace.add(ShipType.Submarine);

        ArrayList<Point> points = new ArrayList<>();
        points.add(new Point(1, 0));
        points.add(new Point(3, 1));
        points.add(new Point(4, 2));
        points.add(new Point(7, 3));
        points.add(new Point(1, 8));

        ArrayList<Direction> directions = new ArrayList<>();
        directions.add(Direction.North);
        directions.add(Direction.East);
        directions.add(Direction.North);
        directions.add(Direction.North);
        directions.add(Direction.East);

        return new PlaceShipCommand(shipsToPlace, points, directions);
    }

    private String loadState() throws IOException {

        StringBuilder jsonText = new StringBuilder();
        BufferedReader bufferedReader = new BufferedReader(new FileReader(new File(workingDirectory, stateFileName)));
        String line = bufferedReader.readLine();
        while (line != null) {
            jsonText.append(line);
            jsonText.append("\r\n");
            line = bufferedReader.readLine();
        }
        return jsonText.toString();
    }

    private Command makeMove(GameState state) {

        int possibleShipCommands = Code.values().length;
        int lowerBounds = 0;
        Code commandCode = Code.values()[ThreadLocalRandom.current().nextInt(lowerBounds, possibleShipCommands)];
        int upperBounds = state.MapDimension;
        int xCoord = ThreadLocalRandom.current().nextInt(lowerBounds, possibleShipCommands + upperBounds);
        int yCoord = ThreadLocalRandom.current().nextInt(lowerBounds, possibleShipCommands + upperBounds);
        return new Command(commandCode, xCoord, yCoord);
    }

    private void writeMove(Command command) throws IOException {

        BufferedWriter bufferedWriter = new BufferedWriter(new FileWriter(new File(workingDirectory, commandFileName)));
        bufferedWriter.write(command.toString());
        bufferedWriter.flush();
        bufferedWriter.close();
    }

    private void writePlaceShips(PlaceShipCommand placeShipCommand) throws IOException {

        BufferedWriter bufferedWriter = new BufferedWriter(new FileWriter(new File(workingDirectory, placeShipFileName)));
        bufferedWriter.write(placeShipCommand.toString());
        bufferedWriter.flush();
        bufferedWriter.close();
    }

    private void log(String message) {

        System.out.println(String.format("[BOT]\t%s", message));
    }
}
