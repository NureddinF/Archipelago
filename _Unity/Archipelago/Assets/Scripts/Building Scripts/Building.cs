using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    // Building parameters
    public float cost = 3;
    public float tileIncomeAfterBuild = 4;
    public List<HexGrid.TileType> tilesAssociatedWith;

    //Construction parameters
    private float currentConstructionProgress = 0;
    public float constructionTime = 5;
    public float buildSpeedPerWorker;
    private bool isConstructed = false;

    private Hex hexAssociatedWith;

    //Building sprites
    public Sprite menuIconSprite;
    public Sprite buildingSprite;
    public Sprite constructionSprite;

    public Hex getHexAssociatedWith() { return hexAssociatedWith; }

    public void setHexAssociatedWith(Hex h) {
        this.hexAssociatedWith = h;
        if (!isConstructed)
        {
            h.changeHexSprite(constructionSprite);
        }
        else
        {
            h.changeHexSprite(buildingSprite);
        }
    }

    public Sprite getBuildingSprite() { return buildingSprite; }

    //Getters & Setters
    public float getCost() { return cost; }

    public float getTileIncomeAfterBuild() { return tileIncomeAfterBuild; }

    public float getConstructionTime() { return constructionTime; }

    public Sprite getConstructionSprite() { return constructionSprite; }

    public List<HexGrid.TileType> getTileTypesAssociatedWith() { return tilesAssociatedWith; }

    public Sprite getMenuIconSprite() { return menuIconSprite; }

    public bool getIsConstructed() { return isConstructed; }

    public void progressConstruction()
    {
        Debug.Log("Progress construction");
        if (!isConstructed)
        {
            currentConstructionProgress += Time.deltaTime * (float) hexAssociatedWith.getNumOfWorkersOnHex() * buildSpeedPerWorker;
            Debug.Log("ConstructionProgress: " + currentConstructionProgress);

            if (currentConstructionProgress >= constructionTime)
            {
                finalizeConstruction();
            }
        }
    }

    private void finalizeConstruction()
    {
        isConstructed = true;
        hexAssociatedWith.changeHexSprite(buildingSprite);
        hexAssociatedWith.setTileIncome(tileIncomeAfterBuild);
    }
}