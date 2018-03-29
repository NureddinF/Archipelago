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
    private Dictionary<string, int> workerLocations;
    private Dictionary<string, int> warriorLocations;

    void Start()
    {        
        //Initialize the dictionarys
        workerLocations = new Dictionary<string, int>();
        warriorLocations = new Dictionary<string, int>();
    }
    
    public void initializeUnits()
    {
        //Initialize number of warriors, only if initial amount specified > 0 
        if (initialNumOfWarriors > 0)
            //Add initial location and amount of warriors into the correct dict
            addWarriors(initialNumOfWarriors, GameObject.Find("Hex Grid").GetComponent<HexGrid>().getPlayer1BaseX() + "_" + GameObject.Find("Hex Grid").GetComponent<HexGrid>().getPlayer1BaseY());

        //Initialize number of warriors, only if initial amount specified > 0 
        if (initialNumOfWorkers > 0)
            //Add initial location and amount of workers into the correct dict
            addWorkers(initialNumOfWorkers, GameObject.Find("Hex Grid").GetComponent<HexGrid>().getPlayer1BaseX() + "_" + GameObject.Find("Hex Grid").GetComponent<HexGrid>().getPlayer1BaseY());
    }
    
    //Method to add new warrior(s) given a specified amount and a hex
    public void addWarriors(int amount, Hex h) {
        //Create the hex location string to use as a key for the warrior location dictionary
        string hexLocationKey = h.getX() + "_" + h.getY();
        //Call the add warriors by key method with this newly created key
        addWarriors(amount, hexLocationKey);
    }

    //Method to add new warrior(s) given a specified amount and coordinate string key
    private void addWarriors(int amount, string key)
    {
        //Check if there are already warriors stored at this location
        //If so then add the amount, else create a new entry
        if (warriorLocations.ContainsKey(key))
            warriorLocations[key] += amount;
        else
            warriorLocations.Add(key, amount);

        if (GameObject.Find("Hex_" + key).GetComponent<CapturableTile>() != null)
        {
            GameObject.Find("Hex_" + key).GetComponent<CapturableTile>().addUnits(amount, gameObject.GetComponent<Player>().playerId);
        }
    }

    //Method to remove new warrior(s) given a specified amount and a hex
    public void removeWarriors(int amount, Hex h)
    {
        //Create the hex location string to use as a key for the warrior location dictionary
        string hexLocationKey = h.getX() + "_" + h.getY();
        //Call the remove warriors by key method with this newly created key
        removeWarriors(amount, hexLocationKey);
    }

    //Method to remove warrior(s) given a specified amount and coordinate string key
    private void removeWarriors(int amount, string key) { 
        //Check if there are warriors stored at this location
        //If so then remove the amount
        if (warriorLocations.ContainsKey(key)) {
            //check that >= stored in hex than requested to remove, if so remove them, else show debug log error message
            if (warriorLocations[key] >= amount)
            {
                warriorLocations[key] -= amount;
                //If amount on this hex now zero then delete this dictionary key value pair
                if (warriorLocations[key] == 0)
                {
                    warriorLocations.Remove(key);
                    if (GameObject.Find("Hex_" + key).GetComponent<CapturableTile>() != null)
                    {
                        GameObject.Find("Hex_" + key).GetComponent<CapturableTile>().removeUnit(amount, gameObject.GetComponent<Player>().playerId);
                    }
                }
            }
            else
                Debug.Log("There were only " + warriorLocations[key] + " warriors at the specified hex, yet " +
                    amount + " were requested to be removed. This is therefore not possible and no action has been taken");
           
        } else
            Debug.Log("There are no warriors stored as being located at this hex, therefore no action taken as can't remove");
    }

    //Method to move warriors using the add remove methods. Removes then adds
    public void moveWarriors(int amount, Hex hexFrom, Hex hexTo)
    {
        string hexFromKey = hexFrom.getX() + "_" + hexFrom.getY();
        string hexToKey = hexTo.getX() + "_" + hexTo.getY();

        moveWarriors(amount, hexFromKey, hexToKey);
    }

    //Method to move warriors using the add remove methods. Removes then adds.
    private void moveWarriors(int amount, string hexFrom, string hexTo)
    {
        removeWarriors(amount, hexFrom);

        GameObject unitToMove;
        unitToMove = (GameObject)Instantiate(warriorPrefab);
		unitToMove.GetComponent<Unit> ().setPlayerId (GetComponent<Player> ().playerId);
//		Debug.Log(unitToMove.GetComponent<Unit> ().getPlayerId());

        //IMPROVE THIS, CURRENTLY IF moveWorkers given a hex then it needs to get key string then uses this to find hex again.

        GameObject from = GameObject.Find("Hex_" + hexFrom);
        GameObject to = GameObject.Find("Hex_" + hexTo);

        Hex fromHex = from.GetComponent<Hex>();
        Hex toHex = to.GetComponent<Hex>();

        unitToMove.GetComponent<Unit>().setInitialHex(fromHex);
        unitToMove.GetComponent<Unit>().setDestinationHex(toHex);
    }

    //Method to add new worker(s) given a specified amount and a hex 
    public void addWorkers(int amount, Hex h)
    {
        //Create the hex location string to use as a key for the worker location dictionary
        string hexLocationKey = h.getX() + "_" + h.getY();

        //Call the add workers by key method with this newly created key
        addWorkers(amount, hexLocationKey);
    }

    //Method to add new worker(s) given a specified amount and coordinate string key
    private void addWorkers(int amount, string key)
    {
        //Check if there are already warriors stored at this location
        //If so then add the amount, else create a new entry
        if (workerLocations.ContainsKey(key))
            workerLocations[key] += amount;
        else
            workerLocations.Add(key, amount);

        if (GameObject.Find("Hex_" + key).GetComponent<CapturableTile>() != null)
        {
            GameObject.Find("Hex_" + key).GetComponent<CapturableTile>().addUnits(amount, gameObject.GetComponent<Player>().playerId);
        }
    }
    
    //Method to remove new worker(s) given a specified amount and a hex
    public void removeWorkers(int amount, Hex h)
    {
        //Create the hex location string to use as a key for the worker location dictionary
        string hexLocationKey = h.getX() + "_" + h.getY();
        //Call the remove workers by key method with this newly created key
        removeWorkers(amount, hexLocationKey);
    }

    //Method to remove worker(s) given a specified amount and coordinate string key
    private void removeWorkers(int amount, string key)
    {
        //Check if there are workers stored at this location
        //If so then remove the amount
        if (workerLocations.ContainsKey(key))
        {
            //check that >= stored in hex than requested to remove, if so remove them, else show debug log error message
            if (workerLocations[key] >= amount)
            {
                workerLocations[key] -= amount;
                //If amount on this hex now zero then delete this dictionary key value pair
                if (workerLocations[key] == 0)
                {
                    workerLocations.Remove(key);
                    if (GameObject.Find("Hex_" + key).GetComponent<CapturableTile>() != null)
                    {
                        GameObject.Find("Hex_" + key).GetComponent<CapturableTile>().removeUnit(amount, gameObject.GetComponent<Player>().playerId);
                    }
                }
            }
            else
                Debug.Log("There were only " + workerLocations[key] + " workers at the specified hex, yet " +
                    amount + " were requested to be removed. This is therefore not possible and no action has been taken");

        }
        else
            Debug.Log("There are no workers stored as being located at this hex, therefore no action taken as can't remove");
    }

    //Method to move workers using the add remove methods. Removes then adds
    public void moveWorkers(int amount, Hex hexFrom, Hex hexTo) {
        string hexFromKey = hexFrom.getX() + "_" + hexFrom.getY();
        string hexToKey = hexTo.getX() + "_" + hexTo.getY();

        moveWorkers(amount, hexFromKey, hexToKey);
    }

    //Method to move workers using the add remove methods. Removes then adds
    private void moveWorkers(int amount, string hexFrom, string hexTo)
    {
        removeWorkers(amount, hexFrom);

        GameObject unitToMove;
        unitToMove = (GameObject)Instantiate(workerPrefab);


        //IMPROVE THIS, CURRENTLY IF moveWorkers given a hex then it needs to get key string then uses this to find hex again.

        GameObject from = GameObject.Find("Hex_" + hexFrom);
        GameObject to = GameObject.Find("Hex_" + hexTo);

        Hex fromHex = from.GetComponent<Hex>();
        Hex toHex = to.GetComponent<Hex>();

        unitToMove.GetComponent<Unit>().setInitialHex(fromHex);
        unitToMove.GetComponent<Unit>().setDestinationHex(toHex);
    }

    //Method to move the closest available worker using the add remove methods. Removes then adds
    public void moveClosestWorker(Hex hexTo)
    {
        //Variables of the x and y coordinate of the destination hex
        int xTo = hexTo.getX();
        int yTo = hexTo.getY();

        //Initiliaze current shortest distance high enough that any distance calculated will return as lower than this
        //Will store the squared distance, however is not a problem since only comparing shorters not working out exact distances.
        //Therefore will suffice as a comparitive feature
        int currentShortestDistance = 999999;
        //Variable to store the key string from the dictionary for the closest available unit.
        string currentShortestX = null;
        string currentShortestY = null;

        //For each key in the worker location dict. Parse it's coordinates and work out the distance to destination hex, via pythagoras theorem
        //If it's closer than the current closest stored then store this instead
        foreach (string s in workerLocations.Keys)
        {
            string[] coords = s.Split('_');
            int xFrom = System.Int32.Parse(coords[0]);
            int yFrom = System.Int32.Parse(coords[1]);

            //Note the actual distance is the squareroot of this, however not needed for comparing shortest.
            //Perform pythagoras to find diagonal distance, c^2 = a^2 + b^2, distance(^2) = (difference in x)^2 + (difference in y)^2
            int distance = (int)System.Math.Pow(xFrom - xTo, 2) + (int)System.Math.Pow(yFrom - yTo, 2);

            //If closer than current closest recorded, and not 0 distance, aka same hex, then set as new closest.
            if (distance < currentShortestDistance && distance != 0)
            {
                currentShortestDistance = distance;
                currentShortestX = xFrom.ToString();
                currentShortestY = yFrom.ToString();
            }
        }

        //If a suitable unit/hex was found
        if (currentShortestX != null)
        {
            moveWorkers(1, currentShortestX + "_"+currentShortestY, hexTo.getX() + "_" + hexTo.getY());
        }
        else
            Debug.Log("No Units avaible to move currently");
    }

    //Method to move the closest available warrior using the add remove methods. Removes then adds
    public void moveClosestWarrior(Hex hexTo)
    {
        //Variables of the x and y coordinate of the destination hex
        int xTo = hexTo.getX();
        int yTo = hexTo.getY();

        //Initiliaze current shortest distance high enough that any distance calculated will return as lower than this
        //Will store the squared distance, however is not a problem since only comparing shorters not working out exact distances.
        //Therefore will suffice as a comparitive feature
        int currentShortestDistance = 999999;
        //Variable to store the key string from the dictionary for the closest available unit.
        string currentShortestX = null;
        string currentShortestY = null;

        //For each key in the warrior location dict. Parse it's coordinates and work out the distance to destination hex, via pythagoras theorem
        //If it's closer than the current closest stored then store this instead
        foreach (string s in warriorLocations.Keys)
        {
            string[] coords = s.Split('_');
            int xFrom = System.Int32.Parse(coords[0]);
            int yFrom = System.Int32.Parse(coords[1]);

            //Note the actual distance is the squareroot of this, however not needed for comparing shortest.
            //Perform pythagoras to find diagonal distance, c^2 = a^2 + b^2, distance(^2) = (difference in x)^2 + (difference in y)^2
            int distance = (int)System.Math.Pow(xFrom - xTo, 2) + (int)System.Math.Pow(yFrom - yTo, 2);

            //If closer than current closest recorded, and not 0 distance, aka same hex, then set as new closest.
            if (distance < currentShortestDistance && distance != 0)
            {
                currentShortestDistance = distance;
                currentShortestX = xFrom.ToString();
                currentShortestY = yFrom.ToString();
            }
        }

        //If a suitable unit/hex was found
        if (currentShortestX != null)
        {
            moveWarriors(1, currentShortestX + "_" + currentShortestY, hexTo.getX() + "_" + hexTo.getY());
        }
        else
            Debug.Log("No Units avaible to move currently");
    }

    //Method to return total number of warriors
    public int getTotalNumberOfWarriors()
    {
        //Int to track running total
        int total = 0;
        //Iterate through the warrior location dictionary and add up all the values
        foreach(KeyValuePair<string, int> entry in warriorLocations)
           total += entry.Value;

        return total;
    }

    //Method to return total number of workers
    public int getTotalNumberOfWorkers()
    {
        //Int to track running total
        int total = 0;
        //Iterate through the warrior location dictionary and add up all the values
        foreach (KeyValuePair<string, int> entry in workerLocations)
            total += entry.Value;

        return total;
    }

    //Given an x and a y coordinate for a tile, return the number of workers on that tile
    public int getWorkerCountByTileCoords(int x, int y)
    {
        string tileString = x + "_" + y;
        //Check if there are workers stored at this location
        //If so then return the amount, else return 0
        if (workerLocations.ContainsKey(tileString))
        {
            return workerLocations[tileString];
        }
        else
            return 0;
    }

    //Given an x and a y coordinate for a tile, return the number of warriors on that tile
    public int getWarriorCountByTileCoords(int x, int y)
    {
        string tileString = x + "_" + y;
        //Check if there are warriors stored at this location
        //If so then return the amount, else return 0
        if (warriorLocations.ContainsKey(tileString))
        {
            return warriorLocations[tileString];
        }
        else
            return 0;
    }

	public bool checkTrap(GameObject hex){
		Hex playerOn = hex.GetComponentInChildren<Hex> ();
		Building buildOnHex = playerOn.getBuilding ();
		if (buildOnHex != null) {
			if (buildOnHex.getTileTypeAssociatedWith ().Equals (HexGrid.TileType.ALL)) {
				return true;
			}
		}
		Debug.Log ("Player on: "+playerOn);
		return false;
	}

	public bool killWarriro(Warrior w){

	}
}
