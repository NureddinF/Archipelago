using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Building : NetworkBehaviour{

    // Building parameters
    public float cost = 3;
    public float tileIncomeAfterBuild = 1;
    private float currentTileIncome;
    public List<HexGrid.TileType> tilesAssociatedWith;

	//Id for each type of building that can be passed over the network
	public enum BuildingType{None, Farm, Fishing, LogCabbin, Mine, Trap};
	public BuildingType buildingId;

    //Construction parameters
	[SyncVar] private float currentBuildTime;
    public float totalBuildTime;
    public float buildSpeedPerWorker;
	[SyncVar] private bool isConstructed;

    private Hex hexAssociatedWith;

    //Building sprites
    public Sprite menuIconSprite;
    public Sprite buildingSprite;
    public Sprite constructionIconSprite;


	////////////////////////// Monobehaviour methods //////////////////////////////////////////

    void Start(){
        currentBuildTime = 0;
    }

	////////////////////////// Getters & Setters //////////////////////////////////////////

    public Hex getHexAssociatedWith() { 
		return hexAssociatedWith; 
	}
		
    public Sprite getBuildingSprite() { 
		return buildingSprite; 
	}

    public void setBuildingSprite(Sprite s) { 
		buildingSprite = s; 
	}


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



	////////////////////////// Custom Methods ///////////////////////////////////////////

	////////////////////////// Commands and Server Methods ///////////////////////////////////////////
	[Command]
	public void CmdSetHexAssociatedWith(GameObject hex) {
		Hex h = hex.GetComponent<Hex> ();
		this.hexAssociatedWith = h;
		if (!isConstructed) {
			h.RpcEnableConstructionBar();
			h.RpcEnableStatusIcon();
		}
		else {
			if (this.buildingId != Building.BuildingType.Trap) {
				h.RpcDisplayBuildingSprite();
				h.RpcDisableConstructionBar();
				h.RpcDisableStatusIcon();
			}
			else {
				h.RpcEnableConstructionBar();
			}
		}
	}

	// Progresss the construction time towards completion if there are workers on the hex
	public void progressConstruction() {
		//Calculate Current Build Time
		currentBuildTime += Time.deltaTime * hexAssociatedWith.getNumOfWorkersOnHex(hexAssociatedWith.getHexOwner()) * buildSpeedPerWorker;

		Debug.Log("% Constructed: " + currentBuildTime / totalBuildTime * 100);
		if(currentBuildTime >= totalBuildTime){
			finalizeConstruction();
		}
	}

	private void finalizeConstruction(){
		isConstructed = true;
		hexAssociatedWith.RpcDisableConstructionBar();
		if (this.buildingId != Building.BuildingType.Trap) {
			hexAssociatedWith.RpcDisableStatusIcon();
			hexAssociatedWith.RpcDisplayBuildingSprite();
		}
		else {
			hexAssociatedWith.RpcDisplayTrap ();
		}
	}

	////////////////////////// RPCs ///////////////////////////////////////////
	[ClientRpc]
	private void RpcSetHexAssociatedWith(GameObject hex){
		
	}
}