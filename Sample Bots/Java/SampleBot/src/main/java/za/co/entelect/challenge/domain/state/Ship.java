package za.co.entelect.challenge.domain.state;

import za.co.entelect.challenge.domain.command.ship.ShipType;

import java.util.ArrayList;

public class Ship {

    public boolean Destroyed;

    public boolean Placed;

    public ShipType ShipType;

    public ArrayList<Weapon> Weapons;

    public ArrayList<Cell> Cells;
}
