﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	// Identify who this player is
	public enum PlayerId {P1,P2, NEUTRAL};
	public PlayerId playerId;

	// Player parameters
	public float rateMultiplier;
	public int baseIncome;
	public int timeFrame; // how long between discrete burst of money

	// UI
	public Text incomeText;
	public Text rateText;

	// State variables
	private float startTime;
	private float totalTileIncome;
	private float currentMoney;

	// Reference to the map so Player can access tiles
	public HexGrid map;

	// Use this for initialization
	void Start () {
		currentMoney = 0;
		incomeText.text = "Hello";
		rateText.text = "+ 0/sec";
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		// Use discrete chunks of income
		generateIncomeDiscrete();
	}

	// Generates income in discrete chunks rather than each frame
	public void generateIncomeDiscrete(){
		float increaseAmt = (baseIncome + totalTileIncome) * rateMultiplier;
		if(Time.time > timeFrame + startTime) {
			// timeframe has passed
			currentMoney += increaseAmt;
			startTime = Time.time;
		}
		incomeText.text = "" + currentMoney;
		rateText.text = "+ " + increaseAmt + "/" + timeFrame + "secs";
	}

	//@Depricated
	//Generate income and add to total money each frame
	public void generateIncomeContinuous(){
		float translation = Time.deltaTime * rateMultiplier;
		currentMoney += translation;
		incomeText.text = "" + Mathf.RoundToInt(currentMoney);
		rateText.text = "+ " + rateMultiplier + "/sec";
	}


	public void captureTile(CapturableTile tile){
		totalTileIncome += tile.tileIncome;
	}

	public void removeTile(CapturableTile tile){
		totalTileIncome -= tile.tileIncome;
	}

	public void makeUnit(GameObject unitObject){
		Unit unitInfo = unitObject.GetComponent<Unit> ();
		if(unitInfo.cost > this.currentMoney){
			// Unit costs too much
			Debug.Log("Can't afford a " + unitObject.name + " for " + unitInfo.cost);
			return;
		}
		Vector3 proposedUnitPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		GameObject hexObject = map.getHex (proposedUnitPosition);
		if(hexObject == null){
			Debug.Log ("Did not place unit on map");
		}
		Hex hex = hexObject.GetComponent<Hex> ();
		if(hex == null){
			Debug.Log ("Can't place unit on something that's not a hex");
			return;
		}
		if(hex.hexOwner != playerId){
			Debug.Log ("Can't place unit on tile you don't own");
			return;
		}
		currentMoney -= unitInfo.cost;
		GameObject newUnit = Instantiate(unitObject);
		newUnit.GetComponent<Unit> ().unitOwner = Player.PlayerId.P1;
		proposedUnitPosition = hex.transform.position;
		proposedUnitPosition.z = -5;
		newUnit.transform.position = proposedUnitPosition;
	}


	public void makeUnit(GameObject unitObject, Vector3 proposedUnitPosition){
		Unit unitInfo = unitObject.GetComponent<Unit> ();
		if(unitInfo.cost > this.currentMoney){
			// Unit costs too much
			Debug.Log("Can't afford a " + unitObject.name + " for " + unitInfo.cost);
			return;
		}
		GameObject hexObject = map.getHex (proposedUnitPosition);
		if(hexObject == null){
			Debug.Log ("Did not place unit on map");
		}
		Hex hex = hexObject.GetComponent<Hex> ();
		if(hex == null){
			Debug.Log ("Can't place unit on something that's not a hex");
			return;
		}
		if(hex.hexOwner != playerId){
			Debug.Log ("Can't place unit on tile you don't own");
			return;
		}
		//Can build the unit
		//remove money from player
		currentMoney -= unitInfo.cost;
		//Instanciate unit in world at the correct position
		GameObject newUnit = Instantiate(unitObject);
		newUnit.GetComponent<Unit> ().unitOwner = Player.PlayerId.P1;
		proposedUnitPosition = hex.transform.position;
		proposedUnitPosition.z = -5;
		newUnit.transform.position = proposedUnitPosition;
	}


	public void upgradeTile(Hex hex, GameObject buildingObject){
		Building buildingInfo = buildingObject.GetComponent<Building> ();
		if(buildingInfo.moneyCost > this.currentMoney){
			// Unit costs too much
			Debug.Log("Can't afford a " + buildingObject.name + " for " + buildingInfo.moneyCost);
			return;
		}
		if(hex == null){
			Debug.Log ("Can't place buildings on something that's not a hex");
			return;
		}
		if(hex.hexOwner != playerId){
			Debug.Log ("Can't place buildings on tile you don't own");
			return;
		}
		switch(buildingInfo.type){
			case Building.BuildingType.ECONOMIC:{
				upgradeToEconBuilding (hex, buildingInfo);
				break;
			}
			case Building.BuildingType.MILITARY:{
				upgradeToMilitaryBuilding (hex, buildingInfo);
				break;	
			}
			default:{
				break;
			}	
		}
	}

	// Tile is making an economic improvement
	private void upgradeToEconBuilding(Hex hex, Building buildingInfo){
		// Can build building
		//Subtract cost of building from player's money
		currentMoney -= buildingInfo.moneyCost;
		//Update the menu
		hex.menuOptions = buildingInfo.constructionMenuOptions;
		//change the sprite to construction site
		hex.GetComponent<SpriteRenderer> ().sprite = buildingInfo.constructionSprite;
		//let hex handle actually building the building
		hex.GetComponentInChildren<CapturableTile> ().beginConstruction (buildingInfo);
	}


	private void upgradeToMilitaryBuilding(Hex hex, Building buildingInfo){
		
	}
}
