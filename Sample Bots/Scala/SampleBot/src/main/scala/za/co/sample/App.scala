package za.co.sample

import java.io._
import za.co.sample.GameMapParser.GameState

object App {

  val r = scala.util.Random

  def doPlace(botDir: String, map: GameState): Unit = {
    val pw = new PrintWriter(new File(botDir + File.separator + "place.txt" ))
    pw.write("Battleship 7 0 West\n")
    pw.write("Carrier 6 3 West\n")
    pw.write("Cruiser 2 4 East\n")
    pw.write("Destroyer 6 7 East\n")
    pw.write("Submarine 8 7 South\n")
    pw.close
  }

  def doShoot(botDir: String, map: GameState): Unit = {
    val pw = new PrintWriter(new File(botDir + File.separator + "command.txt" ))
    pw.write("1,%d,%d".format(r.nextInt(map.MapDimension),r.nextInt(map.MapDimension)))
    pw.close
  }

  def main(args : Array[String]) {
    val startTime = System.currentTimeMillis()

    val botKey = args(0)
    val botDir = args(1).replace("\"","")

    println("Bot Key: " + botKey)
    println("Bot Dir: " + botDir)

    val name = botDir + File.separator + "state.json"

    println("File: " + name)

    val map = GameMapParser.readJsonParse4s(name)

    map.Phase match {
      case 1 => doPlace(botDir, map)
      case 2 => doShoot(botDir, map)
    }

    val endTime   = System.currentTimeMillis()
    val totalTime = endTime - startTime
    println("Total execution time: " + totalTime)
    System.exit(0)
  }
}
