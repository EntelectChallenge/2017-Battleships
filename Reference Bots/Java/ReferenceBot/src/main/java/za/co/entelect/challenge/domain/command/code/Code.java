package za.co.entelect.challenge.domain.command.code;

public enum Code {
    DO_NOTHING(0),
    FIRESHOT(1),
    DOUBLESHOT(2),
    CORNERSHOT(3),
    CROSSSHOTDIAGOL(4),
    CROSSSHOTHORIZONTAL(5),
    SEEKERMISSLE(6);


    private final int value;
    private Code(int value) {
        this.value = value;
    }

    public int getValue() {
        return value;
    }
}
