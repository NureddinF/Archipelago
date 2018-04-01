using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Subclass of unity for a warrior unit
public class Warrior : Unit{
	
    //Every frame
    void Update(){
		
        //Float for how much it should move, based of time passed and it's speed variable
        float step = unitSpeed * Time.deltaTime;
        //Move it's position by amount worked out above
        transform.position = Vector3.MoveTowards(transform.position, getDestinationCoord(), step);

		if (!hasAuthority) {
			//Only let authoritative version check for state change
			return;
		}

		//check if unit got to where it needs to
		CmdCheckReachedDestination ();
        
    }


	[Command]
	private void CmdCheckReachedDestination(){

		//Call to check if there is a trap on the Hex
		unitController.CmdCheckTrap (transform.position, this.gameObject);

		// Get the hex the unit is currently on
		GameObject hex = FindObjectOfType<HexGrid>().getHex(transform.position);

		// If unit walks into enemny unit stop to fight
		if (hex.GetComponent<Hex>().hasEnemyWarriors(id)){
			unitController.CmdAddWarriors(1, hex);
			NetworkServer.Destroy(gameObject);
		}

		//If reached destination, add it to the hex and remove the gameobject
		if (transform.position.Equals(getDestinationCoord())) {
			unitController.CmdAddWarriors(1, getDestinationHex().gameObject);
			NetworkServer.Destroy(gameObject);
		}
	}
}


//TODO: for unit classes allow multiple to travel on same gameobject. Perhaps have a seperate sprite for a group travelling. 