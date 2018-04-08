using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Superclass for units, warrior and worker inherits from this class
public class Unit : NetworkBehaviour {

    //Parameters
    public float unitSpeed = 2f;
    public Hex destinationHex;
    public Hex initialHex;
    private Vector3 currentCoord;
    private Vector3 destinationCoord;
	public Player.PlayerId id;

	//Player who owns this unit
	public UnitController unitController;

    //On script start, set it's position and destination vector
	void Start () {
        
	}

    //Getters/Setters
	//Set Initial Hex (Starting Point of Unit Travel)
    public void setInitialHex(Hex h) { 
		initialHex = h;
		transform.position = new Vector3(initialHex.transform.position.x, initialHex.transform.position.y, -5);
	}
    public void setDestinationHex(Hex h) { 
		destinationHex = h; 
		destinationCoord = new Vector3(destinationHex.transform.position.x, destinationHex.transform.position.y, -5);
		GetComponent<SpriteRenderer> ().flipX = destinationCoord.x < transform.position.x;
	}
	//Get Destination Hex (Endpoint of Unit Travel)
    public Hex getDestinationHex() { 
		return destinationHex; 
	}
	//Get Destination Coordinates
    public Vector3 getDestinationCoord() { 
		return destinationCoord; 
	}
	//Set Player ID
	public void setPlayerId(Player.PlayerId id){
		this.id = id;
		setUnitSprite ();
	}

	//Get Player ID
	public Player.PlayerId getPlayerId(){
		return id;
	}

	protected virtual void setUnitSprite (){
		
	}

	[Command]
	public void CmdInitUnit(GameObject unitControllerObject, GameObject fromHex, GameObject toHex, Player.PlayerId id){
		initUnit(unitControllerObject, fromHex, toHex, id);
		RpcInitUnit(unitControllerObject, fromHex, toHex, id);
	}

	[ClientRpc]
	private void RpcInitUnit(GameObject unitControllerObject, GameObject fromHex, GameObject toHex, Player.PlayerId id){
		initUnit(unitControllerObject, fromHex, toHex, id);
	}

	private void initUnit(GameObject unitControllerObject, GameObject fromHex, GameObject toHex, Player.PlayerId id){
		unitController = unitControllerObject.GetComponent<UnitController> ();
		setDestinationHex (toHex.GetComponent<Hex> ());
		setInitialHex (fromHex.GetComponent<Hex> ());
		setPlayerId (id);
	}
}


//TODO: for unit classes allow multiple to travel on same gameobject. Perhaps have a seperate sprite for a group travelling. 