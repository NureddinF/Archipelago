/** https://stormworks.co.uk/hexagonal-grid-in-unity/ **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {

    public GameObject Hex;

    //Grid dimensions measured by number of hexes
    private int gridWidth;
    private int gridHeight;

    private float hexWidth;
    private float hexHeight;

    private int[,,] mapStructure;

    // Initialization
    void Start () {
        //Set the width and height to the attached gameobjects width/height
        hexHeight = Hex.GetComponent<SpriteRenderer>().bounds.size.y;
        hexWidth = Hex.GetComponent<SpriteRenderer>().bounds.size.x;
       
        gridHeight = mapStructure.GetLength(0);
        gridWidth = mapStructure.GetLength(1);
       
        createHexGrid();
    }


    private void initiateMapStructure()
    {
        int[,] map = new int[4, 4]
        {
            {4,4,4,4 },
            {4,4,4,4 },
            {4,4,4,4 },
            {4,4,4,4 }
        };
    }

    Vector3 calcInitialPos() {
        Vector3 initialPos;
        initialPos = new Vector3(-hexWidth * gridWidth / 2f + hexWidth / 2, gridHeight / 2f * hexHeight / 2, 0);
        return initialPos;
    }

    void createHex(Vector3 pos)
    {
        GameObject thisHex = (GameObject)Instantiate(Hex);
        thisHex.transform.position = pos;
    }

    public Vector3 calcUnityCoord(Vector2 gridPos)
    {
        Vector3 initPos = calcInitialPos();
        float xoffset = 0;
        float yoffset = 0;

        if (gridPos.x % 2 != 0)
          
            yoffset = hexHeight / 2;

        float y = initPos.y + yoffset + gridPos.y * hexHeight;
       
        xoffset = 0.75f;
        float x = initPos.x - gridPos.x * hexWidth * xoffset;
        return new Vector3(x, y, 0);
    }

    void createHexGrid()
    {
        GameObject hexGridObject = new GameObject("HexGrid");
        hexGridObject.transform.parent = this.transform;

        for(int y = 0; y < gridHeight; y++)
        {
            for(int x = 0; x < gridWidth; x++)
            {
                GameObject thisHex = (GameObject)Instantiate(Hex);
                Vector2 gridPos = new Vector2(x, y);
                thisHex.transform.position = calcUnityCoord(gridPos);
            }

        }
    }
}
