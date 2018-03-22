using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	// Building parameters
	public float cost = 3;
	public float tileIncomeAfterBuild = 1;
	public float constructionTime = 5;
    public HexGrid.TileType tileAssociatedWith;

	
    //Building sprites
	public Sprite builtSprite;
    public Sprite constructionSprite;

    //Getters & Setters
    public float getCost() { return cost; }

    public float getTileIncomeAfterBuild() { return tileIncomeAfterBuild; }

    public float getConstructionTime() { return constructionTime; }

    public Sprite getBuiltSprite() { return builtSprite; }

    public Sprite getConstructionSprite() { return constructionSprite; }

    public HexGrid.TileType getTileTypeAssociatedWith() { return tileAssociatedWith; }
}
