package za.co.entelect.challenge.strategy;

import za.co.entelect.challenge.domain.command.Command;
import za.co.entelect.challenge.domain.command.code.Code;
import za.co.entelect.challenge.domain.state.GameState;
import za.co.entelect.challenge.domain.state.OpponentCell;

import java.util.*;
import java.util.concurrent.ThreadLocalRandom;

public class BasicShootStrategy {

    public Command executeStrategy(GameState gameState) {
        return randomShotCommand(gameState);
    }

    private Command randomShotCommand(GameState gameState) {
        ArrayList<OpponentCell> opponentCells = gameState.OpponentMap.Cells;
        Optional<OpponentCell> lastShot = opponentCells.stream()
                .sorted(Comparator.comparingInt(y -> y.Y))
                .sorted(Comparator.comparingInt(y -> y.X))
                .filter(cell -> cell.Damaged || cell.Missed).findFirst();

        if (!lastShot.isPresent()) {
            return new Command(Code.FIRESHOT, 0, 0);
        }

        int x = lastShot.get().X;
        int y = lastShot.get().Y;

        if (x + 2 < gameState.PlayerMap.MapWidth) {
            x += 2;
        } else {
            x = 0;
            y++;
        }
        if (y >= gameState.PlayerMap.MapWidth) {
            return alternateRandomShot(gameState);
        }

        return new Command(Code.FIRESHOT, x, y);
    }

    private Command alternateRandomShot(GameState gameState) {
        ArrayList<OpponentCell> opponentCells = gameState.OpponentMap.Cells;
        Optional<OpponentCell> availableCell = opponentCells.stream().filter(x -> !x.Damaged && !x.Missed).findFirst();

        if (!availableCell.isPresent()) {
            return new Command(Code.FIRESHOT, ThreadLocalRandom.current().nextInt(0, gameState.PlayerMap.MapWidth), ThreadLocalRandom.current().nextInt(gameState.PlayerMap.MapHeight));
        }
        return new Command(Code.FIRESHOT, availableCell.get().X, availableCell.get().Y);
    }
}
