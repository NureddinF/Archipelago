using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Subclass of unity for a warrior unit
public class Warrior : Unit{
	
    //Every frame
    void Update(){
		
        //Float for how much it should move, based of time passed and it's speed variable
        float step = unitSpeed * Time.deltaTime;
        //Move it's position by amount worked out above
        transform.position = Vector3.MoveTowards(transform.position, getDestinationCoord(), step);

		//the hex the warrior is standing on
		GameObject h = FindObjectOfType<HexGrid>().getHex(transform.position);
		//Call to check if there is a trap on the Hex
		unitController.checkTrap (h, this.gameObject);

		//If reached destination, add it to the hex and remove the gameobject
        if (transform.position.Equals(getDestinationCoord()))
        {
			unitController.addWarriors(1, getDestinationHex());
            Destroy(gameObject);
        }
    }
}


//TODO: for unit classes allow multiple to travel on same gameobject. Perhaps have a seperate sprite for a group travelling. 