﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Hex : MonoBehaviour {

	public static bool isEnabled = false; 

    //Stores coordinate in the map
    private int x;
    private int y;

    //Store who owns the hex
    public Player.PlayerId hexOwner;

    //Stores the hex's type
    private HexGrid.TileType tileType;

    //Stores the hex's income
    private float tileIncome;

    //Store the hex's sprites
    public Sprite standard;

    //Store the building on the hex, null if not one
    private Building building;

    //Set maxY and maxX of a hex, -1 for off by one error
    private int maxY = HexGrid.getGridHeight() - 1;
    private int maxX = HexGrid.getGridWidth() - 1;

    private int numOfWorkersOnHex;
    private int numOfWarriorsOnHex;

    //Calls once on object creation
    void Start()
    {
        building = null;

        //Initialize ints to store the units on the tile
        numOfWorkersOnHex = 0;
        numOfWarriorsOnHex = 0;

        //Set it's current sprite to the standard hex sprite
        changeHexSprite(standard);
    }

    void Update()
    {
        if(building != null)
        {
            building.progressConstruction();
        }
    }

    //Getters and setters
    public void setX(int x) { this.x = x; }

    public void setY(int y) { this.y = y; }

    public int getX() { return x; }

    public int getY() { return y; }

    public Player.PlayerId getHexOwner() { return hexOwner; }

    public void setHexOwner(Player.PlayerId pID) { this.hexOwner = pID; }

    public void setTileIncome(float amount) { this.tileIncome = amount; }

    public float getTileIncome() { return tileIncome; }

    public void setTileType(HexGrid.TileType type) { this.tileType = type; }

    public HexGrid.TileType getTileType() { return tileType; }

    public void setBuilding(Building b) {
        this.building = b;
        b.setHexAssociatedWith(this);
    }

    public Building getBuilding() { return building; }
    
    public int getNumOfWorkersOnHex() { return numOfWorkersOnHex; }

    public int getNumOfWarriorsOnHex() { return numOfWarriorsOnHex; }

    public void addWorkersToHex(int amount) {
        numOfWorkersOnHex += amount;
    }

    public void addWarriorsToHex(int amount) {
        numOfWarriorsOnHex += amount;
    }

    public void removeWorkersFromHex(int amount)
    {
        if(numOfWorkersOnHex >= amount)
        {
            numOfWorkersOnHex -= amount;
        }
        else
        {
            Debug.Log("Can't remove workers from hex since requested " + amount + " to be removed, and only " + numOfWorkersOnHex + " recorded to be located on this hex: " + this.name);
        }
    }

    public void removeWarriorsFromHex(int amount)
    {
        if (numOfWarriorsOnHex >= amount)
        {
            numOfWarriorsOnHex -= amount;
        }
        else
        {
            Debug.Log("Can't remove warriors from hex since requested " + amount + " to be removed, and only " + numOfWarriorsOnHex + " recorded to be located on this hex: " + this.name);
        }
    }
    //Method to return a list of a hex's direct neighbors
    public List <GameObject> getNeighbors(){
        List<GameObject> neighbors = new List<GameObject>();

        //Check there are hexes above this object, if not don't add since hex has no neighbors above
        if(y != 0)
            //Hex (N)
            neighbors.Add(GameObject.Find("Hex_" + x + "_" + (y - 1)));

        //Check there are hexes below this object, if not don't add since hex has no neighbors below.
        if(y!= maxY)
            //Hex (S)
            neighbors.Add(GameObject.Find("Hex_" + x + "_" + (y + 1)));

        //If even row(rows are at there highest here)
        if(x % 2 == 0)
        {
            //Check hexes to the left
            if (x != 0)
            {
                //Hex (SW)
                neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + y));
                //Check hexes above
                if (y != 0)
                    //Hex (NW)
                    neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + (y - 1)));
            }

            if(x != maxX)
            {
                //Hex (SE)
                neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + y));
                //Check hexes above
                if (y != 0)
                    //Hex (NE)
                    neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + (y - 1)));
            }
        }
        //Else odd row(rows are at there lowest here)
        else
        {
            //Check hexes to the left
            if (x != maxX)
            {
                //Hex (SE)
                neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + y));
                //Check hexes above
                if (y != maxY)
                    //Hex (NE)
                    neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + (y + 1)));
            }

            if (x != 0)
            {
                //Hex (SW)
                neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + y));
                //Check hexes above
                if (y != maxY)
                    //Hex (NW)
                    neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + (y + 1)));
            }
        }
        
        return neighbors;
    }

    //Method to return true if a neighbor is owned by player
    public bool hasOwnedNeighbor()
    {
        List<GameObject> neighbors = getNeighbors();

        foreach(GameObject gO in neighbors)
        {
            if (gO.GetComponent<Hex>())
            {
                if (gO.GetComponent<Hex>().getHexOwner().Equals(Player.PlayerId.P1)) //TODO: Currently softcoded for P1, need to set so works for anyplayer and passed in
                    return true;
            }
        }
        return false;
    }

    //Method to change the sprite of the hex
    public void changeHexSprite(Sprite s)
    {
        this.GetComponent<SpriteRenderer>().sprite = s;
    }
}
