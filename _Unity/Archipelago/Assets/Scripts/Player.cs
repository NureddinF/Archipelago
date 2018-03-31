using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Player : NetworkBehaviour{
	
	public static bool isDisabled = false;
    // Identify who this player is
    public enum PlayerId { P1, P2, NEUTRAL };
	[SyncVar] public PlayerId playerId;

    // Player parameters
	public float rateMultiplier;
	public int baseIncome;
	public int timeFrame; // how long between discrete burst of money
	public float moneyToWin = 100;

    //The parent gameobject for main UI bar
    public GameObject stateMenu;

    // UI
    private Text incomeText;
    private Text rateText;
    private Text tilesOwnedText;
    private Text numOfWorkersOwned;
    private Text numOfWarriorsOwned;
    private Image redBarTemp;

    // State variables
    private float startTime;
    private float totalTileIncome;
    private float currentMoney;
	[SyncVar] private int totalTilesOwned;
	private bool endedGame = false;
	private bool isInitialised = false;
	
	private WinPanel winPanel; //calls on winPanel object

    // Use this for initialization
    void Start(){
        //Initialize Variables
        currentMoney = 0;
        startTime = Time.time;
    }
	
	void Awake(){
		winPanel = WinPanel.instance ();
	}

    // Update is called once per frame
    void Update(){

		// to pause script
		if (isDisabled) {
			if(hasAuthority && !endedGame){
				// This will be called when another player wins the game
				// Make sure to display Loss UI for players who have lost
				loseGame ();
			}
			return;
		}

		// Need to logically break this script into 2 parts:
		// 1) game state updates - to be run on server
		// 2) UI updates - to be run on the client to who owns this player object
		if (isServer) {
			serverUpdate ();
		}

		if (hasAuthority) {
			authoritativeUpdate ();
		}

    }

	// Run on server, update game state here
	private void serverUpdate(){
		if (!isServer) {
			// make sure clients don't run this code
			return;
		}
			
		// Use discrete chunks of income
		generateIncomeDiscrete();

		//when money threshold reached win the game
		if (currentMoney >= moneyToWin ) { 
			RpcWinGame();
		}
	}

	// Run on client who owns this player object. Do UI things here.
	private void authoritativeUpdate(){
		if (!hasAuthority || !isInitialised) {
			// make sure this code is only run on the client with authority
			return;
		}
		numOfWarriorsOwned.text = "" + gameObject.GetComponent<UnitController>().getTotalNumberOfWarriors();
		numOfWorkersOwned.text = "" + gameObject.GetComponent<UnitController>().getTotalNumberOfWorkers();
		tilesOwnedText.text = "" + totalTilesOwned;
		redBarTemp.transform.localScale = new Vector3((float)totalTilesOwned/((float)totalTilesOwned+1), 1, 1);
	}

    //Getters/Setters
    public float getCurrentMoney() { return currentMoney; }

    public void removeMoney(float amount) { currentMoney -= amount; }

    // Generates income in discrete chunks rather than each frame
    public void generateIncomeDiscrete(){
        if (Time.time > timeFrame + startTime){
			Debug.Log ("Player: generateIncomeDiscrete: updating income");
			float increaseAmt = (baseIncome + totalTileIncome) * rateMultiplier;
			// timeframe has passed, increa money
            currentMoney += increaseAmt;
            startTime = Time.time;
			RpcUpdateMoneyText (currentMoney, increaseAmt);
        }
    }


	//TODO: Multiplayer
    public void captureTile(CapturableTile tile){
        totalTileIncome += tile.getHex().getTileIncome();
        totalTilesOwned += 1;
        this.GetComponent<HexMenuController>().refreshUIValues();
    }

	//TODO: Multiplayer
    public void removeTile(CapturableTile tile)
    {
        totalTileIncome -= tile.getHex().getTileIncome();
        totalTilesOwned -= 1;
    }

	// Display loss screen for players who have not won when game ends
	private void loseGame(){
		endedGame = true;
		rateText.text = "LOSS";
		incomeText.text = "";

		winPanel.back("You Lose!"); //calls on back in winPanel class
	}

	//////////////////////////////////// Commands ///////////////////////////////////////


	//initialize the player ID and starting income
	[Command]
	public void CmdSetPlayerId(PlayerId pid){
		Debug.Log ("Player: CmdSetPlayerId");
		this.playerId = pid;

		// Make sure income from tiles owned at start of game is properly accounted for
		FindObjectOfType<HexGrid> ().initPlayerTiles(pid);
	}

	//////////////////////////////////// RPCs ///////////////////////////////////////

	// Use this to initialize UI things after object is spawned on network
	[ClientRpc]
	public void RpcStartWithAuthority(){
		Debug.Log ("Player: RpcStartWithAuthority");
		if (!hasAuthority) {
			Debug.Log ("Player: RpcStartWithAuthority: No authority");
			return;
		}

		//Connect all UI script elements to their gameobjects
		FindObjectOfType<MouseManager> ().player = this;
		stateMenu = GameObject.Find ("GameStateMenuBar");
		incomeText = stateMenu.transform.Find("Resource Icon/Total Amount Text").gameObject.GetComponent<Text>();
		rateText = stateMenu.transform.Find("Resource Icon/Increase Rate Text").gameObject.GetComponent<Text>();
		tilesOwnedText = stateMenu.transform.Find("Territory Icon/Total Territory Text").gameObject.GetComponent<Text>();
		numOfWorkersOwned = stateMenu.transform.Find("Worker Button/Worker Count Text").gameObject.GetComponent<Text>();
		numOfWarriorsOwned = stateMenu.transform.Find("Warrior Button/Warrior Count Text").gameObject.GetComponent<Text>();
		redBarTemp = stateMenu.transform.Find("TileOwnershipRatioBar/Red Bar").gameObject.GetComponent<Image>();

		Button workerButton = stateMenu.transform.Find ("Worker Button").GetComponent<Button> ();
		workerButton.onClick.AddListener(GetComponent<HexMenuController>().moveWorkerToSelectedHex);
		Button warriorButton = stateMenu.transform.Find ("Warrior Button").GetComponent<Button> ();
		warriorButton.onClick.AddListener(GetComponent<HexMenuController>().moveWarriorToSelectedHex);

		// Initalize UI values
		incomeText.text = "Hello";
		rateText.text = "+ 0/sec";	
		tilesOwnedText.text = "0";
		numOfWarriorsOwned.text = gameObject.GetComponent<UnitController>().initialNumOfWarriors.ToString();
		numOfWorkersOwned.text = gameObject.GetComponent<UnitController>().initialNumOfWorkers.ToString();

		isInitialised = true;
	}

	[ClientRpc]
	private void RpcUpdateMoneyText(float newMoney, float newRate){
		//Update player's UI with the amount of money they're making
		if (!hasAuthority) {
			//Only want to update UI for player who owns this object
			return;
		}
		Debug.Log ("Player: RpcUpdateMoneyText: newMoney = " + newMoney);
		// Update local state and display information
		currentMoney = newMoney;
		incomeText.text = "" + currentMoney;
		char plusMinus = newRate >= 0.0f ? '+' : '-';
		rateText.text = plusMinus + " " + newRate + "/" + timeFrame + "secs";
	}


	[ClientRpc]
	private void RpcWinGame(){
		// This will be run on all clients signaling the game has ended
		Time.timeScale = 0f; //pauses game
		Hex.isDisabled = true;    //Calls on these scripts to disable them, so the game cannot be played when a winner is found
		MouseManager.isDisabled = true;
		isDisabled = true;
		endedGame = true;

		if (hasAuthority) {
			// Only do UI updates if this is the client who ows this player object
			rateText.text = "WIN";
			incomeText.text = "";
			//Display menu to return to android section
			winPanel.back("You Win!");
		}
	}

	//////////////////////////////////// SyncVar hooks ///////////////////////////////////////


}
