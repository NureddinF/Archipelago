﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CapturableTile: MonoBehaviour{

	// Sprites representing progress bar of each player
	public Image captureBoarder; //this image displays the sprites
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

	//Construction parameters
	public float constructionSpeedPerUnit = 1;
	public float passiveConstructionSpeed = 0;

	//Current state of constructing tile
	private Building buildingUnderConstrcution = null;
	private float constructionProgress = 0;

	//number of units on a hex. used to progress state of capturing proccess
	private int numP1UnitsOnHex = 0;
	private int numP2UnitsOnHex = 0;

	//Hex script associated with this tile
	private Hex thisHex;

	//Initialization
	public void Start(){
		thisHex = GetComponent<Hex>();
        tileSprite = GetComponent<SpriteRenderer> ();
		captureBoarder = GetComponentInChildren<Image>();

		// Update player income if this tile is spawned with an owner
		if(thisHex.getHexOwner() != Player.PlayerId.NEUTRAL){
			finalizeCapture();

            //This initiliazies units, only works since this only calls once, for the tile that is player base. Wont work in other places since not every start has been done
            GameObject.Find("Player").GetComponent<UnitController>().initializeUnits();
        }

		// This is mostly for testing. Fill Amount should be set to 0 so the tiles starts neutral.
		amountCaptured = captureBoarder.fillAmount * totalCaptureCost;
	}
	
	// Update is called once per frame
	void Update (){
		// progress tile capturing each frame
		progressTileCapture ();
		// progress tile construction each frame
		progressTileConstruction();
	}

    public Hex getHex()
    {
        return thisHex;
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

        if (thisHex.getHexOwner() == Player.PlayerId.NEUTRAL && !captureBoarder.enabled) {
			captureBoarder.enabled = true;
			captureBoarder.sprite = border;
			captureBoarder.fillClockwise = captureClockwise;
		}
	}

	// Unit calls this when it leaves the tile
	public void removeUnit(int numUnits, Player.PlayerId player){

		if (player == Player.PlayerId.P1) {
			numP1UnitsOnHex -= numUnits;
		} else if(player == Player.PlayerId.P2){
			numP2UnitsOnHex -= numUnits;
		}
	}
    
	// Adnvace state towards capturing the tile
	private void progressTileCapture(){

		if (thisHex.getHexOwner() == Player.PlayerId.NEUTRAL && captureBoarder.enabled && thisHex.hasOwnedNeighbor()) {

			if(amountCaptured >0){
				neutralResetSpeed = Mathf.Abs (neutralResetSpeed);
			} else {
				neutralResetSpeed = -Mathf.Abs (neutralResetSpeed);
			}

			if (numP1UnitsOnHex > 0 && numP2UnitsOnHex > 0) {
				//TODO: units fight or something
				return;
			} 

			float newAmountCaptured = amountCaptured + Time.deltaTime * captureSpeedPerUnit * (numP1UnitsOnHex - numP2UnitsOnHex - neutralResetSpeed);

			if (numP1UnitsOnHex > 0) {
				if(newAmountCaptured > totalCaptureCost){
					//p1 captured tile
					thisHex.setHexOwner(Player.PlayerId.P1);
					captureBoarder.enabled = false;
					tileSprite.sprite = p1CaptureTile;
					finalizeCapture ();
				} else if (newAmountCaptured * amountCaptured < 0){
					// change occured
					captureBoarder.sprite = p1CaptureBorder;
					captureBoarder.fillClockwise = true;
				}
			} else if (numP2UnitsOnHex > 0) {
				if(newAmountCaptured < -totalCaptureCost){
					//p2 captured tile
					thisHex.setHexOwner(Player.PlayerId.P2);
					captureBoarder.enabled = false;
					tileSprite.sprite = p2CaptureTile;
					finalizeCapture();
				} else if (newAmountCaptured * amountCaptured < 0){
					// change occured
					captureBoarder.sprite = p2CaptureBorder;
					captureBoarder.fillClockwise = false;
				}
			} else {
				//capture amount degrades towards neutral
				if (newAmountCaptured * amountCaptured < 0){
					//tile fully reset to neutral captured tile
					captureBoarder.enabled = false;
					amountCaptured = 0;
				}
			}
			// Update visual
			amountCaptured = newAmountCaptured;
			captureBoarder.fillAmount = Mathf.Abs (amountCaptured / totalCaptureCost);
		}
	}

	// Increase income of player once tile is captured
	private void finalizeCapture(){
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
			if(player.playerId.Equals(thisHex.getHexOwner())){
				return player;
			}
		}
		return null;
	}
		
	// Start process of upgrading tile
	public void beginConstruction(Building buildingInfo){
		buildingUnderConstrcution = buildingInfo;
        thisHex.setBuilding(buildingInfo);
		constructionProgress = 0;
		captureBoarder.enabled = true;
	}


	// Advance state towards capturing the tile
	private void progressTileConstruction(){

		if (buildingUnderConstrcution != null && thisHex.getHexOwner() != Player.PlayerId.NEUTRAL) {

			if (numP1UnitsOnHex > 0 && numP2UnitsOnHex > 0) {
				//TODO: units fight or something
				return;
			} 
			//Figure out construction speed
			float playerUnits = 0;
			if(thisHex.getHexOwner() == Player.PlayerId.P1){
				playerUnits = numP1UnitsOnHex;
			} else if (thisHex.getHexOwner() == Player.PlayerId.P2){
				playerUnits = numP2UnitsOnHex;
			}
			// Calculate how much construction is done
			float newAmountConstructed = constructionProgress + Time.deltaTime * constructionSpeedPerUnit * (playerUnits + passiveConstructionSpeed);

			//check if building is done
			if(newAmountConstructed > buildingUnderConstrcution.getConstructionTime()){
				//Building completed
				finalizeConstruction ();
				return;
			}

			// Construction not done, Update progress bar
			constructionProgress = newAmountConstructed;
			captureBoarder.fillAmount = Mathf.Abs (constructionProgress / buildingUnderConstrcution.getConstructionTime());
		}
	}

	// finish vonstructing the building
	private void finalizeConstruction(){
		// Update sprites/UI
		captureBoarder.enabled = false;
		tileSprite.sprite = buildingUnderConstrcution.getBuiltSprite();

		//Adjust the player income
		Player player = getPlayer ();
		player.removeTile (this);
        thisHex.setTileIncome(buildingUnderConstrcution.getTileIncomeAfterBuild());
		player.captureTile (this);

		//terminate the construction
		buildingUnderConstrcution = null;
	}
}