package za.co.entelect.challenge;

import com.google.gson.Gson;
import za.co.entelect.challenge.domain.command.Command;
import za.co.entelect.challenge.domain.command.PlaceShipCommand;
import za.co.entelect.challenge.domain.command.Point;
import za.co.entelect.challenge.domain.command.code.Code;
import za.co.entelect.challenge.domain.command.direction.Direction;
import za.co.entelect.challenge.domain.command.ship.ShipType;
import za.co.entelect.challenge.domain.state.GameState;
import za.co.entelect.challenge.strategy.BasicShootStrategy;
import za.co.entelect.challenge.strategy.RandomPlacementStrategy;

import java.io.*;
import java.util.ArrayList;
import java.util.Arrays;
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

    public void execute() {

        GameState gameState = gson.fromJson(new StringReader(loadState()), GameState.class);

        if (gameState.Phase == 1) {
            PlaceShipCommand placeShipCommand = placeShips(gameState);
            writePlaceShips(placeShipCommand);
        } else {
            Command command = makeMove(gameState);
            writeMove(command);
        }
    }

    private PlaceShipCommand placeShips(GameState gameState) {

        return new RandomPlacementStrategy().getShipPlacement(gameState);
    }

    private Command makeMove(GameState gameState) {

        return new BasicShootStrategy().executeStrategy(gameState);
    }

    private String loadState() {
        try {
            StringBuilder jsonText = new StringBuilder();
            BufferedReader bufferedReader = new BufferedReader(new FileReader(new File(workingDirectory, stateFileName)));
            String line = bufferedReader.readLine();
            while (line != null) {
                jsonText.append(line);
                jsonText.append("\r\n");
                line = bufferedReader.readLine();
            }
            return jsonText.toString();
        } catch (IOException e) {
            log(String.format("Unable to read state file: %s/%s", workingDirectory, stateFileName));
            log(String.format("Stacktrace: %s", Arrays.toString(e.getStackTrace())));
            return null;
        }
    }

    private void writeMove(Command command) {
        try {
            BufferedWriter bufferedWriter = new BufferedWriter(new FileWriter(new File(workingDirectory, commandFileName)));
            bufferedWriter.write(command.toString());
            bufferedWriter.flush();
            bufferedWriter.close();
            log("Command " + command);
        } catch (IOException e) {
            log(String.format("Unable to write command file: %s/%s", workingDirectory, placeShipFileName));
            log(String.format("Stacktrace: %s", Arrays.toString(e.getStackTrace())));
        }
    }

    private void writePlaceShips(PlaceShipCommand placeShipCommand) {

        BufferedWriter bufferedWriter = null;
        try {
            bufferedWriter = new BufferedWriter(new FileWriter(new File(workingDirectory, placeShipFileName)));
            bufferedWriter.write(placeShipCommand.toString());
            bufferedWriter.flush();
            bufferedWriter.close();
            log("Command " + placeShipCommand);
        } catch (IOException e) {
            log(String.format("Unable to write command file: %s/%s", workingDirectory, placeShipFileName));
            log(String.format("Stacktrace: %s", Arrays.toString(e.getStackTrace())));
        }
    }

    private void log(String message) {

        System.out.println(String.format("[BOT]\t%s", message));
    }
}
