using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
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

    //Map layout matrix
    private int[,] mapStructure;

    // Initialization
    void Start()
    {       
        initiateMapStructure();
        initializeSizes();
        createHexGrid();
    }

    private void initializeSizes()
    {
        hexHeight = tileGrass.GetComponent<SpriteRenderer>().bounds.size.y;
        hexWidth = tileGrass.GetComponent<SpriteRenderer>().bounds.size.x;
        xOffset = hexWidth * (0.5f - xOffsetGap);
        yOffset = hexHeight * (0.5f + yOffsetGap);
        gridHeight = mapStructure.GetLength(0);
        gridWidth = mapStructure.GetLength(1);
    }

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

    public Vector3 calcUnityCoord(Vector2 gridPos)
    {
        float x = gridPos.x * (hexWidth - xOffset/2);
        float y = -gridPos.y * hexHeight;

        if (gridPos.x % 2 == 1)
            y -= yOffset;

        return new Vector3(x, y, 0);
    }

    void createHexGrid()
    {
        GameObject hexGridObject = new GameObject("HexGrid");
        hexGridObject.transform.parent = this.transform;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject thisHex;
                if (mapStructure[y, x] == 0)
                    thisHex = (GameObject)Instantiate(tileGrass);
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
                    thisHex = (GameObject)Instantiate(tileWater);

                Vector2 gridPos = new Vector2(x, y);
                thisHex.transform.position = calcUnityCoord(gridPos);
                thisHex.transform.parent = hexGridObject.transform;
                thisHex.name = "Hex_" + x + "_" + y;
                thisHex.GetComponent<Hex>().x = x;
                thisHex.GetComponent<Hex>().y = y;

                //Potential Optimization for hexgrid
                thisHex.isStatic = true;
            }

        }
    }
}
