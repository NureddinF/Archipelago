using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class HexGrid : NetworkBehaviour {
    //Tile variant prefab gameobjects for building the map
    public GameObject tileGrass; //0
    public GameObject tilePlayer1Base; //1
    public GameObject tilePlayer2Base; //2
    public GameObject tileRocks; //3
    public GameObject tileSand; //4
    public GameObject tileTrees; //5
    public GameObject tileWater; //6
	public GameObject parentMapObjectPrefab; // parent object of all hexes

    public float baseGrassIncome = 1f;
    public float basePlayerBaseIncome = 2f;
    public float baseRocksIncome = 3f;
    public float baseSandIncome = 0.5f;
    public float baseTreesIncome = 2f;

	public int levelNumber;

    private Hex player1Base;
	private Hex player2Base;
	//Game Maps - Represented by a matrix
	//Level 1 Map
	int[,] level1 = new int[7, 35]
	{
		{6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
		{6,6,6,3,6,6,6,6,6,4,6,4,6,3,6,6,6,4,6,6,6,3,6,4,6,4,6,6,6,6,6,3,6,6,6},
		{6,4,4,0,4,4,6,4,4,0,4,6,4,6,4,0,0,3,0,0,4,6,4,6,4,0,4,4,6,4,4,0,4,4,6},
		{6,3,5,1,0,3,4,3,5,3,0,5,0,4,5,5,3,3,3,5,5,4,0,5,0,3,5,3,4,3,0,2,5,3,6},
		{6,4,5,0,0,4,4,4,5,0,0,6,0,6,5,0,3,3,3,0,5,6,0,6,0,0,5,4,4,4,0,0,5,4,6},
		{6,6,4,3,4,6,6,6,4,4,4,4,4,3,4,4,0,4,0,4,4,3,4,4,4,4,4,6,6,6,4,3,4,6,6},
		{6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6}

	};

	//Level 2 Map
	int[,] level2 = new int[15, 20]
	{
		{6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
		{6,4,4,4,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
		{6,6,4,4,4,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
		{6,6,6,4,4,4,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
		{6,6,6,6,4,4,4,6,6,6,6,6,6,6,6,6,6,6,6,6},
		{6,6,6,6,6,4,4,4,6,6,6,6,6,6,6,6,6,6,6,6},
		{6,6,6,6,6,6,4,4,4,4,4,4,6,6,6,6,6,6,6,6},
		{6,6,6,6,6,6,6,6,6,6,4,4,4,6,6,6,6,6,6,6},
		{6,6,6,6,6,6,6,6,6,6,6,4,4,4,6,6,6,6,6,6},
		{6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,6,6,6,6,6},
		{6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,6,6,6,6},
		{6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,6,6,6},
		{6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,6,6},
		{6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,6},
		{6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
	};

    //Grid dimensions measured by number of hexes
    private static int gridWidth;
    private static int gridHeight;

    //Get Grid Height
    public static int getGridHeight() { 
		return gridHeight; 
	}
	//Get Grid Width
    public static int getGridWidth() { 
		return gridWidth;  
	}

    //Returns coordinates for Player 1's Base
    public Hex getPlayer1Base() { 
		return player1Base; 
	}

	//Returns coordinates for Player 2's Base
	public Hex getPlayer2Base() {
		return player2Base;
	}

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

	// List of Hexes each player starts with. Created dynamicly based on map layout
	private Dictionary<Player.PlayerId,List<CapturableTile>> startingHexes;

	//Enumerate the different types of tiles
	public enum TileType {GRASS, BASE, TREE, SAND, ROCK, WATER, ALL }

    // Initialization
    void Start(){
		Debug.Log ("HexGrid: Start");
		if (!isServer) {
			// Only let the server actually create the map
			return;
		}

       
    }

	public override  void OnStartServer() {
		Debug.Log("HexGrid: OnStartServer");
		OnStartClient (); // manually make this call to ensure everthings initialised
		//Create the hex grid
		createHexGrid();
	}

	private bool clientInitialised = false;
	public override void OnStartClient() {
		Debug.Log("HexGrid: OnStartClient");
		if (clientInitialised) {
			return;
		}
		//initialize data structure for storing starting hexes
		startingHexes = new Dictionary<Player.PlayerId, List<CapturableTile>>();
		for(Player.PlayerId playId = Player.PlayerId.P1; playId != Player.PlayerId.NEUTRAL ;playId++){
			startingHexes.Add(playId, new List<CapturableTile>());
		}

		//Initialize map, and sizes
		initiateMapStructure();
		initializeSizes();
		clientInitialised = true;
	}

    //Method to store the sizes of grid/hexes/offsets
    private void initializeSizes(){
        hexHeight = tileGrass.GetComponent<SpriteRenderer>().bounds.size.y;
        hexWidth = tileGrass.GetComponent<SpriteRenderer>().bounds.size.x;
        xOffset = hexWidth * (0.5f - xOffsetGap);
        yOffset = hexHeight * (0.5f + yOffsetGap);
        gridHeight = mapStructure.GetLength(0);
        gridWidth = mapStructure.GetLength(1);

		Vector3 maxMapCoord= calcUnityCoord (new Vector2 (gridWidth-1, gridHeight-1));
		BoxCollider2D cameraEdge = GetComponent<BoxCollider2D>();
		cameraEdge.size = new Vector2(maxMapCoord.x + 10*xOffset,Mathf.Abs(maxMapCoord.y) + 5*yOffset);
		cameraEdge.offset = new Vector2 (maxMapCoord.x/2, -cameraEdge.size.y / 2 + yOffset);
		FindObjectOfType<MouseManager> ().setBounds (GetComponent<BoxCollider2D> ());
    }

    //This method chooses which level map to create
    private void initiateMapStructure(){
		//Level 1 map is used by default or if specifically chosen
		if (levelNumber == 0 || levelNumber == 1) {
			mapStructure = level1;
		}
		//Level 2 map is created
		else if(levelNumber == 2) {
			mapStructure = level2;
		}
    }

    //Turns a grid position to unity coordinates
    private Vector3 calcUnityCoord(Vector2 gridPos)
    {
        float x = gridPos.x * (hexWidth - xOffset/2);
        float y = -gridPos.y * hexHeight;

        if (gridPos.x % 2 == 1)
            y -= yOffset;

        return new Vector3(x, y, 0);
    }

    //Method to create the hex grid
    private void createHexGrid() {
		mapHexes = new GameObject[gridHeight,gridWidth];

		GameObject hexGridObject = Instantiate(parentMapObjectPrefab,transform);
        //Makes sure all generated game objects are under a parent. Allows tidier scene management
		hexGridObject.transform.SetParent(transform);
		NetworkServer.Spawn (hexGridObject);

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
						thisHex.GetComponent<Hex> ().setTileType(TileType.GRASS);
                        thisHex.GetComponent<Hex>().setTileIncome(baseGrassIncome);
                        break;
					}
					case 1:{
						thisHex = (GameObject)Instantiate (tilePlayer1Base);
						thisHex.GetComponent<Hex>().setTileType(TileType.BASE);
                        thisHex.GetComponent<Hex>().setTileIncome(basePlayerBaseIncome);
                        thisHex.GetComponent<Hex>().setHexOwner(Player.PlayerId.P1);
						Debug.Log ("HexGrid: createHexGrid: creating player 1 base with pid=" + thisHex.GetComponent<Hex>().getHexOwner());
                        player1Base = thisHex.GetComponent<Hex>();
                        break;
					}
					case 2:{
						thisHex = (GameObject)Instantiate (tilePlayer2Base);
						thisHex.GetComponent<Hex>().setTileType(TileType.BASE);
                        thisHex.GetComponent<Hex>().setTileIncome(basePlayerBaseIncome);
                        thisHex.GetComponent<Hex>().setHexOwner(Player.PlayerId.P2);
						Debug.Log ("HexGrid: createHexGrid: creating player 2 base with pid=" + thisHex.GetComponent<Hex>().getHexOwner());
						player2Base = thisHex.GetComponent<Hex>();
                        break;
					}
					case 3:{
						thisHex = (GameObject)Instantiate (tileRocks);
						thisHex.GetComponent<Hex>().setTileType(TileType.ROCK);
                        thisHex.GetComponent<Hex>().setTileIncome(baseRocksIncome);
                        break;
					}
					case 4:{
						thisHex = (GameObject)Instantiate (tileSand);
						thisHex.GetComponent<Hex>().setTileType(TileType.SAND);
                        thisHex.GetComponent<Hex>().setTileIncome(baseSandIncome);
                        break;
					}
					case 5:{
						thisHex = (GameObject)Instantiate (tileTrees);
						thisHex.GetComponent<Hex>().setTileType(TileType.TREE);
                        thisHex.GetComponent<Hex>().setTileIncome(baseTreesIncome);
                        break;
					}
					default:{
						thisHex = (GameObject)Instantiate (tileWater);
						thisHex.GetComponent<Hex>().setTileType(TileType.WATER);
						break;
					}
				}

				Player.PlayerId playId = thisHex.GetComponent<Hex>().getHexOwner();
				if(playId != Player.PlayerId.NEUTRAL){
					Debug.Log ("HexGrid: createHexGrid: adding starting tile for " + playId);
					startingHexes [playId].Add (thisHex.GetComponent<CapturableTile>());
				}

                //Set it's position, tranformation, name and other variables attached to the hex. 0 0 is top left corner
                Vector2 gridPos = new Vector2(x, y);
                thisHex.transform.position = calcUnityCoord(gridPos);
				thisHex.transform.SetParent(hexGridObject.transform);
                thisHex.name = "Hex_" + x + "_" + y;
                thisHex.GetComponent<Hex>().setX(x);
                thisHex.GetComponent<Hex>().setY(y);

                //Potential Optimization for hexgrid
                thisHex.isStatic = true;

				NetworkServer.Spawn (thisHex);

				mapHexes [y, x] = thisHex;

				RpcSetMap (thisHex, y, x);
            }

        }
    }
	
    //Method to get the hex, given a unity coordinate  TODO: PERHAPS UNUSED METHOD, need to check
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
		float Y = (-unityCoord.y / hexHeight);
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

	// Add income for each starting tile to the coresponding player
	public void initPlayerTiles(Player.PlayerId pid){
		if (!isServer) {
			return;
		}
		List<CapturableTile> playerStartingTiles = startingHexes [pid];
		Debug.Log ("HexGrid: initPlayerTiles: pid="+pid + ", startingTiles=" + playerStartingTiles.Count	);
		foreach (CapturableTile tile in playerStartingTiles) {
			tile.finalizeCapture ();
		}
	}


	[ClientRpc]
	private void RpcSetMap(GameObject hex, int y, int x){
		if(mapHexes == null){
			mapHexes = new GameObject[gridHeight,gridWidth];
		}
		mapHexes [y, x] = hex;
	}
}
