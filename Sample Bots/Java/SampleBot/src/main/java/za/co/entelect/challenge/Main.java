package za.co.entelect.challenge;

import java.io.File;
import java.io.IOException;

public class Main {

    public static void main(String... args) {

        long startTime = System.nanoTime();

        runBot(args);

        long endTime = System.nanoTime();

        System.out.println(String.format("[Bot]\tBot finished in %d ms.", (endTime - startTime) / 1000000));
    }

    private static void runBot(String... args) {

        if (args.length != 2) {
            printUsage();
            System.exit(1);
        }

        File file = new File(args[1]);

        if (!file.exists()) {
            printUsage();
            System.out.println();
            System.out.println(String.format("Error: Working directory \" %s \" does not exist.", args[1]));
            System.exit(1);
        }

        Bot bot = new Bot(args[0], args[1]);
        try {
            bot.execute();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private static void printUsage() {

        System.out.println("Java SampleBot usage: sample-bot.jar <PlayerKey> <WorkingDirectoryFilename>");
        System.out.println();
        System.out.println("\tPlayerKey\tThe key assigned to your bot.");
        System.out.println("\tWorkingDirectoryFilename\tThe working directory folder where the match runner will output map and state files and look for the move file.");
    }
}
