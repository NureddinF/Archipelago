using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateIncome : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
		currentIncome = 0;
		incomeText.text = "Hello";
		rateText.text = "+ 0/sec";
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		float translation = Time.deltaTime * rateMultiplier;
		currentIncome += translation;
		incomeText.text = "" + Mathf.RoundToInt(currentIncome);
		rateText.text = "+ " + rateMultiplier + "/sec";
		*/
		// timeframe has passed
		if(Time.time > timeFrame + startTime) {
			increaseAmt = (baseIncome + grassTerritory + rockTerritory + sandTerritory + treeTerritory) * rateMultiplier;
			currentIncome += increaseAmt;
			incomeText.text = "" + currentIncome;
			rateText.text = "+ " + increaseAmt + "/" + timeFrame + "secs";
			startTime = Time.time;
		}



	}
}
