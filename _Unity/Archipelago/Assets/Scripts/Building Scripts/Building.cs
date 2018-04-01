using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    // Building parameters
    public float cost = 3;
    public float tileIncomeAfterBuild = 1;
    private float currentTileIncome;
    public List<HexGrid.TileType> tilesAssociatedWith;

	//Id for each type of building that can be passed over the network
	public enum BuildingType{None, Farm, Fishing, LogCabbin, Mine, Trap, Barracks};
	public BuildingType buildingId;

    //Construction parameters
    private float currentBuildTime;
    public float totalBuildTime;
    public float buildSpeedPerWorker;
    private bool isConstructed;

    private Hex hexAssociatedWith;

    //Building sprites
    public Sprite menuIconSprite;
    public Sprite buildingSprite;
    public Sprite constructionIconSprite;

    void Start()
    {
        currentBuildTime = 0;
    }

    public Hex getHexAssociatedWith() { 
		return hexAssociatedWith; 
	}

    public void setHexAssociatedWith(Hex h) {
        this.hexAssociatedWith = h;
        if (!isConstructed)
        {
            h.enableConstructionBar();
            h.enableStatusIcon();
            h.setStatusIcon(constructionIconSprite);
        }
        else
        {
            if (this.buildingId != Building.BuildingType.Trap)
            {
                h.changeHexSprite(buildingSprite);
                h.disableStatusIcon();
                h.disableConstructionBar();
            }
            else
            {
                h.enableConstructionBar();
                h.setStatusIcon(buildingSprite);
            }
            
        }
    }

    public Sprite getBuildingSprite() { 
		return buildingSprite; 
	}

    public void setBuildingSprite(Sprite s) { 
		buildingSprite = s; 
	}



    //Getters & Setters
    public float getCost() { 
		return cost; 
	}

    public float getTileIncomeAfterBuild() { 
		return tileIncomeAfterBuild; 
	}

    public float getCurrentTileIncome() { 
		return currentTileIncome; 
	}

    public void setCurrentTileIncome(float currentTileIncome) { 
		this.currentTileIncome = currentTileIncome; 
	}

    public float getTotalBuildTime() { 
		return totalBuildTime; 
	}

    public float getCurrentBuildTime()
    {
        return currentBuildTime;
    }

    public Sprite getConstructionIconSprite() { 
		return constructionIconSprite; 
	}

    public List<HexGrid.TileType> getTileTypesAssociatedWith() { 
		return tilesAssociatedWith; 
	}

    public Sprite getMenuIconSprite() { 
		return menuIconSprite; 
	}

    public bool getIsConstructed() { 
		return isConstructed; 
	}

    public void progressConstruction() {
		//Calculate Current Build Time
        currentBuildTime += Time.deltaTime * hexAssociatedWith.getNumOfWorkersOnHex(hexAssociatedWith.getHexOwner()) * buildSpeedPerWorker;

        Debug.Log("% Constructed: " + currentBuildTime / totalBuildTime * 100);
        if(currentBuildTime >= totalBuildTime)
        {
            finalizeConstruction();
        }
    }

    private void finalizeConstruction()
    {
        isConstructed = true;
        hexAssociatedWith.disableConstructionBar();
        if (this.buildingId != Building.BuildingType.Trap)
        {
            hexAssociatedWith.disableStatusIcon();
            hexAssociatedWith.changeHexSprite(buildingSprite);
        }
        else
        {
            hexAssociatedWith.setStatusIcon(buildingSprite);
        }
    }
}