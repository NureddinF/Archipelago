using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {

    //Stores coordinate in the map
    public int x;
    public int y;
    
    public List <GameObject> getNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();
        if(y != 0)
            neighbors.Add(GameObject.Find("Hex_" + x + "_" + (y - 1)));

        if(y!= HexGrid.getGridHeight())
            neighbors.Add(GameObject.Find("Hex_" + x + "_" + (y + 1)));

        if (x != 0)
            neighbors.Add(null);

        if(x!= HexGrid.getGridHeight())
            neighbors.Add(null);

        return neighbors;
    }
}
