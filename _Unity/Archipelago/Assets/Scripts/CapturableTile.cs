﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CapturableTile: MonoBehaviour{

	// Sprites representing progress bar of each player
	private Image captureBoarder; //this image displays the sprites
	public Sprite p1CaptureBorder;
	public Sprite p2CaptureBorder;

	// This is the hex script on the same tile which holds information about who owns the tile?

	// Factors determining how long it takes to capture a tile
	public float captureSpeedPerUnit = 1;
	public float neutralUnitFactor = 0.25f; //this is how fast the tile will reset if nothing is on it
	const float totalCaptureCost = 10;

	//Current state of capturing the tile
	private float amountCaptured = 0;
	public bool paused = false;

	//number of units on a hex. used to progress state of capturing proccess
	public int numP1UnitsOnHex = 0;
	public int numP2UnitsOnHex = 0;

	//Initializeation
	public void Start(){
		captureBoarder = GetComponent<Image>();
		// This is mostly for testing. Fill Amount should be set to 0 so the tiles starts neutral.
		amountCaptured = captureBoarder.fillAmount * totalCaptureCost;
	}
	
	// Update is called once per frame
	void Update (){
		// progress tile capturing each frame
		progressTileCapture ();
	}


	// Unit calls this when it enters the tile
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

	// Unit calls this when it leaves the tile
	public void removeUnit(int numUnits, bool player1){
		if(tileOwned){return;}

		if (player1) {
			numP1UnitsOnHex -= numUnits;
		} else {
			numP2UnitsOnHex -= numUnits;
		}
	}


	bool tileOwned = false;
	// Adnvace state towards capturing the tile
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