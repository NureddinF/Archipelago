﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CapturableTile: NetworkBehaviour{

	// Sprites representing progress bar of each player
	private Image captureBorder; //this image displays the sprites
	public Sprite p1CaptureBorder;
	public Sprite p2CaptureBorder;

	// Sprites for captured version of tile
	private SpriteRenderer tileSprite; // this displays the sprites
	public Sprite p1CaptureTile;
	public Sprite p2CaptureTile;
	public Sprite neutralCaptureTile;
    
	// Factors determining how long it takes to capture a tile
	public float captureSpeedPerUnit = 1;
	public float neutralResetSpeed = 0.25f; //this is how fast the tile will reset if nothing is on it
	public float totalCaptureCost = 10;

	//Current state of capturing the tile
	[SyncVar] private float amountCaptured = 0;

	//number of units on a hex. used to progress state of capturing proccess
	private int numP1UnitsOnHex = 0;
	private int numP2UnitsOnHex = 0;

	//Used for the battle loop function
	private bool timeFrameStart = false;
	private float timePeriod;

	//Hex script associated with this tile
	private Hex thisHex;

	// State of tile
	[SyncVar] private bool isCapturing = false;

	//////////////////////////////// MonoBehaviour Methods /////////////////////////////////////////////

	//Initialization
	public void Start(){
		thisHex = GetComponent<Hex>();
        tileSprite = GetComponent<SpriteRenderer> ();
		captureBorder = GetComponentInChildren<Image>();

		// End of client initialisation
		if(!isServer){
			return;
		}

		// Set tile to neutral capture amount
		amountCaptured = 0;
		// Unless the tile is owned by a player
		if(Player.PlayerId.NEUTRAL != thisHex.hexOwner){
			amountCaptured = thisHex.hexOwner == Player.PlayerId.P1 ? 1 : -1;
		}
		// Make the border fill amount 
		captureBorder.fillAmount = Mathf.Abs(amountCaptured);
	}
	
	// Update is called once per frame
	void Update (){
		//
		if(isServer){
			//update game state on server
			serverUpdate ();
		}
		if(isClient){
			// update UI on clients
			clientUpdate();
		}

	}

	//////////////////////////////// Custom Methods /////////////////////////////////////////////
    
	// Advance state towards capturing the tile
	private void progressTileCapture() {
		//If the tile is be contested i.e. There are player 1 and player 2 units on the current tile
		//When a tile is contested a battle sequence/loop is started
		//Warriors fight other warriors and workers. Workers do not fight. They merely add to the health of the group.
		if (numP1UnitsOnHex > 0 && numP2UnitsOnHex > 0) {
			beginBattle ();
			return;
		}
			
		if (!isCapturing) {
			//if tile is not being captured don't have to do anything with capturing
			return;
		}
			
		//Only a single Player has units on the tile therefore they can capture
		// Always add decay towards neutral regardless of the player
		if (amountCaptured > 0) {
			neutralResetSpeed = Mathf.Abs (neutralResetSpeed);
		} else {
			neutralResetSpeed = -Mathf.Abs (neutralResetSpeed);
		}

		// Calculate new capture amount
		float newAmountCaptured = amountCaptured + Time.deltaTime * captureSpeedPerUnit * (numP1UnitsOnHex - numP2UnitsOnHex - neutralResetSpeed);
		newAmountCaptured = Mathf.Clamp (newAmountCaptured, -totalCaptureCost, totalCaptureCost);

		//Checks if Player 1 has one or more units on the tile, Owns a tile next to it, and if they don't own it already
		//If all these return true Player 1 begins to capture
		if (numP1UnitsOnHex > 0 && thisHex.hasOwnedNeighbor (Player.PlayerId.P1) && thisHex.getHexOwner () != Player.PlayerId.P1) {
			updateTileCapture (Player.PlayerId.P1, newAmountCaptured);
		} 
		//Checks if Player 2 has one or more units on the tile, Owns a tile next to it, and if they don't own it already
		//If all these return true Player 2 begins to capture
		else if (numP2UnitsOnHex > 0 && thisHex.hasOwnedNeighbor (Player.PlayerId.P2) && thisHex.getHexOwner () != Player.PlayerId.P2) {
			updateTileCapture (Player.PlayerId.P2, newAmountCaptured);
		} else if (numP1UnitsOnHex == 0 && numP1UnitsOnHex == 0 && thisHex.getHexOwner () == Player.PlayerId.NEUTRAL){
			// Capture amount degrades towards neutral
			updateTileCapture (Player.PlayerId.NEUTRAL, newAmountCaptured);
		}

		// Update the amount capture for next frame (also sent to clients)
		amountCaptured = newAmountCaptured;
	}

	private void updateTileCapture(Player.PlayerId pid, float newAmountCaptured){
		if (Mathf.Abs(newAmountCaptured) >= totalCaptureCost) {
			//player captured tile
			//update the hex
			thisHex.setHexOwner (pid);
			//update the map on all the clients
			RpcCaptureTile (pid);
			// Update the player income
			finalizeCapture ();
		} else if (newAmountCaptured * amountCaptured < 0) {
			//Tile was neutralised
			//Update theplayer income
			loseTile (pid);
			//update the map on all the clients
			RpcCaptureTile (Player.PlayerId.NEUTRAL);
		}
	}

	//Fight-Battle Sequence
	private void beginBattle() {
		Hex currentHex = gameObject.GetComponent<Hex>();
		//If a new timeframe hasnt begun start one
		if(!timeFrameStart) {
			currentHex.RpcEnableCombatBar();
			timePeriod = Time.time;
			timeFrameStart = true;
		}
		//If the timeframe has been reached doDamage and reset timeFrameStart
		else if(timeFrameStart) {
			if(timePeriod + 2 <= Time.time) {
				currentHex.doDamage ();
				timeFrameStart = false;
			}
		}
	}

	//////////////////////////////// Getters/Setters /////////////////////////////////////////////

	// Get the player object for the owner of this tile
	private Player getPlayer(Player.PlayerId pid){
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for(int i=0; i<players.Length; i++){
			Player player = players[i].GetComponentInChildren<Player>();
			if(player.playerId.Equals(pid)){
				return player;
			}
		}
		return null;
	}
		

	public Hex getHex(){
		return GetComponent<Hex>();
	}

	//////////////////////////////// Commands and server methods /////////////////////////////////////////////


	private void serverUpdate(){
		if (!isServer) {
			return;
		}
		// progress tile capturing each frame
		progressTileCapture ();
	}


	// Unit calls this when it enters the tile
	public void addUnits(int numUnits, Player.PlayerId player){
		if (!isServer) {
			return;
		}
			
		if (player == Player.PlayerId.P1) {
			numP1UnitsOnHex += numUnits;
		} else if(player == Player.PlayerId.P2){
			numP2UnitsOnHex += numUnits;
		}

		if (GetComponent<Hex>().getHexOwner() == Player.PlayerId.NEUTRAL && !isCapturing) {
			isCapturing = true;
			RpcStartCapture (player);
		}
	}

	// Unit Controller calls this when it wants to move a unit from this tile
	public void removeUnits(int numUnits, Player.PlayerId player) {
		if (!isServer) {
			return;
		}
		if (player == Player.PlayerId.P1) {
			numP1UnitsOnHex -= numUnits;
		} else if(player == Player.PlayerId.P2){
			numP2UnitsOnHex -= numUnits;
		}
	}


	// Increase income of player once tile is captured
	public void finalizeCapture(){
		if (!isServer) {
			return;
		}
		Player player = getPlayer (GetComponent<Hex>().getHexOwner());
		if(player != null){
			player.captureTile (this);
		}
	}

	// Decrement income of player once tile is lost
	private void loseTile(Player.PlayerId pid){
		if (!isServer) {
			return;
		}

		Player player = getPlayer (pid);
		if (player != null) {
			player.removeTile (this);
		}
	}

	//////////////////////////////// RPCs and client methods /////////////////////////////////////////////


	[ClientRpc]
	//Initialise tiles to display capturing on the clients
	private void RpcStartCapture(Player.PlayerId player){
		Sprite border = new Sprite();
		bool captureClockwise = true;
		if (player == Player.PlayerId.P1) {
			border = p1CaptureBorder;
		} else if(player == Player.PlayerId.P2){
			border = p2CaptureBorder;
			captureClockwise = false;
		}


		captureBorder.enabled = true;
		captureBorder.sprite = border;
		captureBorder.fillClockwise = captureClockwise;
	}

	[ClientRpc]
	//Update the map for all the clients so they see the correct sprite when a tile is captured
	private void RpcCaptureTile(Player.PlayerId pid){
		if (pid == Player.PlayerId.P1) {
			captureBorder.enabled = false;
			tileSprite.sprite = p1CaptureTile;
		} else if (pid == Player.PlayerId.P2) {
			captureBorder.enabled = false;
			tileSprite.sprite = p2CaptureTile;
		} else {
			tileSprite.sprite = neutralCaptureTile;
			captureBorder.fillClockwise = !captureBorder.fillClockwise;
		}
	}

	private void clientUpdate(){
		if(!isClient){
			return;
		}
		if (captureBorder.enabled) {
			// Update visual
			captureBorder.fillAmount = Mathf.Abs (amountCaptured / totalCaptureCost);
		}
	}

	//////////////////////////////// SyncVar Hooks /////////////////////////////////////////////


}