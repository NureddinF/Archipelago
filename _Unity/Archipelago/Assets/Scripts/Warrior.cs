﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Subclass of unity for a warrior unit
public class Warrior : Unit
{
    //Every frame
    void Update()
    {
        //Float for how much it should move, based of time passed and it's speed variable
        float step = unitSpeed * Time.deltaTime;
        //Move it's position by amount worked out above
        transform.position = Vector3.MoveTowards(transform.position, getDestinationCoord(), step);
        //If reached destination, add it to the hex and remove the gameobject
		GameObject h = FindObjectOfType<HexGrid>().getHex(transform.position);
		if (GameObject.Find ("Player").GetComponent<UnitController> ().checkTrap (h)) {
			GameObject.Find ("Player").GetComponent<UnitController> ().killWarrior (this);
		}
        if (transform.position.Equals(getDestinationCoord()))
        {
            GameObject.Find("Player").GetComponent<UnitController>().addWarriors(1, getDestinationHex());
            Destroy(gameObject);
        }
    }
}


//TODO: for unit classes allow multiple to travel on same gameobject. Perhaps have a seperate sprite for a group travelling. 