using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour {

	public static bool isEnabled = false; 

	public HexGrid map;
	private GameObject hex;

	private Unit thisUnit;

	//initialization
	public void Start(){
		//initialize references
		thisUnit = GetComponent<Unit> ();
		map = GameObject.Find ("Hex Grid").GetComponent<HexGrid>();
		//place unit on map
		hex = map.getHex(transform.position);
		if(hex != null){
			CapturableTile tile = hex.GetComponentInChildren<CapturableTile>();
			if(tile != null){
				tile.addUnits(1,thisUnit.unitOwner);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		//to pause script
		if (isEnabled) {
			return;
		}
		//Get the hex the unit is standing on
		GameObject newHex = map.getHex (transform.position);
		//Check if this is a new hex
		if (newHex != hex) {
			//it's a new hex so remove unit from previous tile
			CapturableTile tile;
			if (hex != null) {
				tile = hex.GetComponentInChildren<CapturableTile> ();
				if (tile != null) {
					tile.removeUnit (1, thisUnit.unitOwner);
				}
			}
			//Then add unit to new tile
			tile = newHex.GetComponentInChildren<CapturableTile>();
			if(tile != null){
				tile.addUnits(1,thisUnit.unitOwner);
			}
			hex = newHex;
		}
	}
}
