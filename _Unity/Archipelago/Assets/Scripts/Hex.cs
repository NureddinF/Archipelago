using System;
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
	[SyncVar (hook = "updateHexOwner")] public Player.PlayerId hexOwner;

    //Stores the hex's type
	[SyncVar] private HexGrid.TileType tileType;

    //Stores the hex's income
	private float tileIncome;

    //Store the hex's sprites
    public Sprite standard;

    //Store the Image component of the hex status icon's
    private Image hexStatusIcon;
    private Image hexFightingIcon;

    //Store the Image components of the hex construction progress bar
    private Image hexConstructionBarBG;
    private Image hexConstructionBarFill;
    private Image hexFightingBarRed;
    private Image hexFightingBarBlue;

    //Store the building on the hex, null if not one
    private Building building;
	[SyncVar(hook = "setBuilding")]private GameObject buildingObject; //need gameobject version for networking

    //Set maxY and maxX of a hex, -1 for off by one error
    private int maxY = HexGrid.getGridHeight() - 1;
    private int maxX = HexGrid.getGridWidth() - 1;

	//Store Amount of Player 1 (Red) and Player 2 (Blue) Units
	[SyncVar(hook = "setRedWarriors")] private int redWarriors;
	[SyncVar(hook = "setRedWorkers")] private int redWorkers;
	private int redDamage, currentRedWarriorHealth, currentRedWorkerHealth, maxRedWarriorHealth, maxRedWorkerHealth;
	[SyncVar(hook = "setBlueWarriors")] private int blueWarriors;
	[SyncVar(hook = "setBlueWorkers")] private int blueWorkers;
	private int blueDamage, currentBlueWarriorHealth, currentBlueWorkerHealth, maxBlueWarriorHealth, maxBlueWorkerHealth;

    //The amount of health the unit types start with each
    private int workerHealth = 3;
    private int warriorHealth = 6;

	//Used for the battle loop function
	private bool battleStarted = false;
	private bool timeFrameStart = false;
	private float timePeriod;
	private const float timeFrame = 1f;
	private UnitController p1UnitCtrl;
	private UnitController p2UnitCtrl;

	//////////////////////////////// MonoBehaviour Methods ////////////////////////////////////////

    //Calls once on object creation
    void Start(){
        building = null;

        //Initialize ints to store the units on the tile
		redWarriors = 0;
		redWorkers = 0;
		redDamage = 0;
		currentRedWarriorHealth = 0;
		currentRedWorkerHealth = 0;
		maxRedWarriorHealth = 0;
		maxRedWorkerHealth = 0;
		blueWarriors = 0;
		blueWorkers = 0;
		blueDamage = 0;
		currentBlueWarriorHealth = 0;
		currentBlueWorkerHealth = 0;
		maxBlueWarriorHealth = 0;
		maxBlueWorkerHealth = 0;

        //If not a water tile, since doesn't hold the canvas object or have interactibility
        if (this.getTileType() != HexGrid.TileType.WATER) {
            //Connect the respective image parameters to their child hex 'Image' components
            hexStatusIcon = gameObject.transform.Find("Canvas/tileStatusIcon").GetComponent<Image>();
            hexConstructionBarBG = gameObject.transform.Find("Canvas/tileConstructionBar").GetComponent<Image>();
            hexConstructionBarFill = gameObject.transform.Find("Canvas/tileConstructionBar/Fill").GetComponent<Image>();
            hexFightingBarRed = gameObject.transform.Find("Canvas/tileFightingBar/Red").GetComponent<Image>();
            hexFightingBarBlue = gameObject.transform.Find("Canvas/tileFightingBar/Blue").GetComponent<Image>();
            hexFightingIcon = gameObject.transform.Find("Canvas/tileFightingIcon").GetComponent<Image>();
        }

        //Set it's current sprite to the standard hex sprite
		GetComponent<SpriteRenderer>().sprite = standard;
    }

    //Calls once on every frame
    void Update(){

		if (isServer) {
			serverUpdate ();
		}
		if(isClient){
			clientUpdate ();
		}
        
    }


	//////////////////////////////// Commands and Server Methods ////////////////////////////////////////

	//update game stae
	private void serverUpdate(){

		if(blueWarriors + blueWorkers > 0 && redWorkers + redWarriors > 0){
			if(!battleStarted){
				beginBattle ();
			}
			progressBattle ();
		}
		//If there is a building on the hex and it isn't yet constructed
		else if (building != null && !building.getIsConstructed()){
			//Progress the buildings construction
			building.progressConstruction();
		}
	}
		
	[Command]
	public void CmdSetBuilding(Building.BuildingType buildingId) {
		GameObject buildingPrefab = FindObjectOfType<BuildingController>().getBuildingFromType(buildingId);
		//Instantiate a new Building object for the building.
		//Allows it to hold it's own values, rather than statically for all buildings of same type
		GameObject buildingObjectInstance = Instantiate(buildingPrefab,transform);
		NetworkServer.Spawn (buildingObjectInstance);
		buildingObject = buildingObjectInstance;
		building = buildingObject.GetComponent<Building> ();

		//Associate this hex with the building
		building.CmdSetHexAssociatedWith(gameObject);
	}


	private void beginBattle(){
		battleStarted = true;
		//update clients UI
		RpcEnableCombatBar ();

		//initialise battle stats
		currentRedWarriorHealth = maxRedWarriorHealth;
		currentRedWorkerHealth = maxRedWorkerHealth;
		currentBlueWarriorHealth = maxBlueWarriorHealth;
		currentBlueWorkerHealth = maxBlueWorkerHealth;

		RpcUpdateCombatBar (currentRedWarriorHealth , currentBlueWarriorHealth );

		// Get the UnitControler for each player so units can properly be killed
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for(int i=0; i<players.Length; i++){
			Player player = players[i].GetComponentInChildren<Player>();
			if (player.playerId.Equals (Player.PlayerId.P1)) {
				p1UnitCtrl = player.GetComponent<UnitController>();
			} else if (player.playerId.Equals (Player.PlayerId.P2)) {
				p2UnitCtrl = player.GetComponent<UnitController>();
			}
		}
		//There should always be a unit controller for eahc player
		if(p1UnitCtrl == null){
			Debug.LogError ("Hex: beginBattle: could not find player 1 unit controller");
		}
		if(p2UnitCtrl == null){
			Debug.LogError ("Hex: beginBattle: could not find player 2 unit controller");
		}
	}

	//Fight-Battle Sequence
	private void progressBattle() {
		//If a new timeframe hasnt begun start one
		if(!timeFrameStart) {
			timePeriod = Time.time;
			timeFrameStart = true;
		}
		//If the timeframe has been reached doDamage and reset timeFrameStart
		else if(timePeriod + timeFrame <= Time.time) {
			doDamage ();
			timeFrameStart = false;
		}
	}

	//////////////////////////////// RPCs and Client Methods //////////////////////////////////////////

	private void clientUpdate (){
		//If there is a building on the hex and it isn't yet constructed
		if (building != null && !building.getIsConstructed()){
			// Update Construction UI
			hexConstructionBarFill.GetComponent<RectTransform>().localScale = new Vector2(building.getCurrentBuildTime() / building.getTotalBuildTime(), 1);
		}
	}

    //Method to enable the combat bar and icon for this hex
	[ClientRpc]
    public void RpcEnableCombatBar(){
        hexFightingBarBlue.enabled = true;
        hexFightingBarRed.enabled = true;
        hexFightingIcon.enabled = true;
    }

    //Method to disable the combat bar and icon for this hex
	[ClientRpc]
    public void RpcDisableCombatBar(){
        hexFightingBarBlue.enabled = false;
        hexFightingBarRed.enabled  = false;
        hexFightingIcon.enabled = false;
    }


	//Method to enable the construction bar for this hex
	[ClientRpc]
	public void RpcEnableConstructionBar(){
		hexConstructionBarBG.enabled = true;
		hexConstructionBarFill.enabled = true;
	}

	//Method to disable the construction bar for this hex
	[ClientRpc]
	public void RpcDisableConstructionBar(){
		hexConstructionBarBG.enabled = false;
		hexConstructionBarFill.enabled = false;
	}

	//Method to enable the status icon for this hex
	[ClientRpc]
	public void RpcEnableStatusIcon(){
		hexStatusIcon.enabled = true;
	}

	//Method to disable the status icon for this hex
	[ClientRpc]
	public void RpcDisableStatusIcon(){
		hexStatusIcon.enabled = false;
	}

	// Method to change the sprite of the hex to the build
	[ClientRpc]
	public void RpcDisplayBuildingSprite(){
		GetComponent<SpriteRenderer> ().sprite = building.getBuildingSprite (hexOwner);
	}


	// Method to change the sprite of the hex to the build
	[ClientRpc]
	public void RpcDisplayTrap(){
		setStatusIcon(building.getBuildingSprite(hexOwner));
	}

    //Method to update the combat bar for this hex
	[ClientRpc]
	public void RpcUpdateCombatBar(int redHealth, int blueHealth){
		int totalHealth = redHealth + blueHealth;
		hexFightingBarBlue.GetComponent<RectTransform>().localScale = new Vector2((float)blueHealth / (float)totalHealth, 1);
		hexFightingBarRed.GetComponent<RectTransform>().localScale = new Vector2((float)redHealth / (float)totalHealth, 1);
    }
		


	//////////////////////////////// Getters and setters////////////////////////////////////////////
    
	//Method to set the status icon for this hex
	private void setStatusIcon(Sprite s){
		hexStatusIcon.sprite = s;
	}


	public Sprite getSprite(){
		return standard;
	}

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

	//Get Building
    public Building getBuilding() { 
		return building;
	}
		
	///////////////////////////////////////// Custom Methods ///////////////////////////////////////////

	//Do Damage, Performs damage to both factions and updates units, health, and damage respectively
	public void doDamage() {
		int tempRedHealth = currentRedWarriorHealth + currentRedWorkerHealth;
		int tempRedDamage = redDamage;
		int tempBlueHealth = currentBlueWarriorHealth + currentBlueWorkerHealth;
		int tempBlueDamage = blueDamage;

		Debug.Log ("Hex: doDamage:" + Environment.NewLine
			+ "\ttempRedHealth=\t" + tempRedHealth + Environment.NewLine
			+ "\ttempBlueHealth\t" + tempBlueHealth + Environment.NewLine
			+ "\ttempRedDamage=\t" + tempRedDamage + Environment.NewLine
			+ "\ttempBlueDamage=\t" + tempBlueDamage + Environment.NewLine
			+ "\tredWarriors=\t" + redWarriors + Environment.NewLine
			+ "\tblueWarriors=\t" + blueWarriors + Environment.NewLine
			+ "\tredWorkers=\t" + redWorkers + Environment.NewLine
			+ "\tblueWorkers=\t" + blueWorkers + Environment.NewLine);

		//Red Faction Updated
		if (tempRedHealth - tempBlueDamage <= 0) {
			//No Health remaining after attack
			p1UnitCtrl.CmdRemoveWorkers (redWorkers, gameObject);
			p1UnitCtrl.CmdRemoveWarriors (redWarriors, gameObject);
			Debug.Log ("Hex: doDamage: all red units died");
        } 
		//Some health remaining after attack
		else {
			if (currentRedWarriorHealth - tempBlueDamage <= 0) {
				//Red Warrior have no health left, do damage to workers
				currentRedWorkerHealth -= (tempBlueDamage - currentRedWarriorHealth);
				int remainingWorkers = (int)Mathf.Ceil(currentRedWorkerHealth / ((float) workerHealth));
				p1UnitCtrl.CmdRemoveWorkers (redWorkers-remainingWorkers, gameObject);
				p1UnitCtrl.CmdRemoveWarriors (redWarriors, gameObject);
			} 
			else {
				//Red Warriors have some health remaining, damage them
				currentRedWarriorHealth -= tempBlueDamage;
				int remainingWarriors = (int)Mathf.Ceil(currentRedWarriorHealth / ((float)warriorHealth));
				p1UnitCtrl.CmdRemoveWarriors (redWarriors - remainingWarriors, gameObject);
				Debug.Log ("Hex: doDamage: red warriors damaged. Remaining warriors=" + remainingWarriors);
				//Because warriors have health remaining workers are unaffected
			}

		}
        //Blue Faction Updated
        if (tempBlueHealth - tempRedDamage <= 0) {
			//No Health remaining after attack
			p2UnitCtrl.CmdRemoveWorkers (blueWorkers, gameObject);
			p2UnitCtrl.CmdRemoveWarriors (blueWarriors, gameObject);
			Debug.Log ("Hex: doDamage: all red units died");
        }
        else {
			//Some health remaining after attack
            if (currentBlueWarriorHealth - tempRedDamage <= 0) {
				//Blue Warrior have no health left, damage workers
                currentBlueWorkerHealth -= (tempRedDamage - currentBlueWarriorHealth);
				int remainingWorkers = (int)Mathf.Ceil(currentBlueWorkerHealth / ((float)workerHealth));
				p2UnitCtrl.CmdRemoveWorkers (blueWorkers - remainingWorkers, gameObject);
				p2UnitCtrl.CmdRemoveWarriors (blueWarriors, gameObject);
            }
            else {
				//Blue Warrior have some health remaining, damage them
                currentBlueWarriorHealth -= tempRedDamage;
				int remainingWarriors = (int)Mathf.Ceil(currentBlueWarriorHealth / ((float)warriorHealth));
				p2UnitCtrl.CmdRemoveWarriors (blueWarriors - remainingWarriors, gameObject);
				Debug.Log ("Hex: doDamage: blue warriors damaged. Remaining warriors=" + remainingWarriors);
            }
        }
        //If one team has wiped out other team, then reset health of units on tile
        if(redWarriors+redWorkers == 0 || blueWarriors+blueWorkers == 0) {
            endBattle();
        }
        else {
			RpcUpdateCombatBar (currentRedWarriorHealth , currentBlueWarriorHealth );
        }
	}

    //Method to reset health of remaining units on tile
    private void endBattle(){
		battleStarted = false;
		RpcDisableCombatBar ();
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
			currentRedWorkerHealth += (amount * workerHealth);
			maxRedWorkerHealth += (amount * workerHealth);
		} 
		//If Player 2, add to blueWorkers
		else if (player == Player.PlayerId.P2) {
			blueWorkers += amount;
			currentBlueWorkerHealth += (amount * workerHealth);
			maxBlueWorkerHealth += (amount * workerHealth);
		}
		CapturableTile capture = GetComponent<CapturableTile> ();
		if(capture != null){
			capture.addUnits (amount, player);
		}
    }

	//Add Number of Warriors to Hex
	public void addWarriorsToHex(int amount, Player.PlayerId player) {
		//If Player 1, add to redWarriors
		if (player == Player.PlayerId.P1) {
			redWarriors += amount;
			maxRedWarriorHealth += (amount * warriorHealth);
			currentRedWarriorHealth += (amount * warriorHealth);
			redDamage += amount;
		} 
		//If Player 2, add to blueWarriors
		else if (player == Player.PlayerId.P2) {
			blueWarriors += amount;
			maxBlueWarriorHealth += (amount * warriorHealth);
			currentBlueWarriorHealth += (amount * warriorHealth);
			blueDamage += amount;
		}

		CapturableTile capture = GetComponent<CapturableTile> ();
		if(capture != null){
			capture.addUnits(amount, player);
		}
    }

	//Remove Workers from Hex
	public void removeWorkersFromHex(int amount, Player.PlayerId player) {
		//If Player 1, remove redWorkers
		if (player == Player.PlayerId.P1) {
			if(redWorkers >= amount) {
				redWorkers -= amount;
				maxRedWorkerHealth -= (amount * workerHealth);
			}
			else
			{
				Debug.LogError("Can't remove workers from hex since requested " + amount + " to be removed, and only " + redWorkers + " recorded to be located on this hex: " + this.name);
			}
		} 
		//If Player 2, remove blueWorkers
		else if (player == Player.PlayerId.P2) {
			if(blueWorkers >= amount) {
				blueWorkers -= amount;
				maxBlueWorkerHealth -= (amount * workerHealth);
			}
			else
			{
				Debug.LogError("Can't remove workers from hex since requested " + amount + " to be removed, and only " + blueWorkers + " recorded to be located on this hex: " + this.name);
			}
		}
		CapturableTile capture = GetComponent<CapturableTile> ();
		if (capture != null) {
			capture.removeUnits (amount, player);
		}
    }

	//Remove Warriors from Hex
	public void removeWarriorsFromHex(int amount, Player.PlayerId player) {
		//If Player 1, remove redWorkers
		if (player == Player.PlayerId.P1) {
			if(redWarriors >= amount) {
				redWarriors -= amount;
				maxRedWarriorHealth -= (amount * warriorHealth);
				redDamage -= amount;
			}
			else
			{
				Debug.LogError("Can't remove workers from hex since requested " + amount + " to be removed, and only " + redWarriors + " recorded to be located on this hex: " + this.name);
			}
		} 
		//If Player 2, remove blueWorkers
		else if (player == Player.PlayerId.P2) {
			if(blueWarriors >= amount) {
				blueWarriors -= amount;
				maxBlueWarriorHealth -= (amount * warriorHealth);
				blueDamage -= amount;
			}
			else
			{
				Debug.LogError("Can't remove workers from hex since requested " + amount + " to be removed, and only " + blueWarriors + " recorded to be located on this hex: " + this.name);
			}
		}

		CapturableTile capture = GetComponent<CapturableTile> ();
		if (capture != null) {
			capture.removeUnits (amount, player);
		}
    }

    public bool hasEnemyWarriors(Player.PlayerId player){
		
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
    
	public void changeHexSprite(Sprite s) {
        this.GetComponent<SpriteRenderer>().sprite = s;
        //Refresh hex menu to update ui if necessary
        FindObjectOfType<HexMenuController>().RpcRefreshUIValues();
    }


	////////////////////////////////// SyncVar hooks //////////////////////////////
	private void setBuilding(GameObject newBuilding){
		building = newBuilding.GetComponent<Building> ();
		setStatusIcon (building.getConstructionIconSprite(hexOwner));
		FindObjectOfType<HexMenuController> ().refreshUIValues();
	}

	private void setRedWarriors(int newVal){
		if (!isServer) {
			redWarriors = newVal;
		}
		FindObjectOfType<HexMenuController> ().refreshUIValues ();
	}

	private void setRedWorkers(int newVal){
		if (!isServer) {
			redWorkers = newVal;
		}
		FindObjectOfType<HexMenuController> ().refreshUIValues ();
	}

	private void setBlueWarriors(int newVal){
		if (!isServer) {
			blueWarriors = newVal;
		}
		FindObjectOfType<HexMenuController> ().refreshUIValues ();
	}

	private void setBlueWorkers(int newVal){
		if (!isServer) {
			blueWorkers = newVal;
		}
		FindObjectOfType<HexMenuController> ().refreshUIValues ();
	}

	private void updateHexOwner(Player.PlayerId pid){
		hexOwner = pid;
		HexMenuController hexMenu = FindObjectOfType<HexMenuController> ();
		if(hexMenu != null){
			hexMenu.refreshUIValues ();
		}
	}
}
