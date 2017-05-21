package za.co.sample

import scala.io.Source

object GameMapParser {

  def readFile(fileName: String) : String = {
    Source.fromFile(fileName)("UTF-8").getLines().mkString
  }

  case class PlayerCells(
                    Occupied: Boolean,
                    Hit: Boolean,
                    X: Int,
                    Y: Int
                  )
  case class Weapons(
                      WeaponType: String
                    )
  case class PlayerShips(
                    Cells: List[PlayerCells],
                    Destroyed: Boolean,
                    Placed: Boolean,
                    ShipType: String,
                    Weapons: List[Weapons]
                  )
  case class Owner(
                    FailedFirstPhaseCommands: Int,
                    Name: String,
                    Ships: List[PlayerShips],
                    Points: Int,
                    Killed: Boolean,
                    IsWinner: Boolean,
                    ShotsFired: Int,
                    ShotsHit: Int,
                    ShipsRemaining: Int,
                    Key: String
                  )
  case class PlayerMap(
                        Cells: List[PlayerCells],
                        Owner: Owner,
                        MapWidth: Int,
                        MapHeight: Int
                      )
  case class OpponentShips(
                    Destroyed: Boolean,
                    ShipType: String
                  )
  case class OpponentCells(
                    Damaged: Boolean,
                    Missed: Boolean,
                    X: Int,
                    Y: Int
                  )
  case class OpponentMap(
                          Ships: List[OpponentShips],
                          Alive: Boolean,
                          Points: Int,
                          Name: String,
                          Cells: List[OpponentCells]
                        )
  case class GameState(
                             PlayerMap: PlayerMap,
                             OpponentMap: OpponentMap,
                             GameVersion: String,
                             GameLevel: Int,
                             Round: Int,
                             MapDimension: Int,
                             Phase: Int
                           )

  import org.json4s._
  import org.json4s.native.JsonMethods._
  implicit val formats = DefaultFormats

  def readJsonParse4s(fileName: String) : GameState = {
    val file = readFile(fileName)
    println(file)
    val json = parse(file)
    json.extract[GameState]
  }
}
