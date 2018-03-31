using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    // Building parameters
    public float cost = 3;
    public float tileIncomeAfterBuild = 1;
    private float currentTileIncome;
    public float constructionTime = 5;
    public List<HexGrid.TileType> tilesAssociatedWith;

    //Construction parameters
    private float currentBuildTime;
    public float totalBuildTime;
    public float buildSpeedPerWorker;
    private bool isConstructed;

    private Hex hexAssociatedWith;

    //Building sprites
    public Sprite menuIconSprite;
    public Sprite buildingSprite;
    public Sprite constructionSprite;

    void Start()
    {
        currentBuildTime = 0;
    }

    void Update()
    {
        if (!isConstructed)
        {
            progressConstruction();
        } 
    }
    public Hex getHexAssociatedWith() { 
		return hexAssociatedWith; 
	}

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

    public float getConstructionTime() { 
		return constructionTime; 
	}

    public Sprite getConstructionSprite() { 
		return constructionSprite; 
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

    private void progressConstruction() {
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
		//Calculate Current Build Time
        currentBuildTime += Time.deltaTime * hexAssociatedWith.getNumOfWorkersOnHex(pid) * buildSpeedPerWorker;

        if(currentBuildTime >= totalBuildTime)
        {
            finalizeConstruction();
        }
    }

    private void finalizeConstruction()
    {
        isConstructed = true;
        hexAssociatedWith.changeHexSprite(buildingSprite);
    }
}