using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour {

    private Dictionary<string, Building> constructionLocations;

    public List<Sprite> buildingSprites;

    private string hexToString(Hex h)
    {
        return h.getX() + "_" + h.getY();
    }

    public Building getConstructionByHex(Hex h)
    {
        string coords = hexToString(h);
        return getConstructionByString(coords);
    }

    private Building getConstructionByString(string s)
    {
        if (constructionLocations.ContainsKey(s))
        {
            return constructionLocations[s];
        }

        return null;
    }

    public Building getBuildingByHex(Hex h)
    {
        return h.getBuilding();
    }

    public void addBuilding(Hex h, Building b)
    {
        h.setBuilding(b);
    }

    public void removeBuilding(Hex h) //TODO: Implement better building losing mechanics.
    {
        h.setBuilding(null);
    }

    public bool canBeginConstructing(Hex h)
    {
        if (h.getPlayerId().Equals("P1") && h.getBuilding().Equals(null) && !constructionLocations.ContainsKey(hexToString(h)))
        {
            return true;
        }

        return false;
    }

    public void addConstruction(Hex h, Building b)
    {
        string coords = hexToString(h);
        addConstruction(coords, b);
    }

    private void addConstruction(string s, Building b)
    {
        constructionLocations.Add(s,b); 
    }

    public void removeConstruction(Hex h)
    {
        string coords = hexToString(h);
        removeConstruction(coords);
    }

    private void removeConstruction(string s)
    {
        constructionLocations.Remove(s);
    }
}
