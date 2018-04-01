using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CapturableTile: MonoBehaviour{

	// Sprites representing progress bar of each player
	private Image captureBorder; //this image displays the sprites
	public Sprite p1CaptureBorder;
	public Sprite p2CaptureBorder;

	// Sprites for captured version of tile
	private SpriteRenderer tileSprite; // this displays the sprites
	public Sprite p1CaptureTile;
	public Sprite p2CaptureTile;
    
	// Factors determining how long it takes to capture a tile
	public float captureSpeedPerUnit = 1;
	public float neutralResetSpeed = 0.25f; //this is how fast the tile will reset if nothing is on it
	public float totalCaptureCost = 10;

	//Current state of capturing the tile
	private float amountCaptured = 0;

	//number of units on a hex. used to progress state of capturing proccess
	private int numP1UnitsOnHex = 0;
	private int numP2UnitsOnHex = 0;

	//Used for the battle loop function
	private bool timeFrameStart = false;
	private float timePeriod;

	//Hex script associated with this tile
	private Hex thisHex;

	//Initialization
	public void Start(){
		thisHex = GetComponent<Hex>();
        tileSprite = GetComponent<SpriteRenderer> ();
		captureBorder = GetComponentInChildren<Image>();

		// This is mostly for testing. Fill Amount should be set to 0 so the tiles starts neutral.
		amountCaptured = captureBorder.fillAmount * totalCaptureCost;
	}
	
	// Update is called once per frame
	void Update (){
		// progress tile capturing each frame
		progressTileCapture ();
	}

    public Hex getHex(){
		return GetComponent<Hex>();
    }


	// Unit calls this when it enters the tile
	public void addUnits(int numUnits, Player.PlayerId player){

		Sprite border = new Sprite();
		bool captureClockwise = true;
		if (player == Player.PlayerId.P1) {
			numP1UnitsOnHex += numUnits;
			border = p1CaptureBorder;
		} else if(player == Player.PlayerId.P2){
			numP2UnitsOnHex += numUnits;
			border = p2CaptureBorder;
			captureClockwise = false;
		}

		if (GetComponent<Hex>().getHexOwner() == Player.PlayerId.NEUTRAL && !captureBorder.enabled) {
			captureBorder.enabled = true;
			captureBorder.sprite = border;
			captureBorder.fillClockwise = captureClockwise;
		}
	}

	// Unit calls this when it leaves the tile
	public void removeUnits(int numUnits, Player.PlayerId player) { //Does this call the Hex funtion remove
		if (player == Player.PlayerId.P1) {
			numP1UnitsOnHex -= numUnits;
		} else if(player == Player.PlayerId.P2){
			numP2UnitsOnHex -= numUnits;
		}
	}
    
	// Advance state towards capturing the tile
	private void progressTileCapture() {
		//If the tile is be contested i.e. There are player 1 and player 2 units on the current tile
		//When a tile is contested a battle sequence/loop is started
		//Warriors fight other warriors and workers. Workers do not fight. They merely add to the health of the group.
		if (numP1UnitsOnHex > 0 && numP2UnitsOnHex > 0) {
			beginBattle ();
		} 
		//Only a single Player has units on the tile therefore they can capture
		else {
			if (captureBorder.enabled) {
				if(amountCaptured >0){
					neutralResetSpeed = Mathf.Abs (neutralResetSpeed);
				} else {
					neutralResetSpeed = -Mathf.Abs (neutralResetSpeed);
				}

                float newAmountCaptured = amountCaptured - (Time.deltaTime * neutralResetSpeed); 

				//Checks if Player 1 has one or more units on the tile, Owns a tile next to it, and if they don't own it already
				//If all these return true Player 1 begins to capture
				if (numP1UnitsOnHex > 0 && thisHex.hasOwnedNeighbor(Player.PlayerId.P1) && thisHex.getHexOwner() != Player.PlayerId.P1) {
                    newAmountCaptured += Time.deltaTime * captureSpeedPerUnit * numP1UnitsOnHex;
                    if (newAmountCaptured > totalCaptureCost){
						//p1 captured tile
						thisHex.setHexOwner(Player.PlayerId.P1);
						captureBorder.enabled = false;
						tileSprite.sprite = p1CaptureTile;
						finalizeCapture ();
					} 
					else if (newAmountCaptured * amountCaptured < 0){
						// change occured
						captureBorder.sprite = p1CaptureBorder;
						captureBorder.fillClockwise = true;
					}
				} 
				//Checks if Player 2 has one or more units on the tile, Owns a tile next to it, and if they don't own it already
				//If all these return true Player 2 begins to capture
				else if (numP2UnitsOnHex > 0 && thisHex.hasOwnedNeighbor(Player.PlayerId.P2) && thisHex.getHexOwner() != Player.PlayerId.P2) {
                    newAmountCaptured += Time.deltaTime * captureSpeedPerUnit * numP2UnitsOnHex;
                    if (newAmountCaptured < -totalCaptureCost){
						//p2 captured tile
						thisHex.setHexOwner(Player.PlayerId.P2);
						captureBorder.enabled = false;
						tileSprite.sprite = p2CaptureTile;
						finalizeCapture();
					} 
					else if (newAmountCaptured * amountCaptured < 0){
						// change occured
						captureBorder.sprite = p2CaptureBorder;
						captureBorder.fillClockwise = false;
					}
				} 
				else {
					//capture amount degrades towards neutral
					if (newAmountCaptured * amountCaptured < 0){
						//tile fully reset to neutral captured tile
						captureBorder.enabled = false;
						amountCaptured = 0;
					}
				}
				// Update visual
				amountCaptured = newAmountCaptured;
				captureBorder.fillAmount = Mathf.Abs (amountCaptured / totalCaptureCost);
			}
		}
	}

	// Increase income of player once tile is captured
	public void finalizeCapture(){
		Player player = getPlayer ();
		if(player != null){
			player.captureTile (this);
		}
	}

	// Get the player object for the owner of this tile
	private Player getPlayer(){
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for(int i=0; i<players.Length; i++){
			Player player = players[i].GetComponentInChildren<Player>();
			if(player.playerId.Equals(GetComponent<Hex>().getHexOwner())){
				return player;
			}
		}
		return null;
	}

	//Fight-Battle Sequence
	private void beginBattle() {
		Hex currentHex = gameObject.GetComponent<Hex>();
		//If a new timeframe hasnt begun start one
		if(!timeFrameStart) {
            currentHex.enableCombatBar();
            timePeriod = Time.time;
			timeFrameStart = true;
		}
		//If the timeframe has been reached doDamage and reset timeFrameStart
		else if(timeFrameStart) {
			if(timePeriod + 2 <= Time.time) {
				currentHex.doDamage ();
				currentHex.doDamage ();
				timeFrameStart = false;
			}
		}
	}
}