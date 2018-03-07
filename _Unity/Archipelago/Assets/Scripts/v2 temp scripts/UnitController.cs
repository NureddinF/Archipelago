using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour {

    //Parameter to store the initial number of units
    public int initialNumOfWorkers;
    public int initialNumOfWarriors;

    //TODO AUTOMATE FINDING INITIAL TERRITORY (would most likely be base location
    public int initialX;
    public int initialY;
    
    //Parameters to store locations of the units.
    //The string key will store the current tile in the form (grid x coordinate).(grid y coordinate)
    //The int stores the number of the unit type, specified by name of dictionary, on the tile at the given key.
    private Dictionary<string, int> workerLocations;
    private Dictionary<string, int> warriorLocations;

    private void Start()
    {
        //Initialize the dictionarys
        workerLocations = new Dictionary<string, int>();
        warriorLocations = new Dictionary<string, int>();
        //Initialize number of warriors, only if initial amount specified > 0 
        if (initialNumOfWarriors > 0)
            //Add initial location and amount of warriors into the correct dict
            warriorLocations.Add(initialX + "." + initialY, initialNumOfWarriors);

        //Initialize number of warriors, only if initial amount specified > 0 
        if (initialNumOfWorkers > 0)
            //Add initial location and amount of warriors into the correct dict
            workerLocations.Add(initialX + "." + initialY, initialNumOfWorkers);

    }

    //Method to add new warrior(s) given a specified amount and a hex
    public void addWarriors(int amount, Hex h) {
        //Create the hex location string to use as a key for the warrior location dictionary
        string hexLocationKey = h.x + "." + h.y;
        //Check if there are already warriors stored at this location
        //If so then add the amount, else create a new entry
        if (warriorLocations.ContainsKey(hexLocationKey))
            warriorLocations[hexLocationKey] += amount;
        else
            warriorLocations.Add(hexLocationKey, amount);
    }

    //Method to remove new warrior(s) given a specified amount and a hex
    public void removeWarriors(int amount, Hex h)
    {
        //Create the hex location string to use as a key for the warrior location dictionary
        string hexLocationKey = h.x + "." + h.y;
        //Check if there are warriors stored at this location
        //If so then remove the amount
        if (warriorLocations.ContainsKey(hexLocationKey)) {
            //check that >= stored in hex than requested to remove, if so remove them, else show debug log error message
            if (warriorLocations[hexLocationKey] >= amount)
            {
                warriorLocations[hexLocationKey] -= amount;
                //If amount on this hex now zero then delete this dictionary key value pair
                if (warriorLocations[hexLocationKey] == 0)
                {
                    warriorLocations.Remove(hexLocationKey);
                }
            }
            else
                Debug.Log("There were only " + warriorLocations[hexLocationKey] + " warriors at the specified hex, yet " +
                    amount + " were requested to be removed. This is therefore not possible and no action has been taken");
           
        } else
            Debug.Log("There are no warriors stored as being located at this hex, therefore no action taken as can't remove");
    }

    //Method to move warriors using the add remove methods. Removes then adds
    public void moveWarriors(int amount, Hex hexFrom, Hex hexTo)
    {
        removeWarriors(amount, hexFrom);
        addWarriors(amount, hexTo);
    }

    //Method to add new worker(s) given a specified amount and a hex 
    public void addWorkers(int amount, Hex h)
    {
        //Create the hex location string to use as a key for the worker location dictionary
        string hexLocationKey = h.x + "." + h.y;
        //Check if there are already workers stored at this location
        //If so then add the amount, else create a new entry
        if (workerLocations.ContainsKey(hexLocationKey))
            workerLocations[hexLocationKey] += amount;
        else
            workerLocations.Add(hexLocationKey, amount);
    }

    //Method to remove new worker(s) given a specified amount and a hex
    public void removeWorkers(int amount, Hex h)
    {
        //Create the hex location string to use as a key for the worker location dictionary
        string hexLocationKey = h.x + "." + h.y;
        //Check if there are workers stored at this location
        //If so then remove the amount
        if (workerLocations.ContainsKey(hexLocationKey))
        {
            //check that >= stored in hex than requested to remove, if so remove them, else show debug log error message
            if (workerLocations[hexLocationKey] >= amount)
            {
                workerLocations[hexLocationKey] -= amount;
                //If amount on this hex now zero then delete this dictionary key value pair
                if (workerLocations[hexLocationKey] == 0)
                {
                    workerLocations.Remove(hexLocationKey);
                }
            }
            else
                Debug.Log("There were only " + workerLocations[hexLocationKey] + " workers at the specified hex, yet " +
                    amount + " were requested to be removed. This is therefore not possible and no action has been taken");

        }
        else
            Debug.Log("There are no workers stored as being located at this hex, therefore no action taken as can't remove");
    }

    //Method to move workers using the add remove methods. Removes then adds
    public void moveWorkers(int amount, Hex hexFrom, Hex hexTo) {
        removeWorkers(amount, hexFrom);
        addWorkers(amount, hexTo);
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
}
