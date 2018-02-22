using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CapturableTile: MonoBehaviour{

	public Sprite p1CaptureBorder;
	public Sprite p2CaptureBorder;
	public bool paused = false;
	public float captureSpeedPerUnit = 1;
	public float neutralUnitFactor = 0.25f;

	private Image captureBoarder;
	const float totalCaptureCost = 10;
	private float amountCaptured = 0;


	//store the number of units on a hex to determine if it should start a capturing proccess
	public int numP1UnitsOnHex = 0;
	public int numP2UnitsOnHex = 0;

	public void Start(){
		captureBoarder = GetComponent<Image>();
		amountCaptured = captureBoarder.fillAmount * totalCaptureCost;
	}
	
	// Update is called once per frame
	void Update (){

		progressTileCapture ();
	}



	public void addUnits(int numUnits, bool player1){
		if(tileOwned){return;}

		Sprite border;
		if (player1) {
			numP1UnitsOnHex += numUnits;
			border = p1CaptureBorder;
		} else {
			numP2UnitsOnHex += numUnits;
			border = p2CaptureBorder;
		}
		if (!captureBoarder.enabled) {
			captureBoarder.enabled = true;
			captureBoarder.sprite = border;
		}
	}

	public void removeUnit(int numUnits, bool player1){
		if(tileOwned){return;}

		if (player1) {
			numP1UnitsOnHex -= numUnits;
		} else {
			numP2UnitsOnHex -= numUnits;
		}
	}


	bool tileOwned = false;
	private void progressTileCapture(){

		if (!tileOwned && captureBoarder.enabled) {

			if(amountCaptured >0){
				neutralUnitFactor = Mathf.Abs (neutralUnitFactor);
			} else {
				neutralUnitFactor = -Mathf.Abs (neutralUnitFactor);
			}

			if (numP1UnitsOnHex > 0 && numP2UnitsOnHex > 0) {
				//units fight
				return;
			} 

			float newAmountCaptured = amountCaptured + Time.deltaTime * captureSpeedPerUnit * (numP1UnitsOnHex - numP2UnitsOnHex - neutralUnitFactor);
			Debug.Log ("new amount captured: " + newAmountCaptured);

			if (numP1UnitsOnHex > 0) {
				if(newAmountCaptured > totalCaptureCost){
					//p1 captured tile
					tileOwned = true;
					captureBoarder.enabled = false;
				} else if (newAmountCaptured * amountCaptured < 0){
					// change occured
					captureBoarder.sprite = p1CaptureBorder;
				}
			} else if (numP2UnitsOnHex > 0) {
				if(newAmountCaptured > totalCaptureCost){
					//p2 captured tile
					tileOwned = true;
					captureBoarder.enabled = false;
				} else if (newAmountCaptured * amountCaptured < 0){
					// change occured
					captureBoarder.sprite = p2CaptureBorder;
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
}