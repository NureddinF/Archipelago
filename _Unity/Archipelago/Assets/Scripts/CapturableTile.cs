using UnityEngine;
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
			//Units fighting, don't do capture
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

		//Checks if Player 1 has one or more units on the tile and Owns a tile next to it
		//If all these return true Player 1 begins to capture
		if (numP1UnitsOnHex > 0 && thisHex.hasOwnedNeighbor (Player.PlayerId.P1)) {
			if (thisHex.getHexOwner () != Player.PlayerId.P1) {
				//capturing enemy or neutral tile
				updateTileCapture (Player.PlayerId.P1, newAmountCaptured);
			} else {
				//Defending a tile
				resetCapture(Player.PlayerId.P1, totalCaptureCost);
			}
		} 
		//Checks if Player 2 has one or more units on the tile, Owns a tile next to it, and if they don't own it already
		//If all these return true Player 2 begins to capture
		else if (numP2UnitsOnHex > 0 && thisHex.hasOwnedNeighbor (Player.PlayerId.P2) && thisHex.getHexOwner () != Player.PlayerId.P2) {
			if (thisHex.getHexOwner () != Player.PlayerId.P2) {
				//capturing enemy or neutral tile
				updateTileCapture (Player.PlayerId.P2, newAmountCaptured);
			} else {
				//Defending a tile
				resetCapture(Player.PlayerId.P2, -totalCaptureCost);
			}
		} else if (numP1UnitsOnHex == 0 && numP2UnitsOnHex == 0 && thisHex.getHexOwner () == Player.PlayerId.NEUTRAL){
			// Capture amount degrades towards neutral
			isCapturing = !updateTileCapture (Player.PlayerId.NEUTRAL, newAmountCaptured);
		}
	}

	private bool updateTileCapture(Player.PlayerId pid, float newAmountCaptured){
		Debug.Log ("CaptureableTile: updateTileCapture: pid=" + pid + ", this hex = " + name);
		bool switchedToNeutral = false;
		if (Mathf.Abs(newAmountCaptured) >= totalCaptureCost) {
			//player captured tile
			//update the hex
			thisHex.setHexOwner (pid);
			//update the map on all the clients
			RpcCaptureTile (pid,pid);
			// Update the player income
			finalizeCapture ();
		} else if (newAmountCaptured * amountCaptured < 0) {
			//Tile was neutralised
			//Update theplayer income
			loseTile (pid);
			//update the map on all the clients
			RpcCaptureTile (Player.PlayerId.NEUTRAL,pid);
			switchedToNeutral = true;
		}
		// Update the amount capture for next frame (also sent to clients)
		amountCaptured = newAmountCaptured;
		return switchedToNeutral;
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

		if (GetComponent<Hex>().getHexOwner() != player && !isCapturing) {
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
		isCapturing = false;
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

	private void resetCapture(Player.PlayerId pid, float newCaptureAmount){
		amountCaptured = newCaptureAmount;
		RpcCaptureTile (pid, pid);
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
	private void RpcCaptureTile(Player.PlayerId newTileOwner, Player.PlayerId playerCapturing){
		Debug.Log ("CapturableTile: RpcCaptureTile: newTileOwner=" + newTileOwner);
		if (newTileOwner == Player.PlayerId.P1) {
			captureBorder.enabled = false;
			tileSprite.sprite = p1CaptureTile;
		} else if (newTileOwner == Player.PlayerId.P2) {
			captureBorder.enabled = false;
			tileSprite.sprite = p2CaptureTile;
		} else {
			tileSprite.sprite = neutralCaptureTile;
			captureBorder.fillClockwise = !captureBorder.fillClockwise;
			if (playerCapturing == Player.PlayerId.P1) {
				captureBorder.sprite = p1CaptureBorder;	
			} else if (playerCapturing == Player.PlayerId.P2) {
				captureBorder.sprite = p2CaptureBorder;
			}
		}
		// Update UI on clients if needed
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for(int i=0; i<players.Length; i++){
			Player player = players[i].GetComponent<Player>();
			Debug.Log ("CapturableTile: RpcCaptureTile: player=" + player.playerId);
			if(player.hasAuthority){
				Debug.Log ("CapturableTile: RpcCaptureTile: refreshing UI");
				player.GetComponent<HexMenuController> ().refreshUIValues ();
				return;
			}
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