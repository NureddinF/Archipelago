using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour {


	public HexGrid map;
	private GameObject hex;

	public void Start(){
		map = GameObject.Find ("Hex Grid").GetComponent<HexGrid>();
		hex = map.getHex(transform.position);
		CapturableTile tile = hex.GetComponentInChildren<CapturableTile>();
		if(tile != null){
			tile.addUnits(1,true);
		}
		Debug.Log ("Started on tile: " + hex.name);
	}

	// Update is called once per frame
	void Update () {
		GameObject newHex = map.getHex (transform.position);
		if (newHex != hex) {
			CapturableTile tile = hex.GetComponentInChildren<CapturableTile>();
			if(tile != null){
				tile.removeUnit(1,true);
			}
			tile = newHex.GetComponentInChildren<CapturableTile>();
			if(tile != null){
				tile.addUnits(1,true);
			}
			hex = newHex;
			Debug.Log ("moved over tile: " + hex.name);
		}
	}
}
