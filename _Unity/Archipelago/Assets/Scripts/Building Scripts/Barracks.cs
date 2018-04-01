using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building {

    //Needs Code To Spawn Units
	public float warriorAmount = 25;

	public void purchaseWarrior(Hex barracksHex){
//		float currentAmount = GetComponent<Player> ().getCurrentMoney ();
//		if (warriorAmount > currentAmount) {
//			Debug.Log ("Insufficent funds");
//
//		} else {
//			Debug.Log ("Warrior added");
//			GetComponent<Player> ().removeMoney (warriorAmount);
//			GetComponent<UnitController> ().addWarriors (1, barracksHex);
//		}
		Debug.Log ("Warrior added");
		GetComponent<Player> ().removeMoney (warriorAmount);
		GetComponent<UnitController> ().addWarriors (1, barracksHex);

	}

}
