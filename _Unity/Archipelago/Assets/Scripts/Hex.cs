using System.Collections;
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

    //Store the Image component of the hex status icon
    private Image hexStatusIcon;

    //Store the Image components of the hex construction progress bar
    private Image hexConstructionBarBG;
    private Image hexConstructionBarFill;

    //Store the building on the hex, null if not one
    private Building building;

    //Set maxY and maxX of a hex, -1 for off by one error
    private int maxY = HexGrid.getGridHeight() - 1;
    private int maxX = HexGrid.getGridWidth() - 1;

	//Store Amount of Player 1 (Red) and Player 2 (Blue) Units
	private int redWarriors, redWorkers, redDamage, redWarriorHealth, redWorkerHealth;
	private int blueWarriors, blueWorkers, blueDamage, blueWarriorHealth, blueWorkerHealth;


    //Calls once on object creation
    void Start()
    {
        building = null;

        //Initialize ints to store the units on the tile
		redWarriors = 0;
		redWorkers = 0;
		redDamage = 0;
		redWarriorHealth = 0;
		redWorkerHealth = 0;
		blueWarriors = 0;
		blueWorkers = 0;
		blueDamage = 0;
		blueWarriorHealth = 0;
		blueWorkerHealth = 0;

        //If not a water tile, since doesn't hold the canvas object or have interactibility
        if (this.getTileType() != HexGrid.TileType.WATER)
        {
            //Connect the respective image parameters to their child hex 'Image' components
            hexStatusIcon = gameObject.transform.Find("Canvas/tileStatusIcon").GetComponent<Image>();
            hexConstructionBarBG = gameObject.transform.Find("Canvas/tileConstructionBar").GetComponent<Image>();
            hexConstructionBarFill = gameObject.transform.Find("Canvas/tileConstructionBar/Fill").GetComponent<Image>();
        }

        //Set it's current sprite to the standard hex sprite
        changeHexSprite(standard);
    }

    //Calls once on every frame
    void Update()
    {   
        //If there is a building on the hex and it isn't yet constructed
        if (building != null && !building.getIsConstructed())
        {
            //Progress the buildings construction
            building.progressConstruction();
            hexConstructionBarFill.GetComponent<RectTransform>().localScale = new Vector2(building.getCurrentBuildTime() / building.getTotalBuildTime(), 1);
        }
        
    }

    public void enableConstructionBar()
    {
        hexConstructionBarBG.enabled = true;
        hexConstructionBarFill.enabled = true;
    }

    public void disableConstructionBar()
    {
        hexConstructionBarBG.enabled = false;
        hexConstructionBarFill.enabled = false;
    }

    public void enableStatusIcon()
    {
        hexStatusIcon.enabled = true;
    }

    public void disableStatusIcon()
    {
        hexStatusIcon.enabled = false;
    }

    public void setStatusIcon(Sprite s)
    {
        hexStatusIcon.sprite = s;
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
		Building buildingPrefab = FindObjectOfType<BuildingController>().getBuildingFromType(buildingId);
		//Instantiate a new Building object for the building.
		//Allows it to hold it's own values, rather than statically for all buildings of same type
		this.building = (Building)Instantiate(buildingPrefab);
		//Associate this hex with the building
		building.setHexAssociatedWith(this);
    }

	//Get Building
    public Building getBuilding() { 
		return building;
	}

	//Get Health
	public int getHealth(Player.PlayerId player) {
		int result = 0;
		//If Player 1, get redHealth
		if (player == Player.PlayerId.P1) {
			result = redWarriorHealth + redWorkerHealth;
		} 
		//If Player 2, get blueHealth
		else if (player == Player.PlayerId.P2) {
			result = blueWarriorHealth + blueWorkerHealth;
		}
		return result; 
	}

	//Get Damage
	public int getDamage(Player.PlayerId player) {
		int result = 0;
		//If Player 1, get redDamage
		if (player == Player.PlayerId.P1) {
			result = redDamage;
		} 
		//If Player 2, get blueDamage
		else if (player == Player.PlayerId.P2) {
			result = blueDamage;
		}
		return result; 
	}		

	//Set Damage
	public void setDamage(int amount, Player.PlayerId player) {
		//If Player 1, set redDamage
		if (player == Player.PlayerId.P1) {
			redDamage = amount;
		} 
		//If Player 2, set blueDamage
		else if (player == Player.PlayerId.P2) {
			blueDamage = amount;
		}
	}

	//Do Damage, Performs damage to both factions and updates units, health, and damage respectively
	public void doDamage() {
		int tempRedHealth = redWarriorHealth + redWorkerHealth;
		int tempRedDamage = redDamage;
		int tempBlueHealth = blueWarriorHealth + blueWorkerHealth;
		int tempBlueDamage = blueDamage;
		//Red Faction Updated
		//No Health remaining after attack
		if (tempRedHealth - tempBlueDamage <= 0) {
			redDamage = 0;
			redWorkerHealth = 0;
			redWorkers = 0;
			redWarriorHealth = 0;
			redWarriors = 0;
		} 
		//Some health remaining after attack
		else {
			//Red Warrior have no health left
			if (redWarriorHealth - tempBlueDamage <= 0) {
				redWorkerHealth -= (tempBlueDamage - redWarriorHealth);
				redWorkers = (int)((redWorkerHealth / 3) + 1);
				redWarriorHealth = 0;
				redWarriors = 0;
				redDamage = 0;
			} 
			//Red Warriors have some health remaining
			else {
				redWarriors = (int)((redWarriorHealth / 6) + 1);
				redDamage = redWarriors;
				redWarriorHealth -= tempBlueDamage;
				//Because warriors have health remaining workers are unaffected
			}

		}
		//Blue Faction Updated
		//No Health remaining after attack
		if (tempBlueHealth - tempRedDamage <= 0) {
			blueDamage = 0;
			blueWorkerHealth = 0;
			blueWorkers = 0;
			blueWarriorHealth = 0;
			blueWarriors = 0;
		} 
		//Some health remaining after attack
		else {
			//Blue Warrior have no health left
			if(blueWarriorHealth - tempRedDamage <= 0) {
				blueWorkerHealth -= (tempRedDamage - blueWarriorHealth);
				blueWorkers = (int)((blueWorkerHealth / 3) + 1);
				blueWarriorHealth = 0;
				blueWarriors = 0;
				blueDamage = 0;
			}
			//Blue Warrior have some health remaining
			else {
				blueWarriors = (int)((blueWarriorHealth / 6) + 1);
				blueDamage = blueWarriors;
				blueWarriorHealth -= tempRedDamage;
			}
		}
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
			redWorkerHealth += (amount * 3);
		} 
		//If Player 2, add to blueWorkers
		else if (player == Player.PlayerId.P2) {
			blueWorkers += amount;
			blueWorkerHealth += (amount * 3);
		}
    }

	//Add Number of Warriors to Hex
	public void addWarriorsToHex(int amount, Player.PlayerId player) {
		//If Player 1, add to redWarriors
		if (player == Player.PlayerId.P1) {
			redWarriors += amount;
			redWarriorHealth += (amount * 6);
			redDamage += amount;
		} 
		//If Player 2, add to blueWarriors
		else if (player == Player.PlayerId.P2) {
			blueWarriors += amount;
			blueWarriorHealth += (amount * 6);
			blueDamage += amount;
		}
    }

	//Remove Workers from Hex
	public void removeWorkersFromHex(int amount, Player.PlayerId player) {
		//If Player 1, remove redWorkers
		if (player == Player.PlayerId.P1) {
			if(redWorkers >= amount) {
				redWorkers -= amount;
				redWorkerHealth -= (amount * 3);
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
				blueWorkerHealth -= (amount * 3);
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
				redWarriorHealth -= (amount * 6);
				redDamage -= amount;
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
				blueWarriorHealth -= (amount * 6);
				blueDamage -= amount;
			}
			else
			{
				Debug.Log("Can't remove workers from hex since requested " + amount + " to be removed, and only " + blueWarriors + " recorded to be located on this hex: " + this.name);
			}
		}
    }

    public bool hasEnemyWarriors(Player.PlayerId player)
    {
        if (player == Player.PlayerId.P1)
        {
            if (blueWarriors > 0)
            {
                return true;
            }
        }
        else
        {
            if (redWarriors > 0)
            {
                return true;
            }
        }

        return false;
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

}
