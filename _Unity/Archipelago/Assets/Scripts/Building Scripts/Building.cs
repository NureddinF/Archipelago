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
	public Sprite buildingBlueSprite;
	public Sprite buildingRedSprite;
	public Sprite constructionBlueIconSprite;
	public Sprite constructionRedIconSprite;

    void Start()
    {
        currentBuildTime = 0;
    }

	//Returns Current Hex
    public Hex getHexAssociatedWith() { 
		return hexAssociatedWith; 
	}

	//Sets Construction sprite
    public void setHexAssociatedWith(Hex h) {
        this.hexAssociatedWith = h;
		Player.PlayerId hOwner = h.getHexOwner();
        if (!isConstructed)
        {
            h.enableConstructionBar();
            h.enableStatusIcon();
			//If Player 1 set construction icon to red
			if (hOwner == Player.PlayerId.P1) {
				h.setStatusIcon(constructionRedIconSprite);
			}
			//If Player 2 set construction icon to blue
			else if(hOwner == Player.PlayerId.P2) {
				h.setStatusIcon(constructionBlueIconSprite);
			}
        }
        else
        {
            if (this.buildingId != Building.BuildingType.Trap)
            {
				//If Player 1 set building to red
				if (hOwner == Player.PlayerId.P1) {
					h.changeHexSprite(buildingRedSprite);
				}
				//If Player 2 set building to blue
				else if(hOwner == Player.PlayerId.P2) {
					h.changeHexSprite(buildingBlueSprite);
				}
                h.disableStatusIcon();
                h.disableConstructionBar();
            }
            else
            {
                h.enableConstructionBar();
				//If Player 1 set trap to red
				if (hOwner == Player.PlayerId.P1) {
					h.setStatusIcon(buildingRedSprite);
				}
				//If Player 2 set building to blue
				else if(hOwner == Player.PlayerId.P2) {
					h.setStatusIcon(buildingBlueSprite);
				}
            }
            
        }
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

	//Finalizes construction on a tile and updates relevant UI sprites
    private void finalizeConstruction()
    {
        isConstructed = true;
        hexAssociatedWith.disableConstructionBar();
        if (this.buildingId != Building.BuildingType.Trap)
        {
            hexAssociatedWith.disableStatusIcon();
			//If Player 1 make red building type
			if(getHexAssociatedWith().getHexOwner() == Player.PlayerId.P1) {
				hexAssociatedWith.changeHexSprite(buildingRedSprite);
			}
			//If Player 2 make blue building type
			else if(getHexAssociatedWith().getHexOwner() == Player.PlayerId.P2) {
				hexAssociatedWith.changeHexSprite(buildingBlueSprite);
			}
        }
        else
        {
			//If Player 1 make red building type
			if(getHexAssociatedWith().getHexOwner() == Player.PlayerId.P1) {
				hexAssociatedWith.setStatusIcon(buildingRedSprite);
			}
			//If Player 2 make blue building type
			else if(getHexAssociatedWith().getHexOwner() == Player.PlayerId.P2) {
				hexAssociatedWith.setStatusIcon(buildingBlueSprite);
			}
            
        }
    }
}