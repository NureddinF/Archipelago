using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	// Building parameters
	public float cost = 3;
	public float incomeAdjustment = 1;
	public List<MenuItem> menuOptions = new List<MenuItem>();

	//Building sprite
	public Sprite buildingSprite;

}
