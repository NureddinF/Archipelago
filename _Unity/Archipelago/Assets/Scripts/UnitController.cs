using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour {

    public GameObject workerPrefab;
    public GameObject warriorPrefab;

    //Parameter to store the initial number of units
    public int initialNumOfWorkers;
    public int initialNumOfWarriors;
    
    //Parameters to store locations of the units.
    //The string key will store the current tile in the form (grid x coordinate).(grid y coordinate)
    //The int stores the number of the unit type, specified by name of dictionary, on the tile at the given key.
    private List<Hex> workerLocations;
    private List<Hex> warriorLocations;

    void Start()
    {        
        //Initialize the dictionarys
        workerLocations = new List<Hex>();
        warriorLocations = new List<Hex>();
    }
    
    public void initializeUnits()
    {
        //Initialize number of warriors, only if initial amount specified > 0 
        if (initialNumOfWarriors > 0)
            //Add initial location and amount of warriors into the correct dict
            addWarriors(initialNumOfWarriors, GameObject.Find("Hex Grid").GetComponent<HexGrid>().getPlayer1Base());

        //Initialize number of warriors, only if initial amount specified > 0 
        if (initialNumOfWorkers > 0)
            //Add initial location and amount of workers into the correct dict
            addWorkers(initialNumOfWorkers, GameObject.Find("Hex Grid").GetComponent<HexGrid>().getPlayer1Base());
    }
    
    //Method to add new warrior(s) given a specified amount and a hex
    public void addWarriors(int amount, Hex h) {

        h.addWarriorsToHex(amount);

        if (!warriorLocations.Contains(h))
            warriorLocations.Add(h);

        h.gameObject.GetComponent<CapturableTile>().addUnits(amount, Player.PlayerId.P1); //Hard coded P1 -- TODO --
    }

    //Method to remove warrior(s) given a specified amount and a hex
    public void removeWarriors(int amount, Hex h)
    {        
        if (warriorLocations.Contains(h))
        {
            int numOfWarriorsOnHex = h.getNumOfWarriorsOnHex();

            if (numOfWarriorsOnHex > amount)
            {
                h.removeWarriorsFromHex(amount);
                h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, Player.PlayerId.P1); //Hard coded P1 -- TODO --
            }
            else if(numOfWarriorsOnHex == amount)
            {
                h.removeWarriorsFromHex(amount);
                warriorLocations.Remove(h);
                h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, Player.PlayerId.P1); //Hard coded P1 -- TODO --
            }            
            else
                Debug.Log("There were only " + numOfWarriorsOnHex + " warriors at the specified hex, yet " +
                    amount + " were requested to be removed. No action has been taken");
        }
        else
            Debug.Log("There are no warriors stored as being located at this hex, therefore no action taken.");
    }

    //Method to move warrior(s) using the add remove methods. Removes then adds
    public void moveWarriors(int amount, Hex from, Hex to)
    {
        removeWarriors(amount, from);

        GameObject unitToMove;
        unitToMove = (GameObject)Instantiate(warriorPrefab);
		unitToMove.GetComponent<Unit> ().setPlayerId (GetComponent<Player> ().playerId);
//		Debug.Log(unitToMove.GetComponent<Unit> ().getPlayerId());

        unitToMove.GetComponent<Unit>().setInitialHex(from);
        unitToMove.GetComponent<Unit>().setDestinationHex(to);
    }

    //Method to add new worker(s) given a specified amount and a hex
    public void addWorkers(int amount, Hex h)
    {
        h.addWorkersToHex(amount);

        if (!workerLocations.Contains(h))
            workerLocations.Add(h);
            h.gameObject.GetComponent<CapturableTile>().addUnits(amount, Player.PlayerId.P1); //Hard coded P1 -- TODO --
    }

    //Method to remove worker(s) given a specified amount and a hex
    public void removeWorkers(int amount, Hex h)
    {
        if (workerLocations.Contains(h))
        {
            int numOfWorkersOnHex = h.getNumOfWorkersOnHex();

            if (numOfWorkersOnHex > amount)
            {
                h.removeWorkersFromHex(amount);
                h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, Player.PlayerId.P1); //Hard coded P1 -- TODO --
            }
            else if (numOfWorkersOnHex == amount)
            {
                h.removeWorkersFromHex(amount);
                workerLocations.Remove(h);
                h.gameObject.GetComponent<CapturableTile>().removeUnits(amount, Player.PlayerId.P1); //Hard coded P1 -- TODO --
            }
            else
                Debug.Log("There were only " + numOfWorkersOnHex + " workers at the specified hex, yet " +
                    amount + " were requested to be removed. No action has been taken");
        }
        else
            Debug.Log("There are no workers stored as being located at this hex, therefore no action taken.");
    }

    //Method to move worker(s) using the add remove methods. Removes then adds
    public void moveWorkers(int amount, Hex from, Hex to)
    {
        removeWorkers(amount, from);

        GameObject unitToMove;
        unitToMove = (GameObject)Instantiate(workerPrefab);

        unitToMove.GetComponent<Unit>().setInitialHex(from);
        unitToMove.GetComponent<Unit>().setDestinationHex(to);
    }

    //Method to move the closest available worker using the add remove methods. Removes then adds
    public void moveClosestAvailableWorker(Hex hexTo)
    {
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
            if (h.hexOwner.Equals(Player.PlayerId.P1) && !(h.getBuilding() != null && !h.getBuilding().getIsConstructed()))
            {     //TODO - HARD CODED PLAYER, NEEDS TO BE ADAPTED FOR MULTIPLAYER

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
            moveWorkers(1, currentClosestHex, hexTo);
        }
        else
        {
            Debug.Log("No Worker Units avaible to move currently");
        }
    }

    //Method to move the closest available warrior using the add remove methods. Removes then adds
    public void moveClosestAvailableWarrior(Hex hexTo)
    {
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
            if (h.hexOwner.Equals(Player.PlayerId.P1))
            {     //TODO - HARD CODED PLAYER, NEEDS TO BE ADAPTED FOR MULTIPLAYER

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
            moveWarriors(1, currentClosestHex, hexTo);
        }
        else
        {
            Debug.Log("No Warrior Units avaible to move currently");
        }
    }

    //Method to return total number of warriors
    public int getTotalNumberOfWarriors()
    {
        //Int to track running total
        int total = 0;
        //Iterate through the warrior locations and add up all the counts
        foreach (Hex h in warriorLocations)
            total += h.getNumOfWarriorsOnHex();

        return total;
    }

    //Method to return total number of workers
    public int getTotalNumberOfWorkers()
    {
        //Int to track running total
        int total = 0;
        //Iterate through the worker locations and add up all the counts
        foreach (Hex h in workerLocations)
            total += h.getNumOfWorkersOnHex();

        return total;
    }

	public bool checkTrap(GameObject hex){
		Hex playerOn = hex.GetComponentInChildren<Hex> ();
		Building buildOnHex = playerOn.getBuilding ();
		Debug.Log ("Build On Hex: "+buildOnHex);
		if (buildOnHex != null) {
			List<HexGrid.TileType> tileTypes = buildOnHex.getTileTypesAssociatedWith ();
			if (tileTypes.Contains(HexGrid.TileType.ALL)) {
				return true;
			}
		}
		Debug.Log ("Player on: "+playerOn);
		return false;
	}

	public void killWarrior(Warrior w){
		w.gameObject.SetActive (false);
	}
}
