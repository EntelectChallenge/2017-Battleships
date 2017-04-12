package za.co.entelect.challenge.domain.state;


import za.co.entelect.challenge.domain.command.Point;

public class OpponentCell {

    public boolean Damaged;

    public boolean Missed;

    public int X;

    public int Y;

    public Point getPoint() {

        return new Point(X, Y);
    }
}
