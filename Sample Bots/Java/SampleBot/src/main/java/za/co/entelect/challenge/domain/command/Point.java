package za.co.entelect.challenge.domain.command;


public class Point {

    private int x;

    public int getX() {

        return x;
    }

    public void setX(int x) {

        this.x = x;
    }

    public int getY() {

        return y;
    }

    public void setY(int y) {

        this.y = y;
    }

    private int y;

    public Point(int x, int y) {

        this.x = x;
        this.y = y;
    }
}
