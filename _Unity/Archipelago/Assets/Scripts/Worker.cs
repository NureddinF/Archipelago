using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour {

	public HexGrid map;
	private GameObject hex;

	private Unit thisUnit;

	public void Start(){
		thisUnit = GetComponent<Unit> ();
		map = GameObject.Find ("Hex Grid").GetComponent<HexGrid>();
		hex = map.getHex(transform.position);
		CapturableTile tile = hex.GetComponentInChildren<CapturableTile>();
		if(tile != null){
			tile.addUnits(1,thisUnit.unitOwner);
		}
		Debug.Log ("Started on tile: " + hex.name);
	}

	// Update is called once per frame
	void Update () {
		GameObject newHex = map.getHex (transform.position);
		if (newHex != hex) {
			CapturableTile tile = hex.GetComponentInChildren<CapturableTile>();
			if(tile != null){
				tile.removeUnit(1,thisUnit.unitOwner);
			}
			tile = newHex.GetComponentInChildren<CapturableTile>();
			if(tile != null){
				tile.addUnits(1,thisUnit.unitOwner);
			}
			hex = newHex;
			Debug.Log ("moved over tile: " + hex.name);
		}
	}
}
