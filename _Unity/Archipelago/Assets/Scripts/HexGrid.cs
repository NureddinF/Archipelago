/** https://stormworks.co.uk/hexagonal-grid-in-unity/ **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{

    public GameObject tileGrass; //0
    public GameObject tilePlayer1Base; //1
    public GameObject tilePlayer2Base; //2
    public GameObject tileRocks; //3
    public GameObject tileSand; //4
    public GameObject tileTrees; //5
    public GameObject tileWater; //6

    //Grid dimensions measured by number of hexes
    private int gridWidth;
    private int gridHeight;

    private float hexWidth;
    private float hexHeight;

    private int[,] mapStructure;

    // Initialization
    void Start()
    {
        //Set the width and height to the attached gameobjects width/height
        hexHeight = tileGrass.GetComponent<SpriteRenderer>().bounds.size.y;
        hexWidth = tileGrass.GetComponent<SpriteRenderer>().bounds.size.x;
        initiateMapStructure();
        gridHeight = mapStructure.GetLength(0);
        gridWidth = mapStructure.GetLength(1);

        createHexGrid();
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
            {6,4,0,0,0,0,0,0,0,4,6,4,0,0,0,0,0,0,4,6},
            {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6}
        };

        mapStructure = map;
    }

    Vector3 calcInitialPos()
    {
        Vector3 initialPos;
        initialPos = new Vector3(-hexWidth * gridWidth / 2f + hexWidth / 2, gridHeight / 2f * hexHeight / 2, 0);
        return initialPos;
    }

    public Vector3 calcUnityCoord(Vector2 gridPos)
    {
        Vector3 initPos = calcInitialPos();
        float xoffset = 0;
        float yoffset = 0;

        if (gridPos.x % 2 != 0)

            yoffset = hexHeight / 2;

        float y = initPos.y + yoffset - gridPos.y * hexHeight;

        xoffset = 0.75f;
        float x = initPos.x + gridPos.x * hexWidth * xoffset;
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
                if (mapStructure[y,x] == 0)
                    thisHex = (GameObject)Instantiate(tileGrass);
                else if (mapStructure[y,x] == 1)
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
            }

        }
    }
}
