﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Hex : NetworkBehaviour {

	public static bool isDisabled = false; 

    //Stores coordinate in the map
	[SyncVar] private int x;
	[SyncVar] private int y;

    //Store who owns the hex
	[SyncVar] public Player.PlayerId hexOwner;

    //Stores the hex's type
	[SyncVar] private HexGrid.TileType tileType;

    //Stores the hex's income
	private float tileIncome;

    //Store the hex's sprites
    public Sprite standard;

    //Store the building on the hex, null if not one
    private Building building;

    //Set maxY and maxX of a hex, -1 for off by one error
    private int maxY = HexGrid.getGridHeight() - 1;
    private int maxX = HexGrid.getGridWidth() - 1;

	//Store Amount of Player 1 (Red) and Player 2 (Blue) Units
	private int redWarriors;
	private int redWorkers;
	private int blueWarriors;
	private int blueWorkers;

    //Calls once on object creation
    void Start()
    {
        building = null;

        //Initialize ints to store the units on the tile
		redWarriors = 0;
		redWorkers = 0;
		blueWarriors = 0;
		blueWorkers = 0;

        //Set it's current sprite to the standard hex sprite
        changeHexSprite(standard);
    }

    //Getters and setters
	//Set X
    public void setX(int x) { 
		this.x = x; 
	}

	//Set Y
    public void setY(int y) { 
		this.y = y; 
	}

	//Get X
    public int getX() { 
		return x; 
	}

	//Get Y
    public int getY() { 
		return y; 
	}

	//Get Hex Owner
    public Player.PlayerId getHexOwner() { 
		return hexOwner;
	}

	//Set Hex Owner
    public void setHexOwner(Player.PlayerId pID) { 
		this.hexOwner = pID; 
	}

	//Set Tile Income
    public void setTileIncome(float amount) { 
		this.tileIncome = amount;
	}

	//Get Tile Income
    public float getTileIncome() { 
		return tileIncome;
	}

	//Set Tile Type
    public void setTileType(HexGrid.TileType type) { 
		this.tileType = type; 
	}

	//Get Tile Type
    public HexGrid.TileType getTileType() { 
		return tileType; 
	}
		
	[Command]
	public void CmdSetBuilding(Building.BuildingType buildingId) {
		// Update building on server
		setBuilding (buildingId);
		// Update building on all clients
		RpcSetBuilding (buildingId);
	}

	[ClientRpc]
	private void RpcSetBuilding(Building.BuildingType buildingId) {
		setBuilding (buildingId);
	}

	//Set Building
	private void setBuilding(Building.BuildingType buildingId) {
		this.building = FindObjectOfType<BuildingController>().getBuildingFromType(buildingId);
        building.setHexAssociatedWith(this);
    }

	//Get Building
    public Building getBuilding() { 
		return building;
	}
    
	//Get Number of Workers on Hex
	public int getNumOfWorkersOnHex(Player.PlayerId player) {
		int result = 0;
		//If Player 1, get number of redWorkers
		if (player == Player.PlayerId.P1) {
			result = redWorkers;
		} 
		//If Player 2, get number of blueWorkers
		else if (player == Player.PlayerId.P2) {
			result = blueWorkers;
		}
		return result; 
	}

	//Get Number of Warriors on Hex
	public int getNumOfWarriorsOnHex(Player.PlayerId player) { 
		int result = 0;
		//If Player 1, get number of redWarriors
		if (player == Player.PlayerId.P1) {
			result = redWarriors;
		} 
		//If Player 2, get number of blueWarriors
		else if (player == Player.PlayerId.P2) {
			result = blueWarriors;
		}
		return result;  
	}

	//Add Number of Workers to Hex
	public void addWorkersToHex(int amount, Player.PlayerId player) {
		//If Player 1, add to redWorkers
		if (player == Player.PlayerId.P1) {
			redWorkers += amount;
		} 
		//If Player 2, add to blueWorkers
		else if (player == Player.PlayerId.P2) {
			blueWorkers += amount;
		}
    }

	//Add Number of Warriors to Hex
	public void addWarriorsToHex(int amount, Player.PlayerId player) {
		//If Player 1, add to redWarriors
		if (player == Player.PlayerId.P1) {
			redWarriors += amount;
		} 
		//If Player 2, add to blueWarriors
		else if (player == Player.PlayerId.P2) {
			blueWarriors += amount;
		}
    }

	//Remove Workers from Hex
	public void removeWorkersFromHex(int amount, Player.PlayerId player) {
		//If Player 1, remove redWorkers
		if (player == Player.PlayerId.P1) {
			if(redWorkers >= amount) {
				redWorkers -= amount;
			}
			else
			{
				Debug.Log("Can't remove workers from hex since requested " + amount + " to be removed, and only " + redWorkers + " recorded to be located on this hex: " + this.name);
			}
		} 
		//If Player 2, remove blueWorkers
		else if (player == Player.PlayerId.P2) {
			if(blueWorkers >= amount) {
				blueWorkers -= amount;
			}
			else
			{
				Debug.Log("Can't remove workers from hex since requested " + amount + " to be removed, and only " + blueWorkers + " recorded to be located on this hex: " + this.name);
			}
		}
    }

	//Remove Warriors from Hex
	public void removeWarriorsFromHex(int amount, Player.PlayerId player) {
		//If Player 1, remove redWorkers
		if (player == Player.PlayerId.P1) {
			if(redWarriors >= amount) {
				redWarriors -= amount;
			}
			else
			{
				Debug.Log("Can't remove workers from hex since requested " + amount + " to be removed, and only " + redWarriors + " recorded to be located on this hex: " + this.name);
			}
		} 
		//If Player 2, remove blueWorkers
		else if (player == Player.PlayerId.P2) {
			if(blueWarriors >= amount) {
				blueWarriors -= amount;
			}
			else
			{
				Debug.Log("Can't remove workers from hex since requested " + amount + " to be removed, and only " + blueWarriors + " recorded to be located on this hex: " + this.name);
			}
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
	public bool hasOwnedNeighbor(Player.PlayerId player)
    {
        List<GameObject> neighbors = getNeighbors();

        foreach(GameObject gO in neighbors)
        {
            if (gO.GetComponent<Hex>())
            {
				if (gO.GetComponent<Hex>().getHexOwner().Equals(player)) 
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

	public Sprite getSprite(){
		return standard;
	}



	[ClientRpc]
	public void RpcResetSprite(){
		changeHexSprite (standard);
	}

}
