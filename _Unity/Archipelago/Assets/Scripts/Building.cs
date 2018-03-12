using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	// Building parameters
	public float moneyCost = 3;
	public float incomeAdjustment = 1;
	public float constructionTime = 5; //time to build the building in seconds if all other factors are set to 1
	
    //Building sprites
	public Sprite builtSprite;
	public Sprite constructionSprite;

}
