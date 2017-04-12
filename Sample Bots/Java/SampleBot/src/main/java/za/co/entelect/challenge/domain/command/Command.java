package za.co.entelect.challenge.domain.command;

import za.co.entelect.challenge.domain.command.code.Code;

public class Command {

    private Point coordinate;
    private Code commandCode;

    public Command(Code code, int x, int y) {

        this.commandCode = code;
        this.coordinate = new Point(x, y);
    }

    public Point getCoordinate() {

        return coordinate;
    }

    public void setCoordinate(Point coordinate) {

        this.coordinate = coordinate;
    }

    public Code getCommandCode() {

        return commandCode;
    }

    public void setCommandCode(Code commandCode) {

        this.commandCode = commandCode;
    }

    @Override
    public String toString() {

        return String.format("%s,%s,%s", commandCode.getValue(), coordinate.getX(), coordinate.getY());
    }
}
