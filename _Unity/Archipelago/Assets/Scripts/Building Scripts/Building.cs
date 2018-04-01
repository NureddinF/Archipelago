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
	public enum BuildingType{None, Farm, Fishing, LogCabbin, Mine, Trap};
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

<<<<<<< Updated upstream
=======
    void Update()
    {
        if (!isConstructed)
        {
            progressConstruction();
        } 
    }
>>>>>>> Stashed changes
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
            if (!this.GetComponent<Trap>())
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

<<<<<<< Updated upstream
    public float getTotalBuildTime() { 
		return totalBuildTime; 
	}

    public float getCurrentBuildTime()
    {
        return currentBuildTime;
    }

    public Sprite getConstructionIconSprite() { 
		return constructionIconSprite; 
=======
    public float getConstructionTime() { 
		return constructionTime; 
	}

    public Sprite getConstructionSprite() { 
		return constructionSprite; 
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======

    private void progressConstruction() {
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
		//Calculate Current Build Time
        currentBuildTime += Time.deltaTime * hexAssociatedWith.getNumOfWorkersOnHex(pid) * buildSpeedPerWorker;
>>>>>>> Stashed changes

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
        if (!this.GetComponent<Trap>())
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