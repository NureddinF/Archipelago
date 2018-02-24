using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public enum PlayerId {P1,P2, NEUTRAL};

	public PlayerId playerId;

	public float rateMultiplier;
	public int maxIncome;
	public int minIncome;
	public int baseIncome;

	public Text incomeText;
	public Text rateText;
	public int timeFrame;

	private float startTime;
	private float increaseAmt;
	public float currentMoney;

	private Dictionary<HexGrid.TileType , int> ownedTiles;

	public HexGrid map;

	// Use this for initialization
	void Start () {
		ownedTiles = new Dictionary<HexGrid.TileType , int> ();
		ownedTiles.Add (HexGrid.TileType.BASE, 1); // start with 1 base
		ownedTiles.Add (HexGrid.TileType.GRASS, 0);
		ownedTiles.Add (HexGrid.TileType.ROCK, 0);
		ownedTiles.Add (HexGrid.TileType.TREE, 0);
		ownedTiles.Add (HexGrid.TileType.SAND, 0);
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
		if(Time.time > timeFrame + startTime) {
			// timeframe has passed
			increaseAmt = (baseIncome + ownedTiles[HexGrid.TileType.BASE] + ownedTiles[HexGrid.TileType.GRASS] + 
				ownedTiles[HexGrid.TileType.TREE]+ ownedTiles[HexGrid.TileType.ROCK] + 
				ownedTiles[HexGrid.TileType.SAND]) * rateMultiplier;
			currentMoney += increaseAmt;
			startTime = Time.time;
		}
		incomeText.text = "" + currentMoney;
		rateText.text = "+ " + increaseAmt + "/" + timeFrame + "secs";
	}


	//Generate income and add to total money each frame
	public void generateIncomeContinuous(){
		float translation = Time.deltaTime * rateMultiplier;
		currentMoney += translation;
		incomeText.text = "" + Mathf.RoundToInt(currentMoney);
		rateText.text = "+ " + rateMultiplier + "/sec";
	}


	public void captureTile(HexGrid.TileType tileType){
		ownedTiles [tileType]++;
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
		currentMoney -= unitInfo.cost;
		GameObject newUnit = Instantiate(unitObject);
		newUnit.GetComponent<Unit> ().unitOwner = Player.PlayerId.P1;
		proposedUnitPosition = hex.transform.position;
		proposedUnitPosition.z = -5;
		newUnit.transform.position = proposedUnitPosition;
	}


	public void upgradeTile(Hex hex, GameObject buildingObject){
		Building buildingInfo = buildingObject.GetComponent<Building> ();
		if(buildingInfo.cost > this.currentMoney){
			// Unit costs too much
			Debug.Log("Can't afford a " + buildingObject.name + " for " + buildingInfo.cost);
			return;
		}
		if(hex == null){
			Debug.Log ("Can't place building on something that's not a hex");
			return;
		}
		if(hex.hexOwner != playerId){
			Debug.Log ("Can't place unit on tile you don't own");
			return;
		}
		currentMoney -= buildingInfo.cost;
		hex.menuOptions = buildingInfo.menuOptions;
		hex.GetComponent<SpriteRenderer> ().sprite = buildingInfo.buildingSprite;
	}
}
