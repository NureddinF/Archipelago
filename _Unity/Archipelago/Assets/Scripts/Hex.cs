using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Hex : MonoBehaviour {

    //Stores coordinate in the map
    private int x;
    private int y;

	//Store who owns the hex
	public Player.PlayerId hexOwner = Player.PlayerId.NEUTRAL;

    private HexGrid.TileType tileType;
    private float tileIncome;

    //Store the building on the hex, null if not one
    private Building building;

    //Set maxY and maxX of a hex, -1 for off by one error
    private int maxY = HexGrid.getGridHeight() - 1;
    private int maxX = HexGrid.getGridWidth() - 1;

    public void setX(int x) { this.x = x; }

    public void setY(int y) { this.y = y; }

    public int getX() { return x; }

    public int getY() { return y; }

    public Player.PlayerId getPlayerId() { return hexOwner; }

    public void setTileIncome(float amount) { this.tileIncome = amount; }

    public float getTileIncome() { return tileIncome; }

    public void setTileType(HexGrid.TileType type) { this.tileType = type; }

    public HexGrid.TileType getTileType() { return tileType; }

    public void setBuilding(Building b) { this.building = b; }

    public Building getBuilding() { return building; }
    
    //Method to return a list of a hex's direct neighbors
    public List <GameObject> getNeighbors(){
        List<GameObject> neighbors = new List<GameObject>();

        //Check there are hexes above this object, if not don't add since hex has no neighbors above
        if(y != 0)
            //Hex (N)
            neighbors.Add(GameObject.Find("Hex_" + x + "_" + (y - 1)));

        //Check there are hexes below this object, if not don't add since hex has no neighbors below.
        if(y!= maxY)
            //Hex (S)
            neighbors.Add(GameObject.Find("Hex_" + x + "_" + (y + 1)));

        //If even row(rows are at there highest here)
        if(x % 2 == 0)
        {
            //Check hexes to the left
            if (x != 0)
            {
                //Hex (SW)
                neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + y));
                //Check hexes above
                if (y != 0)
                    //Hex (NW)
                    neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + (y - 1)));
            }

            if(x != maxX)
            {
                //Hex (SE)
                neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + y));
                //Check hexes above
                if (y != 0)
                    //Hex (NE)
                    neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + (y - 1)));
            }
        }
        //Else odd row(rows are at there lowest here)
        else
        {
            //Check hexes to the left
            if (x != maxX)
            {
                //Hex (SE)
                neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + y));
                //Check hexes above
                if (y != maxY)
                    //Hex (NE)
                    neighbors.Add(GameObject.Find("Hex_" + (x + 1) + "_" + (y + 1)));
            }

            if (x != 0)
            {
                //Hex (SW)
                neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + y));
                //Check hexes above
                if (y != maxY)
                    //Hex (NW)
                    neighbors.Add(GameObject.Find("Hex_" + (x - 1) + "_" + (y + 1)));
            }
        }
        
        return neighbors;
    }

    public bool hasNeighbor()
    {
        List<GameObject> neighbors = getNeighbors();

        foreach(GameObject gO in neighbors)
        {
            if (gO.GetComponent<Hex>())
            {
                if (gO.GetComponent<Hex>().getPlayerId().Equals(Player.PlayerId.P1)) //TODO: Currently softcoded for P1, need to set so works for anyplayer and passed in
                    return true;
            }
        }
        return false;
    }

    public void changeHexSprite(Sprite s)
    {
        this.GetComponent<SpriteRenderer>().sprite = s;
    }
}
