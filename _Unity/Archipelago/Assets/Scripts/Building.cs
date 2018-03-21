using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	// Building parameters
	private float cost = 3;
	private float tileIncomeAfterBuild = 1;
	private float constructionTime = 5; //time to build the building in seconds if all other factors are set to 1
    private string folderPath = "Images/Tiles/";
	
    //Building sprites
	private Sprite builtSprite;
    private Sprite constructionSprite;

    //Getters & Setters
    public void setCost(float cost){this.cost = cost;}
    
    public void setTileIncomeAfterBuild(float newIncome){this.tileIncomeAfterBuild = newIncome;}

    public void setConstructionTime(float constructionTime) { this.constructionTime = constructionTime;}

    public void setBuiltSprite(Sprite builtSprite) { this.builtSprite = builtSprite; }

    public void setConstructionSprite(Sprite constructionSprite) { this.constructionSprite = constructionSprite; }

    public float getCost() { return cost; }

    public float getTileIncomeAfterBuild() { return tileIncomeAfterBuild; }

    public float getConstructionTime() { return constructionTime; }

    public Sprite getBuiltSprite() { return builtSprite; }

    public Sprite getConstructionSprite() { return constructionSprite; }
}
