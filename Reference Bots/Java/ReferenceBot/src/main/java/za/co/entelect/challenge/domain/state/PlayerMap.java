package za.co.entelect.challenge.domain.state;

import za.co.entelect.challenge.domain.command.Point;
import za.co.entelect.challenge.domain.command.direction.Direction;

import java.util.ArrayList;

public class PlayerMap {

    public BattleshipPlayer Owner;

    public ArrayList<Cell> Cells;

    public int MapWidth;

    public int MapHeight;

    public Cell getCellAt(int x, int y) {
        return Cells.stream().filter(p -> p.X == x && p.Y == y).findFirst().get();
    }

    public Cell getAdjacentCell(Cell cell, Direction direction) {
        if (cell == null) {
            return null;
        }

        switch (direction) {

            case North:
                return getCellAt(cell.X, cell.Y + 1);
            case East:
                return getCellAt(cell.X + 1, cell.Y);
            case South:
                return getCellAt(cell.X, cell.Y - 1);
            case West:
                return getCellAt(cell.X - 1, cell.Y);
            default:
                throw new IllegalArgumentException(String.format("The direction passed %s does not exist", direction));
        }
    }

    public ArrayList<Cell> getAllCellsInDirection(Point startLocation, Direction direction, int length) {
        Cell startCell = getCellAt(startLocation.getX(), startLocation.getY());
        ArrayList<Cell> cells = new ArrayList<>();
        cells.add(startCell);

        if (startCell == null) {
            return cells;
        }

        for (int i = 1; i < length; i++) {
            Cell nextCell = getAdjacentCell(startCell, direction);
            if (nextCell == null) {
                throw new IllegalArgumentException("Not enough cells for the requested length");
            }

            cells.add(nextCell);
            startCell = nextCell;
        }

        return cells;
    }

    public boolean hasCellsForDirection(Point startLocation, Direction direction, int length) {
        Cell startCell = getCellAt(startLocation.getX(), startLocation.getY());

        if (startCell == null) {
            return false;
        }

        for (int i = 1; i < length; i++) {
            Cell nextCell = getAdjacentCell(startCell, direction);
            if (nextCell == null) {
                return false;
            }
            startCell = nextCell;
        }
        return true;
    }
}
