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
	public int grassTerritory;
	public int treeTerritory;
	public int sandTerritory;
	public int rockTerritory;

	public Text incomeText;
	public Text rateText;
	public int timeFrame;

	private float startTime;
	private float increaseAmt;
	public float currentIncome;

	private Dictionary<HexGrid.TileType , int> ownedTiles;

	// Use this for initialization
	void Start () {
		ownedTiles = new Dictionary<HexGrid.TileType , int> ();
		ownedTiles.Add (HexGrid.TileType.BASE, 1);
		ownedTiles.Add (HexGrid.TileType.GRASS, 0);
		ownedTiles.Add (HexGrid.TileType.ROCK, 0);
		ownedTiles.Add (HexGrid.TileType.TREE, 0);
		ownedTiles.Add (HexGrid.TileType.SAND, 0);
		currentIncome = 0;
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
			currentIncome += increaseAmt;
			incomeText.text = "" + currentIncome;
			rateText.text = "+ " + increaseAmt + "/" + timeFrame + "secs";
			startTime = Time.time;
		}
	}


	//Generate income and add to total money each frame
	public void generateIncomeContinuous(){
		float translation = Time.deltaTime * rateMultiplier;
		currentIncome += translation;
		incomeText.text = "" + Mathf.RoundToInt(currentIncome);
		rateText.text = "+ " + rateMultiplier + "/sec";
	}


	public void captureTile(HexGrid.TileType tileType){
		ownedTiles [tileType]++;
	}
}
