#
import time

startTime = time.time()

import argparse
import json
import logging
import os
from time import sleep
from random import randint
import math

logger = logging.getLogger()
logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(levelname)-7s - [%(funcName)s] %(message)s')
# uncomment for submission
logger.disabled = True


def main(player_key, output_path):
    # Load STATE
    with open(os.path.join(output_path, 'state.json'), 'r') as f:
        originalState = f.read().decode('utf-8-sig')
    gameState = json.loads(originalState)

    # Make a plan
    print makeMove(gameState)
    if gameState['Phase'] == 1:
        print "Placement phase"
        p = placeShips()
        print 'p: ', p
        action = p
    else:
        action = makeMove(gameState)
        print "Fight phase"

    # Execute my plan
    if gameState['Phase'] == 1:
        with open(os.path.join(output_path, 'place.txt'), 'w') as f:
            placement = ""
            for r in range(0,len(p[0])):
                placement += "%s %s %s %s\n" % (p[0][r], p[1][r][0], p[1][r][1], p[2][r])
            f.write('{}\n'.format(placement))
    else:
        with open(os.path.join(output_path, 'command.txt'), 'w') as f:
            command = "%s,%s,%s" % (action[0], action[1], action[2])
            f.write('{}\n'.format(command))

class ShipType:
    Battleship = 'Battleship'
    Carrier = 'Carrier'
    Cruiser = 'Cruiser'
    Destroyer = 'Destroyer'
    Submarine = 'Submarine'


class Direction:
    North = 'North'
    East = 'East'
    South = 'South'
    West = 'West'


def makeMove(gameState):
    move = randint(0,1)
    mapDimension = gameState['MapDimension']
    x = randint(0, mapDimension)
    y = randint(0, mapDimension)
    return (move, x, y)


def placeShips():
    shipsToPlace = []
    shipsToPlace.append(ShipType.Battleship)
    shipsToPlace.append(ShipType.Carrier)
    shipsToPlace.append(ShipType.Cruiser)
    shipsToPlace.append(ShipType.Destroyer)
    shipsToPlace.append(ShipType.Submarine)

    points = []
    points.append((1, 0))
    points.append((3, 1))
    points.append((4, 2))
    points.append((7, 3))
    points.append((1, 8))

    directions = []
    directions.append(Direction.North)
    directions.append(Direction.East)
    directions.append(Direction.North)
    directions.append(Direction.North)
    directions.append(Direction.East)

    return (shipsToPlace, points, directions)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('player_key', nargs='?')
    parser.add_argument('output_path', nargs='?', default=os.getcwd())
    parser.add_argument('--user', nargs='?', default=os.getcwd())
    args = parser.parse_args()
    assert(os.path.isdir(args.output_path))
    main(args.player_key, args.output_path)
    logger.info("end difference: %s" % (time.time() - startTime))
