using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Superclass for units, warrior and worker inherits from this class
public class Unit : MonoBehaviour {

    //Parameters
    public float unitSpeed = 2f;
    private Hex destinationHex;
    private Hex initialHex;
    private Vector3 currentCoord;
    private Vector3 destinationCoord;
	public Player.PlayerId id;

    //On script start, set it's position and destination vector
	void Start () {
        transform.position = new Vector3(initialHex.transform.position.x, initialHex.transform.position.y, -5);
        destinationCoord = new Vector3(destinationHex.transform.position.x, destinationHex.transform.position.y, -5);
	}

    //Getters/Setters
    public void setInitialHex(Hex h) { initialHex = h; }
    public void setDestinationHex(Hex h) { destinationHex = h; }
    public Hex getDestinationHex() { return destinationHex; }
    public Vector3 getDestinationCoord() { return destinationCoord; }
	public void setPlayerId(Player.PlayerId id){
		this.id = id;
	}
	public Player.PlayerId getPlayerId(){
		return id;
	}
}


//TODO: for unit classes allow multiple to travel on same gameobject. Perhaps have a seperate sprite for a group travelling. 