using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour {


	public HexGrid map;
	private Hex hex;

	public void Start(){
		map = GameObject.Find ("Hex Grid").GetComponent<HexGrid>();
		hex = map.getHex(transform.position).GetComponent<Hex>();
		Debug.Log ("Started on tile: " + hex.name);
	}

	// Update is called once per frame
	void Update () {
		Hex newHex = map.getHex(transform.position).GetComponent<Hex>();
		if (newHex != hex) {
			
			hex = newHex;
			Debug.Log ("moved over tile: " + hex.name);
		}
	}
}
