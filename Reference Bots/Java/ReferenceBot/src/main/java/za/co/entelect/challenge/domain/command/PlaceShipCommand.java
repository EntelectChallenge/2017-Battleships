package za.co.entelect.challenge.domain.command;

import za.co.entelect.challenge.domain.command.direction.Direction;
import za.co.entelect.challenge.domain.command.ship.ShipType;

import java.util.ArrayList;

public class PlaceShipCommand {

    private ArrayList<ShipType> shipTypes;
    private ArrayList<Point> points;
    private ArrayList<Direction> directions;

    public PlaceShipCommand(ArrayList<ShipType> shipTypes, ArrayList<Point> points, ArrayList<Direction> directions) {

        this.shipTypes = shipTypes;
        this.points = points;
        this.directions = directions;
    }

    public ArrayList<ShipType> getShipTypes() {

        return shipTypes;
    }

    public void setShipTypes(ArrayList<ShipType> shipTypes) {

        this.shipTypes = shipTypes;
    }

    public ArrayList<Point> getPoints() {

        return points;
    }

    public void setPoints(ArrayList<Point> points) {

        this.points = points;
    }

    public ArrayList<Direction> getDirections() {

        return directions;
    }

    public void setDirections(ArrayList<Direction> directions) {

        this.directions = directions;
    }

    @Override
    public String toString() {

        StringBuilder output = new StringBuilder();

        for (int i = 0; i < shipTypes.size(); i++) {
            output.append(String.format("%s %s %s %s",
                    shipTypes.get(i),
                    points.get(i).getX(),
                    points.get(i).getY(),
                    directions.get(i)));
            if (i + 1 != shipTypes.size()) {
                output.append("\r\n");
            }
        }
        return output.toString();
    }
}
