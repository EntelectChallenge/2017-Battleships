package za.co.entelect.challenge.strategy;

import za.co.entelect.challenge.domain.command.PlaceShipCommand;
import za.co.entelect.challenge.domain.command.Point;
import za.co.entelect.challenge.domain.command.direction.Direction;
import za.co.entelect.challenge.domain.command.ship.ShipType;
import za.co.entelect.challenge.domain.state.Cell;
import za.co.entelect.challenge.domain.state.GameState;

import java.util.*;
import java.util.concurrent.ThreadLocalRandom;

public class RandomPlacementStrategy {

    public PlaceShipCommand getShipPlacement(GameState gameState) {
        HashMap shipSizes = new HashMap<ShipType, Integer>();
        shipSizes.put(ShipType.Battleship, 4);
        shipSizes.put(ShipType.Carrier, 5);
        shipSizes.put(ShipType.Cruiser, 3);
        shipSizes.put(ShipType.Destroyer, 2);
        shipSizes.put(ShipType.Submarine, 3);

        ArrayList<ShipType> shipPlacements = new ArrayList<>();
        ArrayList<Point> points = new ArrayList<>();
        ArrayList<Direction> directions = new ArrayList<>();

        Iterator it = shipSizes.entrySet().iterator();
        while (it.hasNext()) {
            Map.Entry<ShipType, Integer> pair = (Map.Entry) it.next();
            ShipWithSize shipWithSize = new ShipWithSize(pair.getKey(), pair.getValue());

            Point location = new Point(ThreadLocalRandom.current().nextInt(0, gameState.PlayerMap.MapWidth - 1), ThreadLocalRandom.current().nextInt(0, gameState.PlayerMap.MapHeight - 1));

            Direction direction;

            CanPlace canPlace = tryToPlace(gameState, shipWithSize.shipSize, location);

            if (canPlace.canPlace) {
                direction = canPlace.direction;
                shipPlacements.add(shipWithSize.shipType);
                points.add(location);
                directions.add(direction);
            }

            it.remove();
        }

        return new PlaceShipCommand(shipPlacements, points, directions);
    }

    private CanPlace tryToPlace(GameState gameState, int size, Point location) {

        ArrayList<Direction> directions = new ArrayList<>();
        directions.add(Direction.North);
        directions.add(Direction.East);
        directions.add(Direction.South);
        directions.add(Direction.West);

        Collections.shuffle(directions);

        for (Direction testDirection : directions) {
            if (!gameState.PlayerMap.hasCellsForDirection(location, testDirection, size)) {
                continue;
            }

            ArrayList<Cell> cells = gameState.PlayerMap.getAllCellsInDirection(location, testDirection, size);
            if (cells.stream().anyMatch(x -> x.Occupied)) {
                continue;
            }

            cells.forEach((x) -> x.Occupied = true);

            return new CanPlace(true, testDirection);
        }

        return new CanPlace(false, Direction.West);
    }

    class CanPlace {

        public boolean canPlace;
        public Direction direction;

        public CanPlace(boolean canPlace, Direction direction) {
            this.canPlace = canPlace;

            this.direction = direction;
        }
    }

    class ShipWithSize {
        public ShipType shipType;
        public int shipSize;

        public ShipWithSize(ShipType shipType, int shipSize) {

            this.shipType = shipType;
            this.shipSize = shipSize;
        }
    }
}

