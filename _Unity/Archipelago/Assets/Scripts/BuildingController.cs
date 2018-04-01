using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour{

    public List<Building> allConstructableBuildings;

	//TODO: Remove this? its not used
    private string hexToString(Hex h)
    {
        return h.getX() + "_" + h.getY();
    }

	//TODO: Remove this? its not used
    public Building getBuildingByHex(Hex h)
    {
        return h.getBuilding();
    }

	//TODO: Remove this? its not used
    public void addBuilding(Hex h, Building b)
    {
        //h.setBuilding(b);
    }

	//TODO: Remove this? its not used
    public void removeBuilding(Hex h) //TODO: Implement better building losing mechanics.
    {
        //h.setBuilding(null);
    }

	//TODO: Remove this? its not used
    public bool canBeginConstructing(Hex h)
    {
        if (h.getHexOwner().Equals(Player.PlayerId.P1) && h.getBuilding().Equals(null))
        {
            return true;
        }

        return false;
    }

    public List<Building> getListOfBuildingByTileType(HexGrid.TileType type){
        List<Building> result = new List<Building>();

        foreach (Building b in allConstructableBuildings)
        {
            List<HexGrid.TileType> tilesTypesAssociatedWith = b.getTileTypesAssociatedWith();
            if (tilesTypesAssociatedWith.Contains(type))
            {
                result.Add(b);
            }
			if (tilesTypesAssociatedWith.Contains(HexGrid.TileType.ALL) && type != HexGrid.TileType.WATER && type != HexGrid.TileType.BASE) {

				result.Add (b);
			}
        }

        return result;
    }

	// Get Building object from Building ID
	public Building getBuildingFromType(Building.BuildingType buildingId){
		foreach (Building b in allConstructableBuildings){
			if(b.buildingId.Equals(buildingId)){
				return b;
			}
		}

		return null;
	}
}
