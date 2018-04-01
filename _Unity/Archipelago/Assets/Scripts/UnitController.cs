using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitController : NetworkBehaviour {

	public GameObject redWarrior;
	public GameObject redWorker;
	public GameObject blueWarrior;
	public GameObject blueWorker;
    public GameObject workerPrefab;
    public GameObject warriorPrefab;

    //Parameter to store the initial number of units
    public int initialNumOfWorkers;
    public int initialNumOfWarriors;

	// store current number of availableunits
	[SyncVar(hook = "updateAvailableWorkerUI")] private int availableWorkers = 0;
	[SyncVar(hook = "updateAvailableWarriorUI")] private int availableWarriors = 0;
    
    //Parameters to store locations of the units.
    //The string key will store the current tile in the form (grid x coordinate).(grid y coordinate)
    //The int stores the number of the unit type, specified by name of dictionary, on the tile at the given key.
    private List<Hex> workerLocations;
    private List<Hex> warriorLocations;

    void Start(){
        //Initialize the dictionarys
        workerLocations = new List<Hex>();
        warriorLocations = new List<Hex>();

		// Only want game state to run on server
		if (!isServer) {
			return;
		}
		initializeUnits ();
    }
    
	//Adds units to the player's base at the start of the game
    public void initializeUnits() {
		if (!isServer) {
			//server keeps track of game state
		}

		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
		//If Player 1
		if(Player.PlayerId.P1 == pid){
			//Initialize number of warriors, only if initial amount specified > 0 
			if (initialNumOfWarriors > 0)
				//Add initial location and amount of warriors into the correct dict
				CmdAddWarriors(initialNumOfWarriors, FindObjectOfType<HexGrid>().getPlayer1Base().gameObject);

			//Initialize number of warriors, only if initial amount specified > 0 
			if (initialNumOfWorkers > 0)
				//Add initial location and amount of workers into the correct dict
				CmdAddWorkers(initialNumOfWorkers, FindObjectOfType<HexGrid>().getPlayer1Base().gameObject);
		}
		//If Player 2
		else if(Player.PlayerId.P2 == pid) {
			//Initialize number of warriors, only if initial amount specified > 0 
			if (initialNumOfWarriors > 0)
				//Add initial location and amount of warriors into the correct dict
				CmdAddWarriors(initialNumOfWarriors, FindObjectOfType<HexGrid>().getPlayer2Base().gameObject);

			//Initialize number of warriors, only if initial amount specified > 0 
			if (initialNumOfWorkers > 0)
				//Add initial location and amount of workers into the correct dict
				CmdAddWorkers(initialNumOfWorkers, FindObjectOfType<HexGrid>().getPlayer2Base().gameObject);
		}
    }
    
    //Method to add new warrior(s) given a specified amount and a hex
	[Command]
	public void CmdAddWarriors(int amount, GameObject hex) {
		Hex h = hex.GetComponent<Hex> ();
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
        h.addWarriorsToHex(amount, pid);
		availableWarriors += amount;

        if (!warriorLocations.Contains(h))
            warriorLocations.Add(h);

		//get component of player to access id
        h.gameObject.GetComponent<CapturableTile>().addUnits(amount, pid);
    }

    //Method to remove warrior(s) given a specified amount and a hex
	[Command]
	public void CmdRemoveWarriors(int amount, GameObject hex) {
		Hex h = hex.GetComponent<Hex> ();
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
        if (warriorLocations.Contains(h))
        {
			int numOfWarriorsOnHex = h.getNumOfWarriorsOnHex(pid);

            if (numOfWarriorsOnHex > amount)
            {
				h.removeWarriorsFromHex(amount, pid);
				availableWarriors -= amount;
                h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, pid); 
            }
            else if(numOfWarriorsOnHex == amount)
            {
                h.removeWarriorsFromHex(amount, pid);
				availableWarriors -= amount;
                warriorLocations.Remove(h);
				h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, pid);
            }            
            else
                Debug.Log("There were only " + numOfWarriorsOnHex + " warriors at the specified hex, yet " +
                    amount + " were requested to be removed. No action has been taken");
        }
        else
            Debug.Log("There are no warriors stored as being located at this hex, therefore no action taken.");
    }

    //Method to move warrior(s) using the add remove methods. Removes then adds
	[Command]
	public void CmdMoveWarriors(int amount, GameObject fromHex, GameObject toHex) {
		Hex from = fromHex.GetComponent<Hex> ();
		Hex to = toHex.GetComponent<Hex> ();
		CmdRemoveWarriors(amount, from.gameObject);

        GameObject unitToMove = Instantiate(warriorPrefab);
		NetworkServer.Spawn(unitToMove);
		unitToMove.GetComponent<Unit> ().CmdInitUnit (gameObject, 
													  fromHex, 
													  toHex, 
													  GetComponent<Player> ().playerId);

		/*unitToMove.GetComponent<Unit> ().setPlayerId (GetComponent<Player> ().playerId);
		unitToMove.GetComponent<Unit> ().unitController = this;
        unitToMove.GetComponent<Unit>().setInitialHex(from);
        unitToMove.GetComponent<Unit>().setDestinationHex(to);*/


    }

    //Method to add new worker(s) given a specified amount and a hex
	[Command]
	public void CmdAddWorkers(int amount, GameObject hex) {
		Hex h = hex.GetComponent<Hex> ();
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
		h.addWorkersToHex(amount, pid);
		availableWorkers += amount;

        if (!workerLocations.Contains(h))
            workerLocations.Add(h);
            h.gameObject.GetComponent<CapturableTile>().addUnits(amount, pid); 
    }

    //Method to remove worker(s) given a specified amount and a hex
	[Command]
	public void CmdRemoveWorkers(int amount, GameObject hex) {
		Hex h = hex.GetComponent<Hex> ();
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
        if (workerLocations.Contains(h)) {
            int numOfWorkersOnHex = h.getNumOfWorkersOnHex(pid);

            if (numOfWorkersOnHex > amount) {
				h.removeWorkersFromHex(amount, pid);
				availableWorkers -= amount;
				h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, pid);
            }
            else if (numOfWorkersOnHex == amount) {
				h.removeWorkersFromHex(amount, pid);
				availableWorkers -= amount;
                workerLocations.Remove(h);
				h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, pid); 
            }
            else
                Debug.Log("There were only " + numOfWorkersOnHex + " workers at the specified hex, yet " +
                    amount + " were requested to be removed. No action has been taken");
        }
        else
            Debug.Log("There are no workers stored as being located at this hex, therefore no action taken.");
    }

    //Method to move worker(s) using the add remove methods. Removes then adds
	[Command]
	public void CmdMoveWorkers(int amount, GameObject fromHex, GameObject toHex) {
		Hex from = fromHex.GetComponent<Hex> ();
		Hex to = toHex.GetComponent<Hex> ();
		CmdRemoveWorkers(amount, from.gameObject);

		GameObject unitToMove = Instantiate(workerPrefab);
		NetworkServer.Spawn(unitToMove);
		unitToMove.GetComponent<Unit> ().CmdInitUnit (gameObject, 
													  fromHex, 
													  toHex, 
													  GetComponent<Player> ().playerId);

		/*unitToMove.GetComponent<Unit> ().unitController = this;
        unitToMove.GetComponent<Unit>().setInitialHex(from);
		unitToMove.GetComponent<Unit> ().setPlayerId (GetComponent<Player> ().playerId);
        unitToMove.GetComponent<Unit>().setDestinationHex(to);*/


    }

    //Method to move the closest available worker using the add remove methods. Removes then adds
	[Command]
	public void CmdMoveClosestAvailableWorker(GameObject hex) {
		Hex hexTo = hex.GetComponent<Hex> ();
        //Variables of the x and y coordinate of the destination hex
        int xTo = hexTo.getX();
        int yTo = hexTo.getY();

        //Initiliaze current shortest distance high enough that any distance calculated will return as lower than this
        //Will store the squared distance, however is not a problem since only comparing shorters not working out exact distances.
        //Therefore will suffice as a comparitive feature
        int currentShortestDistance = 999999;
        //Variable to store the key string from the dictionary for the closest available unit.
        Hex currentClosestHex = null;

        //For each hex in the worker location list. Parse it's coordinates and work out the distance to destination hex, via pythagoras theorem
        //If it's closer than the current closest stored then store this instead
        foreach (Hex h in workerLocations)
        {
            //If hex is already captured, and if there is a building that it isn't under construction, then the workers can be assumed to be free
            //and can be a candidate for the closest available worker
			if (h.hexOwner.Equals(GetComponent<Player>().playerId) && !(h.getBuilding() != null && !h.getBuilding().getIsConstructed())){

                //Get its x/y value
                int xFrom = h.getX();
                int yFrom = h.getY();

                //Note the actual distance is the squareroot of this, however not needed for comparing shortest.
                //Perform pythagoras to find diagonal distance, c^2 = a^2 + b^2, distance(^2) = (difference in x)^2 + (difference in y)^2
                int distance = (int)System.Math.Pow(xFrom - xTo, 2) + (int)System.Math.Pow(yFrom - yTo, 2);

                //If closer than current closest recorded, and not 0 distance, aka same hex, then set as new closest.
                if (distance < currentShortestDistance && distance != 0)
                {
                    currentShortestDistance = distance;
                    currentClosestHex = h;
                }
            }
        }

        //If a suitable unit/hex was found
        if (currentClosestHex != null)
        {
			CmdMoveWorkers(1, currentClosestHex.gameObject, hexTo.gameObject);
        }
        else
        {
            Debug.Log("No Worker Units avaible to move currently");
        }
    }

    //Method to move the closest available warrior using the add remove methods. Removes then adds
	[Command]
	public void CmdMoveClosestAvailableWarrior(GameObject hex) {
		Hex hexTo = hex.GetComponent<Hex> ();
        //Variables of the x and y coordinate of the destination hex
        int xTo = hexTo.getX();
        int yTo = hexTo.getY();

        //Initiliaze current shortest distance high enough that any distance calculated will return as lower than this
        //Will store the squared distance, however is not a problem since only comparing shorters not working out exact distances.
        //Therefore will suffice as a comparitive feature
        int currentShortestDistance = 999999;
        //Variable to store the key string from the dictionary for the closest available unit.
        Hex currentClosestHex = null;

        //For each hex in the warrior location list. Parse it's coordinates and work out the distance to destination hex, via pythagoras theorem
        //If it's closer than the current closest stored then store this instead
        foreach (Hex h in warriorLocations)
        {
            //If hex is already captured, then workers assumed to be free, //////////////////// TODO: When fighting added etc. needs to check if enemy units are present on this hex, if so then they shouldn't be classed as available
			if (h.hexOwner.Equals(GetComponent<Player>().playerId)){

                //Get its x/y value
                int xFrom = h.getX();
                int yFrom = h.getY();

                //Note the actual distance is the squareroot of this, however not needed for comparing shortest.
                //Perform pythagoras to find diagonal distance, c^2 = a^2 + b^2, distance(^2) = (difference in x)^2 + (difference in y)^2
                int distance = (int)System.Math.Pow(xFrom - xTo, 2) + (int)System.Math.Pow(yFrom - yTo, 2);

                //If closer than current closest recorded, and not 0 distance, aka same hex, then set as new closest.
                if (distance < currentShortestDistance && distance != 0)
                {
                    currentShortestDistance = distance;
                    currentClosestHex = h;
                }
            }
        }

        //If a suitable unit/hex was found
        if (currentClosestHex != null)
        {
			CmdMoveWarriors(1, currentClosestHex.gameObject, hexTo.gameObject);
        }
        else
        {
            Debug.Log("No Warrior Units avaible to move currently");
        }
    }

    //Method to return total number of warriors
    public int getTotalNumberOfWarriors() {
		return availableWarriors;
    }

    //Method to return total number of workers
    public int getTotalNumberOfWorkers() {
		return availableWorkers;
    }
		
	//Checks if a trap is placed on the hex where a gameobject is standing
	[Command]
	public void CmdCheckTrap(Vector3 unitPosition, GameObject unit){
		Warrior warrior = null;
		Worker worker = null;
		//the hex the warrior is standing on
		GameObject hex = FindObjectOfType<HexGrid>().getHex(unitPosition);
		//The hex the gameobject is on
		Hex playerOn = hex.GetComponentInChildren<Hex> ();
		//Gets the building thats on the hex
		Building buildOnHex = playerOn.getBuilding ();

		if (buildOnHex != null) {
			List<HexGrid.TileType> tileTypes = buildOnHex.getTileTypesAssociatedWith ();
			Debug.Log(unit.name);
			//Checks if its a warrior
			if (unit.name == "Warrior(Clone)") {
				//gets warrior
				warrior = unit.GetComponent<Warrior> ();
				//Checks in the buildingis associated to ALL, which is a trap building, and the player ids of the hex and player do not match
				if (tileTypes.Contains(HexGrid.TileType.ALL) && !warrior.getPlayerId().Equals(playerOn.getHexOwner())) {
					//calls in the kill warrior method
					killUnitWithTrap (warrior.gameObject, playerOn);
				}
				//other wise its a worker
			} else {
				//gets the worker
				worker = unit.GetComponent<Worker> ();
				//Checks in the buildingis associated to ALL, which is a trap building, and the player ids of the hex and player do not match
				if (tileTypes.Contains(HexGrid.TileType.ALL) && !worker.getPlayerId().Equals(playerOn.getHexOwner())) {
					//calls in the kill warrior method
					killUnitWithTrap (worker.gameObject, playerOn);
				}
			}
		}

	}
	//Takes in the gameobject and the hex it is standing on
	public void killUnitWithTrap(GameObject unit, Hex h){
		if (!isServer) {
			//only server can change game state
			return;
		}
		//sets the sprite to unactive
		unit.SetActive (false);
		//changes the hex sprite back to the original sprite thats under the trap(removes the trap)
		h.RpcDisableStatusIcon();
		//removes the building(trap)
		h.CmdSetBuilding (Building.BuildingType.None);
	}


	private void updateAvailableWorkerUI(int newAvailableWorkers){
		Debug.Log ("UnitController: updateAvailableWorkerUI: newAvailableWorkers=" + newAvailableWorkers);
		availableWorkers = newAvailableWorkers;
		GetComponent<Player> ().updateAvailableWorkerUI(availableWorkers);
	}

	private void updateAvailableWarriorUI(int newAvailableWarriors){
		Debug.Log ("UnitController: updateAvailableWarriorUI: newAvailableWarriors=" + newAvailableWarriors);
		availableWarriors = newAvailableWarriors;
		GetComponent<Player> ().updateAvailableWarriorUI(availableWarriors);
	}
}
