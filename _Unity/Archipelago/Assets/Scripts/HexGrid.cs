using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    //Tile variant prefab gameobjects for building the map
    public GameObject tileGrass; //0
    public GameObject tilePlayer1Base; //1
    public GameObject tilePlayer2Base; //2
    public GameObject tileRocks; //3
    public GameObject tileSand; //4
    public GameObject tileTrees; //5
    public GameObject tileWater; //6

    //Grid dimensions measured by number of hexes
    private static int gridWidth;
    private static int gridHeight;

    //Get methods for grid dimensions
    public static int getGridHeight() { return gridHeight; }
    public static int getGridWidth() { return gridWidth;  }


    //Dimensions of individual hex
    private float hexWidth;
    private float hexHeight;

    //Offset measurements
    public float yOffsetGap = 0.01f;
    public float xOffsetGap = 0.01f;
    private float xOffset;
    private float yOffset;

	//Rounding factor. Needed due to size of hexes causing normal rounding errors
	const float roundingFactor = 0.1f;

    //Map layout matrix
    private int[,] mapStructure;
	//Matrix of instanciated hexes
	private GameObject[,] mapHexes;


	//Enumerate the different types of tiles
	public enum TileType {GRASS, BASE, TREE, SAND, ROCK, WATER }

    // Initialization
    void Start()
    {   
        //Initialize map, and sizes
        initiateMapStructure();
        initializeSizes();

        //Create the hex grid
        createHexGrid();
    }

    //Method to store the sizes of grid/hexes/offsets
    private void initializeSizes()
    {
        hexHeight = tileGrass.GetComponent<SpriteRenderer>().bounds.size.y;
        hexWidth = tileGrass.GetComponent<SpriteRenderer>().bounds.size.x;
        xOffset = hexWidth * (0.5f - xOffsetGap);
        yOffset = hexHeight * (0.5f + yOffsetGap);
        gridHeight = mapStructure.GetLength(0);
        gridWidth = mapStructure.GetLength(1);
    }

    //This method contains the matrix representation of the map
    private void initiateMapStructure()
    {
        int[,] map = new int[10, 20]
        {
            {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
            {6,4,0,0,0,0,0,0,0,4,6,4,0,0,3,0,0,0,4,6},
            {6,4,0,3,0,5,0,0,0,4,6,4,0,0,0,0,3,0,4,6},
            {6,4,0,0,0,5,5,0,0,4,6,4,0,0,0,0,0,0,4,6},
            {6,4,2,0,0,0,5,0,0,0,4,0,0,0,0,0,0,0,4,6},
            {6,4,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,1,4,6},
            {6,4,0,0,0,0,0,0,0,4,6,4,0,0,5,0,0,0,4,6},
            {6,4,0,0,0,0,3,0,0,4,6,4,0,5,5,0,0,0,4,6},
            {6,4,0,5,0,0,0,0,0,4,6,4,0,0,0,0,0,0,4,6},
            {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6}
        };

        mapStructure = map;
    }

    //Turns a grid position to unity coordinates
    public Vector3 calcUnityCoord(Vector2 gridPos)
    {
        float x = gridPos.x * (hexWidth - xOffset/2);
        float y = -gridPos.y * hexHeight;

        if (gridPos.x % 2 == 1)
            y -= yOffset;

        return new Vector3(x, y, 0);
    }

    //Method to create the hex grid
    void createHexGrid() {
		mapHexes = new GameObject[gridHeight,gridWidth];

        GameObject hexGridObject = new GameObject("HexGrid");
        //Makes sure all generated game objects are under a parent. Allows tidier scene management
        hexGridObject.transform.parent = this.transform;

        //Incrementing through the two dimensional map array, row by row.
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject thisHex;
                //What tile type wants to be created
				switch(mapStructure[y, x]){
					case 0:{
						thisHex = (GameObject)Instantiate (tileGrass);
						thisHex.GetComponent<Hex> ().tileType = TileType.GRASS;
						break;
					}
					case 1:{
						thisHex = (GameObject)Instantiate (tilePlayer1Base);
						thisHex.GetComponent<Hex>().tileType = TileType.BASE;
						break;
					}
					case 2:{
						thisHex = (GameObject)Instantiate (tilePlayer2Base);
						thisHex.GetComponent<Hex>().tileType = TileType.BASE;
						break;
					}
					case 3:{
						thisHex = (GameObject)Instantiate (tileRocks);
						thisHex.GetComponent<Hex>().tileType = TileType.ROCK;
						break;
					}
					case 4:{
						thisHex = (GameObject)Instantiate (tileSand);
						thisHex.GetComponent<Hex>().tileType = TileType.SAND;
						break;
					}
					case 5:{
						thisHex = (GameObject)Instantiate (tileTrees);
						thisHex.GetComponent<Hex>().tileType = TileType.TREE;
						break;
					}
					default:{
						thisHex = (GameObject)Instantiate (tileWater);
						thisHex.GetComponent<Hex>().tileType = TileType.WATER;
						break;
					}
				}
				/*
                else if (mapStructure[y, x] == 1)
                    thisHex = (GameObject)Instantiate(tilePlayer1Base);
                else if (mapStructure[y, x] == 2)
                    thisHex = (GameObject)Instantiate(tilePlayer2Base);
                else if (mapStructure[y, x] == 3)
                    thisHex = (GameObject)Instantiate(tileRocks);
                else if (mapStructure[y, x] == 4)
                    thisHex = (GameObject)Instantiate(tileSand);
                else if (mapStructure[y, x] == 5)
                    thisHex = (GameObject)Instantiate(tileTrees);
                else
                    thisHex = (GameObject)Instantiate(tileWater);*/

                //Set it's position, tranformation, name and other variables attached to the hex. 0 0 is top left corner
                Vector2 gridPos = new Vector2(x, y);
                thisHex.transform.position = calcUnityCoord(gridPos);
                thisHex.transform.parent = hexGridObject.transform;
                thisHex.name = "Hex_" + x + "_" + y;
                thisHex.GetComponent<Hex>().x = x;
                thisHex.GetComponent<Hex>().y = y;

                //Potential Optimization for hexgrid
                thisHex.isStatic = true;

				mapHexes [y, x] = thisHex;
            }

        }
    }
		
	public GameObject getHex(Vector3 unityCoord){
		// Perform inver operation used to generate map
		float X = (unityCoord.x / (hexWidth - xOffset / 2));
		int x = (int)X;
		// Have to manually round due to size of hexes
		if(X%1.0f > roundingFactor){
			x++;
		}
		if (x % 2 == 1) {
			unityCoord.y += yOffset;
		}
		float Y =(-unityCoord.y / hexHeight);
		int y = (int)Y;
		if(Y%1.0f > roundingFactor){
			y++;
		}

		GameObject hex = null;
		if(x < gridWidth && y < gridHeight){
			hex = mapHexes [y, x];
		}
		return hex;
	}
}
